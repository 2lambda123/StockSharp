#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: Sum.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;
	using System.Linq;

	using Ecng.ComponentModel;

	using StockSharp.Localization;

	/// <summary>
	/// Sum of N last values.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorSum.html
	/// </remarks>
	[DisplayName("Sum")]
	[DescriptionLoc(LocalizedStrings.Str751Key)]
	[Doc("topics/IndicatorSum.html")]
	public class Sum : LengthIndicator<decimal>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Sum"/>.
		/// </summary>
		public Sum()
		{
			Length = 15;
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.Volume;

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var newValue = input.GetValue<decimal>();

			if (input.IsFinal)
			{
				Buffer.Add(newValue);

				if (Buffer.Count > Length)
					Buffer.RemoveAt(0);
			}

			if (input.IsFinal)
			{
				return new DecimalIndicatorValue(this, Buffer.Sum());
			}
			else
			{
				return new DecimalIndicatorValue(this, (Buffer.Skip(1).Sum() + newValue));
			}
		}
	}
}