namespace StockSharp.Messages
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Backtesting session.
	/// </summary>
	[DataContract]
	[Serializable]
	public class StrategyBacktestResultMessage : Message
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StrategyBacktestResultMessage"/>.
		/// </summary>
		public StrategyBacktestResultMessage()
			: base(MessageTypes.StrategyBacktestResult)
		{
		}

		/// <summary>
		/// Create a copy of <see cref="StrategyBacktestResultMessage"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override Message Clone()
		{
			var clone = new StrategyBacktestResultMessage
			{

			};

			CopyTo(clone);

			return clone;
		}
	}
}