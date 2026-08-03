using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DreamPoeBot.Loki.Game.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllflameBot
{
    internal sealed class AllflamePriceQuote
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double UnitDivines { get; set; }
        public double MinCandidateDivines { get; set; }
        public double MaxCandidateDivines { get; set; }
        public List<string> Candidates { get; set; }
        public bool IsAmbiguousUnique { get; set; }
        public bool IsUnresolvedUnique { get; set; }
        public bool ResolvedByArtwork { get; set; }
        public double TotalDivines
        {
            get { return UnitDivines * Math.Max(1, Quantity); }
        }
        public bool IsPriced
        {
            get { return UnitDivines > 0d; }
        }

        public bool HasCandidateRange
        {
            get { return MaxCandidateDivines > 0d; }
        }
    }

    internal sealed class AllflamePriceFile
    {
        public int SchemaVersion { get; set; }
        public string League { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public double DivineChaos { get; set; }
        public Dictionary<string, double> Prices { get; set; }
    }

    internal static class AllflamePrices
    {
        private const int CurrentSchemaVersion = 3;
        private const string ApiRoot = "https://poe.ninja/poe1/api/economy";
        private static readonly object Sync = new object();
        private static readonly HttpClient Client = CreateClient();
        private static readonly string[] ItemTypes =
        {
            "Map",
            "UniqueMap",
            "UniqueJewel",
            "UniqueFlask",
            "UniqueWeapon",
            "UniqueArmour",
            "UniqueAccessory",
            "SkillGem",
            "ClusterJewel"
        };
        private static readonly string[] ExchangeTypes =
        {
            "Currency",
            "Fragment",
            "AllflameEmber",
            "Tattoo",
            "Omen",
            "DivinationCard",
            "Oil",
            "DeliriumOrb",
            "Scarab",
            "Astrolabe",
            "Fossil",
            "Resonator",
            "Essence"
        };
        private static Dictionary<string, double> _prices =
            new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, double> _normalizedPrices =
            new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private static DateTime _updatedUtc = DateTime.MinValue;
        private static string _league = String.Empty;
        private static double _divineChaos;
        private static int _updateRunning;

        public static string RootPath
        {
            get
            {
                return Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Settings",
                    "AllflameBot");
            }
        }

        public static string PricesFilePath
        {
            get { return Path.Combine(RootPath, "prices.json"); }
        }

        public static string BackupsPath
        {
            get { return Path.Combine(RootPath, "PriceBackups"); }
        }

        public static DateTime UpdatedUtc
        {
            get { lock (Sync) { return _updatedUtc; } }
        }

        public static string League
        {
            get { lock (Sync) { return _league; } }
        }

        public static double DivineChaos
        {
            get { lock (Sync) { return _divineChaos; } }
        }

        public static int Count
        {
            get { lock (Sync) { return _prices.Count; } }
        }

        public static bool HasPrices
        {
            get { return Count > 0; }
        }

        public static bool UpdateIsRunning
        {
            get { return Interlocked.CompareExchange(ref _updateRunning, 0, 0) != 0; }
        }

        public static void Initialize()
        {
            Load();
            ApplyFixedPrices();
        }

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(RootPath);
                if (!File.Exists(PricesFilePath))
                {
                    AllflameLog.Info("PRICES_LOAD|Result=Missing|Path=" + PricesFilePath);
                    return;
                }

                AllflamePriceFile data = JsonConvert.DeserializeObject<AllflamePriceFile>(
                    File.ReadAllText(PricesFilePath));
                if (data == null || data.Prices == null)
                {
                    AllflameLog.Info("PRICES_LOAD|Result=Invalid|Path=" + PricesFilePath);
                    return;
                }

                lock (Sync)
                {
                    _prices = new Dictionary<string, double>(
                        data.Prices
                            .Where(pair =>
                                !String.IsNullOrWhiteSpace(pair.Key) &&
                                pair.Value >= 0d)
                            .ToDictionary(
                                pair => pair.Key.Trim(),
                                pair => pair.Value,
                                StringComparer.OrdinalIgnoreCase),
                        StringComparer.OrdinalIgnoreCase);
                    _updatedUtc = data.UpdatedUtc;
                    _league = data.League ?? String.Empty;
                    _divineChaos = data.DivineChaos;
                    if (data.SchemaVersion < 3)
                    {
                        // Older caches can contain both the incorrect
                        // chaosEquivalent quote under the apostrophized name
                        // and the correct exchange quote under a humanized
                        // name without the apostrophe.
                        _prices.Remove("Dead Man's Sulphur");
                        _prices.Remove("Dead Mans Sulphur");
                        _updatedUtc = DateTime.MinValue;
                    }
                    RebuildNormalizedPrices();
                }
                AllflameLog.Info(
                    "PRICES_LOAD|Result=Success|League=" + League +
                    "|Count=" + Count +
                    "|UpdatedUtc=" + UpdatedUtc.ToString("o"));
            }
            catch (Exception ex)
            {
                AllflameLog.Error("PRICES_LOAD_FAILED", ex);
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(RootPath);
                AllflamePriceFile data;
                lock (Sync)
                {
                    data = new AllflamePriceFile
                    {
                        SchemaVersion = CurrentSchemaVersion,
                        League = _league,
                        UpdatedUtc = _updatedUtc,
                        DivineChaos = _divineChaos,
                        Prices = new Dictionary<string, double>(
                            _prices,
                            StringComparer.OrdinalIgnoreCase)
                    };
                }

                string temporaryPath = PricesFilePath + ".tmp";
                File.WriteAllText(
                    temporaryPath,
                    JsonConvert.SerializeObject(data, Formatting.Indented));
                if (File.Exists(PricesFilePath))
                {
                    File.Copy(temporaryPath, PricesFilePath, true);
                    File.Delete(temporaryPath);
                }
                else
                {
                    File.Move(temporaryPath, PricesFilePath);
                }
                AllflameLog.Info(
                    "PRICES_SAVE|League=" + data.League +
                    "|Count=" + data.Prices.Count +
                    "|Path=" + PricesFilePath);
            }
            catch (Exception ex)
            {
                AllflameLog.Error("PRICES_SAVE_FAILED", ex);
            }
        }

        public static AllflamePriceQuote Quote(Item item)
        {
            if (item == null)
            {
                return new AllflamePriceQuote
                {
                    Name = "Unknown",
                    Quantity = 1
                };
            }

            string fullName = item.FullName ?? String.Empty;
            string baseName = item.Name ?? String.Empty;
            AllflameUniqueArtResolution uniqueResolution =
                AllflameUniqueResolver.Resolve(item);
            if (uniqueResolution.IsMapped)
            {
                var candidatePrices = new List<double>();
                var missingCandidates = new List<string>();
                foreach (string candidate in
                    uniqueResolution.Candidates)
                {
                    double candidatePrice;
                    if (AllflameSettings.Instance.EnablePricing &&
                        TryGetUnitPrice(candidate, out candidatePrice))
                    {
                        candidatePrices.Add(candidatePrice);
                    }
                    else
                    {
                        missingCandidates.Add(candidate);
                    }
                }

                bool unique = String.Equals(
                    item.Rarity.ToString(),
                    "Unique",
                    StringComparison.OrdinalIgnoreCase);
                if (unique && !item.IsIdentified)
                {
                    bool singleResolved =
                        uniqueResolution.Candidates.Count == 1 &&
                        missingCandidates.Count == 0 &&
                        candidatePrices.Count == 1;
                    double minimum = candidatePrices.Count == 0
                        ? 0d
                        : candidatePrices.Min();
                    double maximum = candidatePrices.Count == 0
                        ? 0d
                        : candidatePrices.Max();
                    string resolvedName = singleResolved
                        ? uniqueResolution.Candidates[0]
                        : String.Join(
                            " / ",
                            uniqueResolution.Candidates.ToArray());
                    AllflameLog.Info(
                        "UNIQUE_ART_RESOLUTION" +
                        "|BaseName=" + baseName.Replace("|", "/") +
                        "|RenderArt=" +
                        uniqueResolution.RenderArt.Replace("|", "/") +
                        "|Candidates=" +
                        resolvedName.Replace("|", "/") +
                        "|Result=" +
                        (singleResolved
                            ? "Resolved"
                            : (missingCandidates.Count > 0
                                ? "CandidatePriceMissing"
                                : "Ambiguous")) +
                        "|MinDivines=" +
                        minimum.ToString("0.####", CultureInfo.InvariantCulture) +
                        "|MaxDivines=" +
                        maximum.ToString("0.####", CultureInfo.InvariantCulture));
                    return new AllflamePriceQuote
                    {
                        Name = String.IsNullOrWhiteSpace(resolvedName)
                            ? "Unresolved Unique"
                            : resolvedName,
                        Quantity = Math.Max(1, item.StackCount),
                        UnitDivines = singleResolved
                            ? candidatePrices[0]
                            : 0d,
                        MinCandidateDivines = minimum,
                        MaxCandidateDivines = maximum,
                        Candidates = uniqueResolution.Candidates,
                        IsAmbiguousUnique = !singleResolved,
                        IsUnresolvedUnique =
                            missingCandidates.Count > 0,
                        ResolvedByArtwork = singleResolved
                    };
                }
            }
            bool unidentifiedUnique =
                !item.IsIdentified &&
                String.Equals(
                    item.Rarity.ToString(),
                    "Unique",
                    StringComparison.OrdinalIgnoreCase);
            if (unidentifiedUnique)
            {
                string unresolvedName =
                    !String.IsNullOrWhiteSpace(baseName)
                        ? baseName.Trim()
                        : (!String.IsNullOrWhiteSpace(fullName)
                            ? fullName.Trim()
                            : "Unresolved Unique");
                AllflameLog.Info(
                    "UNIQUE_ART_RESOLUTION" +
                    "|BaseName=" + unresolvedName.Replace("|", "/") +
                    "|RenderArt=" +
                    uniqueResolution.RenderArt.Replace("|", "/") +
                    "|Candidates=" +
                    "|Result=UnresolvedUnique" +
                    "|MinDivines=0" +
                    "|MaxDivines=0");
                return new AllflamePriceQuote
                {
                    Name = unresolvedName,
                    Quantity = Math.Max(1, item.StackCount),
                    Candidates = new List<string>(),
                    IsUnresolvedUnique = true
                };
            }
            string pricingName =
                !String.IsNullOrWhiteSpace(fullName)
                    ? fullName.Trim()
                    : baseName.Trim();
            int quantity = Math.Max(1, item.StackCount);
            double unitPrice;
            if (!AllflameSettings.Instance.EnablePricing)
            {
                unitPrice = 0d;
            }
            else if (!TryGetUnitPrice(pricingName, out unitPrice) &&
                !String.Equals(pricingName, baseName, StringComparison.OrdinalIgnoreCase))
            {
                TryGetUnitPrice(baseName, out unitPrice);
                if (unitPrice > 0d && !String.IsNullOrWhiteSpace(baseName))
                {
                    pricingName = baseName.Trim();
                }
            }

            return new AllflamePriceQuote
            {
                Name = String.IsNullOrWhiteSpace(pricingName) ? "Unknown" : pricingName,
                Quantity = quantity,
                UnitDivines = unitPrice
            };
        }

        public static bool TryGetUnitPrice(string name, out double value)
        {
            value = 0d;
            if (String.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            lock (Sync)
            {
                if (_prices.TryGetValue(name.Trim(), out value) && value > 0d)
                {
                    return true;
                }
                string normalized = NormalizeName(name);
                return normalized.Length > 0 &&
                    _normalizedPrices.TryGetValue(normalized, out value) &&
                    value > 0d;
            }
        }

        public static void ApplyFixedPrices()
        {
            Dictionary<string, double> fixedPrices =
                ParseFixedPrices(AllflameSettings.Instance.FixedPrices);
            if (fixedPrices.Count == 0)
            {
                return;
            }
            lock (Sync)
            {
                foreach (KeyValuePair<string, double> pair in fixedPrices)
                {
                    _prices[pair.Key] = pair.Value;
                }
                RebuildNormalizedPrices();
            }
            AllflameLog.Info("PRICES_FIXED_APPLIED|Count=" + fixedPrices.Count);
        }

        public static Dictionary<string, double> ParseFixedPrices(string source)
        {
            var output = new Dictionary<string, double>(
                StringComparer.OrdinalIgnoreCase);
            if (String.IsNullOrWhiteSpace(source))
            {
                return output;
            }

            string[] entries = source.Split(
                new[] { ';', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string entry in entries)
            {
                int separator = entry.LastIndexOf('=');
                if (separator <= 0 || separator >= entry.Length - 1)
                {
                    continue;
                }
                string name = entry.Substring(0, separator).Trim();
                double price;
                if (name.Length > 0 &&
                    Double.TryParse(
                        entry.Substring(separator + 1).Trim(),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out price) &&
                    price >= 0d)
                {
                    output[name] = price;
                }
            }
            return output;
        }

        public static bool ShouldAutoUpdate()
        {
            AllflameSettings settings = AllflameSettings.Instance;
            if (!settings.EnablePricing ||
                !settings.AutoUpdatePrices ||
                String.IsNullOrWhiteSpace(settings.PricingLeague) ||
                UpdateIsRunning)
            {
                return false;
            }
            DateTime updated = UpdatedUtc;
            return updated == DateTime.MinValue ||
                DateTime.UtcNow - updated >=
                    TimeSpan.FromHours(Math.Max(1, settings.PriceUpdateHours));
        }

        public static async Task<bool> UpdateAsync(string league)
        {
            if (String.IsNullOrWhiteSpace(league) ||
                Interlocked.CompareExchange(ref _updateRunning, 1, 0) != 0)
            {
                return false;
            }

            try
            {
                league = league.Trim();
                AllflameLog.Info("PRICES_UPDATE_START|League=" + league);
                var chaosPrices = new Dictionary<string, double>(
                    StringComparer.OrdinalIgnoreCase);

                JObject currency = await GetJsonAsync(
                    ApiRoot + "/stash/current/currency/overview?league=" +
                    Uri.EscapeDataString(league) +
                    "&type=Currency");
                ReadCurrencyLines(currency, chaosPrices);

                double divineChaos;
                if (!chaosPrices.TryGetValue("Divine Orb", out divineChaos) ||
                    divineChaos <= 0d)
                {
                    throw new InvalidOperationException(
                        "The price feed did not contain a valid Divine Orb rate.");
                }

                foreach (string exchangeType in ExchangeTypes)
                {
                    try
                    {
                        JObject exchangeData = await GetJsonAsync(
                            ApiRoot + "/exchange/current/overview?league=" +
                            Uri.EscapeDataString(league) +
                            "&type=" +
                            Uri.EscapeDataString(exchangeType));
                        ReadExchangeLines(
                            exchangeData,
                            chaosPrices,
                            divineChaos);
                    }
                    catch (Exception ex)
                    {
                        AllflameLog.Info(
                            "PRICES_CATEGORY_FAILED|Type=" + exchangeType +
                            "|Mode=Exchange" +
                            "|Error=" + ex.GetType().Name +
                            "|Message=" + Sanitize(ex.Message));
                    }
                }

                foreach (string itemType in ItemTypes)
                {
                    try
                    {
                        JObject itemData = await GetJsonAsync(
                            ApiRoot + "/stash/current/item/overview?league=" +
                            Uri.EscapeDataString(league) +
                            "&type=" +
                            Uri.EscapeDataString(itemType));
                        ReadItemLines(itemData, chaosPrices);
                    }
                    catch (Exception ex)
                    {
                        AllflameLog.Info(
                            "PRICES_CATEGORY_FAILED|Type=" + itemType +
                            "|Error=" + ex.GetType().Name +
                            "|Message=" + Sanitize(ex.Message));
                    }
                }

                var divinePrices = new Dictionary<string, double>(
                    StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<string, double> pair in chaosPrices)
                {
                    if (!String.IsNullOrWhiteSpace(pair.Key) && pair.Value >= 0d)
                    {
                        divinePrices[pair.Key.Trim()] = pair.Value / divineChaos;
                    }
                }
                divinePrices["Divine Orb"] = 1d;
                divinePrices["Chaos Orb"] = 1d / divineChaos;

                BackupExistingPriceFile();
                lock (Sync)
                {
                    _prices = divinePrices;
                    _league = league;
                    _divineChaos = divineChaos;
                    _updatedUtc = DateTime.UtcNow;
                    RebuildNormalizedPrices();
                }
                ApplyFixedPrices();
                Save();
                AllflameLog.Info(
                    "PRICES_UPDATE_DONE|League=" + league +
                    "|Count=" + Count +
                    "|DivineChaos=" +
                    divineChaos.ToString("0.##", CultureInfo.InvariantCulture));
                return true;
            }
            catch (Exception ex)
            {
                AllflameLog.Error("PRICES_UPDATE_FAILED", ex);
                AllflameNotifications.SendPriceWarning(
                    "Price update failed for league `" + league +
                    "`. The existing cache will continue to be used.");
                return false;
            }
            finally
            {
                Interlocked.Exchange(ref _updateRunning, 0);
            }
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "AllflameBot/0.21 price-cache");
            return client;
        }

        private static async Task<JObject> GetJsonAsync(string url)
        {
            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                string body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        "HTTP " + (int)response.StatusCode + " from price feed.");
                }
                return JObject.Parse(body);
            }
        }

        private static void ReadCurrencyLines(
            JObject source,
            IDictionary<string, double> output)
        {
            JArray lines = source == null ? null : source["lines"] as JArray;
            if (lines == null)
            {
                return;
            }
            foreach (JToken line in lines)
            {
                string name = (string)line["currencyTypeName"];
                double? chaos = (double?)line["chaosEquivalent"];
                if (!String.IsNullOrWhiteSpace(name) &&
                    !String.Equals(
                        name.Trim(),
                        "Dead Man's Sulphur",
                        StringComparison.OrdinalIgnoreCase) &&
                    chaos.HasValue &&
                    chaos.Value >= 0d)
                {
                    output[name.Trim()] = chaos.Value;
                }
            }
            output["Chaos Orb"] = 1d;
        }

        private static void ReadItemLines(
            JObject source,
            IDictionary<string, double> output)
        {
            JArray lines = source == null ? null : source["lines"] as JArray;
            if (lines == null)
            {
                return;
            }
            foreach (JToken line in lines)
            {
                string name = (string)line["name"];
                double? chaos = (double?)line["chaosValue"];
                if (!String.IsNullOrWhiteSpace(name) &&
                    chaos.HasValue &&
                    chaos.Value >= 0d)
                {
                    string key = name.Trim();
                    double existing;
                    if (!output.TryGetValue(key, out existing) ||
                        (chaos.Value > 0d &&
                         (existing <= 0d || chaos.Value < existing)))
                    {
                        output[key] = chaos.Value;
                    }
                }
            }
        }

        private static void ReadExchangeLines(
            JObject source,
            IDictionary<string, double> output,
            double divineChaos)
        {
            JArray lines = source == null ? null : source["lines"] as JArray;
            JObject core = source == null ? null : source["core"] as JObject;
            if (lines == null || core == null)
            {
                return;
            }

            string primary = (string)core["primary"] ?? "chaos";
            double primaryToChaos =
                String.Equals(primary, "divine", StringComparison.OrdinalIgnoreCase)
                    ? divineChaos
                    : 1d;
            foreach (JToken line in lines)
            {
                string id = (string)line["id"];
                double? primaryValue = (double?)line["primaryValue"];
                if (String.IsNullOrWhiteSpace(id) ||
                    !primaryValue.HasValue ||
                    primaryValue.Value < 0d)
                {
                    continue;
                }
                string name = String.Equals(
                    id,
                    "dead-mans-sulphur",
                    StringComparison.OrdinalIgnoreCase)
                        ? "Dead Man's Sulphur"
                        : HumanizeIdentifier(id);
                if (name.Length > 0)
                {
                    double chaosValue =
                        primaryValue.Value * primaryToChaos;
                    string maxVolumeCurrency =
                        (string)line["maxVolumeCurrency"];
                    double? maxVolumeRate =
                        (double?)line["maxVolumeRate"];
                    if (String.Equals(
                            maxVolumeCurrency,
                            "divine",
                            StringComparison.OrdinalIgnoreCase) &&
                        maxVolumeRate.HasValue &&
                        maxVolumeRate.Value > 0d)
                    {
                        // The feed expresses this as item units per Divine.
                        // Store the equivalent Chaos value so the common
                        // conversion later produces exactly 1 / rate Divine.
                        chaosValue = divineChaos / maxVolumeRate.Value;
                    }
                    output[name] = chaosValue;
                }
            }
        }

        private static void RebuildNormalizedPrices()
        {
            var normalized = new Dictionary<string, double>(
                StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, double> pair in _prices)
            {
                string key = NormalizeName(pair.Key);
                if (key.Length > 0)
                {
                    normalized[key] = pair.Value;
                }
            }
            _normalizedPrices = normalized;
        }

        private static string NormalizeName(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }
            return new string(
                value
                    .Where(Char.IsLetterOrDigit)
                    .Select(Char.ToLowerInvariant)
                    .ToArray());
        }

        private static string HumanizeIdentifier(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }
            string spaced = value.Trim().Replace('-', ' ').Replace('_', ' ');
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(spaced);
        }

        private static void BackupExistingPriceFile()
        {
            try
            {
                if (!File.Exists(PricesFilePath))
                {
                    return;
                }
                Directory.CreateDirectory(BackupsPath);
                string stamp = DateTime.Now.ToString(
                    "yyyy-MM-dd_HH-mm-ss-fff");
                string processId = Process.GetCurrentProcess().Id.ToString();
                string backup = Path.Combine(
                    BackupsPath,
                    "prices_" + stamp +
                    "_pid-" + processId + ".json");
                int suffix = 1;
                while (File.Exists(backup))
                {
                    backup = Path.Combine(
                        BackupsPath,
                        "prices_" + stamp +
                        "_pid-" + processId +
                        "-" + suffix + ".json");
                    suffix++;
                }
                File.Copy(PricesFilePath, backup, false);

                FileInfo[] backups = new DirectoryInfo(BackupsPath)
                    .GetFiles("prices_*.json")
                    .OrderByDescending(file => file.LastWriteTimeUtc)
                    .ToArray();
                for (int index = 10; index < backups.Length; index++)
                {
                    backups[index].Delete();
                }
            }
            catch (Exception ex)
            {
                AllflameLog.Info(
                    "PRICES_BACKUP_FAILED|Error=" + ex.GetType().Name +
                    "|Message=" + Sanitize(ex.Message));
            }
        }

        private static string Sanitize(string value)
        {
            return String.IsNullOrEmpty(value)
                ? String.Empty
                : value.Replace("\r", " ").Replace("\n", " ");
        }
    }
}
