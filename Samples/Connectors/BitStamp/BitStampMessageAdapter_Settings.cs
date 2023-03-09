namespace StockSharp.BitStamp;

using System;
using System.ComponentModel.DataAnnotations;
using System.Security;

using Ecng.Common;
using Ecng.ComponentModel;
using Ecng.Serialization;

using StockSharp.Localization;
using StockSharp.Messages;

/// <summary>
/// The message adapter for <see cref="BitStamp"/>.
/// </summary>
[MediaIcon("BitStamp_logo.svg")]
[Doc("topics/BitStamp.html")]
[DisplayNameLoc(LocalizedStrings.BitStampKey)]
[CategoryLoc(LocalizedStrings.CryptocurrencyKey)]
[DescriptionLoc(LocalizedStrings.Str1770Key, LocalizedStrings.BitStampKey)]
[MessageAdapterCategory(MessageAdapterCategories.Crypto | MessageAdapterCategories.RealTime |
	MessageAdapterCategories.Free | MessageAdapterCategories.Ticks | MessageAdapterCategories.MarketDepth |
	MessageAdapterCategories.Level1 | MessageAdapterCategories.Transactions | MessageAdapterCategories.OrderLog)]
public partial class BitStampMessageAdapter : IKeySecretAdapter
{
	/// <summary>
	/// Default value for <see cref="MessageAdapter.HeartbeatInterval"/>.
	/// </summary>
	public static readonly TimeSpan DefaultHeartbeatInterval = TimeSpan.FromSeconds(1);

	/// <inheritdoc />
	[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str3304Key,
			Description = LocalizedStrings.Str3304Key + LocalizedStrings.Dot,
			GroupName = LocalizedStrings.Str174Key,
			Order = 0)]
	public SecureString Key { get; set; }

	/// <inheritdoc />
	[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str3306Key,
			Description = LocalizedStrings.Str3307Key,
			GroupName = LocalizedStrings.Str174Key,
			Order = 1)]
	public SecureString Secret { get; set; }

	private TimeSpan _balanceCheckInterval;

	/// <summary>
	/// Balance check interval. Required in case of deposit and withdraw actions.
	/// </summary>
	[Display(
		ResourceType = typeof(LocalizedStrings),
		Name = LocalizedStrings.Str1325Key,
		Description = LocalizedStrings.BalanceCheckIntervalKey,
		GroupName = LocalizedStrings.Str174Key,
		Order = 3)]
	public TimeSpan BalanceCheckInterval
	{
		get => _balanceCheckInterval;
		set
		{
			if (value < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(value));

			_balanceCheckInterval = value;
		}
	}

	/// <inheritdoc />
	public override void Save(SettingsStorage storage)
	{
		base.Save(storage);

		storage.SetValue(nameof(Key), Key);
		storage.SetValue(nameof(Secret), Secret);
		storage.SetValue(nameof(BalanceCheckInterval), BalanceCheckInterval);
	}

	/// <inheritdoc />
	public override void Load(SettingsStorage storage)
	{
		base.Load(storage);

		Key = storage.GetValue<SecureString>(nameof(Key));
		Secret = storage.GetValue<SecureString>(nameof(Secret));
		BalanceCheckInterval = storage.GetValue<TimeSpan>(nameof(BalanceCheckInterval));
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return base.ToString() + ": " + LocalizedStrings.Str3304 + " = " + Key.ToId();
	}
}