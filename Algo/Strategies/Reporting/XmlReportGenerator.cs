namespace StockSharp.Algo.Strategies.Reporting;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Ecng.Common;

/// <summary>
/// The report generator for the strategy in the xml format.
/// </summary>
public class XmlReportGenerator : BaseReportGenerator
{
	/// <inheritdoc />
	public override string Name => "XML";

	/// <inheritdoc />
	public override string Extension => "xml";

	/// <inheritdoc />
	public override ValueTask Generate(Strategy strategy, string fileName, CancellationToken cancellationToken)
	{
		using var writer = new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = Formatting.Indented };

		void WriteStartElement(string name)
			=> writer.WriteStartElement(name);

		void WriteEndElement()
			=> writer.WriteEndElement();

		void WriteAttributeString(string name, object value)
			=> writer.WriteAttributeString(name, value is TimeSpan ts ? ts.Format() : value.To<string>());

		WriteStartElement("strategy");

		WriteAttributeString("name", strategy.Name);
		WriteAttributeString("security", strategy.Security?.Id);
		WriteAttributeString("portfolio", strategy.Portfolio?.Name);
		WriteAttributeString("totalWorkingTime", strategy.TotalWorkingTime);
		WriteAttributeString("commission", strategy.Commission);
		WriteAttributeString("position", strategy.Position);
		WriteAttributeString("PnL", strategy.PnL);
		WriteAttributeString("slippage", strategy.Slippage);
		WriteAttributeString("latency", strategy.Latency);

		WriteStartElement("parameters");

		foreach (var p in strategy.Parameters.CachedValues)
		{
			cancellationToken.ThrowIfCancellationRequested();

			WriteStartElement("parameter");

			WriteAttributeString("name", p.Name);
			WriteAttributeString("value", p.Value);

			WriteEndElement();
		}

		WriteEndElement();

		WriteStartElement("statisticParameters");

		foreach (var p in strategy.StatisticManager.Parameters)
		{
			cancellationToken.ThrowIfCancellationRequested();

			WriteStartElement("parameter");

			WriteAttributeString("name", p.Name);
			WriteAttributeString("value", p.Value);

			WriteEndElement();
		}

		WriteEndElement();

		WriteStartElement("orders");

		foreach (var o in strategy.Orders)
		{
			cancellationToken.ThrowIfCancellationRequested();

			WriteStartElement("order");

			WriteAttributeString("id", o.Id);
			WriteAttributeString("transactionId", o.TransactionId);
			WriteAttributeString("direction", o.Direction);
			WriteAttributeString("time", o.Time);
			WriteAttributeString("price", o.Price);
			WriteAttributeString("state", o.State);
			WriteAttributeString("balance", o.Balance);
			WriteAttributeString("volume", o.Volume);
			WriteAttributeString("type", o.Type);
			WriteAttributeString("comment", o.Comment);

			WriteEndElement();
		}

		WriteEndElement();

		WriteStartElement("trades");

		foreach (var t in strategy.MyTrades)
		{
			cancellationToken.ThrowIfCancellationRequested();

			WriteStartElement("trade");

			WriteAttributeString("id", t.Trade.Id);
			WriteAttributeString("transactionId", t.Order.TransactionId);
			WriteAttributeString("time", t.Trade.Time);
			WriteAttributeString("price", t.Trade.Price);
			WriteAttributeString("volume", t.Trade.Volume);
			WriteAttributeString("order", t.Order.Id);
			WriteAttributeString("PnL", strategy.PnLManager.ProcessMessage(t.ToMessage())?.PnL);
			WriteAttributeString("slippage", t.Slippage);

			WriteEndElement();
		}

		WriteEndElement();

		WriteEndElement();

		return default;
	}
}