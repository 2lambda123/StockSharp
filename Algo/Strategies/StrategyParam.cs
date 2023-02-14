#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Strategies.Algo
File: StrategyParam.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Strategies
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;

	using Ecng.Common;
	using Ecng.Collections;
	using Ecng.Serialization;

	using StockSharp.Localization;

	/// <summary>
	/// The strategy parameter.
	/// </summary>
	public interface IStrategyParam : IPersistable
	{
		/// <summary>
		/// Parameter identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Parameter name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The type of the parameter value.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// The parameter value.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Check can optimize parameter.
		/// </summary>
		bool CanOptimize { get; }

		/// <summary>
		/// The From value at optimization.
		/// </summary>
		object OptimizeFrom { get; set; }

		/// <summary>
		/// The To value at optimization.
		/// </summary>
		object OptimizeTo { get; set; }

		/// <summary>
		/// The Increment value at optimization.
		/// </summary>
		object OptimizeStep { get; set; }
	}

	/// <summary>
	/// Wrapper for typified access to the strategy parameter.
	/// </summary>
	/// <typeparam name="T">The type of the parameter value.</typeparam>
	public class StrategyParam<T> : IStrategyParam
	{
		private readonly IEqualityComparer<T> _comparer;
		private readonly Strategy _strategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="StrategyParam{T}"/>.
		/// </summary>
		/// <param name="strategy">Strategy.</param>
		/// <param name="name">Parameter name.</param>
		public StrategyParam(Strategy strategy, string name)
			: this(strategy, name, name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StrategyParam{T}"/>.
		/// </summary>
		/// <param name="strategy">Strategy.</param>
		/// <param name="id">Parameter identifier.</param>
		/// <param name="name">Parameter name.</param>
		public StrategyParam(Strategy strategy, string id, string name)
			: this(strategy, id, name, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StrategyParam{T}"/>.
		/// </summary>
		/// <param name="strategy">Strategy.</param>
		/// <param name="name">Parameter name.</param>
		/// <param name="initialValue">The initial value.</param>
		public StrategyParam(Strategy strategy, string name, T initialValue)
			: this(strategy, name, name, initialValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StrategyParam{T}"/>.
		/// </summary>
		/// <param name="strategy">Strategy.</param>
		/// <param name="id">Parameter identifier.</param>
		/// <param name="name">Parameter name.</param>
		/// <param name="initialValue">The initial value.</param>
		public StrategyParam(Strategy strategy, string id, string name, T initialValue)
		{
			if (id.IsEmpty())
				throw new ArgumentNullException(nameof(id));

			if (name.IsEmpty())
				throw new ArgumentNullException(nameof(name));

			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
			Id = id;
			Name = name;
			_value = initialValue;

			if (!_strategy.Parameters.TryAdd2(id, this))
				throw new ArgumentException(LocalizedStrings.CompositionAlreadyExistParams.Put(name, string.Empty), nameof(name));

			CanOptimize = typeof(T).CanOptimize();
			AllowNull = typeof(T).IsNullable();

			_comparer = EqualityComparer<T>.Default;
		}

		/// <inheritdoc />
		public string Id { get; private set; }

		/// <inheritdoc />
		public string Name { get; private set; }

		/// <summary>
		/// Is it possible to store in <see cref="Value"/> a value, equal to <see langword="null" />.
		/// </summary>
		public bool AllowNull { get; set; }

		private T _value;

		/// <inheritdoc />
		public T Value
		{
			get => _value;
			set
			{
				if (value is null)
				{
					if (!AllowNull)
						throw new ArgumentNullException(nameof(value));

					if (_value is null)
						return;
				}
				else
				{
					if (_comparer.Equals(_value, value))
						return;
				}

				if (_value is INotifyPropertyChanged propChange)
					propChange.PropertyChanged -= OnValueInnerStateChanged;

				_value = value;
				_strategy.RaiseParametersChanged(Name);

				if (_value is INotifyPropertyChanged propChange2)
					propChange2.PropertyChanged += OnValueInnerStateChanged;
			}
		}

		Type IStrategyParam.Type => typeof(T);

		object IStrategyParam.Value
		{
			get => Value;
			set => Value = (T)value;
		}

		/// <inheritdoc />
		public bool CanOptimize { get; set; }

		/// <inheritdoc />
		public object OptimizeFrom { get; set; }

		/// <inheritdoc />
		public object OptimizeTo { get; set; }

		/// <inheritdoc />
		public object OptimizeStep { get; set; }

		private void OnValueInnerStateChanged(object sender, PropertyChangedEventArgs e)
		{
			_strategy.RaiseParametersChanged(Name);
		}

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public void Load(SettingsStorage storage)
		{
			Id = storage.GetValue<string>(nameof(Id));
			Name = storage.GetValue<string>(nameof(Name));
			Value = storage.GetValue<T>(nameof(Value));
			CanOptimize = storage.GetValue(nameof(CanOptimize), CanOptimize);
			OptimizeFrom = storage.GetValue<SettingsStorage>(nameof(OptimizeFrom))?.FromStorage();
			OptimizeTo = storage.GetValue<SettingsStorage>(nameof(OptimizeTo))?.FromStorage();
			OptimizeStep = storage.GetValue<SettingsStorage>(nameof(OptimizeStep))?.FromStorage();
		}

		/// <summary>
		/// Save settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public void Save(SettingsStorage storage)
		{
			storage.SetValue(nameof(Id), Id);
			storage.SetValue(nameof(Name), Name);
			storage.SetValue(nameof(Value), Value);
			storage.SetValue(nameof(CanOptimize), CanOptimize);
			storage.SetValue(nameof(OptimizeFrom), OptimizeFrom?.ToStorage());
			storage.SetValue(nameof(OptimizeTo), OptimizeTo?.ToStorage());
			storage.SetValue(nameof(OptimizeStep), OptimizeStep?.ToStorage());
		}

		/// <inheritdoc />
		public override string ToString() => Name;
	}
}