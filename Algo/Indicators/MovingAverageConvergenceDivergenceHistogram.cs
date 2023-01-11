#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: MovingAverageConvergenceDivergenceHistogram.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;

	using Ecng.ComponentModel;

	using StockSharp.Localization;

	/// <summary>
	/// Convergence/divergence of moving averages. Histogram.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorMovingAverageConvergenceDivergenceHistogram.html
	/// </remarks>
	[DisplayName("MACD Histogram")]
	[DescriptionLoc(LocalizedStrings.Str802Key)]
	[Doc("topics/IndicatorMovingAverageConvergenceDivergenceHistogram.html")]
	public class MovingAverageConvergenceDivergenceHistogram : MovingAverageConvergenceDivergenceSignal
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverageConvergenceDivergenceHistogram"/>.
		/// </summary>
		public MovingAverageConvergenceDivergenceHistogram()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverageConvergenceDivergenceHistogram"/>.
		/// </summary>
		/// <param name="macd">Convergence/divergence of moving averages.</param>
		/// <param name="signalMa">Signaling Moving Average.</param>
		public MovingAverageConvergenceDivergenceHistogram(MovingAverageConvergenceDivergence macd, ExponentialMovingAverage signalMa)
			: base(macd, signalMa)
		{
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.MunisOnePlusOne;

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var macdValue = Macd.Process(input);
			var signalValue = Macd.IsFormed ? SignalMa.Process(macdValue) : new DecimalIndicatorValue(SignalMa, 0);

			var value = new ComplexIndicatorValue(this);
			//value.InnerValues.Add(Macd, input.SetValue(this, macdValue.GetValue<decimal>() - signalValue.GetValue<decimal>()));
			value.InnerValues.Add(Macd, macdValue);
			value.InnerValues.Add(SignalMa, signalValue);
			return value;
		}
	}
}