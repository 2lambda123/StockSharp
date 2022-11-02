#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Messages.Messages
File: CandleMessage.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Messages
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	using Ecng.Common;

	using StockSharp.Localization;

	/// <summary>
	/// Candle states.
	/// </summary>
	[DataContract]
	[Serializable]
	public enum CandleStates
	{
		/// <summary>
		/// Empty state (candle doesn't exist).
		/// </summary>
		[EnumMember]
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.Str1658Key)]
		None,

		/// <summary>
		/// Candle active.
		/// </summary>
		[EnumMember]
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.Str238Key)]
		Active,

		/// <summary>
		/// Candle finished.
		/// </summary>
		[EnumMember]
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.FinishedKey)]
		Finished,
	}

	/// <summary>
	/// The message contains information about the candle.
	/// </summary>
	[DataContract]
	[Serializable]
	public abstract class CandleMessage : Message,
		ISubscriptionIdMessage, IServerTimeMessage, ISecurityIdMessage, IGeneratedMessage, ISeqNumMessage
	{
		/// <inheritdoc />
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.SecurityIdKey)]
		[DescriptionLoc(LocalizedStrings.SecurityIdKey, true)]
		[MainCategory]
		public SecurityId SecurityId { get; set; }

		/// <summary>
		/// Open time.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.CandleOpenTimeKey)]
		[DescriptionLoc(LocalizedStrings.CandleOpenTimeKey, true)]
		[MainCategory]
		public DateTimeOffset OpenTime { get; set; }

		/// <summary>
		/// Time of candle high.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.CandleHighTimeKey)]
		[DescriptionLoc(LocalizedStrings.CandleHighTimeKey, true)]
		[MainCategory]
		public DateTimeOffset HighTime { get; set; }

		/// <summary>
		/// Time of candle low.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.CandleLowTimeKey)]
		[DescriptionLoc(LocalizedStrings.CandleLowTimeKey, true)]
		[MainCategory]
		public DateTimeOffset LowTime { get; set; }

		/// <summary>
		/// Close time.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.CandleCloseTimeKey)]
		[DescriptionLoc(LocalizedStrings.CandleCloseTimeKey, true)]
		[MainCategory]
		public DateTimeOffset CloseTime { get; set; }

		/// <summary>
		/// Opening price.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.Str79Key)]
		[DescriptionLoc(LocalizedStrings.Str80Key)]
		[MainCategory]
		public decimal OpenPrice { get; set; }

		/// <summary>
		/// Highest price.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.HighestPriceKey)]
		[DescriptionLoc(LocalizedStrings.Str82Key)]
		[MainCategory]
		public decimal HighPrice { get; set; }

		/// <summary>
		/// Lowest price.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.LowestPriceKey)]
		[DescriptionLoc(LocalizedStrings.Str84Key)]
		[MainCategory]
		public decimal LowPrice { get; set; }

		/// <summary>
		/// Closing price.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.ClosingPriceKey)]
		[DescriptionLoc(LocalizedStrings.Str86Key)]
		[MainCategory]
		public decimal ClosePrice { get; set; }

		/// <summary>
		/// Volume at open.
		/// </summary>
		[DataMember]
		//[Nullable]
		public decimal? OpenVolume { get; set; }

		/// <summary>
		/// Volume at close.
		/// </summary>
		[DataMember]
		//[Nullable]
		public decimal? CloseVolume { get; set; }

		/// <summary>
		/// Volume at high.
		/// </summary>
		[DataMember]
		//[Nullable]
		public decimal? HighVolume { get; set; }

		/// <summary>
		/// Volume at low.
		/// </summary>
		[DataMember]
		//[Nullable]
		public decimal? LowVolume { get; set; }

		/// <summary>
		/// Relative volume.
		/// </summary>
		[DataMember]
		public decimal? RelativeVolume { get; set; }

		/// <summary>
		/// Total price size.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.TotalPriceKey)]
		public decimal TotalPrice { get; set; }

		/// <summary>
		/// Total volume.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.VolumeKey)]
		[DescriptionLoc(LocalizedStrings.TotalCandleVolumeKey)]
		[MainCategory]
		public decimal TotalVolume { get; set; }

		/// <summary>
		/// Buy volume.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.XamlStr493Key)]
		[MainCategory]
		public decimal? BuyVolume { get; set; }

		/// <summary>
		/// Sell volume.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.XamlStr579Key)]
		[MainCategory]
		public decimal? SellVolume { get; set; }

		/// <summary>
		/// Open interest.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.OIKey)]
		[DescriptionLoc(LocalizedStrings.OpenInterestKey)]
		[MainCategory]
		public decimal? OpenInterest { get; set; }

		/// <summary>
		/// Number of ticks.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.TicksKey)]
		[DescriptionLoc(LocalizedStrings.TickCountKey)]
		[MainCategory]
		public int? TotalTicks { get; set; }

		/// <summary>
		/// Number of up trending ticks.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.TickUpKey)]
		[DescriptionLoc(LocalizedStrings.TickUpCountKey)]
		[MainCategory]
		public int? UpTicks { get; set; }

		/// <summary>
		/// Number of down trending ticks.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.TickDownKey)]
		[DescriptionLoc(LocalizedStrings.TickDownCountKey)]
		[MainCategory]
		public int? DownTicks { get; set; }

		/// <summary>
		/// State.
		/// </summary>
		[DataMember]
		[DisplayNameLoc(LocalizedStrings.StateKey)]
		[DescriptionLoc(LocalizedStrings.CandleStateKey, true)]
		[MainCategory]
		public CandleStates State { get; set; }

		/// <summary>
		/// Price levels.
		/// </summary>
		[DataMember]
		[XmlIgnore]
		public IEnumerable<CandlePriceLevel> PriceLevels { get; set; }

		/// <summary>
		/// Candle arg.
		/// </summary>
		public abstract object Arg { get; set; }

		DataType ISubscriptionIdMessage.DataType => DataType.Create(GetType(), Arg);

		/// <summary>
		/// Initialize <see cref="CandleMessage"/>.
		/// </summary>
		/// <param name="type">Message type.</param>
		protected CandleMessage(MessageTypes type)
			: base(type)
		{
		}

		/// <summary>
		/// Clone <see cref="Arg"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public virtual object CloneArg() => Arg;

		/// <inheritdoc />
		[DataMember]
		public long OriginalTransactionId { get; set; }

		/// <inheritdoc />
		[XmlIgnore]
		public long SubscriptionId { get; set; }

		/// <inheritdoc />
		[XmlIgnore]
		public long[] SubscriptionIds { get; set; }

		/// <inheritdoc />
		[DataMember]
		public DataType BuildFrom { get; set; }

		/// <inheritdoc />
		[DataMember]
		public long SeqNum { get; set; }

		/// <summary>
		/// Copy parameters.
		/// </summary>
		/// <param name="copy">Copy.</param>
		/// <returns>Copy.</returns>
		protected CandleMessage CopyTo(CandleMessage copy)
		{
			base.CopyTo(copy);

			copy.OriginalTransactionId = OriginalTransactionId;
			copy.SubscriptionId = SubscriptionId;
			copy.SubscriptionIds = SubscriptionIds;//?.ToArray();

			copy.OpenPrice = OpenPrice;
			copy.OpenTime = OpenTime;
			copy.OpenVolume = OpenVolume;
			copy.ClosePrice = ClosePrice;
			copy.CloseTime = CloseTime;
			copy.CloseVolume = CloseVolume;
			copy.HighPrice = HighPrice;
			copy.HighVolume = HighVolume;
			copy.HighTime = HighTime;
			copy.LowPrice = LowPrice;
			copy.LowVolume = LowVolume;
			copy.LowTime = LowTime;
			copy.OpenInterest = OpenInterest;
			copy.SecurityId = SecurityId;
			copy.TotalVolume = TotalVolume;
			copy.BuyVolume = BuyVolume;
			copy.SellVolume = SellVolume;
			copy.RelativeVolume = RelativeVolume;
			copy.DownTicks = DownTicks;
			copy.UpTicks = UpTicks;
			copy.TotalTicks = TotalTicks;
			copy.PriceLevels = PriceLevels?/*.Select(l => l.Clone())*/.ToArray();
			copy.State = State;
			copy.BuildFrom = BuildFrom;
			copy.SeqNum = SeqNum;

			return copy;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var str = $"{Type},Sec={SecurityId},A={Arg},T={OpenTime:yyyy/MM/dd HH:mm:ss.fff},O={OpenPrice},H={HighPrice},L={LowPrice},C={ClosePrice},V={TotalVolume},S={State},TransId={OriginalTransactionId}";

			if (SeqNum != default)
				str += $",SQ={SeqNum}";

			return str;
		}

		DateTimeOffset IServerTimeMessage.ServerTime
		{
			get => OpenTime;
			set => OpenTime = value;
		}
	}

	/// <summary>
	/// The message contains information about the time-frame candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.TimeFrameCandleKey)]
	public class TimeFrameCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeFrameCandleMessage"/>.
		/// </summary>
		public TimeFrameCandleMessage()
			: this(MessageTypes.CandleTimeFrame)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeFrameCandleMessage"/>.
		/// </summary>
		/// <param name="type">Message type.</param>
		protected TimeFrameCandleMessage(MessageTypes type)
			: base(type)
		{
		}

		/// <summary>
		/// Time-frame.
		/// </summary>
		[DataMember]
		public TimeSpan TimeFrame { get; set; }

		/// <summary>
		/// Create a copy of <see cref="TimeFrameCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new TimeFrameCandleMessage
			{
				TimeFrame = TimeFrame
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => TimeFrame;
			set => TimeFrame = (TimeSpan)value;
		}
	}

	/// <summary>
	/// The message contains information about the tick candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.TickCandleKey)]
	public class TickCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TickCandleMessage"/>.
		/// </summary>
		public TickCandleMessage()
			: base(MessageTypes.CandleTick)
		{
		}

		/// <summary>
		/// Maximum tick count.
		/// </summary>
		[DataMember]
		public int MaxTradeCount { get; set; }

		/// <summary>
		/// Create a copy of <see cref="TickCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new TickCandleMessage
			{
				MaxTradeCount = MaxTradeCount
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => MaxTradeCount;
			set => MaxTradeCount = (int)value;
		}
	}

	/// <summary>
	/// The message contains information about the volume candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.VolumeCandleKey)]
	public class VolumeCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VolumeCandleMessage"/>.
		/// </summary>
		public VolumeCandleMessage()
			: base(MessageTypes.CandleVolume)
		{
		}

		/// <summary>
		/// Maximum volume.
		/// </summary>
		[DataMember]
		public decimal Volume { get; set; }

		/// <summary>
		/// Create a copy of <see cref="VolumeCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new VolumeCandleMessage
			{
				Volume = Volume
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => Volume;
			set => Volume = (decimal)value;
		}
	}

	/// <summary>
	/// The message contains information about the range candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.RangeCandleKey)]
	public class RangeCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RangeCandleMessage"/>.
		/// </summary>
		public RangeCandleMessage()
			: base(MessageTypes.CandleRange)
		{
		}

		private Unit _priceRange = new();

		/// <summary>
		/// Range of price.
		/// </summary>
		[DataMember]
		public Unit PriceRange
		{
			get => _priceRange;
			set => _priceRange = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Create a copy of <see cref="RangeCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new RangeCandleMessage
			{
				PriceRange = PriceRange.Clone()
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => PriceRange;
			set => PriceRange = (Unit)value;
		}

		/// <inheritdoc />
		public override object CloneArg() => PriceRange.Clone();
	}

	/// <summary>
	/// Point in figure (X0) candle arg.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PnFArg : Equatable<PnFArg>
	{
		private Unit _boxSize = new();

		/// <summary>
		/// Range of price above which increase the candle body.
		/// </summary>
		[DataMember]
		public Unit BoxSize
		{
			get => _boxSize;
			set => _boxSize = value ?? throw new ArgumentNullException(nameof(value));
		}

		private int _reversalAmount = 1;

		/// <summary>
		/// The number of boxes required to cause a reversal.
		/// </summary>
		[DataMember]
		public int ReversalAmount
		{
			get => _reversalAmount;
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException(nameof(value), value, LocalizedStrings.Str1219);

				_reversalAmount = value;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"Box = {BoxSize} RA = {ReversalAmount}";
		}

		/// <summary>
		/// Create a copy of <see cref="PnFArg"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override PnFArg Clone()
		{
			return new PnFArg
			{
				BoxSize = BoxSize.Clone(),
				ReversalAmount = ReversalAmount,
			};
		}

		/// <summary>
		/// Compare <see cref="PnFArg"/> on the equivalence.
		/// </summary>
		/// <param name="other">Another value with which to compare.</param>
		/// <returns><see langword="true" />, if the specified object is equal to the current object, otherwise, <see langword="false" />.</returns>
		protected override bool OnEquals(PnFArg other)
		{
			return other.BoxSize == BoxSize && other.ReversalAmount == ReversalAmount;
		}

		/// <summary>
		/// Get the hash code of the object <see cref="PnFArg"/>.
		/// </summary>
		/// <returns>A hash code.</returns>
		public override int GetHashCode()
		{
			return BoxSize.GetHashCode() ^ ReversalAmount.GetHashCode();
		}
	}

	/// <summary>
	/// The message contains information about the X0 candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.PnFCandleKey)]
	public class PnFCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PnFCandleMessage"/>.
		/// </summary>
		public PnFCandleMessage()
			: base(MessageTypes.CandlePnF)
		{
		}

		private PnFArg _pnFArg = new();

		/// <summary>
		/// Value of arguments.
		/// </summary>
		[DataMember]
		public PnFArg PnFArg
		{
			get => _pnFArg;
			set => _pnFArg = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Create a copy of <see cref="PnFCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new PnFCandleMessage
			{
				PnFArg = PnFArg.Clone(),
				//PnFType = PnFType
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => PnFArg;
			set => PnFArg = (PnFArg)value;
		}

		/// <inheritdoc />
		public override object CloneArg() => PnFArg.Clone();
	}

	/// <summary>
	/// The message contains information about the renko candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.RenkoCandleKey)]
	public class RenkoCandleMessage : CandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RenkoCandleMessage"/>.
		/// </summary>
		public RenkoCandleMessage()
			: base(MessageTypes.CandleRenko)
		{
		}

		private Unit _boxSize = new();

		/// <summary>
		/// Possible price change range.
		/// </summary>
		[DataMember]
		public Unit BoxSize
		{
			get => _boxSize;
			set => _boxSize = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Create a copy of <see cref="RenkoCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new RenkoCandleMessage
			{
				BoxSize = BoxSize.Clone()
			});
		}

		/// <inheritdoc />
		public override object Arg
		{
			get => BoxSize;
			set => BoxSize = (Unit)value;
		}

		/// <inheritdoc />
		public override object CloneArg() => BoxSize.Clone();
	}

	/// <summary>
	/// The message contains information about the Heikin-Ashi candle.
	/// </summary>
	[DataContract]
	[Serializable]
	[DisplayNameLoc(LocalizedStrings.HeikinAshiKey)]
	public class HeikinAshiCandleMessage : TimeFrameCandleMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HeikinAshiCandleMessage"/>.
		/// </summary>
		public HeikinAshiCandleMessage()
			: base(MessageTypes.CandleHeikinAshi)
		{
		}

		/// <summary>
		/// Create a copy of <see cref="HeikinAshiCandleMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			return CopyTo(new HeikinAshiCandleMessage
			{
				TimeFrame = TimeFrame
			});
		}
	}
}
