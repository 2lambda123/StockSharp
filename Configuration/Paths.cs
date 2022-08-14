﻿namespace StockSharp.Configuration
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using System.Collections.Generic;

	using Ecng.Common;
	using Ecng.Security;
	using Ecng.Configuration;
	using Ecng.Serialization;
	using Ecng.ComponentModel;
	using Ecng.Localization;

	using Newtonsoft.Json;

	using NuGet.Configuration;

	using StockSharp.Localization;
	using StockSharp.Messages;
	using StockSharp.Logging;

	/// <summary>
	/// System paths.
	/// </summary>
	public static class Paths
	{
		static Paths()
		{
			var companyPath = ConfigManager.TryGet<string>("companyPath");
			CompanyPath = companyPath.IsEmpty() ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StockSharp") : companyPath.ToFullPathIfNeed();

			AppName = ConfigManager.TryGet("appName", TypeHelper.ApplicationName);

			var settingsPath = ConfigManager.TryGet<string>("settingsPath");
			AppDataPath = settingsPath.IsEmpty() ? Path.Combine(CompanyPath, AppName2) : settingsPath.ToFullPathIfNeed();

			PlatformConfigurationFile = Path.Combine(AppDataPath, $"platform_config{DefaultSettingsExt}");
			ProxyConfigurationFile = Path.Combine(CompanyPath, $"proxy_config{DefaultSettingsExt}");
			SecurityNativeIdDir = Path.Combine(AppDataPath, "NativeId");
			SecurityMappingDir = Path.Combine(AppDataPath, "Symbol mapping");
			SecurityExtendedInfo = Path.Combine(AppDataPath, "Extended info");
			StorageDir = Path.Combine(AppDataPath, "Storage");
			SnapshotsDir = Path.Combine(AppDataPath, "Snapshots");
			InstallerDir = Path.Combine(CompanyPath, "Installer");
			InstallerInstallationsConfigPath = Path.Combine(InstallerDir, $"installer_apps_installed{DefaultSettingsExt}");

			var settings = Settings.LoadDefaultSettings(null);
			HistoryDataPath = GetHistoryDataPath(SettingsUtility.GetGlobalPackagesFolder(settings));
		}

		/// <summary>
		/// Get history data path.
		/// </summary>
		/// <param name="startDir">Directory.</param>
		/// <returns>History data path.</returns>
		public static string GetHistoryDataPath(string startDir)
		{
			static DirectoryInfo FindHistoryDataSubfolder(DirectoryInfo packageRoot)
			{
				if (!packageRoot.Exists)
					return null;

				foreach (var di in packageRoot.GetDirectories().OrderByDescending(di => di.Name))
				{
					var d = new DirectoryInfo(Path.Combine(di.FullName, "HistoryData"));

					if (d.Exists)
						return d;
				}

				return null;
			}

			var dir = new DirectoryInfo(Path.GetDirectoryName(startDir));

			while (dir != null)
			{
				var hdRoot = FindHistoryDataSubfolder(new DirectoryInfo(Path.Combine(dir.FullName, "packages", "stocksharp.samples.historydata")));
				if (hdRoot != null)
					return hdRoot.FullName;

				dir = dir.Parent;
			}

			return null;
		}

		/// <summary>
		/// App title.
		/// </summary>
		public static readonly string AppName;

		/// <summary>
		///
		/// </summary>
		public static string AppName2 => AppName.Remove("S#.", true);

		/// <summary>
		/// App title with version.
		/// </summary>
		public static string AppNameWithVersion => $"{AppName} v{InstalledVersion}";

		/// <summary>
		/// The path to directory with all applications.
		/// </summary>
		public static readonly string CompanyPath;

		/// <summary>
		/// The path to the settings directory.
		/// </summary>
		public static readonly string AppDataPath;

		/// <summary>
		/// The path to the configuration file of platform definition.
		/// </summary>
		public static readonly string PlatformConfigurationFile;

		/// <summary>
		/// The path to the configuration file of proxy settings.
		/// </summary>
		public static readonly string ProxyConfigurationFile;

		/// <summary>
		/// The path to the directory with native security identifiers.
		/// </summary>
		public static readonly string SecurityNativeIdDir;

		/// <summary>
		/// The path to the directory with securities id mapping.
		/// </summary>
		public static readonly string SecurityMappingDir;

		/// <summary>
		/// The path to the directory with securities extended info.
		/// </summary>
		public static readonly string SecurityExtendedInfo;

		/// <summary>
		/// The path to the directory with market data.
		/// </summary>
		public static readonly string StorageDir;

		/// <summary>
		/// The path to the directory with snapshots of market data.
		/// </summary>
		public static readonly string SnapshotsDir;

		/// <summary>
		/// The path to the installer directory.
		/// </summary>
		public static readonly string InstallerDir;

		/// <summary>
		/// The path to the installer directory.
		/// </summary>
		public static readonly string InstallerInstallationsConfigPath;

		/// <summary>
		/// Web site domain.
		/// </summary>
		public static string Domain => LocalizedStrings.ActiveLanguage == LangCodes.Ru ? "ru" : "com";

		/// <summary>
		/// Get website url.
		/// </summary>
		/// <returns>Localized url.</returns>
		public static string GetWebSiteUrl() => $"https://stocksharp.{Domain}";

		/// <summary>
		/// Get logo url.
		/// </summary>
		/// <returns>Logo url.</returns>
		public static string GetLogoUrl() => $"{GetWebSiteUrl()}/images/logo.png";

		/// <summary>
		/// </summary>
		public static class Pages
		{
			/// <summary>
			/// </summary>
			public const long Eula = 274;
			/// <summary>
			/// </summary>
			public const long Pricing = 157;
			/// <summary>
			/// </summary>
			public const long NugetManual = 241;
			/// <summary>
			/// </summary>
			public const long Message = 278;
			/// <summary>
			/// </summary>
			public const long Topic = 275;
			/// <summary>
			/// </summary>
			public const long File = 276;
			/// <summary>
			/// </summary>
			public const long Users = 246;
			/// <summary>
			/// </summary>
			public const long Register = 252;
			/// <summary>
			/// </summary>
			public const long Forgot = 253;
			/// <summary>
			/// </summary>
			public const long Faq = 239;
			/// <summary>
			/// </summary>
			public const long Store = 164;
		}

		/// <summary>
		/// Get page url.
		/// </summary>
		/// <param name="id">Page id.</param>
		/// <param name="urlPart">Url part (topic id, file name etc.).</param>
		/// <returns>Localized url.</returns>
		public static string GetPageUrl(long id, object urlPart = default)
		{
			var url = GetWebSiteUrl() + "/";

			url += id switch
			{
				Pages.Eula => "products/eula",
				Pages.Pricing => "products/pricing",
				Pages.NugetManual => "products/nuget_manual",
				Pages.Message => "posts/m",
				Pages.Topic => "topic",
				Pages.File => "file",
				Pages.Users => "users",
				Pages.Register => "register",
				Pages.Forgot => "forgot",
				Pages.Faq => "store/faq",
				Pages.Store => "store",
				_ => throw new ArgumentOutOfRangeException(nameof(id), id, LocalizedStrings.Str1219),
			};

			url += "/";

			if (urlPart is not null)
				url += $"{urlPart}/";

			return url;
		}

		/// <summary>
		/// To create localized url.
		/// </summary>
		/// <param name="docUrl">Help topic.</param>
		/// <returns>Localized url.</returns>
		public static string GetDocUrl(string docUrl) => $"https://doc.stocksharp.{Domain}/{docUrl}";

		private static string _installedVersion;

		/// <summary>
		/// Installed version of the product.
		/// </summary>
		public static string InstalledVersion
		{
			get
			{
				if (_installedVersion != null)
					return _installedVersion;

				static string GetAssemblyVersion() => (Assembly.GetEntryAssembly() ?? typeof(Paths).Assembly).GetName().Version.To<string>();

				try
				{
					_installedVersion = GetInstalledVersion(Directory.GetCurrentDirectory()) ?? GetAssemblyVersion();
				}
				catch
				{
					_installedVersion = GetAssemblyVersion();
				}

				_installedVersion ??= "<error>";

				return _installedVersion;
			}
		}

		/// <summary>
		/// Get currently installed version of the product.
		/// </summary>
		/// <param name="productInstallPath">File system path to product installation.</param>
		/// <returns>Installed version of the product.</returns>
		public static string GetInstalledVersion(string productInstallPath)
		{
			if (productInstallPath.IsEmpty())
				throw new ArgumentException(nameof(productInstallPath));

			if (!InstallerInstallationsConfigPath.IsConfigExists())
				return null;

			var storage = Do.Invariant(() =>
				InstallerInstallationsConfigPath.Deserialize<SettingsStorage>());

			if (storage is null)
				return null;

			var installations = storage?.GetValue<SettingsStorage[]>("Installations");
			if (!(installations?.Length > 0))
				return null;

			var installation = installations.FirstOrDefault(ss => productInstallPath.ComparePaths(ss.TryGet<string>("InstallDirectory")));
			if(installation == null)
				return null;

			var identityStr = installation
				.TryGet<SettingsStorage>("Version")?
				.TryGet<SettingsStorage>("Metadata")?
				.TryGet<string>("Identity");

			if (identityStr.IsEmpty())
				return null;

			// ReSharper disable once PossibleNullReferenceException
			var parts = identityStr.Split('|');

			return parts.Length != 2 ? null : parts[1];
		}

		/// <summary>
		/// Sample history data.
		/// </summary>
		public static readonly string HistoryDataPath;

		private static ProcessSingleton _isRunningMutex;

		/// <summary>
		/// Check if an instance of the application already started.
		/// </summary>
		/// <returns>Check result.</returns>
		public static bool StartIsRunning() => StartIsRunning(AppDataPath);

		/// <summary>
		/// Check if an instance of the application already started.
		/// </summary>
		/// <returns>Check result.</returns>
		public static bool StartIsRunning(string appKey)
		{
			if (_isRunningMutex != null)
				throw new InvalidOperationException("mutex was already initialized");

			try
			{
				_isRunningMutex = new ProcessSingleton(appKey);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Release all resources allocated by <see cref="StartIsRunning()"/>.
		/// </summary>
		public static void StopIsRunning()
		{
			_isRunningMutex?.Dispose();
			_isRunningMutex = null;
		}

		private class ProcessSingleton : Disposable
		{
			private readonly ManualResetEvent _stop = new(false);
			private readonly ManualResetEvent _stopped = new(false);

			public ProcessSingleton(string key)
			{
				Exception error = null;
				var started = new ManualResetEvent(false);

				// mutex должен освобождаться из того же потока, в котором захвачен. некоторые приложения вызывают StopIsRunning из другого потока нежели StartIsRunning
				// выделяя отдельный поток, обеспечивается гарантия корректной работы в любом случае
				ThreadingHelper.Thread(() =>
				{
					Mutex mutex;

					try
					{
						var mutexName = "stocksharp_app_" + key.UTF8().Md5();
						if (!ThreadingHelper.TryGetUniqueMutex(mutexName, out mutex))
							throw new InvalidOperationException($"can't acquire the mutex {mutexName}, (key={key})");
					}
					catch (Exception e)
					{
						error = e;
						_stopped.Set();
						return;
					}
					finally
					{
						started.Set();
					}

					try
					{
						_stop.WaitOne();
						mutex.ReleaseMutex();
					}
					finally
					{
						_stopped.Set();
					}
				})
				.Name("process_singleton")
				.Launch();

				started.WaitOne();
				if (error != null)
					throw error;
			}

			protected override void DisposeManaged()
			{
				_stop.Set();
				_stopped.WaitOne();
				base.DisposeManaged();
			}
		}

		/// <summary>
		/// Default extension for settings file.
		/// </summary>
		public const string DefaultSettingsExt = ".json";

		/// <summary>
		/// Legacy extension for settings file.
		/// </summary>
		[Obsolete]
		public const string LegacySettingsExt = ".xml";

		/// <summary>
		/// Backup extension for settings file.
		/// </summary>
		public const string BackupExt = ".bak";

		/// <summary>
		/// Returns an files with <see cref="DefaultSettingsExt"/> and <see cref="LegacySettingsExt"/> extensions.
		/// </summary>
		/// <param name="path">The relative or absolute path to the directory to search.</param>
		/// <param name="filter">The search string to match against the names of files in path.</param>
		/// <returns>Files.</returns>
		public static IEnumerable<string> EnumerateConfigs(this string path, string filter = "*")
			=> Directory.EnumerateFiles(path, $"{filter}{DefaultSettingsExt}").Concat(
#pragma warning disable CS0612 // Type or member is obsolete
				Directory.EnumerateFiles(path, $"{filter}{LegacySettingsExt}")
#pragma warning restore CS0612 // Type or member is obsolete
		);

		/// <summary>
		/// Make the specified <paramref name="filePath"/> with <see cref="LegacySettingsExt"/> extension.
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <returns>File path.</returns>
		[Obsolete]
		public static string MakeLegacy(this string filePath)
			=> Path.ChangeExtension(filePath, LegacySettingsExt);

		/// <summary>
		/// Make the specified <paramref name="filePath"/> with <see cref="BackupExt"/> extension.
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <returns>File path.</returns>
		public static string MakeBackup(this string filePath)
			=> $"{filePath}{BackupExt}";

		/// <summary>
		/// Rename the specified file with <see cref="BackupExt"/> extension.
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <param name="backupFilePath">Backup file path.</param>
		public static void MoveToBackup(this string filePath, string backupFilePath = null)
		{
			var target = backupFilePath ?? filePath;
			var bak = target.MakeBackup();
			var idx = 0;
			do
			{
				if(!File.Exists(bak))
					break;

				bak = (target + $".{++idx}").MakeBackup();
			} while(true);

			File.Move(filePath, bak);
		}

		/// <summary>
		/// Create serializer.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <returns>Serializer.</returns>
		public static ISerializer<T> CreateSerializer<T>()
			=> new JsonSerializer<T>
			{
				Indent = true,
				EnumAsString = true,
				NullValueHandling = NullValueHandling.Ignore,
			};

		/// <summary>
		/// Create serializer.
		/// </summary>
		/// <param name="type">Value type.</param>
		/// <returns>Serializer.</returns>
		public static ISerializer CreateSerializer(Type type)
			=> CreateSerializer<int>().GetSerializer(type);

		/// <summary>
		/// Serialize value into the specified file.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="value">Value.</param>
		/// <param name="filePath">File path.</param>
		public static void Serialize<T>(this T value, string filePath)
			=> CreateSerializer<T>().Serialize(value, filePath);

		/// <summary>
		/// Serialize value into byte array.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="value">Value.</param>
		/// <returns>Serialized data.</returns>
		public static byte[] Serialize<T>(this T value)
			=> CreateSerializer<T>().Serialize(value);

		/// <summary>
		/// 
		/// </summary>
		[Obsolete("Use Deserialize instead.")]
		public static T DeserializeWithMigration<T>(this string filePath)
			=> filePath.Deserialize<T>();

		/// <summary>
		/// 
		/// </summary>
		[Obsolete]
		public static ISerializer LegacySerializer { get; set; }

		/// <summary>
		/// Deserialize value from the specified file.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="filePath">File path.</param>
		/// <returns>Value.</returns>
		public static T Deserialize<T>(this string filePath)
		{
			var defFile = Path.ChangeExtension(filePath, DefaultSettingsExt);
			var defSer = CreateSerializer<T>();

			T value;

#pragma warning disable CS0612 // Type or member is obsolete
			var legacyFile = filePath.MakeLegacy();

			if (File.Exists(legacyFile) && LegacySerializer is not null)
			{
				// TODO 2021-09-09 remove 1 year later

				value = LegacySerializer.GetSerializer<T>().Deserialize(legacyFile);
#pragma warning restore CS0612 // Type or member is obsolete

				static void TryFix(SettingsStorage storage)
				{
					foreach (var pair in storage.ToArray())
					{
						var value = pair.Value;

						if (value is List<Range<TimeSpan>> times)
						{
							storage.Set(pair.Key, times.Select(r => r.ToStorage()).ToArray());
						}
						else if (value is Dictionary<DayOfWeek, Range<TimeSpan>[]> specialDays)
						{
							storage.Set(pair.Key, specialDays.Select(p => new SettingsStorage()
								.Set("Day", p.Key)
								.Set("Periods", p.Value.Select(r => r.ToStorage()).ToArray())
							).ToArray());
						}
						else if (value is Dictionary<DateTime, Range<TimeSpan>[]> specialDays2)
						{
							storage.Set(pair.Key, specialDays2.Select(p => new SettingsStorage()
								.Set("Day", p.Key)
								.Set("Periods", p.Value.Select(p1 => p1.ToStorage()).ToArray())
							).ToArray());
						}
						else if (value is Dictionary<UserPermissions, IDictionary<Tuple<string, string, object, DateTime?>, bool>> permissions)
						{
							storage.Set(pair.Key, permissions
								.Select(p =>
									new SettingsStorage()
										.Set("Permission", p.Key)
										.Set("Settings", p.Value
											.Select(p1 =>
												new SettingsStorage()
													.Set("Name", p1.Key.Item1)
													.Set("Param", p1.Key.Item2)
													.Set("Extra", p1.Key.Item3)
													.Set("Till", p1.Key.Item4)
													.Set("IsEnabled", p1.Value)
											).ToArray()
										)
								).ToArray()
							);
						}
						else if (value is IEnumerable<RefPair<Guid, string>> pairs)
						{
							storage.Set(pair.Key, pairs.Select(p => p.ToStorage()).ToArray());
						}
						else if (value is SettingsStorage s1)
							TryFix(s1);
						else if (value is IEnumerable<SettingsStorage> set)
						{
							foreach (var item in set)
								TryFix(item);
						}
					}
				}

				if (value is SettingsStorage s)
					TryFix(s);
				else if (value is IEnumerable<SettingsStorage> set)
				{
					foreach (var item in set)
						TryFix(item);
				}

				try
				{
					// !!! serialize and deserialize (check our new serializer)
					value = defSer.Deserialize(defSer.Serialize(value));

					// saving data in new format
					defSer.Serialize(value, defFile);

					// make backup only if everything is ok
					legacyFile.MoveToBackup();
				}
				catch (Exception ex)
				{
					ex.LogError();
				}
			}
			else
				value = File.Exists(defFile) ? defSer.Deserialize(defFile) : default;

			return value;
		}

		/// <summary>
		/// Deserialize value from the serialized data.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="data">Serialized data.</param>
		/// <returns>Value.</returns>
		public static T Deserialize<T>(this byte[] data)
		{
			var serializer = CreateSerializer<T>();

			try
			{
				return serializer.Deserialize(data);
			}
			catch
			{
#pragma warning disable CS0612 // Type or member is obsolete
				if (LegacySerializer is null)
					return default;

				var xmlSer = LegacySerializer.GetSerializer(serializer.Type);

				if (xmlSer.GetType() == serializer.GetType())
					throw;

				return (T)xmlSer.Deserialize(data);
#pragma warning restore CS0612 // Type or member is obsolete
			}
		}

		/// <summary>
		/// Get file name for the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <returns>File name.</returns>
		public static string GetFileName(this Guid id)
			=> $"{id.ToString().Replace('-', '_')}{DefaultSettingsExt}";

		/// <summary>
		/// Determines the specified config file exists.
		/// </summary>
		/// <param name="configFile">Config file.</param>
		/// <returns>Check result.</returns>
		public static bool IsConfigExists(this string configFile)
			=> File.Exists(configFile) ||
#pragma warning disable CS0612 // Type or member is obsolete
			File.Exists(configFile.MakeLegacy())
#pragma warning restore CS0612 // Type or member is obsolete
		;
	}
}