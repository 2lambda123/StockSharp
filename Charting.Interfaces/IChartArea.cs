﻿namespace StockSharp.Charting
{
	using Ecng.Collections;

	/// <summary>
	/// Chart area.
	/// </summary>
	public interface IChartArea : IChartPart<IChartArea>
	{
		/// <summary>
		/// To use automatic range for the X-axis. The default is off.
		/// </summary>
		bool IsAutoRange { get; set; }

		/// <summary>
		/// Area elements (<see cref="IChartIndicatorElement"/>, <see cref="IChartCandleElement"/>, etc.).
		/// </summary>
		INotifyList<IChartElement> Elements { get; }

		/// <summary>
		/// The list of horizontal axes.
		/// </summary>
		INotifyList<IChartAxis> XAxises { get; }

		/// <summary>
		/// The list of vertical axes.
		/// </summary>
		INotifyList<IChartAxis> YAxises { get; }

		/// <summary>
		/// Type of X axis for this chart.
		/// </summary>
		ChartAxisType XAxisType { get; set; }

		/// <summary>
		/// Chart area name.
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// The height of the area.
		/// </summary>
		double Height { get; set; }

		/// <summary>
		/// Chart.
		/// </summary>
		IChart Chart { get; }
	}
}