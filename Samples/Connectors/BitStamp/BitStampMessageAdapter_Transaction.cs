namespace StockSharp.BitStamp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ecng.Collections;
using Ecng.Common;

using StockSharp.BitStamp.Native.Model;
using StockSharp.Localization;
using StockSharp.Messages;

partial class BitStampMessageAdapter
{
	private string PortfolioName => nameof(BitStamp) + "_" + Key.ToId();

	/// <inheritdoc />
	protected override async ValueTask OnRegisterOrderAsync(OrderRegisterMessage regMsg, CancellationToken cancellationToken)
	{
		var condition = (BitStampOrderCondition)regMsg.Condition;

		switch (regMsg.OrderType)
		{
			case null:
			case OrderTypes.Limit:
			case OrderTypes.Market:
				break;
			case OrderTypes.Conditional:
			{
				if (!condition.IsWithdraw)
					break;

				var withdrawId = await _httpClient.Withdraw(regMsg.SecurityId.SecurityCode, regMsg.Volume, condition.WithdrawInfo, cancellationToken);

				SendOutMessage(new ExecutionMessage
				{
					DataTypeEx = DataType.Transactions,
					OrderId = withdrawId,
					ServerTime = CurrentTime.ConvertToUtc(),
					OriginalTransactionId = regMsg.TransactionId,
					OrderState = OrderStates.Done,
					HasOrderInfo = true,
				});

				await OnPortfolioLookupAsync(null, cancellationToken);
				return;
			}
			default:
				throw new NotSupportedException(LocalizedStrings.Str1601Params.Put(regMsg.OrderType, regMsg.TransactionId));
		}

		var price = regMsg.OrderType == OrderTypes.Market ? (decimal?)null : regMsg.Price;

		var result = await _httpClient.RegisterOrder(regMsg.SecurityId.ToCurrency(), regMsg.Side.ToString().ToLowerInvariant(), price, regMsg.Volume, condition?.StopPrice, regMsg.TillDate.IsToday(), regMsg.TimeInForce == TimeInForce.CancelBalance, cancellationToken);

		_orderInfo.Add(result.Id, RefTuple.Create(regMsg.TransactionId, regMsg.Volume));

		SendOutMessage(new ExecutionMessage
		{
			DataTypeEx = DataType.Transactions,
			OrderId = result.Id,
			ServerTime = result.Time,
			OriginalTransactionId = regMsg.TransactionId,
			OrderState = OrderStates.Active,
			HasOrderInfo = true,
		});
	}

	/// <inheritdoc />
	protected override async ValueTask OnCancelOrderAsync(OrderCancelMessage cancelMsg, CancellationToken cancellationToken)
	{
		if (cancelMsg.OrderId == null)
			throw new InvalidOperationException(LocalizedStrings.Str2252Params.Put(cancelMsg.OriginalTransactionId));

		await _httpClient.CancelOrder(cancelMsg.OrderId.Value, cancellationToken);

		//SendOutMessage(new ExecutionMessage
		//{
		//	ServerTime = CurrentTime.ConvertToUtc(),
		//	DataTypeEx = DataType.Transactions,
		//	OriginalTransactionId = cancelMsg.TransactionId,
		//	OrderState = OrderStates.Done,
		//	HasOrderInfo = true,
		//});

		await OnOrderStatusAsync(null, cancellationToken);
		await OnPortfolioLookupAsync(null, cancellationToken);
	}

	/// <inheritdoc />
	protected override async ValueTask OnCancelOrderGroupAsync(OrderGroupCancelMessage cancelMsg, CancellationToken cancellationToken)
	{
		await _httpClient.CancelAllOrders(cancellationToken);

		SendOutMessage(new ExecutionMessage
		{
			ServerTime = CurrentTime.ConvertToUtc(),
			DataTypeEx = DataType.Transactions,
			OriginalTransactionId = cancelMsg.TransactionId,
			HasOrderInfo = true,
		});

		await OnOrderStatusAsync(null, cancellationToken);
		await OnPortfolioLookupAsync(null, cancellationToken);
	}

	private void ProcessOrder(UserOrder order, decimal balance, long transId, long origTransId)
	{
		SendOutMessage(new ExecutionMessage
		{
			DataTypeEx = DataType.Transactions,
			OrderId = order.Id,
			TransactionId = transId,
			OriginalTransactionId = origTransId,
			OrderPrice = (decimal)order.Price,
			Balance = balance,
			OrderVolume = (decimal)order.Amount,
			Side = order.Type.ToSide(),
			SecurityId = order.CurrencyPair.ToStockSharp(),
			ServerTime = transId != 0 ? order.Time : CurrentTime.ConvertToUtc(),
			PortfolioName = PortfolioName,
			OrderState = OrderStates.Active,
			HasOrderInfo = true,
		});
	}

	private void ProcessTrade(UserTransaction transaction)
	{
		// not trade
		if (transaction.Type != 2)
			throw new InvalidOperationException("type = {0}".Put(transaction.Type));

		var info = _orderInfo.TryGetValue(transaction.OrderId);

		if (info == null/* || info.Second <= 0*/)
			return;

		string pair;
		decimal price;
		decimal volume;

		if (transaction.BtcUsd != null)
		{
			pair = "btcusd";
			price = (decimal)transaction.BtcUsd.Value;
			volume = (decimal)transaction.BtcAmount.Value;
		}
		else if (transaction.BtcEur != null)
		{
			pair = "btceur";
			price = (decimal)transaction.BtcEur.Value;
			volume = (decimal)transaction.BtcAmount.Value;
		}
		else if (transaction.BchBtc != null)
		{
			pair = "bchbtc";
			price = (decimal)transaction.BchBtc.Value;
			volume = (decimal)transaction.BchAmount.Value;
		}
		else if (transaction.BchUsd != null)
		{
			pair = "bchusd";
			price = (decimal)transaction.BchUsd.Value;
			volume = (decimal)transaction.BchAmount.Value;
		}
		else if (transaction.BchEur != null)
		{
			pair = "btceur";
			price = (decimal)transaction.BchEur.Value;
			volume = (decimal)transaction.BchAmount.Value;
		}
		else if (transaction.EthBtc != null)
		{
			pair = "ethbtc";
			price = (decimal)transaction.EthBtc.Value;
			volume = (decimal)transaction.EthAmount.Value;
		}
		else if (transaction.EthUsd != null)
		{
			pair = "ethusd";
			price = (decimal)transaction.EthUsd.Value;
			volume = (decimal)transaction.EthAmount.Value;
		}
		else if (transaction.EthEur != null)
		{
			pair = "etheur";
			price = (decimal)transaction.EthEur.Value;
			volume = (decimal)transaction.EthAmount.Value;
		}
		else if (transaction.LtcBtc != null)
		{
			pair = "ltcbtc";
			price = (decimal)transaction.LtcBtc.Value;
			volume = (decimal)transaction.LtcAmount.Value;
		}
		else if (transaction.LtcUsd != null)
		{
			pair = "ltcusd";
			price = (decimal)transaction.LtcUsd.Value;
			volume = (decimal)transaction.LtcAmount.Value;
		}
		else if (transaction.LtcEur != null)
		{
			pair = "ltceur";
			price = (decimal)transaction.LtcEur.Value;
			volume = (decimal)transaction.LtcAmount.Value;
		}
		else if (transaction.XrpBtc != null)
		{
			pair = "xrpbtc";
			price = (decimal)transaction.XrpBtc.Value;
			volume = (decimal)transaction.XrpAmount.Value;
		}
		else if (transaction.XrpUsd != null)
		{
			pair = "xrpusd";
			price = (decimal)transaction.XrpUsd.Value;
			volume = (decimal)transaction.XrpAmount.Value;
		}
		else if (transaction.XrpEur != null)
		{
			pair = "xrpeur";
			price = (decimal)transaction.XrpEur.Value;
			volume = (decimal)transaction.XrpAmount.Value;
		}
		else
			throw new InvalidOperationException("Unknown pair.");

		volume = volume.Abs();

		SendOutMessage(new ExecutionMessage
		{
			DataTypeEx = DataType.Transactions,
			OrderId = transaction.OrderId,
			TradeId = transaction.Id,
			TradePrice = price,
			TradeVolume = volume,
			SecurityId = pair.ToStockSharp(),
			ServerTime = transaction.Time.ToDto(),
			PortfolioName = PortfolioName,
			HasTradeInfo = true,
			Commission = (decimal)transaction.Fee,
			OriginalTransactionId = info.First,
		});

		info.Second -= volume;

		if (info.Second < 0)
			throw new InvalidOperationException(LocalizedStrings.Str3301Params.Put(transaction.OrderId, info.Second));

		SendOutMessage(new ExecutionMessage
		{
			DataTypeEx = DataType.Transactions,
			OrderId = transaction.OrderId,
			Balance = info.Second,
			OrderState = info.Second > 0 ? OrderStates.Active : OrderStates.Done,
			HasOrderInfo = true,
			SecurityId = pair.ToStockSharp(),
			ServerTime = transaction.Time.ToDto(),
			PortfolioName = PortfolioName,
			OriginalTransactionId = info.First,
		});

		if (info.Second == 0)
			_orderInfo.Remove(transaction.OrderId);
	}

	/// <inheritdoc />
	protected override async ValueTask OnOrderStatusAsync(OrderStatusMessage statusMsg, CancellationToken cancellationToken)
	{
		if (statusMsg == null)
		{
			var portfolioRefresh = false;

			var orders = await _httpClient.RequestOpenOrders("all", cancellationToken);

			var ids = _orderInfo.Keys.ToSet();

			foreach (var order in orders)
			{
				ids.Remove(order.Id);

				var info = _orderInfo.TryGetValue(order.Id);

				if (info == null)
				{
					info = RefTuple.Create(TransactionIdGenerator.GetNextId(), (decimal)order.Amount);

					_orderInfo.Add(order.Id, info);

					ProcessOrder(order, (decimal)order.Amount, info.First, 0);

					portfolioRefresh = true;
				}
				else
				{
					// balance existing orders tracked by trades
				}
			}

			var trades = await GetTrades(cancellationToken);

			foreach (var trade in trades)
			{
				ProcessTrade(trade);
			}

			foreach (var id in ids)
			{
				// can be removed from ProcessTrade
				if (!_orderInfo.TryGetAndRemove(id, out var info))
					return;

				SendOutMessage(new ExecutionMessage
				{
					DataTypeEx = DataType.Transactions,
					HasOrderInfo = true,
					OrderId = id,
					OriginalTransactionId = info.First,
					ServerTime = CurrentTime.ConvertToUtc(),
					OrderState = OrderStates.Done,
				});

				portfolioRefresh = true;
			}

			if (portfolioRefresh)
				await OnPortfolioLookupAsync(null, cancellationToken);
		}
		else
		{
			if (!statusMsg.IsSubscribe)
				return;

			var orders = (await _httpClient.RequestOpenOrders("all", cancellationToken)).ToArray();

			foreach (var order in orders)
			{
				var info = RefTuple.Create(TransactionIdGenerator.GetNextId(), (decimal)order.Amount);

				_orderInfo.Add(order.Id, info);

				ProcessOrder(order, (decimal)order.Amount, info.First, statusMsg.TransactionId);
			}

			var trades = await GetTrades(cancellationToken);

			foreach (var trade in trades)
			{
				ProcessTrade(trade);
			}

			SendSubscriptionResult(statusMsg);
		}
	}

	private async ValueTask<IEnumerable<UserTransaction>> GetTrades(CancellationToken cancellationToken)
	{
		const int pageSize = 100;
		const int maxOffset = 10000;

		var trades = new List<UserTransaction>();

		var offset = 0;

		while (true)
		{
			var batch = await _httpClient.RequestUserTransactions(null, offset, offset + pageSize, cancellationToken);

			var hasLess = false;
			trades.AddRange(batch.Where(t => t.Type == 2).Where(t =>
			{
				if (t.Id > _lastMyTradeId)
					return true;

				hasLess = true;
				return false;
			}).ToArray());

			if (hasLess || batch.Length < pageSize)
				break;

			offset += pageSize;

			if (offset >= maxOffset)
				break;
		}

		if (trades.Count > 0)
		{
			trades = trades.DistinctBy(t => t.Id).OrderBy(t => t.Id).ToList();
			_lastMyTradeId = trades.Last().Id;
		}

		return trades;
	}

	/// <inheritdoc />
	protected override async ValueTask OnPortfolioLookupAsync(PortfolioLookupMessage lookupMsg, CancellationToken cancellationToken)
	{
		if (lookupMsg != null)
		{
			if (!lookupMsg.IsSubscribe)
				return;
		}

		var transactionId = lookupMsg?.TransactionId ?? 0;

		var pfName = PortfolioName;

		SendOutMessage(new PortfolioMessage
		{
			PortfolioName = pfName,
			BoardCode = BoardCodes.BitStamp,
			OriginalTransactionId = transactionId,
		});

		if (lookupMsg != null)
			SendSubscriptionResult(lookupMsg);

		var tuple = await _httpClient.GetBalances(null, cancellationToken);

		foreach (var pair in tuple.Item1)
		{
			var currValue = pair.Value.First;
			var currPrice = pair.Value.Second;
			var blockValue = pair.Value.Third;

			if (currValue == null && currPrice == null && blockValue == null)
				continue;

			var msg = this.CreatePositionChangeMessage(pfName, pair.Key.ToUpperInvariant().ToStockSharp(false));

			msg.TryAdd(PositionChangeTypes.CurrentValue, currValue, true);
			msg.TryAdd(PositionChangeTypes.CurrentPrice, currPrice, true);
			msg.TryAdd(PositionChangeTypes.BlockedValue, blockValue, true);

			SendOutMessage(msg);
		}

		foreach (var pair in tuple.Item2)
		{
			SendOutMessage(new Level1ChangeMessage
			{
				SecurityId = pair.Key.ToStockSharp(),
				ServerTime = CurrentTime.ConvertToUtc()
			}.TryAdd(Level1Fields.CommissionTaker, pair.Value));
		}

		_lastTimeBalanceCheck = CurrentTime;
	}
}
