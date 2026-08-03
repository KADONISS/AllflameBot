using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AllflameBot
{
    public sealed class AllflameBotGui : UserControl
    {
        private static readonly Brush BackgroundBrush = Brush("#202020");
        private static readonly Brush CardBrush = Brush("#252525");
        private static readonly Brush BlueBrush = Brush("#0d4dcc");
        private static readonly Brush TextBrush = Brushes.White;
        private static readonly Brush MutedBrush = Brush("#a8a8a8");

        private readonly AllflameSettings _settings;
        private TextBlock _status;
        private TextBlock _priceStatus;
        private TextBlock _notificationStatus;
        private TextBlock _chartStorageDebugStatus;
        private StackPanel _chestRows;
        private readonly DispatcherTimer _timer;

        private sealed class ChestOption
        {
            public ChestOption(
                string name,
                string propertyName,
                string priorityPropertyName)
            {
                Name = name;
                PropertyName = propertyName;
                PriorityPropertyName = priorityPropertyName;
            }

            public string Name { get; private set; }
            public string PropertyName { get; private set; }
            public string PriorityPropertyName { get; private set; }
            public override string ToString() { return Name; }
        }

        private static readonly ChestOption[] ChestOptions =
        {
            new ChestOption("Sunken Opulence", "OpenSunkenOpulence", "PrioritySunkenOpulence"),
            new ChestOption("Sunken Gems", "OpenSunkenGems", "PrioritySunkenGems"),
            new ChestOption("Diviner's Strongbox", "OpenDivinerStrongbox", "PriorityDivinerStrongbox"),
            new ChestOption("Arcanist's Strongbox", "OpenArcanistStrongbox", "PriorityArcanistStrongbox"),
            new ChestOption("Operative's Strongbox", "OpenOperativeStrongbox", "PriorityOperativeStrongbox"),
            new ChestOption("Sunken Loot", "OpenSunkenLoot", "PrioritySunkenLoot"),
            new ChestOption("Sodden Augur's Chest", "OpenSoddenAugur", "PrioritySoddenAugur"),
            new ChestOption("Message in a Bottle", "OpenMessageInBottle", "PriorityMessageInBottle"),
            new ChestOption("Waterlogged Scarab Chest", "OpenScarabChest", "PriorityScarabChest"),
            new ChestOption("Eroded Cartographer's Chest", "OpenCartographerChest", "PriorityCartographerChest"),
            new ChestOption("Glowing Allflame Chest", "OpenAllflameChest", "PriorityAllflameChest"),
            new ChestOption("Gold Pile", "OpenGoldPile", "PriorityGoldPile"),
            new ChestOption("Treasure Clam", "OpenTreasureClam", "PriorityTreasureClam"),
            new ChestOption("Encrusted Armour Chest", "OpenUniqueArmourChest", "PriorityUniqueArmourChest"),
            new ChestOption("Encrusted Weapon Chest", "OpenUniqueWeaponChest", "PriorityUniqueWeaponChest"),
            new ChestOption("Encrusted Trinket Chest", "OpenUniqueTrinketChest", "PriorityUniqueTrinketChest")
        };

        public AllflameBotGui()
        {
            _settings = AllflameSettings.Instance;
            if (_settings.MaxMinutesPerChart <= 0)
            {
                _settings.ApplyRecommendedDefaults();
            }
            AllflamePrices.Initialize();

            Background = BackgroundBrush;
            Foreground = TextBrush;

            var root = new DockPanel();
            root.Children.Add(BuildHeader());

            var tabs = new TabControl
            {
                Background = BackgroundBrush,
                Foreground = TextBrush,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(8)
            };
            DockPanel.SetDock(tabs, Dock.Bottom);
            root.Children.Add(tabs);

            tabs.Items.Add(Tab("General", BuildGeneralTab()));
            tabs.Items.Add(Tab("Stash", BuildStashTab()));
            tabs.Items.Add(Tab("Voyage", BuildVoyageTab()));
            tabs.Items.Add(Tab("Loot", BuildLootTab()));
            tabs.Items.Add(Tab("Prices", BuildPricesTab()));
            tabs.Items.Add(Tab("Notifications", BuildNotificationsTab()));
            tabs.Items.Add(Tab("Secret", BuildSecretTab()));
            tabs.Items.Add(Tab("Extra", BuildExtraTab()));

            Content = root;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += delegate
            {
                if (_status != null)
                {
                    _status.Text = AllflameContext.Describe();
                }
                RefreshPriceStatus();
                RefreshNotificationStatus();
                if (_chartStorageDebugStatus != null)
                {
                    _chartStorageDebugStatus.Text =
                        AllflameChartStorageDebug.Status;
                }
            };
            Loaded += delegate { _timer.Start(); };
            Unloaded += delegate { _timer.Stop(); };
        }

        private UIElement BuildHeader()
        {
            var bar = new Border
            {
                Background = BlueBrush,
                Padding = new Thickness(10, 7, 10, 7)
            };
            bar.Child = new TextBlock
            {
                Text = "Active  |  Author: BoTBien  |  Version: " + AllflameVersion.Current + "  |  Description: Allflame Chart + Voyage Runner",
                Foreground = Brushes.White,
                FontSize = 13
            };
            DockPanel.SetDock(bar, Dock.Top);
            return bar;
        }

        private UIElement BuildGeneralTab()
        {
            var grid = PanelGrid(4);
            _status = new TextBlock
            {
                Text = "Start the bot to populate live state.",
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap
            };
            grid.Children.Add(Card("MODE", Stack(
                Check("Automatically run Chart Areas", "AutoRunChartAreas"),
                Check("Run Charts from inventory only", "RunInventoryChartsOnly"),
                Check("Check Voyage Stash tab first", "CheckVoyageStashFirst"),
                Check("Explore whole Chart Area", "ExploreWholeChart"),
                Number("Explore target (%)", "ChartExplorePercent", 50, 100),
                Number("Maximum minutes per Chart", "MaxMinutesPerChart", 1, 60),
                Number("Interaction distance", "InteractDistance", 8, 35))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("LANTERNS", Stack(
                Check("Use game's Auto-Lantern", "UseGameAutoLantern"),
                Check("Press V fallback when auto-place misses", "UseVForLantern"),
                Check("Return when Lanterns are exhausted", "FinishSmallChartWhenLanternsEmpty"),
                Number("Chest grace at 0/x (seconds)", "ZeroLanternChestGraceSeconds", 0, 60),
                Note("Finish pending chest/loot work for at most this duration. 0 exits immediately."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("CHESTS", BuildChestPicker()));
            Grid.SetColumn(grid.Children[2], 2);

            var statusCard = Card("LIVE STATUS", Stack(
                _status,
                Button("Reset recommended defaults", delegate
                {
                    _settings.ApplyRecommendedDefaults();
                })));
            grid.Children.Add(statusCard);
            Grid.SetColumn(statusCard, 3);
            return grid;
        }

        private UIElement BuildStashTab()
        {
            var grid = PanelGrid(4);
            _chartStorageDebugStatus = new TextBlock
            {
                Text = AllflameChartStorageDebug.Status,
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 5, 0, 5)
            };
            grid.Children.Add(Card("PERSONAL STASH", Stack(
                Check("Enable stashing", "EnableStashing"),
                Text("Loot tab names/numbers", "PersonalLootTabs"),
                Note("Separate multiple tabs with commas, for example: Loot 1, Loot 2."))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("GUILD STASH", Stack(
                Check("Use Guild Stash", "UseGuildStash"),
                Text("Guild loot tab names/numbers", "GuildLootTabs"),
                Note("Personal stash is used when Guild Stash is disabled."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("STASH POLICY", Stack(
                Check("Stash after Collect Loot", "StashAfterCollectLoot"),
                Check("Stash loot + Charts before Voyage", "StashBeforeVoyage"),
                Number("Stash when free slots reach", "StashWhenFreeSlotsAtOrBelow", 0, 60),
                Number("Item delay minimum (ms)", "ItemTransferDelayMinMilliseconds", 40, 1500),
                Number("Item delay maximum (ms)", "ItemTransferDelayMaxMilliseconds", 40, 1500),
                Button("DEBUG: Stash non-Chart inventory", delegate
                {
                    _settings.Save();
                    AllflameStashDebug.Request();
                }),
                Note("Debug runs while the bot is started on The Sovereign. It preserves every Chart Map."),
                Note("Inventory-threshold policy is active. After-Collect policy still waits for a confirmed Collect Loot transfer."))));
            Grid.SetColumn(grid.Children[2], 2);

            grid.Children.Add(Card("VOYAGE STASH", Stack(
                Check("Withdraw Charts before running", "CheckVoyageStashFirst"),
                Text("Chart stash tab name/number", "VoyageStashTab"),
                Number("Charts per withdrawal", "ChartsPerWithdrawal", 1, 60),
                Button("DEBUG: Read visible Chart Storage page", delegate
                {
                    AllflameChartStorageDebug.Request();
                }),
                _chartStorageDebugStatus,
                Note("Start the bot on The Sovereign. Debug opens Chart Storage, reads the visible page and never withdraws an item."),
                Note("The bot withdraws the configured Chart batch only when no Charts remain in inventory."))));
            Grid.SetColumn(grid.Children[3], 3);
            return grid;
        }

        private UIElement BuildChestPicker()
        {
            var panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(Check("Open and loot selected chests", "OpenKnownChestsFirst"));
            panel.Children.Add(Note("Priority 0-100: a higher value opens first; distance breaks ties."));
            _chestRows = new StackPanel();
            panel.Children.Add(_chestRows);
            panel.Children.Add(Button("+", delegate { AddChestRow(); }));
            RebuildChestRows();
            return panel;
        }

        private UIElement BuildVoyageTab()
        {
            var grid = PanelGrid(4);
            grid.Children.Add(Card("CHARTS", Stack(
                Check("Automatically plan Voyage", "AutoPlanVoyage"),
                Number("Charts required", "ChartsRequiredForVoyage", 0, 60),
                Check("Start Voyage when ready", "StartVoyageWhenReady"))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("VOYAGE RUNNER", Stack(
                Check("Enable Voyage runner", "EnableVoyageRunner"),
                Number("Micro-frontier spacing", "VoyageFrontierSpacing", 70, 140),
                Number("Frontier timeout (seconds)", "VoyageFrontierTimeoutSeconds", 7, 30),
                Check("Leave Voyage after time limit", "EnableVoyageTimeLimit"),
                Number("Voyage time limit (minutes)", "VoyageTimeLimitMinutes", 1, 120),
                Check("Leave Voyage when inventory is low", "LeaveVoyageWhenInventoryLow"),
                Number("Leave when free squares reach", "LeaveVoyageFreeSquares", 0, 60),
                Check("Transfer at sector boundaries", "TransferAtVoyageSectorBoundaries"),
                Note("Uses dynamic sectors grown from reachable positions; it does not force a fixed 3x3 map."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("CAPSULE", Stack(
                Check("Send loot to Surface", "SendLootToSurface"),
                Number("Send when free squares reach", "SendToSurfaceFreeSlots", 0, 60),
                Number("Send after bottles collected", "BottlesPerSurfaceSend", 1, 20),
                Check("Always send before extraction", "AlwaysSendBeforeVoyageExtraction"),
                Check("Call Bathysphere at 0 lanterns", "CallBathysphereWhenLanternsEmpty"),
                Button("DEBUG: Send to Surface now", delegate
                {
                    AllflameVoyageDebug.RequestSendToSurface();
                }),
                Note("Debug runs while the bot is started inside a Voyage and sends all non-Chart inventory items through a Capsule."),
                Note("Voyage uses the first 9 Crossing Charts; no rotation is performed."))));
            Grid.SetColumn(grid.Children[2], 2);

            grid.Children.Add(Card("BATHYSPHERE WAIT", Stack(
                Check("Keep moving while waiting", "KeepMovingWhileWaiting"),
                Number("Movement radius", "WaitMoveRadius", 5, 80),
                Number("Timeout in seconds", "BathysphereWaitTimeoutSeconds", 15, 180))));
            Grid.SetColumn(grid.Children[3], 3);
            return grid;
        }

        private UIElement BuildLootTab()
        {
            var grid = PanelGrid(4);
            grid.Children.Add(Card("SURFACE LOOT", Stack(
                Check("Collect loot on The Sovereign", "CollectSurfaceLoot"),
                Check("Use Ctrl + Click to withdraw", "CtrlClickCollectLoot"),
                Button("DEBUG: Loot Collect Loot now", delegate
                {
                    AllflameVoyageDebug.RequestCollectSurfaceLoot();
                }),
                Note("Debug runs while the bot is started on The Sovereign and withdraws every item currently in Collect Loot."),
                Note("Items sent through Capsules are expected in the Collect Loot container."))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("INVENTORY", Stack(
                Number("Reserve free inventory squares", "SendToSurfaceFreeSlots", 0, 60),
                Check("Use Capsule when threshold is reached", "SendLootToSurface"))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("GROUND LOOT", Stack(
                Check("Loot after opening chests", "EnableGroundLoot"),
                Check("Wait until nearby monsters are gone", "LootOnlyWhenSafe"),
                Check("Only loot items visible by the in-game filter", "OnlyLootVisibleByGameFilter"),
                Number("Loot radius around chest", "GroundLootRadius", 15, 200),
                Number("Drop settle delay (ms)", "LootSettleMilliseconds", 150, 2000),
                Number("Delay between loot actions (ms)", "LootActionDelayMilliseconds", 30, 500),
                Number("Failed pickup retry delay (ms)", "LootRetryDelayMilliseconds", 100, 1000),
                Number("Maximum attempts per item", "MaxLootAttempts", 1, 8),
                Number("At 0 Lantern: loot items worth at least (Divine)", "ZeroLanternLootMinDivines", 1, 10000),
                Note("At 0/x Lantern the bot leaves immediately unless a tracked chest drop meets this price threshold."),
                Note("Filter-visible drops remain required until the WorldItem disappears."))));
            Grid.SetColumn(grid.Children[2], 2);

            grid.Children.Add(Card("PICKIT ITEM LOGIC", Stack(
                Check("Pickup everything", "PickupEverything"),
                Check("Seasonal rewards", "PickupSeasonalRewards"),
                Check("All currency", "PickupCurrency"),
                Check("Ignore Scroll of Wisdom", "IgnoreScrollOfWisdom"),
                Check("All Divination Cards", "PickupDivinationCards"),
                Check("All Scarabs", "PickupScarabs"),
                Check("All Uniques", "PickupUniques"),
                DecimalNumber("Minimum Unique value (Divine, 0 = all)", "MinimumUniqueValueDivines", 0d, 10000d),
                Check("Maps", "PickupMaps"),
                Number("Minimum map tier", "MinimumMapTier", 1, 16),
                Check("Quest items", "PickupQuestItems"),
                Check("Gems", "PickupGems"),
                Number("Minimum gem quality", "MinimumGemQuality", 0, 20),
                DecimalNumber("Minimum Gem value (Divine, 0 = all)", "MinimumGemValueDivines", 0d, 10000d),
                Check("Flasks", "PickupFlasks"),
                Number("Minimum flask quality", "MinimumFlaskQuality", 0, 20),
                Check("Rares", "PickupRares"),
                Check("Socket items", "PickupSixSockets"),
                Number("Minimum sockets", "MinimumSockets", 1, 6),
                Check("Linked items", "PickupLinkedItems"),
                Number("Minimum links", "MinimumLinks", 1, 6),
                Check("Walk over Gold (do not click)", "WalkOverGold"),
                Note("Unique/Gem value thresholds apply when Pickup everything is off. With a positive threshold, unpriced items are rejected."))));
            Grid.SetColumn(grid.Children[3], 3);
            return grid;
        }

        private UIElement BuildExtraTab()
        {
            var grid = PanelGrid();
            grid.Children.Add(Card("SAFETY", Stack(
                Check("Avoid unlit areas", "AvoidUnlitAreas"),
                Check("Prioritize DPB topology rooms", "EnableTopologyRoomRouting"),
                Check("Use hybrid layout-frontier planner", "EnableHybridLayoutPlanner"),
                Number("Layout frontier spacing", "LayoutFrontierStep", 70, 140),
                Check("Stop on repeated failure", "StopOnRepeatedFailure"),
                Number("Maximum repeated failures", "MaxRepeatedFailures", 1, 10),
                Check("Close game on stop", "CloseGameOnStop"),
                Note("When enabled, every UserStop, RecoveryStop or FatalStop requests DreamPoeBot to exit the game client."))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("LIFE FLASK", Stack(
                Check("Use Life Flask below threshold", "EnableLifeFlask"),
                Number("Life threshold (%)", "LifeFlaskUseThreshold", 1, 100),
                Check("Use utility flasks periodically", "EnableUtilityFlasks"),
                Text("Utility flask slots", "UtilityFlaskSlots"),
                Number("Utility interval (seconds)", "UtilityFlaskIntervalSeconds", 1, 60),
                Check("Disable utility flasks on boat", "DisableUtilityFlasksOnBoat"),
                Check("Use Enduring Cry when ready", "EnableEnduringCry"),
                Number("Enduring Cry slot (0 = auto)", "EnduringCrySlot", 0, 13),
                Note("Slots use the in-game numbering 1-5; default utility slots are 2,3,4,5."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("IMPLEMENTATION STATUS", Stack(
                Note("Version " + AllflameVersion.Current + " adds Secret Bottle stash withdrawal and independent processing in the Sovereign Hub."),
                Note("Topology rooms are optional discovery hints; invalid or blocked rooms automatically fall back to the existing frontier planner."),
                Note("The planner uses 9 Crossing Charts without rotation."),
                Note("Settings are persisted by DreamPoeBot JsonSettings."))));
            Grid.SetColumn(grid.Children[2], 2);
            return grid;
        }

        private UIElement BuildSecretTab()
        {
            var grid = PanelGrid(3);
            grid.Children.Add(Card("MESSAGE IN A BOTTLE", Stack(
                Check("Enable automatic Bottle processing", "EnableSecretBottleProcessing"),
                Number("Open when Bottle count reaches", "SecretBottlesToOpen", 1, 100),
                Number("Maximum Bottles per session", "SecretMaxBottlesPerSession", 1, 100),
                Number("Minimum free inventory squares", "SecretBottleMinimumFreeSquares", 2, 60),
                Check("Read Bottles from Guild Stash", "SecretBottleUseGuildStash"),
                Text("Bottle stash tab name or number", "SecretBottleStashTab"),
                Note("The threshold is counted from the live DPB inventory, independently from Send after bottles collected."))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("PRICE DECISION", Stack(
                DecimalNumber("Keep when value reaches (Divine)", "SecretBottleKeepMinDivines", 0d, 10000d),
                Check("Sell rewards below threshold", "SecretBottleSellLowValue"),
                Note("Unpriced rewards, stale price data and ambiguous stack merges are always kept."),
                Note("Fixed price overrides from the Prices tab take priority."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("NPC SELL", Stack(
                Text("Vendor NPC name (blank = auto detect)", "SecretBottleVendorName"),
                Note("Selling only proceeds after DPB confirms SellUi and the exact low-value reward IDs."),
                Note("If no valid vendor or UI is found, every reward remains in inventory and falls back to the normal stash task."),
                Number("Reveal timeout (seconds)", "SecretBottleRevealTimeoutSeconds", 3, 30),
                Number("Cursor placement retries", "SecretBottleCursorPlacementRetries", 1, 5),
                Note("Bottle reveal detection prefers the DPB element tree and keeps image matching as a fallback."))));
            Grid.SetColumn(grid.Children[2], 2);
            return grid;
        }

        private UIElement BuildPricesTab()
        {
            var grid = PanelGrid(3);
            _priceStatus = new TextBlock
            {
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap
            };
            RefreshPriceStatus();

            grid.Children.Add(Card("PRICE SOURCE", Stack(
                Check("Enable item pricing", "EnablePricing"),
                Check("Update prices automatically", "AutoUpdatePrices"),
                Text("League name", "PricingLeague"),
                Number("Update every (hours)", "PriceUpdateHours", 1, 168),
                Note("League must match the public price feed, for example Standard or the current challenge league."))));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("FIXED PRICE OVERRIDES", Stack(
                Text("Item=divines; Item 2=divines", "FixedPrices"),
                Button("Apply fixed prices", delegate
                {
                    AllflamePrices.ApplyFixedPrices();
                    AllflamePrices.Save();
                    RefreshPriceStatus();
                }),
                Note("Overrides are expressed in Divine Orbs and take priority over downloaded prices."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("PRICE CACHE", Stack(
                _priceStatus,
                Button("Update prices now", async delegate
                {
                    _settings.Save();
                    await AllflamePrices.UpdateAsync(_settings.PricingLeague);
                    RefreshPriceStatus();
                }),
                Note("The cache is stored under Settings\\AllflameBot\\prices.json. The latest 10 backups are retained."))));
            Grid.SetColumn(grid.Children[2], 2);
            return grid;
        }

        private UIElement BuildNotificationsTab()
        {
            var grid = PanelGrid(4);
            _notificationStatus = new TextBlock
            {
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap
            };
            RefreshNotificationStatus();

            grid.Children.Add(Card("DISCORD", Stack(
                Check("Enable Discord notifications", "DiscordNotificationsEnabled"),
                Text("Webhook URL", "DiscordWebhookUrl"),
                Button("Send test notification", delegate
                {
                    _settings.Save();
                    AllflameNotifications.SendTest();
                }),
                _notificationStatus)));
            Grid.SetColumn(grid.Children[0], 0);

            grid.Children.Add(Card("SESSION SUMMARY", Stack(
                Number("Maximum item rows", "DiscordMaxItemCount", 1, 15),
                Note("The dashboard updates only at session start, map enter, map exit and terminal stop."),
                Note("Loot, chests, Sulphur, deaths and alerts are staged until the next boundary."))));
            Grid.SetColumn(grid.Children[1], 1);

            grid.Children.Add(Card("SUPPLY WARNINGS", Stack(
                Number("Warn when total Charts reach", "LowChartWarningThreshold", 0, 1000),
                Check("Tag @everyone when Voyage Stash is low", "TagEveryoneForLowVoyageStash"),
                Number("Voyage Stash @everyone threshold", "VoyageStashEveryoneThreshold", 0, 1000),
                Note("Remaining resources combine inventory Charts and the last scanned Voyage Stash count."),
                Note("The Voyage Stash alert is sent once when the confirmed count reaches the configured threshold."))));
            Grid.SetColumn(grid.Children[2], 2);

            grid.Children.Add(Card("VALUABLE ITEMS", Stack(
                Check("Notify valuable item drops", "NotifyValuableItems"),
                Check("Tag @everyone for valuable items", "TagEveryoneForValuableItems"),
                Number("Minimum value (Divine Orbs)", "ValuableItemMinDivines", 1, 10000),
                Note("Set the minimum to 1 to ping @everyone for confirmed pickups worth at least 1 Divine."),
                Note("The alert is emitted only after the pickup is confirmed, so retries do not duplicate it."))));
            Grid.SetColumn(grid.Children[3], 3);
            return grid;
        }

        private void RefreshPriceStatus()
        {
            if (_priceStatus == null)
            {
                return;
            }
            DateTime updated = AllflamePrices.UpdatedUtc;
            _priceStatus.Text =
                "Entries: " + AllflamePrices.Count +
                "\nLeague: " +
                (String.IsNullOrWhiteSpace(AllflamePrices.League)
                    ? "Not configured"
                    : AllflamePrices.League) +
                "\nUpdated: " +
                (updated == DateTime.MinValue
                    ? "Never"
                    : updated.ToLocalTime().ToString("yyyy-MM-dd HH:mm")) +
                "\nStatus: " +
                (AllflamePrices.UpdateIsRunning ? "Updating..." : "Ready");
        }

        private void RefreshNotificationStatus()
        {
            if (_notificationStatus == null)
            {
                return;
            }
            _notificationStatus.Text =
                String.IsNullOrWhiteSpace(_settings.DiscordWebhookUrl)
                    ? "Webhook is not configured."
                    : "Webhook configured. Use the test button to verify delivery.";
        }

        private void RebuildChestRows()
        {
            if (_chestRows == null)
            {
                return;
            }

            _chestRows.Children.Clear();
            foreach (ChestOption option in ChestOptions)
            {
                if (GetChestEnabled(option))
                {
                    _chestRows.Children.Add(BuildChestRow(option));
                }
            }

            if (_chestRows.Children.Count == 0)
            {
                _chestRows.Children.Add(Note("No chest selected. Press + to add one."));
            }
        }

        private UIElement BuildChestRow(ChestOption selected)
        {
            var row = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            row.ColumnDefinitions.Add(new ColumnDefinition());
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(62) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(34) });

            var combo = new ComboBox
            {
                ItemsSource = ChestOptions,
                SelectedItem = selected,
                Background = Brush("#777777"),
                Foreground = Brushes.White,
                BorderBrush = Brush("#b0b0b0"),
                Padding = new Thickness(4),
                FontSize = 12
            };
            row.Children.Add(combo);

            var priority = PriorityInput(selected);
            priority.Margin = new Thickness(4, 0, 0, 0);
            Grid.SetColumn(priority, 1);
            row.Children.Add(priority);

            var remove = Button("X", delegate
            {
                SetChestEnabled(selected, false);
                _settings.Save();
                RebuildChestRows();
            });
            remove.Margin = new Thickness(4, 0, 0, 0);
            remove.Padding = new Thickness(4, 2, 4, 2);
            Grid.SetColumn(remove, 2);
            row.Children.Add(remove);

            combo.SelectionChanged += delegate
            {
                var replacement = combo.SelectedItem as ChestOption;
                if (replacement == null || replacement == selected)
                {
                    return;
                }

                SetChestEnabled(selected, false);
                SetChestEnabled(replacement, true);
                _settings.Save();
                RebuildChestRows();
            };
            return row;
        }

        private TextBox PriorityInput(ChestOption option)
        {
            var property = typeof(AllflameSettings).GetProperty(
                option.PriorityPropertyName);
            var input = new TextBox
            {
                Text = Convert.ToString(
                    property.GetValue(_settings, null),
                    CultureInfo.InvariantCulture),
                ToolTip = "Chest priority (0-100)",
                Foreground = Brushes.White,
                Background = Brush("#3a3a3a"),
                BorderBrush = Brush("#b0b0b0"),
                Padding = new Thickness(4),
                FontSize = 12
            };
            input.LostFocus += delegate
            {
                int value;
                if (!Int32.TryParse(input.Text, out value))
                {
                    value = (int)property.GetValue(_settings, null);
                }
                value = Math.Max(0, Math.Min(100, value));
                input.Text = value.ToString(CultureInfo.InvariantCulture);
                property.SetValue(_settings, value, null);
                _settings.Save();
            };
            return input;
        }

        private void AddChestRow()
        {
            foreach (ChestOption option in ChestOptions)
            {
                if (!GetChestEnabled(option))
                {
                    SetChestEnabled(option, true);
                    _settings.Save();
                    RebuildChestRows();
                    return;
                }
            }
        }

        private bool GetChestEnabled(ChestOption option)
        {
            var property = typeof(AllflameSettings).GetProperty(option.PropertyName);
            return property != null && (bool)property.GetValue(_settings, null);
        }

        private void SetChestEnabled(ChestOption option, bool enabled)
        {
            var property = typeof(AllflameSettings).GetProperty(option.PropertyName);
            if (property != null)
            {
                property.SetValue(_settings, enabled, null);
            }
        }

        private TabItem Tab(string title, UIElement content)
        {
            return new TabItem
            {
                Header = title,
                Content = new ScrollViewer
                {
                    Content = content,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                },
                Foreground = TextBrush,
                Background = BackgroundBrush,
                FontSize = 20
            };
        }

        private Grid PanelGrid()
        {
            return PanelGrid(3);
        }

        private Grid PanelGrid(int columns)
        {
            var grid = new Grid { Margin = new Thickness(8) };
            for (int index = 0; index < columns; index++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            return grid;
        }

        private Border Card(string title, UIElement body)
        {
            var panel = new DockPanel();
            var header = new Border
            {
                Background = BlueBrush,
                Padding = new Thickness(10, 7, 10, 7),
                Child = new TextBlock
                {
                    Text = title,
                    Foreground = Brushes.White,
                    FontSize = 12
                }
            };
            DockPanel.SetDock(header, Dock.Top);
            panel.Children.Add(header);
            panel.Children.Add(body);

            return new Border
            {
                Background = CardBrush,
                BorderBrush = BlueBrush,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(6),
                Child = panel,
                VerticalAlignment = VerticalAlignment.Top
            };
        }

        private StackPanel Stack(params UIElement[] children)
        {
            var stack = new StackPanel { Margin = new Thickness(10) };
            foreach (UIElement child in children)
            {
                stack.Children.Add(child);
            }
            return stack;
        }

        private CheckBox Check(string label, string propertyName)
        {
            var property = typeof(AllflameSettings).GetProperty(propertyName);
            var box = new CheckBox
            {
                Content = label,
                IsChecked = (bool)property.GetValue(_settings, null),
                Foreground = TextBrush,
                Margin = new Thickness(0, 5, 0, 5),
                FontSize = 13
            };
            box.Checked += delegate
            {
                property.SetValue(_settings, true, null);
                _settings.Save();
            };
            box.Unchecked += delegate
            {
                property.SetValue(_settings, false, null);
                _settings.Save();
            };
            return box;
        }

        private UIElement Number(string label, string propertyName, int min, int max)
        {
            var property = typeof(AllflameSettings).GetProperty(propertyName);
            var panel = new Grid { Margin = new Thickness(0, 5, 0, 5) };
            panel.ColumnDefinitions.Add(new ColumnDefinition());
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            var text = new TextBlock
            {
                Text = label,
                Foreground = TextBrush,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13
            };
            panel.Children.Add(text);

            var input = new TextBox
            {
                Text = Convert.ToString(property.GetValue(_settings, null), CultureInfo.InvariantCulture),
                Foreground = Brushes.White,
                Background = Brush("#3a3a3a"),
                BorderBrush = Brush("#b0b0b0"),
                Padding = new Thickness(5),
                FontSize = 13
            };
            Grid.SetColumn(input, 1);
            panel.Children.Add(input);

            input.LostFocus += delegate
            {
                int value;
                if (!Int32.TryParse(input.Text, out value))
                {
                    value = (int)property.GetValue(_settings, null);
                }
                value = Math.Max(min, Math.Min(max, value));
                input.Text = value.ToString(CultureInfo.InvariantCulture);
                property.SetValue(_settings, value, null);
                _settings.Save();
            };

            return panel;
        }

        private UIElement DecimalNumber(
            string label,
            string propertyName,
            double min,
            double max)
        {
            var property =
                typeof(AllflameSettings).GetProperty(propertyName);
            var panel =
                new Grid { Margin = new Thickness(0, 5, 0, 5) };
            panel.ColumnDefinitions.Add(new ColumnDefinition());
            panel.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(80) });

            panel.Children.Add(new TextBlock
            {
                Text = label,
                Foreground = TextBrush,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13
            });

            var input = new TextBox
            {
                Text = Convert.ToDouble(
                    property.GetValue(_settings, null),
                    CultureInfo.InvariantCulture)
                    .ToString("0.###", CultureInfo.InvariantCulture),
                Foreground = Brushes.White,
                Background = Brush("#3a3a3a"),
                BorderBrush = Brush("#b0b0b0"),
                Padding = new Thickness(5),
                FontSize = 13
            };
            Grid.SetColumn(input, 1);
            panel.Children.Add(input);

            input.LostFocus += delegate
            {
                double value;
                if (!Double.TryParse(
                        input.Text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out value) &&
                    !Double.TryParse(
                        input.Text,
                        NumberStyles.Float,
                        CultureInfo.CurrentCulture,
                        out value))
                {
                    value = Convert.ToDouble(
                        property.GetValue(_settings, null),
                        CultureInfo.InvariantCulture);
                }
                value = Math.Max(min, Math.Min(max, value));
                input.Text =
                    value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture);
                property.SetValue(_settings, value, null);
                _settings.Save();
            };

            return panel;
        }

        private UIElement Text(string label, string propertyName)
        {
            var property = typeof(AllflameSettings).GetProperty(propertyName);
            var panel = new Grid { Margin = new Thickness(0, 5, 0, 5) };
            panel.RowDefinitions.Add(new RowDefinition());
            panel.RowDefinitions.Add(new RowDefinition());

            var text = new TextBlock
            {
                Text = label,
                Foreground = TextBrush,
                FontSize = 13
            };
            panel.Children.Add(text);

            var input = new TextBox
            {
                Text = Convert.ToString(property.GetValue(_settings, null), CultureInfo.InvariantCulture),
                Foreground = Brushes.White,
                Background = Brush("#3a3a3a"),
                BorderBrush = Brush("#b0b0b0"),
                Padding = new Thickness(5),
                Margin = new Thickness(0, 4, 0, 0),
                FontSize = 13
            };
            Grid.SetRow(input, 1);
            panel.Children.Add(input);

            input.TextChanged += delegate
            {
                property.SetValue(
                    _settings,
                    input.Text == null ? String.Empty : input.Text.Trim(),
                    null);
                _settings.Save();
            };

            return panel;
        }

        private Button Button(string label, RoutedEventHandler handler)
        {
            var button = new Button
            {
                Content = label,
                Background = Brush("#4a4a4a"),
                Foreground = Brushes.White,
                BorderBrush = Brush("#b0b0b0"),
                Margin = new Thickness(0, 8, 0, 0),
                Padding = new Thickness(8, 5, 8, 5)
            };
            button.Click += handler;
            return button;
        }

        private TextBlock Note(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 5, 0, 5),
                FontSize = 12
            };
        }

        private static Brush Brush(string color)
        {
            return (Brush)new BrushConverter().ConvertFromString(color);
        }
    }
}
