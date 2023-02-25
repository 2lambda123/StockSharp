﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: DiPart.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using StockSharp.Messages;

	/// <summary>
	/// The part of the indicator <see cref="DirectionalIndex"/>.
	/// </summary>
	[IndicatorIn(typeof(CandleIndicatorValue))]
	public abstract class DiPart : LengthIndicator<decimal>
	{
		private readonly AverageTrueRange _averageTrueRange;
		private readonly LengthIndicator<decimal> _movingAverage;
		private ICandleMessage _lastCandle;

		/// <summary>
		/// Initialize <see cref="DiPart"/>.
		/// </summary>
		protected DiPart()
		{
			_averageTrueRange = new AverageTrueRange(new WilderMovingAverage(), new TrueRange());
			_movingAverage = new WilderMovingAverage();

			Length = 5;
		}

		/// <inheritdoc />
		public override void Reset()
		{
			base.Reset();

			_averageTrueRange.Length = Length;
			_movingAverage.Length = Length;

			_lastCandle = null;
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			decimal? result = null;

			var candle = input.GetValue<ICandleMessage>();

			// 1 period delay
			if (_averageTrueRange.IsFormed && _movingAverage.IsFormed)
				IsFormed = true;

			_averageTrueRange.Process(input);

			if (_lastCandle != null)
			{
				var trValue = _averageTrueRange.GetCurrentValue();

				var maValue = _movingAverage.Process(new DecimalIndicatorValue(this, GetValue(candle, _lastCandle)) { IsFinal = input.IsFinal });

				if (!maValue.IsEmpty)
					result = trValue != 0m ? 100m * maValue.GetValue<decimal>() / trValue : 0m;
			}

			if (input.IsFinal)
				_lastCandle = candle;

			return result == null ? new DecimalIndicatorValue(this) : new DecimalIndicatorValue(this, result.Value);
		}

		/// <summary>
		/// To get the part value.
		/// </summary>
		/// <param name="current">The current candle.</param>
		/// <param name="prev">The previous candle.</param>
		/// <returns>Value.</returns>
		protected abstract decimal GetValue(ICandleMessage current, ICandleMessage prev);
	}
}