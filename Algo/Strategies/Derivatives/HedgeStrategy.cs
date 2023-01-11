namespace StockSharp.Algo.Strategies.Derivatives
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	using Ecng.Collections;
	using Ecng.Common;

	using StockSharp.Algo.Derivatives;
	using StockSharp.Algo.Storages;
	using StockSharp.Algo.Strategies.Quoting;
	using StockSharp.Logging;
	using StockSharp.BusinessEntities;
	using StockSharp.Messages;
	using StockSharp.Localization;

	/// <summary>
	/// The base strategy of hedging.
	/// </summary>
	public abstract class HedgeStrategy : Strategy
	{
		private sealed class AssetStrategy : Strategy
		{
			public AssetStrategy(Security asset)
			{
				if (asset == null)
					throw new ArgumentNullException(nameof(asset));

				Name = asset.Id;
			}
		}

		private readonly SynchronizedDictionary<Security, Strategy> _strategies = new();
		//private bool _isSuspended;
		//private int _reHedgeOrders;
		private Strategy _assetStrategy;
		private readonly HashSet<Order> _awaitingOrders = new();
		private readonly SyncObject _syncRoot = new();

		/// <summary>
		/// Initialize <see cref="HedgeStrategy"/>.
		/// </summary>
		/// <param name="exchangeInfoProvider">Exchanges and trading boards provider.</param>
		protected HedgeStrategy(IExchangeInfoProvider exchangeInfoProvider)
		{
			BlackScholes = new BasketBlackScholes(this, this, exchangeInfoProvider, this);

			_useQuoting = new StrategyParam<bool>(this, nameof(UseQuoting));
			_priceOffset = new StrategyParam<Unit>(this, nameof(PriceOffset), new Unit());
		}

		/// <summary>
		/// Portfolio model for calculating the values of Greeks by the Black-Scholes formula.
		/// </summary>
		protected BasketBlackScholes BlackScholes { get; }

		private readonly StrategyParam<bool> _useQuoting;

		/// <summary>
		/// Whether to quote the registered order by the market price. The default mode is disabled.
		/// </summary>
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str1264Key,
			Description = LocalizedStrings.Str1265Key,
			GroupName = LocalizedStrings.Str1244Key,
			Order = 0)]
		public bool UseQuoting
		{
			get => _useQuoting.Value;
			set => _useQuoting.Value = value;
		}

		private readonly StrategyParam<Unit> _priceOffset;

		/// <summary>
		/// The price shift for the registering order. It determines the amount of shift from the best quote (for the buy it is added to the price, for the sell it is subtracted).
		/// </summary>
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str1266Key,
			Description = LocalizedStrings.Str1267Key,
			GroupName = LocalizedStrings.Str1244Key,
			Order = 1)]
		public Unit PriceOffset
		{
			get => _priceOffset.Value;
			set => _priceOffset.Value = value;
		}

		/// <summary>
		/// To get a list of rules on which the rehedging will respond.
		/// </summary>
		/// <returns>Rule list.</returns>
		protected virtual IEnumerable<IMarketRule> GetNotificationRules()
		{
			yield return Security.WhenNewTrade(this);
		}

		/// <inheritdoc />
		protected override void OnStarted()
		{
			base.OnStarted();

			//_reHedgeOrders = 0;
			_awaitingOrders.Clear();

			_strategies.Clear();

			if (_assetStrategy == null)
			{
				_assetStrategy = ChildStrategies.FirstOrDefault(s => s.Security == Security);
				
				if (_assetStrategy == null)
				{
					_assetStrategy = new AssetStrategy(Security);
					ChildStrategies.Add(_assetStrategy);

					this.AddInfoLog(LocalizedStrings.Str1268);
				}
				else
					this.AddInfoLog(LocalizedStrings.Str1269Params.Put(_assetStrategy));
			}

			_strategies.Add(Security, _assetStrategy);

			if (BlackScholes.UnderlyingAsset == null)
			{
				BlackScholes.UnderlyingAsset = _assetStrategy.Security;
				this.AddInfoLog(LocalizedStrings.Str1270);
			}

			BlackScholes.InnerModels.Clear();

			foreach (var strategy in ChildStrategies)
			{
				if (strategy.Security.Type == SecurityTypes.Option && strategy.Security.GetAsset(this) == Security)
				{
					BlackScholes.InnerModels.Add(new BlackScholes(strategy.Security, this, this, BlackScholes.ExchangeInfoProvider));
					_strategies.Add(strategy.Security, strategy);

					this.AddInfoLog(LocalizedStrings.Str1271Params.Put(strategy));
				}
			}

			this.SuspendRules(() =>
				GetNotificationRules().Or()
					.Do(ReHedge)
					.Apply(this));

			if (!IsRulesSuspended)
			{
				lock (_syncRoot)
					ReHedge();
			}
		}

		/// <summary>
		/// To get a list of orders rehedging the option position.
		/// </summary>
		/// <returns>Rehedging orders.</returns>
		protected abstract IEnumerable<Order> GetReHedgeOrders();

		/// <summary>
		/// To add the rehedging strategy.
		/// </summary>
		/// <param name="parentStrategy">The parent strategy (by the strike or the underlying asset).</param>
		/// <param name="order">The rehedging order.</param>
		protected virtual void AddReHedgeQuoting(Strategy parentStrategy, Order order)
		{
			if (parentStrategy == null)
				throw new ArgumentNullException(nameof(parentStrategy));

			var quoting = CreateQuoting(order);

			quoting.Name = parentStrategy.Name + "_" + quoting.Name;

			quoting
				.WhenStopped()
				.Do((rule, s) => TryResumeMonitoring(order))
				.Once()
				.Apply(parentStrategy);

			parentStrategy.ChildStrategies.Add(quoting);
		}

		/// <summary>
		/// To add the rehedging order.
		/// </summary>
		/// <param name="parentStrategy">The parent strategy (by the strike or the underlying asset).</param>
		/// <param name="order">The rehedging order.</param>
		protected virtual void AddReHedgeOrder(Strategy parentStrategy, Order order)
		{
			var doneRule = order.WhenMatched(this)
				.Or(order.WhenCanceled(this))
				.Do((rule, o) =>
				{
					parentStrategy.AddInfoLog(LocalizedStrings.Str1272Params, o.TransactionId, o.IsMatched() ? LocalizedStrings.Str1328 : LocalizedStrings.Str1329, o.LastChangeTime);

					Rules.RemoveRulesByToken(o, rule);

					TryResumeMonitoring(order);
				})
				.Once()
				.Apply(parentStrategy);

			var regRule = order
				.WhenRegistered(this)
				.Do(o => parentStrategy.AddInfoLog(LocalizedStrings.Str1275Params, o.TransactionId, o.Id, o.Time))
				.Once()
				.Apply(parentStrategy);

			var regFailRule = order
				.WhenRegisterFailed(this)
				.Do((rule, fail) =>
				{
					parentStrategy.AddErrorLog(LocalizedStrings.Str1276Params, fail.Order.TransactionId, fail.Error);

					TryResumeMonitoring(order);
					ReHedge();
				})
				.Once()
				.Apply(parentStrategy);

			doneRule.Exclusive(regFailRule);
			regRule.Exclusive(regFailRule);

			parentStrategy.RegisterOrder(order);
		}

		/// <summary>
		/// To start rehedging.
		/// </summary>
		/// <param name="orders">Rehedging orders.</param>
		protected virtual void ReHedge(IEnumerable<Order> orders)
		{
			if (orders == null)
				throw new ArgumentNullException(nameof(orders));

			foreach (var order in orders)
			{
				this.AddInfoLog(LocalizedStrings.Str1277Params, order.Security, order.Direction, order.Volume, order.Price);

				var strategy = _strategies.TryGetValue(order.Security);

				if (strategy == null)
					throw new InvalidOperationException(LocalizedStrings.Str1278Params.Put(order.Security.Id));

				if (UseQuoting)
				{
					AddReHedgeQuoting(strategy, order);
				}
				else
				{
					AddReHedgeOrder(strategy, order);
				}
			}
		}

		/// <summary>
		/// Whether the rehedging is paused.
		/// </summary>
		/// <returns><see langword="true" /> if paused, otherwise, <see langword="false" />.</returns>
		protected virtual bool IsSuspended()
		{
			return !_awaitingOrders.IsEmpty();
		}

		private void ReHedge()
		{
			if (IsSuspended())
			{
				//this.AddWarningLog("Рехеджирование уже запущено.");
				return;
			}

			//_isSuspended = false;
			_awaitingOrders.Clear();

			var orders = GetReHedgeOrders();

			_awaitingOrders.AddRange(orders);

			if (!_awaitingOrders.IsEmpty())
			{
				this.AddInfoLog(LocalizedStrings.Str1279Params, _awaitingOrders.Count);
				ReHedge(orders);
			}
		}

		private void TryResumeMonitoring(Order order)
		{
			if (!_awaitingOrders.Remove(order))
				return;

			if (_awaitingOrders.IsEmpty())
				this.AddInfoLog(LocalizedStrings.Str1280);
			else
				this.AddInfoLog(LocalizedStrings.Str1281Params, _awaitingOrders.Count);
		}

		/// <summary>
		/// To create a quoting strategy to change the position.
		/// </summary>
		/// <param name="order">Quoting order.</param>
		/// <returns>The strategy of quoting.</returns>
		protected virtual QuotingStrategy CreateQuoting(Order order)
		{
			return new MarketQuotingStrategy(order, new Unit(), new Unit()) { Volume = Volume };
		}
	}
}