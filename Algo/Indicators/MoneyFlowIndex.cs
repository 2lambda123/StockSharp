#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: MoneyFlowIndex.cs
Created: 2015, 11, 23, 2:02 AM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;

	using Ecng.ComponentModel;

	using StockSharp.Messages;
	using StockSharp.Localization;

	/// <summary>
	/// Money Flow Index.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorMoneyFlowIndex.html
	/// </remarks>
	[DisplayName("MFI")]
	[DescriptionLoc(LocalizedStrings.MoneyFlowIndexKey)]
	[IndicatorIn(typeof(CandleIndicatorValue))]
	[Doc("topics/IndicatorMoneyFlowIndex.html")]
	public class MoneyFlowIndex : LengthIndicator<decimal>
	{
		private decimal _previousPrice;
		private readonly Sum _positiveFlow = new();
		private readonly Sum _negativeFlow = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="MoneyFlowIndex"/>.
		/// </summary>
		public MoneyFlowIndex()
		{
		    Length = 14;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MoneyFlowIndex"/>.
		/// </summary>
		/// <param name="length">Period length.</param>
		public MoneyFlowIndex(int length)
		{
		    Length = length;
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.Persent;

		/// <inheritdoc />
		public override void Reset()
		{
			base.Reset();

			_positiveFlow.Length = _negativeFlow.Length = Length;
			_previousPrice = 0;
		}

		/// <inheritdoc />
		protected override bool CalcIsFormed() => _positiveFlow.IsFormed && _negativeFlow.IsFormed;

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var candle = input.GetValue<ICandleMessage>();

			var typicalPrice = (candle.HighPrice + candle.LowPrice + candle.ClosePrice) / 3.0m;
			var moneyFlow = typicalPrice * candle.TotalVolume;
			
			var positiveFlow = _positiveFlow.Process(input.SetValue(this, typicalPrice > _previousPrice ? moneyFlow : 0.0m)).GetValue<decimal>();
			var negativeFlow = _negativeFlow.Process(input.SetValue(this, typicalPrice < _previousPrice ? moneyFlow : 0.0m)).GetValue<decimal>();

			_previousPrice = typicalPrice;
			
			if (negativeFlow == 0)
				return new DecimalIndicatorValue(this, 100m);
			
			if (positiveFlow / negativeFlow == 1)
				return new DecimalIndicatorValue(this, 0m);

			return negativeFlow != 0 
				? new DecimalIndicatorValue(this, 100m - 100m / (1m + positiveFlow / negativeFlow))
				: new DecimalIndicatorValue(this);
		}
	}
}
