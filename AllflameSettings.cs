using System;
using System.IO;
using DreamPoeBot.Loki.Common;

namespace AllflameBot
{
    public sealed class AllflameSettings : JsonSettings
    {
        private static AllflameSettings _instance;
        private static string _settingsFilePath;
        private static string _legacySettingsFilePath;
        private static string _settingsStorageStatus = "NotInitialized";

        public static AllflameSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AllflameSettings();
                }
                return _instance;
            }
        }

        public static string SettingsFilePath
        {
            get
            {
                if (String.IsNullOrEmpty(_settingsFilePath))
                {
                    ResolveSettingsFilePath();
                }
                return _settingsFilePath;
            }
        }

        public static void LogStorageStatus()
        {
            string path = SettingsFilePath;
            AllflameLog.Info(
                "SETTINGS_STORAGE|Status=" + _settingsStorageStatus +
                "|Path=" + path +
                "|LegacyPath=" + (_legacySettingsFilePath ?? String.Empty));
        }

        public bool AutoRunChartAreas { get; set; }
        public bool RunInventoryChartsOnly { get; set; }
        public bool CheckChartStorageFirst { get; set; }
        public bool EnableChartStorageWithdrawal { get; set; }
        public bool CheckVoyageStashFirst { get; set; }
        public string VoyageStashTab { get; set; }
        public int ChartsPerWithdrawal { get; set; }
        public bool ExploreWholeChart { get; set; }
        public bool OpenKnownChestsFirst { get; set; }
        public bool UseGameAutoLantern { get; set; }
        public bool UseVForLantern { get; set; }
        public int MaxMinutesPerChart { get; set; }
        public int ChartExplorePercent { get; set; }
        public int InteractDistance { get; set; }

        public bool AutoPlanVoyage { get; set; }
        public int ChartsRequiredForVoyage { get; set; }
        public bool StartVoyageWhenReady { get; set; }
        public bool EnableVoyageRunner { get; set; }
        public int VoyageFrontierSpacing { get; set; }
        public int VoyageFrontierTimeoutSeconds { get; set; }
        public bool EnableVoyageTimeLimit { get; set; }
        public int VoyageTimeLimitMinutes { get; set; }
        public bool LeaveVoyageWhenInventoryLow { get; set; }
        public int LeaveVoyageFreeSquares { get; set; }
        public bool TransferAtVoyageSectorBoundaries { get; set; }
        public bool AlwaysSendBeforeVoyageExtraction { get; set; }
        public bool CallBathysphereWhenLanternsEmpty { get; set; }

        public bool OpenSunkenLoot { get; set; }
        public bool OpenSunkenOpulence { get; set; }
        public bool OpenSunkenGems { get; set; }
        public bool OpenDivinerStrongbox { get; set; }
        public bool OpenArcanistStrongbox { get; set; }
        public bool OpenOperativeStrongbox { get; set; }
        public bool OpenSoddenAugur { get; set; }
        public bool OpenMessageInBottle { get; set; }
        public bool OpenScarabChest { get; set; }
        public bool OpenCartographerChest { get; set; }
        public bool OpenAllflameChest { get; set; }
        public bool OpenGoldPile { get; set; }
        public bool OpenTreasureClam { get; set; }
        public bool OpenUniqueArmourChest { get; set; }
        public bool OpenUniqueWeaponChest { get; set; }
        public bool OpenUniqueTrinketChest { get; set; }
        public int PrioritySunkenLoot { get; set; }
        public int PrioritySunkenOpulence { get; set; }
        public int PrioritySunkenGems { get; set; }
        public int PriorityDivinerStrongbox { get; set; }
        public int PriorityArcanistStrongbox { get; set; }
        public int PriorityOperativeStrongbox { get; set; }
        public int PrioritySoddenAugur { get; set; }
        public int PriorityMessageInBottle { get; set; }
        public int PriorityScarabChest { get; set; }
        public int PriorityCartographerChest { get; set; }
        public int PriorityAllflameChest { get; set; }
        public int PriorityGoldPile { get; set; }
        public int PriorityTreasureClam { get; set; }
        public int PriorityUniqueArmourChest { get; set; }
        public int PriorityUniqueWeaponChest { get; set; }
        public int PriorityUniqueTrinketChest { get; set; }

        public bool SendLootToSurface { get; set; }
        public int SendToSurfaceFreeSlots { get; set; }
        public int BottlesPerSurfaceSend { get; set; }
        public bool CollectSurfaceLoot { get; set; }
        public bool CtrlClickCollectLoot { get; set; }
        public bool EnableGroundLoot { get; set; }
        public bool LootOnlyWhenSafe { get; set; }
        public bool OnlyLootVisibleByGameFilter { get; set; }
        public int GroundLootRadius { get; set; }
        public int LootSettleMilliseconds { get; set; }
        public int LootActionDelayMilliseconds { get; set; }
        public int LootRetryDelayMilliseconds { get; set; }
        public int MaxLootAttempts { get; set; }
        public bool PickupEverything { get; set; }
        public bool PickupSeasonalRewards { get; set; }
        public bool PickupCurrency { get; set; }
        public bool IgnoreScrollOfWisdom { get; set; }
        public bool PickupDivinationCards { get; set; }
        public bool PickupScarabs { get; set; }
        public bool PickupUniques { get; set; }
        public double MinimumUniqueValueDivines { get; set; }
        public bool PickupMaps { get; set; }
        public int MinimumMapTier { get; set; }
        public bool PickupQuestItems { get; set; }
        public bool PickupGems { get; set; }
        public int MinimumGemQuality { get; set; }
        public double MinimumGemValueDivines { get; set; }
        public bool PickupFlasks { get; set; }
        public int MinimumFlaskQuality { get; set; }
        public bool PickupRares { get; set; }
        public bool PickupSixSockets { get; set; }
        public int MinimumSockets { get; set; }
        public bool PickupLinkedItems { get; set; }
        public int MinimumLinks { get; set; }
        public bool WalkOverGold { get; set; }
        public bool EnableLifeFlask { get; set; }
        public int LifeFlaskUseThreshold { get; set; }
        public bool EnableUtilityFlasks { get; set; }
        public string UtilityFlaskSlots { get; set; }
        public int UtilityFlaskIntervalSeconds { get; set; }
        public bool DisableUtilityFlasksOnBoat { get; set; }
        public bool EnableEnduringCry { get; set; }
        public int EnduringCrySlot { get; set; }

        public bool EnableStashing { get; set; }
        public string PersonalLootTabs { get; set; }
        public bool UseGuildStash { get; set; }
        public string GuildLootTabs { get; set; }
        public bool StashAfterCollectLoot { get; set; }
        public int StashWhenFreeSlotsAtOrBelow { get; set; }
        public bool StashBeforeVoyage { get; set; }
        public int ItemTransferDelayMinMilliseconds { get; set; }
        public int ItemTransferDelayMaxMilliseconds { get; set; }

        public bool ChartDeviceUnlocked { get; set; }
        public int LastChartStorageTotal { get; set; }
        public int LastVoyageStashTotal { get; set; }
        public string LastChartStorageScan { get; set; }

        public bool KeepMovingWhileWaiting { get; set; }
        public int WaitMoveRadius { get; set; }
        public int BathysphereWaitTimeoutSeconds { get; set; }
        public bool AvoidUnlitAreas { get; set; }
        public bool EnableTopologyRoomRouting { get; set; }
        public bool EnableHybridLayoutPlanner { get; set; }
        public int LayoutFrontierStep { get; set; }
        public bool StopOnRepeatedFailure { get; set; }
        public int MaxRepeatedFailures { get; set; }
        public bool CloseGameOnStop { get; set; }
        public bool EnablePricing { get; set; }
        public bool AutoUpdatePrices { get; set; }
        public string PricingLeague { get; set; }
        public int PriceUpdateHours { get; set; }
        public string FixedPrices { get; set; }

        public bool EnableSecretBottleProcessing { get; set; }
        public int SecretBottlesToOpen { get; set; }
        public int SecretMaxBottlesPerSession { get; set; }
        public int SecretBottleMinimumFreeSquares { get; set; }
        public double SecretBottleKeepMinDivines { get; set; }
        public bool SecretBottleSellLowValue { get; set; }
        public string SecretBottleVendorName { get; set; }
        public bool SecretBottleUseGuildStash { get; set; }
        public string SecretBottleStashTab { get; set; }
        public int SecretBottleRevealTimeoutSeconds { get; set; }
        public int SecretBottleCursorPlacementRetries { get; set; }

        public bool DiscordNotificationsEnabled { get; set; }
        public string DiscordWebhookUrl { get; set; }
        public bool SendPeriodicSummary { get; set; }
        public int NotificationIntervalMinutes { get; set; }
        public int DiscordMaxItemCount { get; set; }
        public int LowChartWarningThreshold { get; set; }
        public bool TagEveryoneForLowVoyageStash { get; set; }
        public int VoyageStashEveryoneThreshold { get; set; }
        public bool NotifyValuableItems { get; set; }
        public bool NotifySecretBottleRewards { get; set; }
        public int ValuableItemMinDivines { get; set; }
        public bool TagEveryoneForValuableItems { get; set; }
        public bool NotifyMapStarted { get; set; }
        public bool NotifyMapCompleted { get; set; }
        public bool NotifySulphurCollected { get; set; }

        public int SettingsSchemaVersion { get; set; }

        public AllflameSettings()
            : base(ResolveSettingsFilePath())
        {
            if (MaxMinutesPerChart <= 0)
            {
                ApplyRecommendedDefaults();
            }
            else if (SettingsSchemaVersion < 40)
            {
                int oldSchema = SettingsSchemaVersion;
                if (oldSchema < 3)
                {
                    ApplyStashDefaults();
                }
                if (oldSchema < 4)
                {
                    ApplyChartAreaDefaults();
                }
                LastChartStorageScan = LastChartStorageScan ?? String.Empty;
                if (oldSchema < 6)
                {
                    ApplyGroundLootDefaults();
                }
                if (oldSchema < 7)
                {
                    CheckChartStorageFirst = true;
                }
                if (oldSchema < 8)
                {
                    RunInventoryChartsOnly = true;
                }
                if (oldSchema < 9)
                {
                    ApplyChestPriorityDefaults();
                }
                if (oldSchema < 10)
                {
                    EnableLifeFlask = true;
                    LifeFlaskUseThreshold = 80;
                }
                if (oldSchema < 11)
                {
                    ApplyPickitDefaults();
                    CheckVoyageStashFirst = true;
                    VoyageStashTab = "Charts";
                    RunInventoryChartsOnly = false;
                    EnableUtilityFlasks = true;
                    UtilityFlaskSlots = "2,3,4,5";
                    UtilityFlaskIntervalSeconds = 5;
                    DisableUtilityFlasksOnBoat = true;
                }
                if (oldSchema < 12)
                {
                    PickupScarabs = true;
                }
                if (oldSchema < 13)
                {
                    LootActionDelayMilliseconds = 40;
                }
                if (oldSchema < 14)
                {
                    OnlyLootVisibleByGameFilter = true;
                }
                if (oldSchema < 15)
                {
                    PickupEverything = true;
                    OnlyLootVisibleByGameFilter = false;
                }
                if (oldSchema < 16)
                {
                    // The in-game loot filter is authoritative. PickupEverything
                    // means every highlighted chest drop is accepted without a
                    // second, conflicting category filter inside the bot.
                    PickupEverything = true;
                    OnlyLootVisibleByGameFilter = true;
                }
                if (oldSchema < 17)
                {
                    // Restore the conservative item interaction cadence used
                    // by the stable chest-loot implementation. Layout targets
                    // are now sampled around the immutable Chart spawn.
                    LootSettleMilliseconds = 600;
                    LootActionDelayMilliseconds = 160;
                    LootRetryDelayMilliseconds = 300;
                    EnableHybridLayoutPlanner = true;
                    LayoutFrontierStep = 95;
                }
                if (oldSchema < 18)
                {
                    ApplyVoyageRunnerDefaults();
                }
                if (oldSchema < 19)
                {
                    ApplyPricingAndNotificationDefaults();
                }
                if (oldSchema < 20)
                {
                    NotifyMapStarted = true;
                    NotifyMapCompleted = true;
                    NotifySulphurCollected = true;
                }
                if (oldSchema < 21)
                {
                    EnableVoyageTimeLimit = true;
                    VoyageTimeLimitMinutes = 30;
                }
                if (oldSchema < 22)
                {
                    StashBeforeVoyage = true;
                    ItemTransferDelayMinMilliseconds = 250;
                    ItemTransferDelayMaxMilliseconds = 380;
                }
                if (oldSchema < 23)
                {
                    BottlesPerSurfaceSend = 2;
                }
                if (oldSchema < 24)
                {
                    TagEveryoneForValuableItems = false;
                }
                if (oldSchema < 26)
                {
                    EnableEnduringCry = true;
                    EnduringCrySlot = 0;
                }
                if (oldSchema < 27)
                {
                    TagEveryoneForLowVoyageStash = false;
                    VoyageStashEveryoneThreshold = 10;
                }
                if (oldSchema < 28)
                {
                    LeaveVoyageWhenInventoryLow = true;
                    LeaveVoyageFreeSquares = 8;
                }
                if (oldSchema < 29)
                {
                    ChartsPerWithdrawal = 20;
                }
                if (oldSchema < 30)
                {
                    EnableTopologyRoomRouting = true;
                }
                if (oldSchema < 31)
                {
                    OpenSunkenOpulence = true;
                    OpenSunkenGems = true;
                    OpenDivinerStrongbox = true;
                    OpenArcanistStrongbox = true;
                    OpenOperativeStrongbox = true;
                    PrioritySunkenOpulence = 100;
                    PrioritySunkenGems = 98;
                    PriorityDivinerStrongbox = 95;
                    PriorityOperativeStrongbox = 92;
                    PriorityArcanistStrongbox = 90;
                    MinimumUniqueValueDivines = 0d;
                    MinimumGemValueDivines = 0d;
                }
                if (oldSchema < 34)
                {
                    OpenUniqueTrinketChest = true;
                    PriorityUniqueTrinketChest = 50;
                }
                if (oldSchema < 35)
                {
                    ApplySecretBottleDefaults();
                }
                if (oldSchema < 36)
                {
                    SecretBottleUseGuildStash = false;
                    SecretBottleStashTab = "1";
                }
                if (oldSchema < 37)
                {
                    CloseGameOnStop = false;
                }
                if (oldSchema < 38)
                {
                    SecretBottleRevealTimeoutSeconds = 8;
                    SecretBottleCursorPlacementRetries = 3;
                }
                if (oldSchema < 39)
                {
                    EnableChartStorageWithdrawal = false;
                }
                if (oldSchema < 40)
                {
                    NotifySecretBottleRewards = true;
                }
                SettingsSchemaVersion = 40;
                Save();
            }
        }

        private static string ResolveSettingsFilePath()
        {
            if (!String.IsNullOrEmpty(_settingsFilePath))
            {
                return _settingsFilePath;
            }

            string legacyPath;
            try
            {
                legacyPath =
                    JsonSettings.GetSettingsFilePath("AllflameBot.json");
            }
            catch
            {
                legacyPath = null;
            }
            if (String.IsNullOrWhiteSpace(legacyPath))
            {
                legacyPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Settings",
                    "AllflameBot.json");
            }
            _legacySettingsFilePath = legacyPath;

            try
            {
                string documents = Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);
                if (String.IsNullOrWhiteSpace(documents))
                {
                    string userProfile = Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile);
                    if (String.IsNullOrWhiteSpace(userProfile))
                    {
                        throw new DirectoryNotFoundException(
                            "Windows user profile path is unavailable.");
                    }
                    documents = Path.Combine(userProfile, "Documents");
                }

                string directory = Path.Combine(documents, "AllflameBot");
                string documentsPath =
                    Path.Combine(directory, "AllflameBot.json");
                Directory.CreateDirectory(directory);

                if (File.Exists(documentsPath))
                {
                    _settingsStorageStatus = "DocumentsExisting";
                }
                else if (File.Exists(legacyPath))
                {
                    File.Copy(legacyPath, documentsPath, false);
                    _settingsStorageStatus = "MigratedFromLegacy";
                }
                else
                {
                    _settingsStorageStatus = "DocumentsNew";
                }

                _settingsFilePath = documentsPath;
            }
            catch (Exception ex)
            {
                _settingsFilePath = legacyPath;
                _settingsStorageStatus =
                    "LegacyFallback:" + ex.GetType().Name;
            }

            return _settingsFilePath;
        }

        public int GetChartWithdrawalCount()
        {
            return Math.Max(1, Math.Min(60, ChartsPerWithdrawal));
        }

        public bool ShouldOpenChest(string name, string metadata)
        {
            name = name ?? string.Empty;
            metadata = metadata ?? string.Empty;

            if (IsEssenceObject(name, metadata))
            {
                return false;
            }

            if (Contains(metadata, "CurrencyTreasureChestOpulent") ||
                Contains(name, "Sunken Opulence"))
            {
                return OpenSunkenOpulence;
            }
            if (Contains(name, "Sunken Gems") ||
                Contains(metadata, "GemTreasureChest") ||
                Contains(metadata, "DeepwaterChestGems") ||
                (Contains(metadata, "LeagueDeepwater") &&
                 Contains(metadata, "Chest") &&
                 Contains(metadata, "Gem")))
            {
                return OpenSunkenGems;
            }
            if (Contains(metadata, "StrongBoxes/Diviner") ||
                Contains(metadata, "StrongBoxes/Divination") ||
                Contains(name, "Diviner's Strongbox"))
            {
                return OpenDivinerStrongbox;
            }
            if (Contains(metadata, "StrongBoxes/Arcanist") ||
                Contains(name, "Arcanist's Strongbox"))
            {
                return OpenArcanistStrongbox;
            }
            if (Contains(metadata, "StrongBoxes/StrongboxScarab") ||
                Contains(name, "Operative's Strongbox"))
            {
                return OpenOperativeStrongbox;
            }
            if (Contains(metadata, "CurrencyTreasureChest")) return OpenSunkenLoot;
            if (metadata.Contains("DeepwaterChestStackedDecks")) return OpenSoddenAugur;
            if (metadata.Contains("BottledItemChest")) return OpenMessageInBottle;
            if (metadata.Contains("DeepwaterChestScarabs")) return OpenScarabChest;
            if (metadata.Contains("DeepwaterChestMaps")) return OpenCartographerChest;
            if (metadata.Contains("DeepwaterChestAllflameEmbers")) return OpenAllflameChest;
            if (metadata.Contains("GoldTreasureChest")) return OpenGoldPile;
            if (metadata.Contains("ClamTreasureChest")) return OpenTreasureClam;
            if (metadata.Contains("DeepwaterAnchorUniqueArmour")) return OpenUniqueArmourChest;
            if (metadata.Contains("DeepwaterAnchorUniqueWeapon")) return OpenUniqueWeaponChest;
            if (IsUniqueTrinketChest(name, metadata)) return OpenUniqueTrinketChest;
            return false;
        }

        public int GetChestPriority(string name, string metadata)
        {
            name = name ?? string.Empty;
            metadata = metadata ?? string.Empty;

            if (IsEssenceObject(name, metadata))
            {
                return 0;
            }

            if (Contains(metadata, "CurrencyTreasureChestOpulent") ||
                Contains(name, "Sunken Opulence"))
            {
                return PrioritySunkenOpulence;
            }
            if (Contains(name, "Sunken Gems") ||
                Contains(metadata, "GemTreasureChest") ||
                Contains(metadata, "DeepwaterChestGems") ||
                (Contains(metadata, "LeagueDeepwater") &&
                 Contains(metadata, "Chest") &&
                 Contains(metadata, "Gem")))
            {
                return PrioritySunkenGems;
            }
            if (Contains(metadata, "StrongBoxes/Diviner") ||
                Contains(metadata, "StrongBoxes/Divination") ||
                Contains(name, "Diviner's Strongbox"))
            {
                return PriorityDivinerStrongbox;
            }
            if (Contains(metadata, "StrongBoxes/Arcanist") ||
                Contains(name, "Arcanist's Strongbox"))
            {
                return PriorityArcanistStrongbox;
            }
            if (Contains(metadata, "StrongBoxes/StrongboxScarab") ||
                Contains(name, "Operative's Strongbox"))
            {
                return PriorityOperativeStrongbox;
            }
            if (Contains(metadata, "CurrencyTreasureChest")) return PrioritySunkenLoot;
            if (metadata.Contains("DeepwaterChestStackedDecks")) return PrioritySoddenAugur;
            if (metadata.Contains("BottledItemChest")) return PriorityMessageInBottle;
            if (metadata.Contains("DeepwaterChestScarabs")) return PriorityScarabChest;
            if (metadata.Contains("DeepwaterChestMaps")) return PriorityCartographerChest;
            if (metadata.Contains("DeepwaterChestAllflameEmbers")) return PriorityAllflameChest;
            if (metadata.Contains("GoldTreasureChest")) return PriorityGoldPile;
            if (metadata.Contains("ClamTreasureChest")) return PriorityTreasureClam;
            if (metadata.Contains("DeepwaterAnchorUniqueArmour")) return PriorityUniqueArmourChest;
            if (metadata.Contains("DeepwaterAnchorUniqueWeapon")) return PriorityUniqueWeaponChest;
            if (IsUniqueTrinketChest(name, metadata)) return PriorityUniqueTrinketChest;
            return 0;
        }

        public static bool IsEssenceObject(
            string name,
            string metadata)
        {
            name = name ?? string.Empty;
            metadata = metadata ?? string.Empty;
            return Contains(name, "Essence") ||
                Contains(metadata, "/Essence/") ||
                Contains(metadata, "EssenceMonolith") ||
                Contains(metadata, "EssenceEncounter");
        }

        private static bool IsUniqueTrinketChest(
            string name,
            string metadata)
        {
            return Contains(
                       name,
                       "Encrusted Ancient Trinket Chest") ||
                   Contains(
                       metadata,
                       "DeepwaterAnchorUniqueTrinket") ||
                   Contains(
                       metadata,
                       "DeepwaterAnchorUniqueJewell") ||
                   Contains(
                       metadata,
                       "DeepwaterAnchorUniqueAccessory") ||
                   (Contains(metadata, "DeepwaterAnchorUnique") &&
                    (Contains(metadata, "Trinket") ||
                     Contains(metadata, "Jewell") ||
                     Contains(metadata, "Accessory")));
        }

        private static bool Contains(string value, string fragment)
        {
            return (value ?? String.Empty).IndexOf(
                fragment,
                StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void ApplyRecommendedDefaults()
        {
            ApplyChartAreaDefaults();
            ExploreWholeChart = true;
            OpenKnownChestsFirst = true;
            UseGameAutoLantern = true;
            UseVForLantern = true;
            MaxMinutesPerChart = 8;

            AutoPlanVoyage = false;
            ChartsRequiredForVoyage = 9;
            StartVoyageWhenReady = false;
            ApplyVoyageRunnerDefaults();

            OpenSunkenLoot = true;
            OpenSunkenOpulence = true;
            OpenSunkenGems = true;
            OpenDivinerStrongbox = true;
            OpenArcanistStrongbox = true;
            OpenOperativeStrongbox = true;
            OpenSoddenAugur = true;
            OpenMessageInBottle = true;
            OpenScarabChest = true;
            OpenCartographerChest = true;
            OpenAllflameChest = true;
            OpenGoldPile = true;
            OpenTreasureClam = true;
            OpenUniqueArmourChest = true;
            OpenUniqueWeaponChest = true;
            OpenUniqueTrinketChest = true;
            ApplyChestPriorityDefaults();

            SendLootToSurface = true;
            SendToSurfaceFreeSlots = 8;
            BottlesPerSurfaceSend = 2;
            CollectSurfaceLoot = true;
            CtrlClickCollectLoot = true;
            ApplyGroundLootDefaults();

            ApplyStashDefaults();

            ChartDeviceUnlocked = false;
            CheckChartStorageFirst = true;
            EnableChartStorageWithdrawal = false;
            CheckVoyageStashFirst = true;
            VoyageStashTab = "Charts";
            RunInventoryChartsOnly = false;
            LastChartStorageTotal = 0;
            LastVoyageStashTotal = 0;
            LastChartStorageScan = String.Empty;

            KeepMovingWhileWaiting = true;
            WaitMoveRadius = 25;
            BathysphereWaitTimeoutSeconds = 45;
            AvoidUnlitAreas = true;
            EnableTopologyRoomRouting = true;
            EnableHybridLayoutPlanner = true;
            LayoutFrontierStep = 95;
            StopOnRepeatedFailure = true;
            MaxRepeatedFailures = 3;
            CloseGameOnStop = false;
            EnableLifeFlask = true;
            LifeFlaskUseThreshold = 80;
            EnableUtilityFlasks = true;
            UtilityFlaskSlots = "2,3,4,5";
            UtilityFlaskIntervalSeconds = 5;
            DisableUtilityFlasksOnBoat = true;
            EnableEnduringCry = true;
            EnduringCrySlot = 0;
            ApplyPricingAndNotificationDefaults();
            ApplySecretBottleDefaults();
            Save();
        }

        private void ApplyStashDefaults()
        {
            EnableStashing = true;
            PersonalLootTabs = "1";
            UseGuildStash = false;
            GuildLootTabs = String.Empty;
            StashAfterCollectLoot = true;
            StashWhenFreeSlotsAtOrBelow = 8;
            StashBeforeVoyage = true;
            ItemTransferDelayMinMilliseconds = 250;
            ItemTransferDelayMaxMilliseconds = 380;
            SettingsSchemaVersion = 22;
        }

        private void ApplyChartAreaDefaults()
        {
            AutoRunChartAreas = true;
            RunInventoryChartsOnly = false;
            CheckChartStorageFirst = true;
            EnableChartStorageWithdrawal = false;
            CheckVoyageStashFirst = true;
            VoyageStashTab = "Charts";
            ChartsPerWithdrawal = 20;
            ChartExplorePercent = 95;
            InteractDistance = 18;
            SettingsSchemaVersion = 22;
        }

        private void ApplyGroundLootDefaults()
        {
            EnableGroundLoot = true;
            LootOnlyWhenSafe = false;
            OnlyLootVisibleByGameFilter = true;
            GroundLootRadius = 150;
            LootSettleMilliseconds = 600;
            LootActionDelayMilliseconds = 160;
            LootRetryDelayMilliseconds = 300;
            MaxLootAttempts = 3;
            ApplyPickitDefaults();
        }

        private void ApplyVoyageRunnerDefaults()
        {
            EnableVoyageRunner = true;
            VoyageFrontierSpacing = 95;
            VoyageFrontierTimeoutSeconds = 12;
            EnableVoyageTimeLimit = true;
            VoyageTimeLimitMinutes = 30;
            LeaveVoyageWhenInventoryLow = true;
            LeaveVoyageFreeSquares = 8;
            TransferAtVoyageSectorBoundaries = true;
            AlwaysSendBeforeVoyageExtraction = true;
            CallBathysphereWhenLanternsEmpty = true;
        }

        private void ApplyPricingAndNotificationDefaults()
        {
            EnablePricing = true;
            AutoUpdatePrices = true;
            PricingLeague = "Allflame";
            PriceUpdateHours = 12;
            FixedPrices = String.Empty;

            DiscordNotificationsEnabled = false;
            DiscordWebhookUrl = String.Empty;
            SendPeriodicSummary = true;
            NotificationIntervalMinutes = 60;
            DiscordMaxItemCount = 12;
            LowChartWarningThreshold = 10;
            TagEveryoneForLowVoyageStash = false;
            VoyageStashEveryoneThreshold = 10;
            NotifyValuableItems = true;
            NotifySecretBottleRewards = true;
            ValuableItemMinDivines = 10;
            TagEveryoneForValuableItems = false;
            NotifyMapStarted = true;
            NotifyMapCompleted = true;
            NotifySulphurCollected = true;
            SettingsSchemaVersion = 40;
        }

        private void ApplySecretBottleDefaults()
        {
            EnableSecretBottleProcessing = false;
            SecretBottlesToOpen = 10;
            SecretMaxBottlesPerSession = 20;
            SecretBottleMinimumFreeSquares = 4;
            SecretBottleKeepMinDivines = 1d;
            SecretBottleSellLowValue = true;
            SecretBottleVendorName = String.Empty;
            SecretBottleUseGuildStash = false;
            SecretBottleStashTab = "1";
            SecretBottleRevealTimeoutSeconds = 8;
            SecretBottleCursorPlacementRetries = 3;
        }

        private void ApplyPickitDefaults()
        {
            PickupEverything = true;
            PickupSeasonalRewards = true;
            PickupCurrency = true;
            IgnoreScrollOfWisdom = true;
            PickupDivinationCards = true;
            PickupScarabs = true;
            PickupUniques = true;
            MinimumUniqueValueDivines = 0d;
            PickupMaps = true;
            MinimumMapTier = 1;
            PickupQuestItems = true;
            PickupGems = true;
            MinimumGemQuality = 1;
            MinimumGemValueDivines = 0d;
            PickupFlasks = true;
            MinimumFlaskQuality = 1;
            PickupRares = false;
            PickupSixSockets = true;
            MinimumSockets = 6;
            PickupLinkedItems = true;
            MinimumLinks = 6;
            WalkOverGold = true;
        }

        private void ApplyChestPriorityDefaults()
        {
            PrioritySunkenOpulence = 100;
            PrioritySunkenGems = 98;
            PriorityDivinerStrongbox = 95;
            PriorityOperativeStrongbox = 92;
            PriorityArcanistStrongbox = 90;
            PriorityAllflameChest = 100;
            PriorityScarabChest = 90;
            PrioritySoddenAugur = 85;
            PriorityMessageInBottle = 80;
            PriorityCartographerChest = 75;
            PrioritySunkenLoot = 70;
            PriorityGoldPile = 65;
            PriorityTreasureClam = 60;
            PriorityUniqueArmourChest = 50;
            PriorityUniqueWeaponChest = 50;
            PriorityUniqueTrinketChest = 50;
        }
    }
}
