#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: QStick.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using Ecng.ComponentModel;

	using StockSharp.Algo.Candles;

	/// <summary>
	/// QStick.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorQStick.html
	/// </remarks>
	[IndicatorIn(typeof(CandleIndicatorValue))]
	[Doc("topics/IndicatorQStick.html")]
	public class QStick : LengthIndicator<IIndicatorValue>
	{
		private readonly SimpleMovingAverage _sma;

		/// <summary>
		/// Initializes a new instance of the <see cref="QStick"/>.
		/// </summary>
		public QStick()
		{
			_sma = new SimpleMovingAverage();
			Length = 15;
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.MunisOnePlusOne;

		/// <inheritdoc />
		protected override bool CalcIsFormed() => _sma.IsFormed;

		/// <inheritdoc />
		public override void Reset()
		{
			_sma.Length = Length;
			base.Reset();
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var candle = input.GetValue<Candle>();
			return _sma.Process(input.SetValue(this, candle.OpenPrice - candle.ClosePrice));
		}
	}
}