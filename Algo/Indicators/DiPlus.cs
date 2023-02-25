#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: DiPlus.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;

	using StockSharp.Messages;

	/// <summary>
	/// DIPlus is a component of the Directional Movement System developed by Welles Wilder.
	/// </summary>
	[Browsable(false)]
	public class DiPlus : DiPart
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DiPlus"/>.
		/// </summary>
		public DiPlus()
		{
		}

		/// <inheritdoc />
		protected override decimal GetValue(ICandleMessage current, ICandleMessage prev)
		{
			if (current.HighPrice > prev.HighPrice && current.HighPrice - prev.HighPrice > prev.LowPrice - current.LowPrice)
				return current.HighPrice - prev.HighPrice;
			else
				return 0;
		}
	}
}