﻿namespace StockSharp.Charting
{
	using System;
	using System.Drawing;

	using StockSharp.Algo.Indicators;
	using StockSharp.BusinessEntities;
	using StockSharp.Messages;

	/// <summary>
	/// Chart drawing data.
	/// </summary>
	public interface IChartDrawData
	{
		/// <summary>
		/// Chart drawing data item.
		/// </summary>
		public interface IChartDrawDataItem
		{
			/// <summary>
			/// Put candle color data.
			/// </summary>
			/// <param name="element">The chart element representing a candle.</param>
			/// <param name="color">Candle draw color.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartCandleElement element, Color? color);

			/// <summary>
			/// Put the candle data.
			/// </summary>
			/// <param name="element">The chart element representing a candle.</param>
			/// <param name="candle">The candle data.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartCandleElement element, ICandleMessage candle);

			/// <summary>
			/// Put the candle data.
			/// </summary>
			/// <param name="element">The chart element representing a candle.</param>
			/// <param name="candle">Candle.</param>
			/// <param name="openPrice">Opening price.</param>
			/// <param name="highPrice">Highest price.</param>
			/// <param name="lowPrice">Lowest price.</param>
			/// <param name="closePrice">Closing price.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartCandleElement element, ICandleMessage candle, decimal openPrice, decimal highPrice, decimal lowPrice, decimal closePrice);

			/// <summary>
			/// Put the candle data.
			/// </summary>
			/// <param name="element">The chart element representing a candle.</param>
			/// <param name="candle">Candle.</param>
			/// <param name="openPrice">Opening price.</param>
			/// <param name="highPrice">Highest price.</param>
			/// <param name="lowPrice">Lowest price.</param>
			/// <param name="closePrice">Closing price.</param>
			/// <param name="priceLevels">Price levels.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartCandleElement element, ICandleMessage candle, decimal openPrice, decimal highPrice, decimal lowPrice, decimal closePrice, CandlePriceLevel[] priceLevels);

			/// <summary>
			/// Put the candle data.
			/// </summary>
			/// <param name="element">The chart element representing a candle.</param>
			/// <param name="candleArg">Candle arg.</param>
			/// <param name="openPrice">Opening price.</param>
			/// <param name="highPrice">Highest price.</param>
			/// <param name="lowPrice">Lowest price.</param>
			/// <param name="closePrice">Closing price.</param>
			/// <param name="priceLevels">Price levels.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartCandleElement element, object candleArg, decimal openPrice, decimal highPrice, decimal lowPrice, decimal closePrice, CandlePriceLevel[] priceLevels);

			/// <summary>
			/// Put the indicator data.
			/// </summary>
			/// <param name="element">The chart element representing the indicator.</param>
			/// <param name="value">The indicator value.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartIndicatorElement element, IIndicatorValue value);

			/// <summary>
			/// Put the order data.
			/// </summary>
			/// <param name="element">The chart element representing orders.</param>
			/// <param name="order">The order value.</param>
			/// <param name="errorMessage">Error registering/cancelling order.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartOrderElement element, Order order, string errorMessage = null);

			/// <summary>
			/// Put the trade data.
			/// </summary>
			/// <param name="element">The chart element representing orders.</param>
			/// <param name="orderId">Order ID.</param>
			/// <param name="orderStringId">Order ID (as string, if electronic board does not use numeric order ID representation).</param>
			/// <param name="side">Order side (buy or sell).</param>
			/// <param name="price">Order price.</param>
			/// <param name="volume">Number of contracts in the order.</param>
			/// <param name="errorMessage">Error registering/cancelling order.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartOrderElement element, long orderId, string orderStringId, Sides side, decimal price, decimal volume, string errorMessage = null);

			/// <summary>
			/// Put the order data.
			/// </summary>
			/// <param name="element">The chart element representing trades.</param>
			/// <param name="trade">The trade value.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartTradeElement element, MyTrade trade);

			/// <summary>
			/// Put the trade data.
			/// </summary>
			/// <param name="element">The chart element representing trades.</param>
			/// <param name="tradeId">Trade ID.</param>
			/// <param name="tradeStringId">Trade ID (as string, if electronic board does not use numeric order ID representation).</param>
			/// <param name="side">Order side (buy or sell).</param>
			/// <param name="price">Trade price.</param>
			/// <param name="volume">Number of contracts in the trade.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartTradeElement element, long tradeId, string tradeStringId, Sides side, decimal price, decimal volume);

			/// <summary>
			/// Put the line data.
			/// </summary>
			/// <param name="element">The chart element representing a line.</param>
			/// <param name="value1">The value1.</param>
			/// <param name="value2">The value2.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartLineElement element, double value1, double value2 = double.NaN);

			/// <summary>
			/// Put the line data.
			/// </summary>
			/// <param name="element">The chart element representing a band.</param>
			/// <param name="value">The value.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartBandElement element, decimal value);

			/// <summary>
			/// Put the line data.
			/// </summary>
			/// <param name="element">The chart element representing a band.</param>
			/// <param name="value1">The value1.</param>
			/// <param name="value2">The value2.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartBandElement element, double value1, double value2);

			/// <summary>
			/// Put the chart data.
			/// </summary>
			/// <param name="element">The chart element.</param>
			/// <param name="value">The chart value.</param>
			/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
			IChartDrawDataItem Add(IChartElement element, object value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IChartDrawDataItem"/>.
		/// </summary>
		/// <param name="timeStamp">The time stamp of the new data generation.</param>
		/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
		IChartDrawDataItem Group(DateTimeOffset timeStamp);

		/// <summary>
		/// Initializes a new instance of the <see cref="IChartDrawDataItem"/>.
		/// </summary>
		/// <param name="xValue">Value of X coordinate for the data.</param>
		/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
		IChartDrawDataItem Group(double xValue);

		/// <summary>
		/// Put the annotation data.
		/// </summary>
		/// <param name="element">The chart element representing an annotation.</param>
		/// <param name="data">Annotation draw data.</param>
		/// <returns><see cref="IChartDrawDataItem"/> instance.</returns>
		void Add(IChartAnnotation element, IAnnotationData data);

		/// <summary>
		/// Put the active order data.
		/// </summary>
		/// <param name="element">The chart element representing active orders.</param>
		/// <param name="order">The order. Can be null to draw just error animation without data.</param>
		/// <param name="isFrozen">Do not allow user to edit the order from chart.</param>
		/// <param name="autoRemoveFromChart">Auto remove this order from chart when its state is final (<see cref="OrderStates.Done"/>, <see cref="OrderStates.Failed"/>).</param>
		/// <param name="isHidden">Whether an order operation has failed.</param>
		/// <param name="isError">Whether an order operation has failed.</param>
		/// <param name="price">Order price.</param>
		/// <param name="balance">Balance.</param>
		/// <param name="state">Use this state to draw the order.</param>
		/// <returns><see cref="IChartDrawData"/> instance.</returns>
		IChartDrawData Add(IChartActiveOrdersElement element, Order order, bool? isFrozen = null, bool autoRemoveFromChart = true, bool isHidden = false, bool? isError = null, decimal? price = null, decimal? balance = null, OrderStates? state = null);
	}
}