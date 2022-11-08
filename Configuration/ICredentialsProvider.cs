﻿namespace StockSharp.Configuration
{
	using Ecng.ComponentModel;

	/// <summary>
	/// Interface describing credentials provider.
	/// </summary>
	public interface ICredentialsProvider
	{
		/// <summary>
		/// Try load credentials.
		/// </summary>
		/// <param name="credentials"><see cref="ServerCredentials"/>.</param>
		/// <returns>Operation result.</returns>
		bool TryLoad(out ServerCredentials credentials);

		/// <summary>
		/// Save credentials.
		/// </summary>
		/// <param name="credentials"><see cref="ServerCredentials"/>.</param>
		void Save(ServerCredentials credentials);
	}
}