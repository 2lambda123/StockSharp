#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Strategies.Analytics.Algo
File: BaseAnalyticsStrategy.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Strategies.Analytics
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	using StockSharp.Algo.Storages;
	using StockSharp.BusinessEntities;
	using StockSharp.Localization;

	/// <summary>
	/// Types of result.
	/// </summary>
	public enum AnalyticsResultTypes
	{
		/// <summary>
		/// Table.
		/// </summary>
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.Str3280Key)]
		Grid,

		/// <summary>
		/// Bubble chart.
		/// </summary>
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.Str1977Key)]
		Bubble,

		/// <summary>
		/// Histogram.
		/// </summary>
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.Str1976Key)]
		Histogram,

		/// <summary>
		/// Heatmap.
		/// </summary>
		[Display(ResourceType = typeof(LocalizedStrings), Name = LocalizedStrings.HeatmapKey)]
		Heatmap,
	}

	/// <summary>
	/// The base analytic strategy.
	/// </summary>
	public abstract class BaseAnalyticsStrategy : Strategy
	{
		private readonly StrategyParam<DateTime> _from;

		/// <summary>
		/// Start date.
		/// </summary>
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str343Key,
			Description = LocalizedStrings.Str1222Key,
			GroupName = LocalizedStrings.AnalyticsKey,
			Order = 0)]
		[Browsable(false)]
		public DateTime From
		{
			get => _from.Value;
			set => _from.Value = value;
		}

		private readonly StrategyParam<DateTime> _to;

		/// <summary>
		/// End date.
		/// </summary>
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str345Key,
			Description = LocalizedStrings.Str345Key + LocalizedStrings.Dot,
			GroupName = LocalizedStrings.AnalyticsKey,
			Order = 1)]
		[Browsable(false)]
		public DateTime To
		{
			get => _to.Value;
			set => _to.Value = value;
		}

		private readonly StrategyParam<AnalyticsResultTypes> _resultType;

		/// <summary>
		/// Result type.
		/// </summary>
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str1738Key,
			Description = LocalizedStrings.ResultTypeKey + LocalizedStrings.Dot,
			GroupName = LocalizedStrings.AnalyticsKey,
			Order = 2)]
		[Browsable(false)]
		public AnalyticsResultTypes ResultType
		{
			get => _resultType.Value;
			set => _resultType.Value = value;
		}

		/// <summary>
		/// Market-data storage.
		/// </summary>
		[Browsable(false)]
		public IStorageRegistry StorateRegistry { get; set; }

		/// <inheritdoc />
		[Browsable(false)]
		public override Portfolio Portfolio
		{
			get => base.Portfolio;
			set => base.Portfolio = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override decimal Volume
		{
			get => base.Volume;
			set => base.Volume = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override StrategyCommentModes CommentMode
		{
			get => base.CommentMode;
			set => base.CommentMode = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override bool UnsubscribeOnStop
		{
			get => base.UnsubscribeOnStop;
			set => base.UnsubscribeOnStop = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override int MaxOrderRegisterErrorCount
		{
			get => base.MaxOrderRegisterErrorCount;
			set => base.MaxOrderRegisterErrorCount = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override bool WaitRulesOnStop
		{
			get => base.WaitRulesOnStop;
			set => base.WaitRulesOnStop = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override int MaxRegisterCount
		{
			get => base.MaxRegisterCount;
			set => base.MaxRegisterCount = value;
		}

		/// <inheritdoc />
		[Browsable(false)]
		public override TimeSpan RegisterInterval
		{
			get => base.RegisterInterval;
			set => base.RegisterInterval = value;
		}

		/// <summary>
		/// Initialize <see cref="BaseAnalyticsStrategy"/>.
		/// </summary>
		protected BaseAnalyticsStrategy()
		{
			_from = this.Param<DateTime>(nameof(From));
			_to = this.Param(nameof(To), DateTime.MaxValue);
			_resultType = this.Param(nameof(ResultType), AnalyticsResultTypes.Bubble);
		}

		/// <inheritdoc />
		protected override void ProcessCancelActiveOrders()
		{
		}

		/// <summary>
		/// Result panel.
		/// </summary>
		protected IAnalyticsPanel Panel => Environment.GetValue<IAnalyticsPanel>(nameof(Panel));

		/// <summary>
		/// Data format.
		/// </summary>
		protected StorageFormats StorageFormat => Environment.GetValue<StorageFormats>(nameof(StorageFormat));

		/// <inheritdoc />
		protected override void OnStarted()
		{
			InitStartValues();

			OnAnalyze();
		}

		/// <summary>
		/// To analyze.
		/// </summary>
		protected abstract void OnAnalyze();
	}
}