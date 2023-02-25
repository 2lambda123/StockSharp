#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: IIndicatorValue.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Ecng.Common;

	using StockSharp.BusinessEntities;
	using StockSharp.Messages;
	using StockSharp.Localization;

	/// <summary>
	/// The indicator value, based on which it will renew its value, as well as value, containing result of indicator calculation.
	/// </summary>
	public interface IIndicatorValue : IComparable<IIndicatorValue>, IComparable
	{
		/// <summary>
		/// Indicator.
		/// </summary>
		IIndicator Indicator { get; }

		/// <summary>
		/// No indicator value.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Is the value final (indicator finalizes its value and will not be changed anymore in the given point of time).
		/// </summary>
		bool IsFinal { get; set; }

		/// <summary>
		/// Whether the indicator is set.
		/// </summary>
		bool IsFormed { get; }

		/// <summary>
		/// The input value.
		/// </summary>
		IIndicatorValue InputValue { get; set; }

		/// <summary>
		/// Does value support data type, required for the indicator.
		/// </summary>
		/// <param name="valueType">The data type, operated by indicator.</param>
		/// <returns><see langword="true" />, if data type is supported, otherwise, <see langword="false" />.</returns>
		bool IsSupport(Type valueType);

		/// <summary>
		/// To get the value by the data type.
		/// </summary>
		/// <typeparam name="T">The data type, operated by indicator.</typeparam>
		/// <returns>Value.</returns>
		T GetValue<T>();

		/// <summary>
		/// To replace the indicator input value by new one (for example it is received from another indicator).
		/// </summary>
		/// <typeparam name="T">The data type, operated by indicator.</typeparam>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		/// <returns>New object, containing input value.</returns>
		IIndicatorValue SetValue<T>(IIndicator indicator, T value);
	}

	/// <summary>
	/// The base class for the indicator value.
	/// </summary>
	public abstract class BaseIndicatorValue : IIndicatorValue
	{
		/// <summary>
		/// Initialize <see cref="BaseIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		protected BaseIndicatorValue(IIndicator indicator)
		{
			Indicator = indicator ?? throw new ArgumentNullException(nameof(indicator));
			IsFormed = indicator.IsFormed;
		}

		/// <inheritdoc />
		public IIndicator Indicator { get; }

		/// <inheritdoc />
		public abstract bool IsEmpty { get; set; }

		/// <inheritdoc />
		public abstract bool IsFinal { get; set; }

		/// <inheritdoc />
		public bool IsFormed { get; }

		/// <inheritdoc />
		public abstract IIndicatorValue InputValue { get; set; }

		/// <inheritdoc />
		public abstract bool IsSupport(Type valueType);

		/// <inheritdoc />
		public abstract T GetValue<T>();

		/// <inheritdoc />
		public abstract IIndicatorValue SetValue<T>(IIndicator indicator, T value);

		/// <inheritdoc />
		public abstract int CompareTo(IIndicatorValue other);

		/// <inheritdoc />
		int IComparable.CompareTo(object other)
		{
			var value = other as IIndicatorValue;

			if (other == null)
				throw new ArgumentException(LocalizedStrings.Str911, nameof(other));

			return CompareTo(value);
		}
	}

	/// <summary>
	/// The base value of the indicator, operating with one data type.
	/// </summary>
	/// <typeparam name="TValue">Value type.</typeparam>
	public class SingleIndicatorValue<TValue> : BaseIndicatorValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SingleIndicatorValue{T}"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		public SingleIndicatorValue(IIndicator indicator, TValue value)
			: base(indicator)
		{
			Value = value;
			IsEmpty = value.IsNull();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleIndicatorValue{T}"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		public SingleIndicatorValue(IIndicator indicator)
			: base(indicator)
		{
			IsEmpty = true;
		}

		/// <summary>
		/// Value.
		/// </summary>
		public TValue Value { get; }

		/// <inheritdoc />
		public override bool IsEmpty { get; set; }

		/// <inheritdoc />
		public override bool IsFinal { get; set; }

		/// <inheritdoc />
		public override IIndicatorValue InputValue { get; set; }

		/// <inheritdoc />
		public override bool IsSupport(Type valueType) => valueType == typeof(TValue);

		/// <inheritdoc />
		public override T GetValue<T>()
		{
			ThrowIfEmpty();
			return Value is T t ? t : throw new InvalidCastException($"Cannot convert {typeof(TValue).Name} to {typeof(T).Name}."); ;
		}

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value)
		{
			return new SingleIndicatorValue<T>(indicator, value) { IsFinal = IsFinal, InputValue = this };
		}

		private void ThrowIfEmpty()
		{
			if (IsEmpty)
				throw new InvalidOperationException(LocalizedStrings.Str910);
		}

		/// <inheritdoc />
		public override int CompareTo(IIndicatorValue other) => Value.Compare(other.GetValue<TValue>());

		/// <inheritdoc />
		public override string ToString() => IsEmpty ? "Empty" : Value.ToString();
	}

	/// <summary>
	/// The indicator value, operating with data type <see cref="decimal"/>.
	/// </summary>
	public class DecimalIndicatorValue : SingleIndicatorValue<decimal>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		public DecimalIndicatorValue(IIndicator indicator, decimal value)
			: base(indicator, value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		public DecimalIndicatorValue(IIndicator indicator)
			: base(indicator)
		{
		}

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value)
		{
			return typeof(T) == typeof(decimal)
				? new DecimalIndicatorValue(indicator, value.To<decimal>()) { IsFinal = IsFinal, InputValue = this }
				: base.SetValue(indicator, value);
		}
	}

	/// <summary>
	/// The indicator value, operating with data type <see cref="ICandleMessage"/>.
	/// </summary>
	public class CandleIndicatorValue : SingleIndicatorValue<ICandleMessage>
	{
		private readonly Func<ICandleMessage, decimal> _getPart;

		/// <summary>
		/// Initializes a new instance of the <see cref="CandleIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		public CandleIndicatorValue(IIndicator indicator, ICandleMessage value)
			: this(indicator, value, ByClose)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CandleIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		/// <param name="getPart">The candle converter, through which its parameter can be got. By default, the <see cref="CandleIndicatorValue.ByClose"/> is used.</param>
		public CandleIndicatorValue(IIndicator indicator, ICandleMessage value, Func<ICandleMessage, decimal> getPart)
			: base(indicator, value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			_getPart = getPart ?? throw new ArgumentNullException(nameof(getPart));

			IsFinal = value.State == CandleStates.Finished;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CandleIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		private CandleIndicatorValue(IIndicator indicator)
			: base(indicator)
		{
		}

		/// <summary>
		/// The converter, taking from candle closing price <see cref="ICandleMessage.ClosePrice"/>.
		/// </summary>
		public static readonly Func<ICandleMessage, decimal> ByClose = c => c.ClosePrice;

		/// <summary>
		/// The converter, taking from candle opening price <see cref="ICandleMessage.OpenPrice"/>.
		/// </summary>
		public static readonly Func<ICandleMessage, decimal> ByOpen = c => c.OpenPrice;

		/// <summary>
		/// The converter, taking from candle middle of the body (<see cref="ICandleMessage.OpenPrice"/> + <see cref="ICandleMessage.ClosePrice"/>) / 2.
		/// </summary>
		public static readonly Func<ICandleMessage, decimal> ByMiddle = c => (c.ClosePrice + c.OpenPrice) / 2;

		/// <inheritdoc />
		public override bool IsSupport(Type valueType) => valueType == typeof(decimal) || valueType.Is<ICandleMessage>();

		/// <inheritdoc />
		public override T GetValue<T>()
		{
			var candle = base.GetValue<ICandleMessage>();

			if (typeof(T) == typeof(decimal))
				return _getPart(candle) is T t ? t : throw new InvalidCastException($"Cannot convert decimal to {typeof(T).Name}.");
			else
				return candle is T t ? t : throw new InvalidCastException($"Cannot convert candle to {typeof(T).Name}.");
		}

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value)
		{
			return value is ICandleMessage candle
					? new CandleIndicatorValue(indicator, candle) { InputValue = this }
					: value.IsNull() ? new CandleIndicatorValue(indicator) : base.SetValue(indicator, value);
		}
	}

	/// <summary>
	/// The indicator value, operating with data type <see cref="MarketDepth"/>.
	/// </summary>
	public class MarketDepthIndicatorValue : SingleIndicatorValue<MarketDepth>
	{
		private readonly Func<MarketDepth, decimal?> _getPart;

		/// <summary>
		/// Initializes a new instance of the <see cref="MarketDepthIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="depth">Market depth.</param>
		public MarketDepthIndicatorValue(IIndicator indicator, MarketDepth depth)
			: this(indicator, depth, ByMiddle)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MarketDepthIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="depth">Market depth.</param>
		/// <param name="getPart">The order book converter, through which its parameter can be got. By default, the <see cref="MarketDepthIndicatorValue.ByMiddle"/> is used.</param>
		public MarketDepthIndicatorValue(IIndicator indicator, MarketDepth depth, Func<MarketDepth, decimal?> getPart)
			: base(indicator, depth)
		{
			if (depth == null)
				throw new ArgumentNullException(nameof(depth));

			_getPart = getPart ?? throw new ArgumentNullException(nameof(getPart));
		}

		/// <summary>
		/// The converter, taking from the order book the best bid price <see cref="MarketDepth.BestBid2"/>.
		/// </summary>
		public static readonly Func<MarketDepth, decimal?> ByBestBid = d => d.BestBid2?.Price;

		/// <summary>
		/// The converter, taking from the order book the best offer price <see cref="MarketDepth.BestAsk2"/>.
		/// </summary>
		public static readonly Func<MarketDepth, decimal?> ByBestAsk = d => d.BestAsk2?.Price;

		/// <summary>
		/// The converter, taking from the order book the middle of the spread <see cref="MarketDepthPair.MiddlePrice"/>.
		/// </summary>
		public static readonly Func<MarketDepth, decimal?> ByMiddle = d => d.BestPair?.MiddlePrice;

		/// <inheritdoc />
		public override bool IsSupport(Type valueType)
		{
			return valueType == typeof(decimal) || base.IsSupport(valueType);
		}

		/// <inheritdoc />
		public override T GetValue<T>()
		{
			var depth = base.GetValue<MarketDepth>();
			return typeof(T) == typeof(decimal) ? (_getPart(depth) ?? 0).To<T>() : depth.To<T>();
		}

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value)
		{
			return new MarketDepthIndicatorValue(indicator, base.GetValue<MarketDepth>(), _getPart)
			{
				IsFinal = IsFinal,
				InputValue = this
			};
		}
	}

	/// <summary>
	/// The value of the indicator, operating with pair <see ref="Tuple{TValue, TValue}" />.
	/// </summary>
	/// <typeparam name="TValue">Value type.</typeparam>
	public class PairIndicatorValue<TValue> : SingleIndicatorValue<Tuple<TValue, TValue>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PairIndicatorValue{T}"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		/// <param name="value">Value.</param>
		public PairIndicatorValue(IIndicator indicator, Tuple<TValue, TValue> value)
			: base(indicator, value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PairIndicatorValue{T}"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		public PairIndicatorValue(IIndicator indicator)
			: base(indicator)
		{
		}

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value)
		{
			return new PairIndicatorValue<TValue>(indicator, GetValue<Tuple<TValue, TValue>>())
			{
				IsFinal = IsFinal,
				InputValue = this
			};
		}
	}

	/// <summary>
	/// The complex value of the indicator <see cref="IComplexIndicator"/>, derived as result of calculation.
	/// </summary>
	public class ComplexIndicatorValue : BaseIndicatorValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComplexIndicatorValue"/>.
		/// </summary>
		/// <param name="indicator">Indicator.</param>
		public ComplexIndicatorValue(IIndicator indicator)
			: base(indicator)
		{
			InnerValues = new Dictionary<IIndicator, IIndicatorValue>();
		}

		/// <inheritdoc />
		public override bool IsEmpty { get; set; }

		/// <inheritdoc />
		public override bool IsFinal { get; set; }

		/// <inheritdoc />
		public override IIndicatorValue InputValue { get; set; }

		/// <summary>
		/// Embedded values.
		/// </summary>
		public IDictionary<IIndicator, IIndicatorValue> InnerValues { get; }

		/// <inheritdoc />
		public override bool IsSupport(Type valueType) => InnerValues.Any(v => v.Value.IsSupport(valueType));

		/// <inheritdoc />
		public override T GetValue<T>() => throw new NotSupportedException();

		/// <inheritdoc />
		public override IIndicatorValue SetValue<T>(IIndicator indicator, T value) => throw new NotSupportedException();

		/// <inheritdoc />
		public override int CompareTo(IIndicatorValue other) => throw new NotSupportedException();
	}
}