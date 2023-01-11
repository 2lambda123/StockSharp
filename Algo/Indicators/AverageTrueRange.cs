#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Indicators.Algo
File: AverageTrueRange.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Indicators
{
	using System;
	using System.ComponentModel;

	using StockSharp.Localization;

	/// <summary>
	/// The average true range <see cref="Indicators.TrueRange"/>.
	/// </summary>
	[DisplayName("ATR")]
	[DescriptionLoc(LocalizedStrings.Str758Key)]
	public class AverageTrueRange : LengthIndicator<IIndicatorValue>
	{
		private bool _isFormed;

		/// <summary>
		/// Initializes a new instance of the <see cref="AverageTrueRange"/>.
		/// </summary>
		public AverageTrueRange()
			: this(new WilderMovingAverage(), new TrueRange())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AverageTrueRange"/>.
		/// </summary>
		/// <param name="movingAverage">Moving Average.</param>
		/// <param name="trueRange">True range.</param>
		public AverageTrueRange(LengthIndicator<decimal> movingAverage, TrueRange trueRange)
		{
			MovingAverage = movingAverage ?? throw new ArgumentNullException(nameof(movingAverage));
			TrueRange = trueRange ?? throw new ArgumentNullException(nameof(trueRange));
		}

		/// <inheritdoc />
		public override IndicatorMeasures Measure => IndicatorMeasures.MunisOnePlusOne;

		/// <summary>
		/// Moving Average.
		/// </summary>
		[Browsable(false)]
		public LengthIndicator<decimal> MovingAverage { get; }

		/// <summary>
		/// True range.
		/// </summary>
		[Browsable(false)]
		public TrueRange TrueRange { get; }

		/// <inheritdoc />
		public override bool IsFormed => _isFormed;

		/// <inheritdoc />
		public override void Reset()
		{
			base.Reset();

			_isFormed = false;

			MovingAverage.Length = Length;
			TrueRange.Reset();
		}

		/// <inheritdoc />
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			// используем дополнительную переменную IsFormed, 
			// т.к. нужна задержка в один период для корректной инициализации скользящей средней
			_isFormed = MovingAverage.IsFormed;

			return MovingAverage.Process(TrueRange.Process(input));
		}
	}
}