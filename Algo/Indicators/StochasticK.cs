﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: StochasticK.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;

	using Ecng.ComponentModel;

	using StockSharp.Algo.Candles;
	using StockSharp.Localization;

	/// <summary>
	/// Stochastic %K.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorStochasticK.html
	/// </remarks>
	[DisplayName("Stochastic %K")]
	[DescriptionLoc(LocalizedStrings.Str774Key)]
	[IndicatorIn(typeof(CandleIndicatorValue))]
	[Doc("topics/IndicatorStochasticK.html")]
	public class StochasticK : LengthIndicator<decimal>
	{
		// Минимальная цена за период.
		private readonly Lowest _low = new();

		// Максимальная цена за период.
		private readonly Highest _high = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="StochasticK"/>.
		/// </summary>
		public StochasticK()
		{
			Length = 14;
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.Persent;

		/// <inheritdoc />
		public override bool IsFormed => _high.IsFormed;

		/// <inheritdoc />
		public override void Reset()
		{
			_high.Length = _low.Length = Length;
			base.Reset();
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var candle = input.GetValue<Candle>();

			var highValue = _high.Process(input.SetValue(this, candle.HighPrice)).GetValue<decimal>();
			var lowValue = _low.Process(input.SetValue(this, candle.LowPrice)).GetValue<decimal>();

			var diff = highValue - lowValue;

			if (diff == 0)
				return new DecimalIndicatorValue(this, 0);

			return new DecimalIndicatorValue(this, 100 * (candle.ClosePrice - lowValue) / diff);
		}
	}
}