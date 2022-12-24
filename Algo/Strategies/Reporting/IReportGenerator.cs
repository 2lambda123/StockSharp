namespace StockSharp.Algo.Strategies.Reporting;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The interface describe report generator for strategies.
/// </summary>
public interface IReportGenerator
{
	/// <summary>
	/// Name.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Extension without leading dot char.
	/// </summary>
	string Extension { get; }

	/// <summary>
	/// To generate the report.
	/// </summary>
	/// <param name="strategy"><see cref="Strategy"/>.</param>
	/// <param name="fileName">The name of the file, in which the report is generated.</param>
	/// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
	ValueTask Generate(Strategy strategy, string fileName, CancellationToken cancellationToken = default);
}

/// <summary>
/// The base report generator for strategies.
/// </summary>
public abstract class BaseReportGenerator : IReportGenerator
{
	/// <summary>
	/// Initialize <see cref="BaseReportGenerator"/>.
	/// </summary>
	protected BaseReportGenerator()
	{
	}

	/// <inheritdoc />
	public abstract string Name { get; }

	/// <inheritdoc />
	public abstract string Extension { get; }

	/// <inheritdoc />
	public abstract ValueTask Generate(Strategy strategy, string fileName, CancellationToken cancellationToken);
}