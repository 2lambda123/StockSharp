#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: Lowest.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System;
	using System.ComponentModel;
	using System.Linq;

	using Ecng.ComponentModel;

	using StockSharp.Algo.Candles;
	using StockSharp.Localization;

	/// <summary>
	/// Minimum value for a period.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorLowest.html
	/// </remarks>
	[DisplayName("Lowest")]
	[DescriptionLoc(LocalizedStrings.Str743Key)]
	[Doc("topics/IndicatorLowest.html")]
	public class Lowest : LengthIndicator<decimal>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Lowest"/>.
		/// </summary>
		public Lowest()
		{
			Length = 5;
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var newValue = input.IsSupport(typeof(Candle)) ? input.GetValue<Candle>().LowPrice : input.GetValue<decimal>();
			var lastValue = Buffer.Count == 0 ? newValue : this.GetCurrentValue();

			if (newValue < lastValue)
			{
				// Новое значение и есть экстремум 
				lastValue = newValue;
			}

			// добавляем новое начало
			if (input.IsFinal)
			{
				var recalc = Buffer.Count == Length && Buffer[0] == lastValue && lastValue != newValue;
				Buffer.PushBack(newValue);

				// ищем новый экстремум
				if (recalc)
					lastValue = Buffer.Aggregate(newValue, (current, t) => Math.Min(t, current));
			}

			return new DecimalIndicatorValue(this, lastValue);
		}
	}
}