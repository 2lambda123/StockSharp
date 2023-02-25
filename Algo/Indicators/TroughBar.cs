#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: TroughBar.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System;
	using System.ComponentModel;

	using Ecng.Serialization;
	using Ecng.ComponentModel;

	using StockSharp.Localization;
	using StockSharp.Messages;

	/// <summary>
	/// TroughBar.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorTroughBar.html
	/// </remarks>
	[DisplayName("TroughBar")]
	[DescriptionLoc(LocalizedStrings.Str822Key)]
	[IndicatorIn(typeof(CandleIndicatorValue))]
	[Doc("topics/IndicatorTroughBar.html")]
	public class TroughBar : BaseIndicator
	{
		private decimal _currentMinimum = decimal.MaxValue;
		private int _currentBarCount;
		private int _valueBarCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="TroughBar"/>.
		/// </summary>
		public TroughBar()
		{
		}

		private Unit _reversalAmount = new();

		/// <summary>
		/// Indicator changes threshold.
		/// </summary>
		[DisplayNameLoc(LocalizedStrings.Str783Key)]
		[DescriptionLoc(LocalizedStrings.Str784Key)]
		[CategoryLoc(LocalizedStrings.GeneralKey)]
		public Unit ReversalAmount
		{
			get => _reversalAmount;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				_reversalAmount = value;

				Reset();
			}
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var candle = input.GetValue<ICandleMessage>();
			var cm = _currentMinimum;
			var vbc = _valueBarCount;

			try
			{
				if (candle.LowPrice < cm)
				{
					cm = candle.LowPrice;
					vbc = _currentBarCount;
				}
				else if (candle.HighPrice >= cm + ReversalAmount.Value)
				{
					if (input.IsFinal)
						IsFormed = true;

					return new DecimalIndicatorValue(this, vbc);
				}

				return new DecimalIndicatorValue(this, this.GetCurrentValue());
			}
			finally
			{
				if(input.IsFinal)
				{
					_currentBarCount++;
					_currentMinimum = cm;
					_valueBarCount = vbc;
				}
			}
		}

		/// <inheritdoc />
		public override void Load(SettingsStorage storage)
		{
			base.Load(storage);

			ReversalAmount.Load(storage.GetValue<SettingsStorage>(nameof(ReversalAmount)));
		}

		/// <inheritdoc />
		public override void Save(SettingsStorage storage)
		{
			base.Save(storage);

			storage.SetValue(nameof(ReversalAmount), ReversalAmount.Save());
		}
	}
}