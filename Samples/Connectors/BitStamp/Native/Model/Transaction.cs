﻿namespace StockSharp.BitStamp.Native.Model
{
	using System;

	using Ecng.Serialization;

	using Newtonsoft.Json;

	class Transaction
	{
		[JsonProperty("date")]
		[JsonConverter(typeof(JsonDateTimeConverter))]
		public DateTime Time { get; set; }

		[JsonProperty("tid")]
		public long Id { get; set; }

		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("amount")]
		public double Amount { get; set; }

		[JsonProperty("type")]
		public int Type { get; set; }
	}
}