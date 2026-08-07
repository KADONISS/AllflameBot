using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllflameBot
{
    internal sealed class AllflameLootEntry
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double UnitDivines { get; set; }
        public double TotalDivines
        {
            get { return UnitDivines * Quantity; }
        }
        public bool IsPriced
        {
            get { return UnitDivines > 0d; }
        }
    }

    internal sealed class AllflameRealtimeMap
    {
        public string Type { get; set; }
        public int Number { get; set; }
        public string AreaName { get; set; }
        public DateTime StartedUtc { get; set; }
        public int SulphurAtStart { get; set; }
        public int ChestsAtStart { get; set; }
        public double ProfitAtStart { get; set; }
    }

    internal sealed class DiscordEmbedField
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }

    internal sealed class DiscordEmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    internal sealed class DiscordEmbed
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("fields")]
        public List<DiscordEmbedField> Fields { get; set; }

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedFooter Footer { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    internal sealed class DiscordWebhookPayload
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty("embeds")]
        public List<DiscordEmbed> Embeds { get; set; }

        [JsonProperty("allowed_mentions", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordAllowedMentions AllowedMentions { get; set; }
    }

    internal sealed class DiscordAllowedMentions
    {
        [JsonProperty("parse")]
        public List<string> Parse { get; set; }
    }

    internal sealed class AllflameNotificationRequest
    {
        public List<DiscordEmbed> Embeds { get; set; }
        public string Content { get; set; }
        public bool MentionEveryone { get; set; }
        public string EventName { get; set; }
        public int Generation { get; set; }
    }

    internal static class AllflameSession
    {
        private const string SulphurPriceName = "Dead Man's Sulphur";
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, AllflameLootEntry> Loot =
            new Dictionary<string, AllflameLootEntry>(
                StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, AllflameLootEntry> MapLoot =
            new Dictionary<string, AllflameLootEntry>(
                StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> ConfirmedPickupSignatures =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static string _mapType = String.Empty;
        private static uint _mapAreaHash;
        private static int _mapRejectedItems;
        private static DateTime _startedUtc;
        private static int _chartsStarted;
        private static int _chartsCompleted;
        private static int _chartsFailed;
        private static int _voyagesStarted;
        private static int _voyagesCompleted;
        private static int _voyagesFailed;
        private static int _deaths;
        private static int _chestsOpened;
        private static int _sulphurCollected;
        private static bool _stopped;

        public static DateTime StartedUtc
        {
            get { lock (Sync) { return _startedUtc; } }
        }

        public static TimeSpan Duration
        {
            get
            {
                DateTime started = StartedUtc;
                return started == DateTime.MinValue
                    ? TimeSpan.Zero
                    : DateTime.UtcNow - started;
            }
        }

        public static int ChartsStarted
        {
            get { lock (Sync) { return _chartsStarted; } }
        }

        public static int ChartsCompleted
        {
            get { lock (Sync) { return _chartsCompleted; } }
        }

        public static int VoyagesStarted
        {
            get { lock (Sync) { return _voyagesStarted; } }
        }

        public static int VoyagesCompleted
        {
            get { lock (Sync) { return _voyagesCompleted; } }
        }

        public static int Deaths
        {
            get { lock (Sync) { return _deaths; } }
        }

        public static int ChestsOpened
        {
            get { lock (Sync) { return _chestsOpened; } }
        }

        public static int SulphurCollected
        {
            get { lock (Sync) { return _sulphurCollected; } }
        }

        public static int TotalRuns
        {
            get
            {
                lock (Sync)
                {
                    return _chartsCompleted + _chartsFailed +
                        _voyagesCompleted + _voyagesFailed;
                }
            }
        }

        public static int SuccessfulRuns
        {
            get { return ChartsCompleted + VoyagesCompleted; }
        }

        public static double SuccessRate
        {
            get
            {
                int total = TotalRuns;
                return total <= 0 ? 100d : SuccessfulRuns * 100d / total;
            }
        }

        public static double LootProfitDivines
        {
            get
            {
                lock (Sync)
                {
                    return Loot.Values.Sum(entry => entry.TotalDivines);
                }
            }
        }

        public static double SulphurUnitDivines
        {
            get
            {
                double unitPrice;
                return AllflameSettings.Instance.EnablePricing &&
                    AllflamePrices.TryGetUnitPrice(
                        SulphurPriceName,
                        out unitPrice)
                    ? unitPrice
                    : 0d;
            }
        }

        public static double SulphurProfitDivines
        {
            get
            {
                return SulphurCollected * SulphurUnitDivines;
            }
        }

        public static double ProfitDivines
        {
            get
            {
                return LootProfitDivines + SulphurProfitDivines;
            }
        }

        public static double ProfitPerHour
        {
            get
            {
                double hours = Duration.TotalHours;
                return hours <= 0.01d ? 0d : ProfitDivines / hours;
            }
        }

        public static int PricedItemTypes
        {
            get
            {
                int lootTypes;
                int sulphur;
                lock (Sync)
                {
                    lootTypes = Loot.Values.Count(entry => entry.IsPriced);
                    sulphur = _sulphurCollected;
                }
                return lootTypes +
                    (sulphur > 0 && SulphurUnitDivines > 0d ? 1 : 0);
            }
        }

        public static int UnpricedItemTypes
        {
            get
            {
                int lootTypes;
                int sulphur;
                lock (Sync)
                {
                    lootTypes = Loot.Values.Count(entry => !entry.IsPriced);
                    sulphur = _sulphurCollected;
                }
                return lootTypes +
                    (sulphur > 0 && SulphurUnitDivines <= 0d ? 1 : 0);
            }
        }

        public static void Start()
        {
            lock (Sync)
            {
                Loot.Clear();
                MapLoot.Clear();
                ConfirmedPickupSignatures.Clear();
                _mapType = String.Empty;
                _mapAreaHash = 0;
                _mapRejectedItems = 0;
                _startedUtc = DateTime.UtcNow;
                _chartsStarted = 0;
                _chartsCompleted = 0;
                _chartsFailed = 0;
                _voyagesStarted = 0;
                _voyagesCompleted = 0;
                _voyagesFailed = 0;
                _deaths = 0;
                _chestsOpened = 0;
                _sulphurCollected = 0;
                _stopped = false;
            }
            AllflameNotifications.Reset();
            AllflameLog.Info("SESSION_START|Utc=" + StartedUtc.ToString("o"));
        }

        public static bool Stop()
        {
            lock (Sync)
            {
                if (_stopped || _startedUtc == DateTime.MinValue)
                {
                    return false;
                }
                _stopped = true;
                return true;
            }
        }

        public static void RecordChartStarted()
        {
            int number;
            lock (Sync)
            {
                _chartsStarted++;
                number = _chartsStarted;
            }
            BeginMapLedger("Chart", AllflameContext.AreaHash);
            AllflameNotifications.SendMapStarted("Chart", number);
        }

        public static void RecordChartCompleted(bool success)
        {
            int number;
            lock (Sync)
            {
                if (success)
                {
                    _chartsCompleted++;
                }
                else
                {
                    _chartsFailed++;
                }
                number = _chartsStarted;
            }
            CommitMapLedger(success);
            AllflameNotifications.SendMapCompleted("Chart", number, success);
        }

        public static void RecordVoyageStarted()
        {
            int number;
            lock (Sync)
            {
                _voyagesStarted++;
                number = _voyagesStarted;
            }
            BeginMapLedger("Voyage", AllflameContext.AreaHash);
            AllflameNotifications.SendMapStarted("Voyage", number);
        }

        public static void RecordVoyageCompleted(bool success)
        {
            int number;
            lock (Sync)
            {
                if (success)
                {
                    _voyagesCompleted++;
                }
                else
                {
                    _voyagesFailed++;
                }
                number = _voyagesStarted;
            }
            CommitMapLedger(success);
            AllflameNotifications.SendMapCompleted("Voyage", number, success);
        }

        public static void RecordDeath()
        {
            lock (Sync) { _deaths++; }
        }

        public static void RecordChestOpened()
        {
            lock (Sync) { _chestsOpened++; }
        }

        public static void RecordSulphurCollected(int quantity)
        {
            quantity = Math.Max(1, quantity);
            int total;
            lock (Sync)
            {
                _sulphurCollected += quantity;
                total = _sulphurCollected;
            }
            AllflameLog.Info(
                "SESSION_SULPHUR|Quantity=" + quantity +
                "|SessionTotal=" + total +
                "|UnitDivines=" +
                SulphurUnitDivines.ToString(
                    "0.########",
                    CultureInfo.InvariantCulture) +
                "|SulphurValueDivines=" +
                SulphurProfitDivines.ToString(
                    "0.####",
                    CultureInfo.InvariantCulture) +
                "|SessionProfitDivines=" +
                ProfitDivines.ToString(
                    "0.####",
                    CultureInfo.InvariantCulture));
            AllflameNotifications.SendSulphurCollected(quantity, total);
        }

        public static void RecordLoot(
            Item item,
            string source,
            int confirmedQuantity,
            int inventoryBefore,
            int inventoryAfter,
            string pickupSignature)
        {
            if (item == null)
            {
                return;
            }

            AllflamePriceQuote quote = AllflamePrices.Quote(item);
            int quantity = Math.Max(1, confirmedQuantity);
            AllflameLootEntry entry;
            lock (Sync)
            {
                if (!String.IsNullOrWhiteSpace(pickupSignature) &&
                    !ConfirmedPickupSignatures.Add(pickupSignature))
                {
                    AllflameLog.Info(
                        "NOTIFICATION_PICKUP_REJECTED|Item=" +
                        quote.Name.Replace("|", "/") +
                        "|Reason=DuplicatePickupSignature|PlayerDead=False|AreaChanged=False|InventoryConfirmed=True");
                    return;
                }
                Dictionary<string, AllflameLootEntry> destination =
                    String.IsNullOrWhiteSpace(_mapType) ? Loot : MapLoot;
                if (!destination.TryGetValue(quote.Name, out entry))
                {
                    entry = new AllflameLootEntry
                    {
                        Name = quote.Name,
                        UnitDivines = quote.UnitDivines
                    };
                    destination[quote.Name] = entry;
                }
                entry.Quantity += quantity;
                if (quote.UnitDivines > 0d)
                {
                    entry.UnitDivines = quote.UnitDivines;
                }
            }

            AllflameLog.Info(
                "NOTIFICATION_PICKUP_CONFIRMED|MapType=" +
                (String.IsNullOrWhiteSpace(_mapType) ? "None" : _mapType) +
                "|AreaHash=" + _mapAreaHash +
                "|Item=" + quote.Name.Replace("|", "/") +
                "|Quantity=" + quantity +
                "|InventoryBefore=" + inventoryBefore +
                "|InventoryAfter=" + inventoryAfter +
                "|UnitDivines=" +
                quote.UnitDivines.ToString("0.####", CultureInfo.InvariantCulture) +
                "|CandidateMinDivines=" +
                quote.MinCandidateDivines.ToString(
                    "0.####",
                    CultureInfo.InvariantCulture) +
                "|CandidateMaxDivines=" +
                quote.MaxCandidateDivines.ToString(
                    "0.####",
                    CultureInfo.InvariantCulture) +
                "|ArtResolved=" + quote.ResolvedByArtwork +
                "|AmbiguousUnique=" + quote.IsAmbiguousUnique +
                "|UnresolvedUnique=" + quote.IsUnresolvedUnique +
                "|ValueDivines=" +
                (quote.UnitDivines * quantity).ToString("0.####", CultureInfo.InvariantCulture) +
                "|Signature=" + (pickupSignature ?? String.Empty).Replace("|", "/") +
                "|Source=" + (source ?? String.Empty).Replace("|", "/"));

            AllflameSettings settings = AllflameSettings.Instance;
            if (settings.NotifyValuableItems &&
                (quote.TotalDivines >= settings.ValuableItemMinDivines ||
                 (quote.IsAmbiguousUnique &&
                  quote.MaxCandidateDivines * quantity >=
                    settings.ValuableItemMinDivines)))
            {
                AllflameNotifications.SendValuableItem(quote, source);
            }
        }

        public static void RecordRejectedPickup()
        {
            lock (Sync)
            {
                if (!String.IsNullOrWhiteSpace(_mapType))
                {
                    _mapRejectedItems++;
                }
            }
        }

        private static void BeginMapLedger(string mapType, uint areaHash)
        {
            lock (Sync)
            {
                MapLoot.Clear();
                _mapType = mapType ?? String.Empty;
                _mapAreaHash = areaHash;
                _mapRejectedItems = 0;
            }
        }

        public static void CommitActiveMapForStop()
        {
            CommitMapLedger(false);
        }

        private static void CommitMapLedger(bool success)
        {
            string mapType;
            int confirmed;
            int rejected;
            double profit;
            lock (Sync)
            {
                mapType = _mapType;
                confirmed = MapLoot.Values.Sum(entry => entry.Quantity);
                rejected = _mapRejectedItems;
                profit = MapLoot.Values.Sum(entry => entry.TotalDivines);
                foreach (AllflameLootEntry staged in MapLoot.Values)
                {
                    AllflameLootEntry committed;
                    if (!Loot.TryGetValue(staged.Name, out committed))
                    {
                        committed = new AllflameLootEntry
                        {
                            Name = staged.Name,
                            UnitDivines = staged.UnitDivines
                        };
                        Loot[staged.Name] = committed;
                    }
                    committed.Quantity += staged.Quantity;
                    if (staged.UnitDivines > 0d)
                    {
                        committed.UnitDivines = staged.UnitDivines;
                    }
                }
                MapLoot.Clear();
                _mapType = String.Empty;
                _mapAreaHash = 0;
                _mapRejectedItems = 0;
            }
            if (!String.IsNullOrWhiteSpace(mapType))
            {
                AllflameLog.Info(
                    "NOTIFICATION_MAP_COMMIT|MapType=" + mapType +
                    "|Success=" + success +
                    "|ConfirmedItems=" + confirmed +
                    "|RejectedItems=" + rejected +
                    "|CommittedProfitDivines=" +
                    profit.ToString("0.####", CultureInfo.InvariantCulture));
            }
        }

        public static List<AllflameLootEntry> GetLootSnapshot()
        {
            lock (Sync)
            {
                return Loot.Values
                    .Select(entry => new AllflameLootEntry
                    {
                        Name = entry.Name,
                        Quantity = entry.Quantity,
                        UnitDivines = entry.UnitDivines
                    })
                    .OrderByDescending(entry => entry.TotalDivines)
                    .ThenByDescending(entry => entry.Quantity)
                    .ThenBy(entry => entry.Name)
                    .ToList();
            }
        }
    }

    internal static class AllflameNotifications
    {
        private static readonly object Sync = new object();
        private static readonly HttpClient Client = CreateClient();
        private static readonly Queue<AllflameNotificationRequest> ImmediateQueue =
            new Queue<AllflameNotificationRequest>();
        private static bool _lowChartsSent;
        private static bool _noChartsSent;
        private static bool _lowVoyageStashSent;
        private static bool _priceWarningSent;
        private static string _stopReason = String.Empty;
        private static string _stopDetail = String.Empty;
        private static AllflameRealtimeMap _activeChart;
        private static AllflameRealtimeMap _activeVoyage;
        private static string _activityStatus = "Session starting";
        private static string _lastEvent = "Waiting for the first map";
        private static string _warningText = String.Empty;
        private static string _characterName = "Unknown Character";
        private static string _pendingAlert = String.Empty;
        private static bool _pendingEveryone;
        private static string _boundaryEvent = "SessionStart";
        private static string _dashboardMessageId = String.Empty;
        private static string _dashboardWebhook = String.Empty;
        private static bool _dashboardDirty;
        private static bool _dashboardWorkerRunning;
        private static bool _dashboardStopped;
        private static DateTime _dashboardNotBeforeUtc = DateTime.MinValue;
        private static DateTime _lastPeriodicUpdateUtc = DateTime.MinValue;
        private static DateTime _lastSulphurUpdateUtc = DateTime.MinValue;
        private static bool _immediateWorkerRunning;
        private static int _dashboardGeneration;
        private static DateTime _lastDeliveryUtc = DateTime.MinValue;
        private static string _lastDeliveryStatus = "Never";
        private static string _lastDeliveryError = String.Empty;

        public static void Reset()
        {
            lock (Sync)
            {
                _lowChartsSent = false;
                _noChartsSent = false;
                _lowVoyageStashSent = false;
                _priceWarningSent = false;
                _stopReason = String.Empty;
                _stopDetail = String.Empty;
                _activeChart = null;
                _activeVoyage = null;
                _activityStatus = "Session starting";
                _lastEvent = "Waiting for the first map";
                _warningText = String.Empty;
                _characterName = ResolveCharacterName();
                _pendingAlert = String.Empty;
                _pendingEveryone = false;
                _boundaryEvent = "SessionStart";
                _dashboardMessageId = String.Empty;
                _dashboardWebhook = String.Empty;
                _dashboardDirty = false;
                _dashboardWorkerRunning = false;
                _dashboardStopped = false;
                _dashboardNotBeforeUtc = DateTime.MinValue;
                _lastPeriodicUpdateUtc = DateTime.MinValue;
                _lastSulphurUpdateUtc = DateTime.MinValue;
                ImmediateQueue.Clear();
                _immediateWorkerRunning = false;
                _dashboardGeneration++;
            }
            AllflameLog.Info(
                "NOTIFICATION_SESSION_START|Character=" + _characterName.Replace("|", "/") +
                "|CharacterResolved=" +
                !String.Equals(_characterName, "Unknown Character", StringComparison.Ordinal) +
                "|Version=" + AllflameVersion.Current);
        }

        public static void StartSessionDashboard()
        {
            lock (Sync)
            {
                _dashboardStopped = false;
                _activityStatus = "Session running";
                _lastEvent = "Session started";
                _boundaryEvent = "SessionStart";
                _lastPeriodicUpdateUtc = DateTime.UtcNow;
            }
            RequestBoundaryUpdate("SessionStart");
        }

        public static void SendMapStarted(string mapType, int number)
        {
            var snapshot = new AllflameRealtimeMap
            {
                Type = mapType,
                Number = Math.Max(1, number),
                AreaName = AllflameContext.AreaName,
                StartedUtc = DateTime.UtcNow,
                SulphurAtStart = AllflameSession.SulphurCollected,
                ChestsAtStart = AllflameSession.ChestsOpened,
                ProfitAtStart = AllflameSession.ProfitDivines
            };
            lock (Sync)
            {
                if (String.Equals(
                    mapType,
                    "Voyage",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _activeVoyage = snapshot;
                }
                else
                {
                    _activeChart = snapshot;
                }
                _activityStatus =
                    "Running " + mapType + " #" + snapshot.Number;
                _lastEvent =
                    "Entered " +
                    (String.IsNullOrWhiteSpace(snapshot.AreaName)
                        ? AllflameContext.AreaId
                        : snapshot.AreaName) +
                    " | Confirmed Sulphur: " +
                    AllflameSession.SulphurCollected;
                _boundaryEvent = "MapEnter";
            }
            ResolveCharacterNameAtBoundary();
            AllflameLog.Info(
                "NOTIFICATION_MAP_ENTER|Character=" + _characterName.Replace("|", "/") +
                "|MapType=" + mapType +
                "|MapNumber=" + snapshot.Number +
                "|AreaId=" + AllflameContext.AreaId +
                "|AreaHash=" + AllflameContext.AreaHash);
            if (AllflameSettings.Instance.NotifyMapStarted)
            {
                RequestBoundaryUpdate(
                    String.Equals(
                        mapType,
                        "Voyage",
                        StringComparison.OrdinalIgnoreCase)
                        ? "VoyageStart"
                        : "ChartStart");
            }
        }

        public static void SendMapCompleted(
            string mapType,
            int number,
            bool success)
        {
            lock (Sync)
            {
                bool voyage = String.Equals(
                    mapType,
                    "Voyage",
                    StringComparison.OrdinalIgnoreCase);
                if (voyage)
                {
                    _activeVoyage = null;
                }
                else
                {
                    _activeChart = null;
                }
                _activityStatus =
                    mapType + (success ? " completed" : " failed");
                _lastEvent = success
                    ? mapType +
                      " finished; session totals updated"
                    : mapType + " ended without successful completion";
                _boundaryEvent = String.Equals(
                    mapType,
                    "Voyage",
                    StringComparison.OrdinalIgnoreCase)
                    ? "VoyageComplete"
                    : "ChartComplete";
            }
            ResolveCharacterNameAtBoundary();
            if (AllflameSettings.Instance.NotifyMapCompleted)
            {
                RequestDashboardUpdate(true);
            }
        }

        public static void SendSulphurCollected(int quantity, int total)
        {
            bool updateDashboard = false;
            lock (Sync)
            {
                _lastEvent =
                    "Sulphur +" + Math.Max(1, quantity) +
                    " confirmed | Session total: " +
                    Math.Max(0, total);
                if (AllflameSettings.Instance.NotifySulphurCollected &&
                    (DateTime.UtcNow - _lastSulphurUpdateUtc).TotalSeconds >= 30d)
                {
                    _lastSulphurUpdateUtc = DateTime.UtcNow;
                    _boundaryEvent = "SulphurUpdate";
                    updateDashboard = true;
                }
            }
            if (updateDashboard)
            {
                RequestDashboardUpdate(false);
            }
        }

        public static void Tick()
        {
            AllflameSettings settings = AllflameSettings.Instance;
            if (AllflamePrices.ShouldAutoUpdate())
            {
                Task<bool> ignoredUpdate =
                    AllflamePrices.UpdateAsync(settings.PricingLeague);
            }
            if (!settings.DiscordNotificationsEnabled)
            {
                return;
            }

            if (settings.SendPeriodicSummary)
            {
                bool periodicUpdate;
                lock (Sync)
                {
                    periodicUpdate =
                        _lastPeriodicUpdateUtc == DateTime.MinValue ||
                        DateTime.UtcNow - _lastPeriodicUpdateUtc >=
                        TimeSpan.FromMinutes(
                            Math.Max(
                                1,
                                Math.Min(
                                    60,
                                    settings.NotificationIntervalMinutes)));
                    if (periodicUpdate)
                    {
                        _lastPeriodicUpdateUtc = DateTime.UtcNow;
                        _boundaryEvent = "PeriodicSummary";
                    }
                }
                if (periodicUpdate)
                {
                    RequestDashboardUpdate(false);
                }
            }

            DateTime priceUpdated = AllflamePrices.UpdatedUtc;
            bool priceDataStale =
                settings.EnablePricing &&
                settings.NotifyPriceWarnings &&
                AllflameSession.Duration >= TimeSpan.FromMinutes(2) &&
                (priceUpdated == DateTime.MinValue ||
                 DateTime.UtcNow - priceUpdated >=
                    TimeSpan.FromHours(
                        Math.Max(48, settings.PriceUpdateHours * 2)));
            if (priceDataStale)
            {
                SendPriceWarning(
                    priceUpdated == DateTime.MinValue
                        ? "No price cache is available. Profit will remain zero until prices are updated."
                        : "Price data is stale. Last successful update: " +
                          priceUpdated.ToLocalTime().ToString("yyyy-MM-dd HH:mm") + ".");
            }

            if (AllflameContext.Phase != AllflamePhase.SovereignHub &&
                AllflameContext.Phase != AllflamePhase.VoyagePlanning)
            {
                return;
            }

            int inventory = Math.Max(0, AllflameContext.InventoryChartCount);
            int stash = Math.Max(0, settings.LastVoyageStashTotal);
            bool stashKnown =
                settings.RunInventoryChartsOnly ||
                stash > 0 ||
                (!String.IsNullOrWhiteSpace(settings.LastChartStorageScan) &&
                 settings.LastChartStorageScan.IndexOf(
                     "VoyageStash:",
                     StringComparison.OrdinalIgnoreCase) >= 0);
            if (!stashKnown)
            {
                return;
            }
            int voyageStashThreshold = Math.Max(
                0,
                settings.VoyageStashEveryoneThreshold);
            if (stash > voyageStashThreshold)
            {
                lock (Sync)
                {
                    _lowVoyageStashSent = false;
                }
            }
            else if (!settings.NotifyLowVoyageStash)
            {
                lock (Sync)
                {
                    _lowVoyageStashSent = false;
                }
            }
            else if (settings.NotifyLowVoyageStash)
            {
                bool sendVoyageStashAlert;
                lock (Sync)
                {
                    sendVoyageStashAlert =
                        !_lowVoyageStashSent;
                    _lowVoyageStashSent = true;
                }
                if (sendVoyageStashAlert)
                {
                    MarkPendingAlert(
                        "Low Voyage Stash supply: " + stash +
                        " (threshold " + voyageStashThreshold + ").",
                        settings.TagEveryoneForLowVoyageStash);
                }
            }
            int total = inventory + stash;
            if (total <= 0)
            {
                bool shouldSend;
                lock (Sync)
                {
                    shouldSend = settings.NotifyEmptyChartSupply && !_noChartsSent;
                    _noChartsSent = true;
                }
                if (shouldSend)
                {
                    SendChartSupply(true, inventory, stash);
                }
            }
            else if (total <= Math.Max(0, settings.LowChartWarningThreshold))
            {
                bool shouldSend;
                lock (Sync)
                {
                    shouldSend = settings.NotifyLowChartSupply && !_lowChartsSent;
                    _lowChartsSent = true;
                }
                if (shouldSend)
                {
                    SendChartSupply(false, inventory, stash);
                }
            }
            else
            {
                lock (Sync)
                {
                    _lowChartsSent = false;
                    _noChartsSent = false;
                    _lowVoyageStashSent = false;
                }
            }
            if (!settings.NotifyEmptyChartSupply)
            {
                lock (Sync)
                {
                    _noChartsSent = false;
                }
            }
            if (!settings.NotifyLowChartSupply)
            {
                lock (Sync)
                {
                    _lowChartsSent = false;
                }
            }
        }

        public static void RecordStopReason(string reason, string detail)
        {
            lock (Sync)
            {
                _stopReason = reason ?? String.Empty;
                _stopDetail = detail ?? String.Empty;
                _lastEvent = "Stop requested: " +
                    HumanizeStopReason(_stopReason);
            }
        }

        public static void SendStoppedSummary(string stopType)
        {
            if (!AllflameSession.Stop())
            {
                return;
            }
            string reason;
            string detail;
            lock (Sync)
            {
                reason = String.IsNullOrWhiteSpace(_stopReason)
                    ? "Stopped by user"
                    : HumanizeStopReason(_stopReason);
                detail = _stopDetail;
            }
            AllflameSession.CommitActiveMapForStop();
            lock (Sync)
            {
                _boundaryEvent = "TerminalStop";
            }
            AllflameLog.Info(
                "NOTIFICATION_TERMINAL_FLUSH|StopType=" +
                (String.IsNullOrWhiteSpace(stopType) ? "UserStop" : stopType) +
                "|Reason=" + reason.Replace("|", "/") +
                "|ConfirmedItems=Committed|DiscardedPendingItems=0");
            if (AllflameSettings.Instance.NotifyRecoveryFailures &&
                !String.Equals(
                    stopType,
                    "UserStop",
                    StringComparison.OrdinalIgnoreCase))
            {
                MarkPendingAlert(
                    "Bot stopped after a runtime recovery or terminal condition: " +
                    reason,
                    false);
            }
            if (!AllflameSettings.Instance.NotifyTerminalStop)
            {
                return;
            }
            SendSessionSummary(reason, true);
            if (!String.IsNullOrWhiteSpace(detail))
            {
                AllflameLog.Info(
                    "NOTIFICATION_STOP_DETAIL|Reason=" + reason +
                    "|Detail=" + detail);
            }
        }

        public static void SendChartAbandoned()
        {
            AllflameSession.RecordDeath();
            lock (Sync)
            {
                _activityStatus = "Chart abandoned after death";
                _lastEvent =
                    "Death | Exploration " +
                    AllflameRunState.ExplorationPercent.ToString("0.#") +
                    "% | Lanterns " +
                    AllflameContext.LanternCounterCurrent + "/" +
                    AllflameContext.LanternCounterMaximum;
                _warningText =
                    "The current Chart cannot be re-entered after death.";
            }
            if (AllflameSettings.Instance.NotifyDeaths)
            {
                MarkPendingAlert(_warningText, false);
            }
            else
            {
                RequestDashboardUpdate(false);
            }
        }

        public static void SendVoyageAbandoned()
        {
            AllflameSession.RecordDeath();
            lock (Sync)
            {
                _activityStatus = "Voyage abandoned after death";
                _lastEvent = "Voyage death confirmed";
                _warningText =
                    "The current Voyage could not be resumed.";
            }
            if (AllflameSettings.Instance.NotifyDeaths)
            {
                MarkPendingAlert(_warningText, false);
            }
            else
            {
                RequestDashboardUpdate(false);
            }
        }

        public static void SendValuableItem(
            AllflamePriceQuote quote,
            string source)
        {
            string valueText;
            if (quote.IsAmbiguousUnique &&
                quote.MaxCandidateDivines > 0d)
            {
                valueText = quote.MinCandidateDivines > 0d &&
                    Math.Abs(
                        quote.MaxCandidateDivines -
                        quote.MinCandidateDivines) > 0.0001d
                    ? FormatDivines(
                        quote.MinCandidateDivines * quote.Quantity) +
                      " - " +
                      FormatDivines(
                        quote.MaxCandidateDivines * quote.Quantity)
                    : "up to " +
                      FormatDivines(
                        quote.MaxCandidateDivines * quote.Quantity);
            }
            else
            {
                valueText = FormatDivines(quote.TotalDivines);
            }
            lock (Sync)
            {
                _lastEvent =
                    "Valuable item: " + quote.Name +
                    " x" + quote.Quantity +
                    " | " + valueText;
            }
            var embed = NewEmbed(
                "Allflame Bot - Valuable Loot",
                BuildTable(new[]
                {
                    Pair("Item", quote.Name),
                    Pair("Quantity", quote.Quantity.ToString()),
                    Pair("Value", valueText),
                    Pair("Source", source ?? "Confirmed pickup")
                }),
                0xF1C40F);
            embed.Footer = new DiscordEmbedFooter
            {
                Text = PriceFooter()
            };
            Send(
                new List<DiscordEmbed> { embed },
                AllflameSettings.Instance.TagEveryoneForValuableItems
                    ? "@everyone"
                    : null,
                AllflameSettings.Instance.TagEveryoneForValuableItems,
                "ValuableLoot");
            RequestDashboardUpdate(false);
        }

        public static void SendSecretBottleReward(
            AllflamePriceQuote quote,
            string rarity,
            string decision,
            string placement)
        {
            if (quote == null)
            {
                return;
            }

            AllflameSettings settings = AllflameSettings.Instance;
            int quantity = Math.Max(1, quote.Quantity);
            double valuableValue = quote.IsAmbiguousUnique
                ? quote.MaxCandidateDivines * quantity
                : quote.TotalDivines;
            bool bottleNotification = settings.NotifySecretBottleRewards;
            bool valuableNotification =
                settings.NotifyValuableItems &&
                valuableValue >= settings.ValuableItemMinDivines;
            bool tagEveryone =
                valuableNotification &&
                settings.TagEveryoneForValuableItems;
            bool immediateNotification =
                bottleNotification || valuableNotification;

            AllflameLog.Info(
                "SECRET_BOTTLE_NOTIFICATION" +
                "|Name=" + (quote.Name ?? "Unknown").Replace("|", "/") +
                "|Quantity=" + quantity +
                "|Rarity=" + (rarity ?? String.Empty).Replace("|", "/") +
                "|InventoryConfirmed=True" +
                "|ValueDivines=" +
                valuableValue.ToString("0.####", CultureInfo.InvariantCulture) +
                "|BottleNotification=" + bottleNotification +
                "|ValuableNotification=" + valuableNotification +
                "|TagEveryone=" + tagEveryone +
                "|Delivery=" +
                (immediateNotification
                    ? "ImmediateWebhook"
                    : "Disabled") +
                "|Decision=" + (decision ?? String.Empty).Replace("|", "/") +
                "|Placement=" + (placement ?? String.Empty).Replace("|", "/"));

            if (immediateNotification)
            {
                string valueText = quote.IsPriced
                    ? FormatDivines(quote.TotalDivines)
                    : quote.IsAmbiguousUnique &&
                      quote.MaxCandidateDivines > 0d
                        ? "up to " + FormatDivines(valuableValue)
                        : "Unpriced";
                var embed = NewEmbed(
                    valuableNotification
                        ? "Allflame Bot - Valuable Bottle Reward"
                        : "Allflame Bot - Bottle Reward Confirmed",
                    BuildTable(new[]
                    {
                        Pair("Item", quote.Name ?? "Unknown"),
                        Pair("Quantity", quantity.ToString()),
                        Pair("Rarity", rarity ?? "Unknown"),
                        Pair("Value", valueText),
                        Pair("Decision", decision ?? "Keep")
                    }),
                    0xE67E22);
                embed.Footer = new DiscordEmbedFooter
                {
                    Text = PriceFooter()
                };
                Send(
                    new List<DiscordEmbed> { embed },
                    tagEveryone ? "@everyone" : null,
                    tagEveryone,
                    "BottleReward");
            }

            if (valuableNotification)
            {
                lock (Sync)
                {
                    _lastEvent =
                        "Valuable bottle reward: " + quote.Name +
                        " x" + quantity +
                        " | " + FormatDivines(valuableValue);
                }
            }
            else if (bottleNotification)
            {
                lock (Sync)
                {
                    _lastEvent =
                        "Bottle reward: " + quote.Name +
                        " x" + quantity +
                        " | " + (decision ?? "Keep");
                }
            }
            if (immediateNotification)
            {
                RequestDashboardUpdate(false);
            }
        }

        public static void SendPriceWarning(string message)
        {
            if (!AllflameSettings.Instance.NotifyPriceWarnings)
            {
                return;
            }
            bool shouldSend;
            lock (Sync)
            {
                shouldSend = !_priceWarningSent;
                _priceWarningSent = true;
            }
            if (!shouldSend)
            {
                return;
            }
            lock (Sync)
            {
                _warningText = message;
            }
            MarkPendingAlert(message, false);
        }

        public static void SendTest()
        {
            var embed = NewEmbed(
                "Allflame Bot - Notification Test",
                null,
                0x0D4DCC);
            embed.Description = BuildTable(new[]
            {
                Pair("Webhook", "Connected"),
                Pair("Dashboard mode", "One live edited message"),
                Pair("Price entries", AllflamePrices.Count.ToString()),
                Pair(
                    "League",
                    String.IsNullOrWhiteSpace(AllflamePrices.League)
                        ? "Not configured"
                        : AllflamePrices.League)
            });
            Send(
                new List<DiscordEmbed> { embed },
                null,
                false,
                "Test");
        }

        public static void SendSessionSummary(string stopReason, bool stopped)
        {
            lock (Sync)
            {
                _dashboardStopped = stopped;
                if (stopped)
                {
                    _activityStatus = "Session stopped";
                    if (!String.IsNullOrWhiteSpace(stopReason))
                    {
                        _stopReason = stopReason;
                    }
                }
            }
            if (stopped)
            {
                RequestBoundaryUpdate("TerminalStop");
            }
        }

        private static List<DiscordEmbed> BuildSessionEmbeds(
            string stopReason,
            bool stopped)
        {
            AllflameSettings settings = AllflameSettings.Instance;
            var embeds = new List<DiscordEmbed>();

            string warningText;
            string stopDetail;
            string characterName;
            string activityStatus;
            string lastEvent;
            AllflameRealtimeMap activeMap;
            lock (Sync)
            {
                warningText = _warningText;
                stopDetail = _stopDetail;
                characterName = _characterName;
                activityStatus = _activityStatus;
                lastEvent = _lastEvent;
                activeMap = _activeVoyage ?? _activeChart;
            }

            DiscordEmbed stats = NewEmbed(
                characterName + " - Allflame Bot Session Dashboard",
                null,
                stopped ? 0xD64545 : 0x0D4DCC);
            var statsRows = new List<KeyValuePair<string, string>>
            {
                Pair("Status", activityStatus),
                Pair("Last event", lastEvent),
                Pair("Phase", AllflameContext.Phase.ToString()),
                Pair("Duration", FormatDuration(AllflameSession.Duration)),
                Pair("Chart Areas completed", AllflameSession.ChartsCompleted.ToString()),
                Pair("Voyages completed", AllflameSession.VoyagesCompleted.ToString()),
                Pair("Success rate", AllflameSession.SuccessRate.ToString("0.#") + "%"),
                Pair("Deaths", AllflameSession.Deaths.ToString()),
                Pair("Chests opened", AllflameSession.ChestsOpened.ToString()),
                Pair("Sulphur collected", AllflameSession.SulphurCollected.ToString())
            };
            if (activeMap != null)
            {
                statsRows.Add(Pair(
                    "Current run",
                    activeMap.Type + " #" + activeMap.Number));
                statsRows.Add(Pair(
                    "Current area",
                    String.IsNullOrWhiteSpace(activeMap.AreaName)
                        ? AllflameContext.AreaId
                        : activeMap.AreaName));
                statsRows.Add(Pair(
                    "Run duration",
                    FormatDuration(DateTime.UtcNow - activeMap.StartedUtc)));
                statsRows.Add(Pair(
                    "Run Sulphur",
                    Math.Max(
                        0,
                        AllflameSession.SulphurCollected -
                        activeMap.SulphurAtStart).ToString()));
                statsRows.Add(Pair(
                    "Run chests",
                    Math.Max(
                        0,
                        AllflameSession.ChestsOpened -
                        activeMap.ChestsAtStart).ToString()));
            }
            if (stopped)
            {
                statsRows.Add(Pair("Stop reason", stopReason));
                if (!String.IsNullOrWhiteSpace(stopDetail))
                {
                    statsRows.Add(Pair("Stop detail", stopDetail));
                }
            }
            stats.Description = BuildTable(statsRows);
            if (!String.IsNullOrWhiteSpace(warningText))
            {
                stats.Fields.Add(new DiscordEmbedField
                {
                    Name = "Alert",
                    Value = Truncate(warningText, 1000),
                    Inline = false
                });
            }
            embeds.Add(stats);

            DiscordEmbed resources = NewEmbed("Allflame Bot - Supply and Runner", null, 0x3498DB);
            int voyageRequired = Math.Max(1, settings.ChartsRequiredForVoyage);
            int voyageAvailable = Math.Max(0, AllflameRunState.VoyageAvailableCharts);
            resources.Description = BuildTable(new[]
            {
                Pair(
                    "Charts in inventory",
                    Math.Max(0, AllflameContext.InventoryChartCount).ToString()),
                Pair(
                    "Charts in Voyage Stash",
                    Math.Max(0, settings.LastVoyageStashTotal).ToString()),
                Pair(
                    "Charts in Chart Storage",
                    Math.Max(0, settings.LastChartStorageTotal).ToString()),
                Pair(
                    "Voyage progress",
                    Math.Min(voyageAvailable, voyageRequired) + "/" +
                    voyageRequired),
                Pair(
                    "Next runner",
                    voyageAvailable >= voyageRequired
                        ? "Voyage Runner"
                        : "Chart Runner"),
                Pair(
                    "Lantern",
                    Math.Max(0, AllflameContext.LanternCounterCurrent) + "/" +
                    Math.Max(0, AllflameContext.LanternCounterMaximum))
            });
            embeds.Add(resources);

            List<AllflameLootEntry> loot = AllflameSession.GetLootSnapshot();
            DiscordEmbed items = NewEmbed("Allflame Bot - Confirmed Loot", null, 0x9B59B6);
            int maxItems = Math.Max(1, Math.Min(15, settings.DiscordMaxItemCount));
            int sulphurQuantity = AllflameSession.SulphurCollected;
            if (loot.Count == 0 && sulphurQuantity <= 0)
            {
                items.Description = BuildTable(new[]
                {
                    Pair("Status", "No tracked loot collected")
                });
            }
            else
            {
                var itemRows = new List<KeyValuePair<string, string>>();
                if (sulphurQuantity > 0)
                {
                    string sulphurValue = sulphurQuantity.ToString();
                    if (AllflameSession.SulphurUnitDivines > 0d)
                    {
                        sulphurValue += " | " +
                            FormatDivines(
                                AllflameSession.SulphurProfitDivines);
                    }
                    itemRows.Add(Pair(
                        "Dead Man's Sulphur",
                        sulphurValue));
                }
                int chestRows = Math.Max(
                    0,
                    maxItems - itemRows.Count);
                foreach (AllflameLootEntry entry in loot.Take(chestRows))
                {
                    string value = entry.Quantity.ToString();
                    if (entry.IsPriced)
                    {
                        value += " | " + FormatDivines(entry.TotalDivines);
                    }
                    itemRows.Add(Pair(entry.Name, value));
                }
                if (loot.Count > chestRows)
                {
                    itemRows.Add(Pair(
                        "Other item types",
                        (loot.Count - chestRows).ToString()));
                }
                items.Description = BuildTable(itemRows);
            }
            embeds.Add(items);

            DiscordEmbed profit = NewEmbed("Allflame Bot - Total Profit", null, 0x2ECC71);
            profit.Description = BuildTable(new[]
            {
                Pair("Chest loot", FormatDivines(AllflameSession.LootProfitDivines)),
                Pair("Sulphur value", FormatDivines(AllflameSession.SulphurProfitDivines)),
                Pair("Session profit", FormatDivines(AllflameSession.ProfitDivines)),
                Pair("Profit per hour", FormatDivines(AllflameSession.ProfitPerHour) + "/h"),
                Pair("Priced item types", AllflameSession.PricedItemTypes.ToString()),
                Pair("Unpriced item types", AllflameSession.UnpricedItemTypes.ToString())
            });
            profit.Footer = new DiscordEmbedFooter
            {
                Text = PriceFooter()
            };
            embeds.Add(profit);

            return embeds;
        }

        private static void SendChartSupply(
            bool empty,
            int inventory,
            int stash)
        {
            lock (Sync)
            {
                _warningText = empty
                    ? "No Charts remain in inventory or Voyage Stash."
                    : "Low Chart supply: " + (inventory + stash) +
                      " remaining (inventory " + inventory +
                      ", Voyage Stash " + stash + ").";
            }
            MarkPendingAlert(_warningText, false);
        }

        public static void RequestDashboardUpdate(bool immediate)
        {
            AllflameSettings settings = AllflameSettings.Instance;
            string webhook;
            string validationError;
            if (!TryGetWebhook(settings, out webhook, out validationError))
            {
                SetDeliveryStatus(
                    "Suppressed",
                    validationError);
                return;
            }

            bool startWorker = false;
            int generation;
            lock (Sync)
            {
                DateTime target = immediate
                    ? DateTime.UtcNow
                    : DateTime.UtcNow.AddSeconds(1);
                _dashboardDirty = true;
                if (immediate ||
                    _dashboardNotBeforeUtc == DateTime.MinValue ||
                    target < _dashboardNotBeforeUtc)
                {
                    _dashboardNotBeforeUtc = target;
                }
                if (!_dashboardWorkerRunning)
                {
                    _dashboardWorkerRunning = true;
                    startWorker = true;
                }
                generation = _dashboardGeneration;
            }

            if (startWorker)
            {
                Task.Run(async delegate
                {
                    try
                    {
                        await DashboardWorkerAsync(generation);
                    }
                    catch (Exception ex)
                    {
                        SetDeliveryStatus(
                            "Failed",
                            ex.GetType().Name);
                        AllflameLog.Error(
                            "NOTIFICATION_WORKER_FAILED",
                            ex);
                        lock (Sync)
                        {
                            if (generation == _dashboardGeneration)
                            {
                                _dashboardWorkerRunning = false;
                            }
                        }
                    }
                });
            }
        }

        private static void RequestBoundaryUpdate(string eventName)
        {
            lock (Sync)
            {
                _boundaryEvent = eventName ?? "Boundary";
            }
            RequestDashboardUpdate(true);
        }

        private static void MarkPendingAlert(string message, bool everyone)
        {
            if (String.IsNullOrWhiteSpace(message))
            {
                return;
            }
            bool changed = false;
            lock (Sync)
            {
                if (_pendingAlert.IndexOf(message, StringComparison.Ordinal) < 0)
                {
                    _pendingAlert = String.IsNullOrWhiteSpace(_pendingAlert)
                        ? message
                        : Truncate(_pendingAlert + "\n" + message, 1800);
                    changed = true;
                }
                _pendingEveryone = _pendingEveryone || everyone;
                _warningText = _pendingAlert;
            }
            if (changed)
            {
                RequestDashboardUpdate(true);
            }
        }

        private static string ResolveCharacterName()
        {
            try
            {
                string name = LokiPoe.Me == null ? null : LokiPoe.Me.Name;
                return String.IsNullOrWhiteSpace(name)
                    ? "Unknown Character"
                    : name.Trim();
            }
            catch
            {
                return "Unknown Character";
            }
        }

        private static void ResolveCharacterNameAtBoundary()
        {
            string resolved = ResolveCharacterName();
            if (String.Equals(resolved, "Unknown Character", StringComparison.Ordinal))
            {
                return;
            }
            lock (Sync)
            {
                _characterName = resolved;
            }
        }

        private static async Task DashboardWorkerAsync(int generation)
        {
            while (true)
            {
                DateTime notBefore;
                lock (Sync)
                {
                    if (generation != _dashboardGeneration)
                    {
                        return;
                    }
                    if (!_dashboardDirty)
                    {
                        _dashboardWorkerRunning = false;
                        _dashboardNotBeforeUtc = DateTime.MinValue;
                        return;
                    }
                    notBefore = _dashboardNotBeforeUtc;
                }

                TimeSpan delay = notBefore - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                    continue;
                }

                bool stopped;
                string reason;
                string eventName;
                string character;
                string pendingAlert;
                bool pendingEveryone;
                lock (Sync)
                {
                    if (generation != _dashboardGeneration)
                    {
                        return;
                    }
                    _dashboardDirty = false;
                    _dashboardNotBeforeUtc = DateTime.MinValue;
                    stopped = _dashboardStopped;
                    reason = String.IsNullOrWhiteSpace(_stopReason)
                        ? "Stopped by user"
                        : HumanizeStopReason(_stopReason);
                    eventName = _boundaryEvent;
                    character = _characterName;
                    pendingAlert = _pendingAlert;
                    pendingEveryone = _pendingEveryone;
                }

                List<DiscordEmbed> embeds =
                    BuildSessionEmbeds(reason, stopped);
                AllflameLog.Info(
                    "NOTIFICATION_BOUNDARY_UPDATE|Event=" + eventName +
                    "|Character=" + character.Replace("|", "/") +
                    "|MessageIdPresent=" + !String.IsNullOrWhiteSpace(_dashboardMessageId) +
                    "|EmbedCount=" + embeds.Count +
                    "|PendingAlerts=" +
                    (String.IsNullOrWhiteSpace(pendingAlert) ? 0 : 1));
                bool updated = await UpsertDashboardAsync(
                    embeds,
                    generation,
                    eventName,
                    pendingEveryone);
                if (updated)
                {
                    lock (Sync)
                    {
                        if (generation == _dashboardGeneration &&
                            String.Equals(_pendingAlert, pendingAlert, StringComparison.Ordinal))
                        {
                            _pendingAlert = String.Empty;
                            _pendingEveryone = false;
                        }
                    }
                }
            }
        }

        private static async Task<bool> UpsertDashboardAsync(
            List<DiscordEmbed> embeds,
            int generation,
            string eventName,
            bool mentionEveryone)
        {
            AllflameSettings settings = AllflameSettings.Instance;
            string webhook;
            string validationError;
            if (!TryGetWebhook(settings, out webhook, out validationError))
            {
                SetDeliveryStatus("Suppressed", validationError);
                return false;
            }

            string messageId;
            lock (Sync)
            {
                if (generation != _dashboardGeneration)
                {
                    return false;
                }
                if (!String.Equals(
                    _dashboardWebhook,
                    webhook,
                    StringComparison.Ordinal))
                {
                    _dashboardWebhook = webhook;
                    _dashboardMessageId = String.Empty;
                }
                messageId = _dashboardMessageId;
            }

            var payload = new DiscordWebhookPayload
            {
                Username = "Allflame Bot",
                Content = mentionEveryone ? "@everyone" : null,
                AllowedMentions = mentionEveryone
                    ? new DiscordAllowedMentions
                    {
                        Parse = new List<string> { "everyone" }
                    }
                    : null,
                Embeds = embeds.Take(10).ToList()
            };
            string json = JsonConvert.SerializeObject(payload);
            int lastStatus = 0;
            string lastException = "None";
            bool messageRecovery = false;
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                lock (Sync)
                {
                    if (generation != _dashboardGeneration)
                    {
                        return false;
                    }
                }
                try
                {
                    if (!String.IsNullOrWhiteSpace(messageId))
                    {
                        string editUrl = WebhookBaseUrl(webhook) + "/messages/" + messageId;
                        using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), editUrl))
                        {
                            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                            using (HttpResponseMessage response = await Client.SendAsync(request))
                            {
                                lastStatus = (int)response.StatusCode;
                                if (response.IsSuccessStatusCode)
                                {
                                    SetDeliveryStatus("Dashboard updated", String.Empty);
                                    AllflameLog.Info("NOTIFICATION_UPDATE_SUCCESS|Action=Edit|Event=" + eventName + "|Attempt=" + attempt);
                                    return true;
                                }
                                if (lastStatus == 404)
                                {
                                    lock (Sync)
                                    {
                                        if (generation == _dashboardGeneration)
                                        {
                                            _dashboardMessageId = String.Empty;
                                        }
                                    }
                                    AllflameLog.Info("NOTIFICATION_MESSAGE_RECOVERY|Reason=MessageNotFound|OldMessageIdPresent=True|CreateResult=Pending");
                                    messageRecovery = true;
                                    messageId = String.Empty;
                                    continue;
                                }
                                int retryDelay = GetRetryDelaySeconds(response, attempt);
                                if (attempt < 3)
                                {
                                    AllflameLog.Info("NOTIFICATION_UPDATE_RETRY|Attempt=" + attempt + "|DelaySeconds=" + retryDelay + "|StatusCode=" + lastStatus + "|Event=" + eventName);
                                    await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                                }
                            }
                        }
                    }
                    else
                    {
                        string createUrl = webhook + (webhook.IndexOf("?", StringComparison.Ordinal) >= 0 ? "&wait=true" : "?wait=true");
                        using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                        using (HttpResponseMessage response = await Client.PostAsync(createUrl, content))
                        {
                            lastStatus = (int)response.StatusCode;
                            string body = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                string createdId = String.Empty;
                                try { createdId = (string)JObject.Parse(body)["id"]; } catch { }
                                if (!String.IsNullOrWhiteSpace(createdId))
                                {
                                    lock (Sync)
                                    {
                                        if (generation == _dashboardGeneration)
                                        {
                                            _dashboardMessageId = createdId;
                                            _dashboardWebhook = webhook;
                                        }
                                    }
                                    AllflameLog.Info("NOTIFICATION_UPDATE_SUCCESS|Action=Create|Event=" + eventName + "|Attempt=" + attempt);
                                    SetDeliveryStatus("Dashboard created", String.Empty);
                                    if (messageRecovery)
                                    {
                                        AllflameLog.Info("NOTIFICATION_MESSAGE_RECOVERY|Reason=MessageNotFound|OldMessageIdPresent=True|CreateResult=Success");
                                    }
                                    return true;
                                }
                                lastException = "MessageIdMissing";
                            }
                            int retryDelay = GetRetryDelaySeconds(response, attempt);
                            if (attempt < 3)
                            {
                                AllflameLog.Info("NOTIFICATION_UPDATE_RETRY|Attempt=" + attempt + "|DelaySeconds=" + retryDelay + "|StatusCode=" + lastStatus + "|Event=" + eventName);
                                await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex.GetType().Name;
                    if (attempt < 3)
                    {
                        int retryDelay = attempt == 1 ? 10 : 30;
                        AllflameLog.Info("NOTIFICATION_UPDATE_RETRY|Attempt=" + attempt + "|DelaySeconds=" + retryDelay + "|StatusCode=0|Event=" + eventName);
                        await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                    }
                }
            }
            AllflameLog.Info("NOTIFICATION_UPDATE_FAILED|Action=" + (String.IsNullOrWhiteSpace(messageId) ? "Create" : "Edit") + "|Event=" + eventName + "|Attempts=3|StatusCode=" + lastStatus + "|ExceptionType=" + lastException);
            SetDeliveryStatus(
                "Failed",
                lastStatus == 0
                    ? lastException
                    : "HTTP " + lastStatus);
            if (messageRecovery)
            {
                AllflameLog.Info("NOTIFICATION_MESSAGE_RECOVERY|Reason=MessageNotFound|OldMessageIdPresent=True|CreateResult=Failed");
            }
            return false;
        }

        private static int GetRetryDelaySeconds(HttpResponseMessage response, int attempt)
        {
            if ((int)response.StatusCode == 429 && response.Headers.RetryAfter != null)
            {
                TimeSpan delay = response.Headers.RetryAfter.Delta ?? TimeSpan.Zero;
                if (response.Headers.RetryAfter.Date.HasValue)
                {
                    delay = response.Headers.RetryAfter.Date.Value.UtcDateTime - DateTime.UtcNow;
                }
                return Math.Max(1, Math.Min(60, (int)Math.Ceiling(delay.TotalSeconds)));
            }
            return attempt == 1 ? 10 : 30;
        }

        private static string WebhookBaseUrl(string webhook)
        {
            int queryIndex =
                webhook.IndexOf("?", StringComparison.Ordinal);
            return (queryIndex < 0
                    ? webhook
                    : webhook.Substring(0, queryIndex))
                .TrimEnd('/');
        }

        private static DiscordEmbed NewEmbed(
            string title,
            string description,
            int color)
        {
            return new DiscordEmbed
            {
                Title = title,
                Description = description,
                Color = color,
                Fields = new List<DiscordEmbedField>(),
                Timestamp = DateTime.UtcNow.ToString("o")
            };
        }

        public static string GetStatusDescription()
        {
            AllflameSettings settings = AllflameSettings.Instance;
            string webhook;
            string validationError;
            if (!TryGetWebhook(settings, out webhook, out validationError))
            {
                return settings.DiscordNotificationsEnabled
                    ? "Status: " + validationError
                    : "Status: Disabled";
            }

            lock (Sync)
            {
                string pending = (ImmediateQueue.Count +
                    (_dashboardDirty ? 1 : 0)).ToString();
                string last = _lastDeliveryUtc == DateTime.MinValue
                    ? "Never"
                    : _lastDeliveryUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                string error = String.IsNullOrWhiteSpace(_lastDeliveryError)
                    ? String.Empty
                    : " | Error: " + _lastDeliveryError;
                return "Status: Ready | Last delivery: " +
                    _lastDeliveryStatus + " at " + last +
                    " | Pending: " + pending + error;
            }
        }

        private static void Send(DiscordEmbed embed)
        {
            Send(new List<DiscordEmbed> { embed });
        }

        private static void Send(List<DiscordEmbed> embeds)
        {
            Send(embeds, null, false, "ImmediateAlert");
        }

        private static void Send(
            List<DiscordEmbed> embeds,
            string contentText,
            bool allowEveryone)
        {
            Send(embeds, contentText, allowEveryone, "ImmediateAlert");
        }

        private static void Send(
            List<DiscordEmbed> embeds,
            string contentText,
            bool allowEveryone,
            string eventName)
        {
            EnqueueImmediate(
                embeds,
                contentText,
                allowEveryone,
                eventName);
        }

        private static void EnqueueImmediate(
            List<DiscordEmbed> embeds,
            string contentText,
            bool allowEveryone,
            string eventName)
        {
            AllflameSettings settings = AllflameSettings.Instance;
            string webhook;
            string validationError;
            if (!TryGetWebhook(settings, out webhook, out validationError))
            {
                SetDeliveryStatus("Suppressed", validationError);
                return;
            }

            bool startWorker = false;
            int queueSize;
            lock (Sync)
            {
                if (ImmediateQueue.Count >= 25)
                {
                    SetDeliveryStatus("Dropped", "Immediate queue is full");
                    AllflameLog.Info(
                        "NOTIFICATION_QUEUE_DROP|Event=" +
                        (eventName ?? "ImmediateAlert") +
                        "|Reason=QueueFull");
                    return;
                }
                ImmediateQueue.Enqueue(new AllflameNotificationRequest
                {
                    Embeds = (embeds ?? new List<DiscordEmbed>()).Take(10).ToList(),
                    Content = contentText,
                    MentionEveryone = allowEveryone,
                    EventName = eventName ?? "ImmediateAlert",
                    Generation = _dashboardGeneration
                });
                queueSize = ImmediateQueue.Count;
                if (!_immediateWorkerRunning)
                {
                    _immediateWorkerRunning = true;
                    startWorker = true;
                }
            }
            AllflameLog.Info(
                "NOTIFICATION_EVENT_ENQUEUED|Event=" +
                (eventName ?? "ImmediateAlert") +
                "|Queue=" + queueSize +
                "|MentionEveryone=" + allowEveryone);
            if (startWorker)
            {
                Task.Run(async delegate
                {
                    try
                    {
                        await ImmediateWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        SetDeliveryStatus("Failed", ex.GetType().Name);
                        AllflameLog.Error(
                            "NOTIFICATION_IMMEDIATE_WORKER_FAILED",
                            ex);
                        lock (Sync)
                        {
                            _immediateWorkerRunning = false;
                        }
                    }
                });
            }
        }

        private static async Task ImmediateWorkerAsync()
        {
            while (true)
            {
                AllflameNotificationRequest request;
                lock (Sync)
                {
                    if (ImmediateQueue.Count == 0)
                    {
                        _immediateWorkerRunning = false;
                        return;
                    }
                    request = ImmediateQueue.Dequeue();
                }
                bool currentGeneration;
                lock (Sync)
                {
                    currentGeneration =
                        request.Generation == _dashboardGeneration;
                }
                if (currentGeneration)
                {
                    await SendImmediateAsync(request);
                }
            }
        }

        private static async Task SendImmediateAsync(
            AllflameNotificationRequest request)
        {
            AllflameSettings settings = AllflameSettings.Instance;
            string webhook;
            string validationError;
            if (!TryGetWebhook(settings, out webhook, out validationError))
            {
                SetDeliveryStatus("Suppressed", validationError);
                return;
            }

            var payload = new DiscordWebhookPayload
            {
                Username = "Allflame Bot",
                Content = request.Content,
                AllowedMentions = request.MentionEveryone
                    ? new DiscordAllowedMentions
                    {
                        Parse = new List<string> { "everyone" }
                    }
                    : null,
                Embeds = request.Embeds.Take(10).ToList()
            };
            string json = JsonConvert.SerializeObject(payload);
            int lastStatus = 0;
            string lastError = "None";
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    using (var content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"))
                    using (HttpResponseMessage response =
                        await Client.PostAsync(webhook, content))
                    {
                        lastStatus = (int)response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            SetDeliveryStatus("Sent", String.Empty);
                            AllflameLog.Info(
                                "DISCORD_SEND|Event=" + request.EventName +
                                "|Embeds=" + request.Embeds.Count +
                                "|MentionEveryone=" + request.MentionEveryone +
                                "|Attempt=" + attempt +
                                "|Result=Success");
                            return;
                        }
                        lastError = "HTTP " + lastStatus;
                        if (attempt < 3)
                        {
                            int retryDelay = GetRetryDelaySeconds(response, attempt);
                            AllflameLog.Info(
                                "NOTIFICATION_SEND_RETRY|Event=" +
                                request.EventName +
                                "|Attempt=" + attempt +
                                "|DelaySeconds=" + retryDelay +
                                "|StatusCode=" + lastStatus);
                            await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.GetType().Name;
                    if (attempt < 3)
                    {
                        int retryDelay = attempt == 1 ? 3 : 10;
                        AllflameLog.Info(
                            "NOTIFICATION_SEND_RETRY|Event=" +
                            request.EventName +
                            "|Attempt=" + attempt +
                            "|DelaySeconds=" + retryDelay +
                            "|StatusCode=0");
                        await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                    }
                }
            }
            SetDeliveryStatus("Failed", lastError);
            AllflameLog.Info(
                "DISCORD_SEND_FAILED|Event=" + request.EventName +
                "|Attempts=3|StatusCode=" + lastStatus +
                "|Error=" + lastError);
        }

        private static bool TryGetWebhook(
            AllflameSettings settings,
            out string webhook,
            out string error)
        {
            webhook = String.Empty;
            error = String.Empty;
            if (settings == null || !settings.DiscordNotificationsEnabled)
            {
                error = "Notifications are disabled.";
                return false;
            }
            webhook = (settings.DiscordWebhookUrl ?? String.Empty).Trim();
            if (String.IsNullOrWhiteSpace(webhook))
            {
                error = "Webhook is not configured.";
                return false;
            }

            Uri uri;
            if (!Uri.TryCreate(webhook, UriKind.Absolute, out uri) ||
                !String.Equals(
                    uri.Scheme,
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase))
            {
                error = "Webhook must be an HTTPS URL.";
                return false;
            }

            string host = uri.Host ?? String.Empty;
            bool validHost =
                String.Equals(host, "discord.com", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(host, "discordapp.com", StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith(".discord.com", StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith(".discordapp.com", StringComparison.OrdinalIgnoreCase);
            if (!validHost ||
                uri.AbsolutePath.IndexOf(
                    "/api/webhooks/",
                    StringComparison.OrdinalIgnoreCase) < 0)
            {
                error = "Webhook host or path is not a Discord webhook.";
                return false;
            }
            return true;
        }

        private static void SetDeliveryStatus(string status, string error)
        {
            lock (Sync)
            {
                _lastDeliveryUtc = DateTime.UtcNow;
                _lastDeliveryStatus = status ?? "Unknown";
                _lastDeliveryError = String.IsNullOrWhiteSpace(error)
                    ? String.Empty
                    : Truncate(error.Replace("|", "/"), 180);
            }
        }

        private static HttpClient CreateClient()
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        private static string PriceFooter()
        {
            DateTime updated = AllflamePrices.UpdatedUtc;
            if (updated == DateTime.MinValue)
            {
                return "Price data has not been updated";
            }
            TimeSpan age = DateTime.UtcNow - updated;
            string ageText = age.TotalDays >= 1d
                ? Math.Floor(age.TotalDays) + " day(s) ago"
                : age.TotalHours >= 1d
                    ? Math.Floor(age.TotalHours) + " hour(s) ago"
                    : Math.Max(0, Math.Floor(age.TotalMinutes)) + " minute(s) ago";
            return "Prices updated " + ageText +
                (String.IsNullOrWhiteSpace(AllflamePrices.League)
                    ? String.Empty
                    : " • " + AllflamePrices.League);
        }

        private static string FormatDuration(TimeSpan duration)
        {
            int totalHours = (int)Math.Floor(duration.TotalHours);
            return totalHours.ToString("00") + ":" +
                duration.Minutes.ToString("00") + ":" +
                duration.Seconds.ToString("00");
        }

        private static string FormatDivines(double value)
        {
            return value.ToString(
                value >= 10d ? "0.0" : "0.###",
                CultureInfo.InvariantCulture) + " div";
        }

        private static KeyValuePair<string, string> Pair(
            string name,
            string value)
        {
            return new KeyValuePair<string, string>(
                name ?? String.Empty,
                value ?? String.Empty);
        }

        private static string BuildTable(
            IEnumerable<KeyValuePair<string, string>> rows)
        {
            List<KeyValuePair<string, string>> safeRows = rows
                .Select(row => Pair(
                    Truncate(
                        (row.Key ?? String.Empty)
                            .Replace("`", "'")
                            .Replace("\r", " ")
                            .Replace("\n", " "),
                        30),
                    Truncate(
                        (row.Value ?? String.Empty)
                            .Replace("`", "'")
                            .Replace("\r", " ")
                            .Replace("\n", " "),
                        46)))
                .ToList();
            if (safeRows.Count == 0)
            {
                safeRows.Add(Pair("Status", "No data"));
            }
            int keyWidth = Math.Min(30, safeRows.Max(row => row.Key.Length));
            int valueWidth = Math.Min(46, safeRows.Max(row => row.Value.Length));
            var output = new StringBuilder("```\n");
            string top = "╔" + new string('═', keyWidth + 2) +
                "╦" + new string('═', valueWidth + 2) + "╗";
            string separator = "╠" + new string('═', keyWidth + 2) +
                "╬" + new string('═', valueWidth + 2) + "╣";
            string bottom = "╚" + new string('═', keyWidth + 2) +
                "╩" + new string('═', valueWidth + 2) + "╝";
            output.AppendLine(top);
            for (int index = 0; index < safeRows.Count; index++)
            {
                KeyValuePair<string, string> row = safeRows[index];
                output.Append("║ ");
                output.Append(row.Key.PadRight(keyWidth));
                output.Append(" ║ ");
                output.Append(row.Value.PadRight(valueWidth));
                output.AppendLine(" ║");
                if (index + 1 < safeRows.Count)
                {
                    output.AppendLine(separator);
                }
            }
            output.AppendLine(bottom);
            output.Append("```");
            return output.ToString();
        }

        private static string HumanizeStopReason(string reason)
        {
            if (String.Equals(reason, "NoChartsRemaining", StringComparison.OrdinalIgnoreCase))
            {
                return "No Charts Remaining";
            }
            if (String.Equals(reason, "RepeatedFailure", StringComparison.OrdinalIgnoreCase))
            {
                return "Repeated failures exceeded the configured limit";
            }
            if (String.IsNullOrWhiteSpace(reason))
            {
                return "Stopped by user";
            }
            var output = new StringBuilder();
            for (int index = 0; index < reason.Length; index++)
            {
                char current = reason[index];
                if (index > 0 &&
                    Char.IsUpper(current) &&
                    !Char.IsWhiteSpace(reason[index - 1]))
                {
                    output.Append(' ');
                }
                output.Append(current);
            }
            return output.ToString();
        }

        private static string Truncate(string value, int maxLength)
        {
            value = value ?? String.Empty;
            return value.Length <= maxLength
                ? value
                : value.Substring(0, Math.Max(0, maxLength - 1)) + "…";
        }
    }
}
