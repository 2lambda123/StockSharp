﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: KaufmannAdaptiveMovingAverage.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;
	using System.Linq;
	using System;

	using Ecng.Serialization;
	using Ecng.ComponentModel;

	using StockSharp.Localization;
	using System.Collections.Generic;

	/// <summary>
	/// Kaufman adaptive moving average.
	/// </summary>
	/// <remarks>
	/// https://doc.stocksharp.com/topics/IndicatorKaufmannAdaptiveMovingAverage.html
	/// </remarks>
	[DisplayName("KAMA")]
	[DescriptionLoc(LocalizedStrings.Str792Key)]
	[Doc("topics/IndicatorKaufmannAdaptiveMovingAverage.html")]
	public class KaufmannAdaptiveMovingAverage : LengthIndicator<decimal>
	{
		private decimal _prevFinalValue;
		private bool _isInitialized;

		/// <summary>
		/// Initializes a new instance of the <see cref="KaufmannAdaptiveMovingAverage"/>.
		/// </summary>
		public KaufmannAdaptiveMovingAverage()
		{
			FastSCPeriod = 2;
			SlowSCPeriod = 30;
		}

		/// <summary>
		/// 'Rapid' EMA period. The default value is 2.
		/// </summary>
		[DisplayNameLoc(LocalizedStrings.Str793Key)]
		[DescriptionLoc(LocalizedStrings.Str794Key)]
		[CategoryLoc(LocalizedStrings.GeneralKey)]
		public int FastSCPeriod { get; set; }

		/// <summary>
		/// 'Slow' EMA period. The default value is 30.
		/// </summary>
		[DisplayNameLoc(LocalizedStrings.Str795Key)]
		[DescriptionLoc(LocalizedStrings.Str796Key)]
		[CategoryLoc(LocalizedStrings.GeneralKey)]
		public int SlowSCPeriod { get; set; }

		/// <inheritdoc />
		public override bool IsFormed => Buffer.Count > Length;

		/// <inheritdoc />
		public override void Reset()
		{
			_prevFinalValue = 0;
			_isInitialized = false;

			base.Reset();

			Buffer.Capacity = Length + 1;
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var newValue = input.GetValue<decimal>();
			var lastValue = this.GetCurrentValue();

			if (input.IsFinal)
				Buffer.PushBack(newValue);

			if (!IsFormed)
				return new DecimalIndicatorValue(this, lastValue);

			if (!_isInitialized && Buffer.Count == Length + 1)
			{
				_isInitialized = true;
				// Начальное значение - последнее входное значение.
				return new DecimalIndicatorValue(this, _prevFinalValue = newValue);
			}

			var buff = input.IsFinal ? Buffer : (IList<decimal>)Buffer.Skip(1).Append(newValue).ToArray();

			var direction = newValue - buff[0];

			decimal volatility = 0;

			for (var i = 1; i < buff.Count; i++)
			{
				volatility += Math.Abs(buff[i] - buff[i - 1]);
			}

			volatility = volatility > 0 ? volatility : 0.00001m;

			var er = Math.Abs(direction / volatility);

			var fastSC = 2m / (FastSCPeriod + 1m);
			var slowSC = 2m / (SlowSCPeriod + 1m);

			var ssc = er * (fastSC - slowSC) + slowSC;
			var smooth = (ssc * ssc);

			var curValue = (newValue - _prevFinalValue) * smooth + _prevFinalValue;
			if (input.IsFinal)
				_prevFinalValue = curValue;

			return new DecimalIndicatorValue(this, curValue);
		}

		/// <inheritdoc />
		public override void Load(SettingsStorage storage)
		{
			base.Load(storage);
			FastSCPeriod = storage.GetValue<int>(nameof(FastSCPeriod));
			FastSCPeriod = storage.GetValue<int>(nameof(FastSCPeriod));
		}

		/// <inheritdoc />
		public override void Save(SettingsStorage storage)
		{
			base.Save(storage);
			storage.SetValue(nameof(FastSCPeriod), FastSCPeriod);
			storage.SetValue(nameof(SlowSCPeriod), SlowSCPeriod);
		}
	}
}