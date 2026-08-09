using System;
using System.Reflection;
using System.Threading.Tasks;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Game;

namespace AllflameBot
{
    internal sealed class DpbActionResult
    {
        public string Mode { get; set; }
        public bool ApiAvailable { get; set; }
        public bool InputSent { get; set; }
        public bool PostconditionVerified { get; set; }
        public string Reason { get; set; }
        public string Exception { get; set; }

        public static DpbActionResult Failure(
            string reason,
            Exception exception = null)
        {
            return new DpbActionResult
            {
                Mode = "Native",
                ApiAvailable = exception == null,
                InputSent = false,
                PostconditionVerified = false,
                Reason = reason ?? "Unknown",
                Exception = exception == null
                    ? String.Empty
                    : exception.GetType().Name
            };
        }
    }

    internal sealed class DpbCapsuleSnapshot
    {
        public bool IsOpened { get; set; }
        public bool SendToSurfaceEnabled { get; set; }
        public bool CallBathysphereEnabled { get; set; }
        public InventoryControlWrapper InventoryControl { get; set; }
    }

    internal sealed class DpbDeepwaterCounterSnapshot
    {
        public int MaxLanterns { get; set; }
        public int RemainingLanterns { get; set; }
        public int CollectedSulphur { get; set; }
    }

    internal sealed class DpbBlockingUiSnapshot
    {
        public bool IsReadComplete { get; set; }
        public int ReadFailures { get; set; }
        public string ReadFailureNames { get; set; }
        public bool VoyagePlannerOpened { get; set; }
        public bool DeepwaterChartDeviceOpened { get; set; }
        public bool MapDeviceOpened { get; set; }
        public bool LootOpened { get; set; }
        public bool BottleRevealOpened { get; set; }
        public bool CapsuleOpened { get; set; }
        public bool StashOpened { get; set; }
        public bool GuildStashOpened { get; set; }
        public bool NpcDialogOpened { get; set; }
        public bool SellOpened { get; set; }
        public bool RewardOpened { get; set; }
        public bool ContextPlannerVisible { get; set; }
        public bool ContextChartDeviceVisible { get; set; }
        public bool ContextCapsuleVisible { get; set; }

        public bool HasNativeWindow
        {
            get
            {
                return
                    VoyagePlannerOpened ||
                    DeepwaterChartDeviceOpened ||
                    MapDeviceOpened ||
                    LootOpened ||
                    BottleRevealOpened ||
                    CapsuleOpened ||
                    StashOpened ||
                    GuildStashOpened ||
                    NpcDialogOpened ||
                    SellOpened ||
                    RewardOpened;
            }
        }

        public bool HasBlockingSignal
        {
            get
            {
                return
                    HasNativeWindow ||
                    ContextPlannerVisible ||
                    ContextChartDeviceVisible ||
                    ContextCapsuleVisible ||
                    !IsReadComplete;
            }
        }

        public string Describe()
        {
            return
                "Planner=" + VoyagePlannerOpened +
                ",DeepwaterChart=" + DeepwaterChartDeviceOpened +
                ",MapDevice=" + MapDeviceOpened +
                ",Loot=" + LootOpened +
                ",Bottle=" + BottleRevealOpened +
                ",Capsule=" + CapsuleOpened +
                ",Stash=" + StashOpened +
                ",GuildStash=" + GuildStashOpened +
                ",Dialog=" + NpcDialogOpened +
                ",Sell=" + SellOpened +
                ",Reward=" + RewardOpened +
                ",ContextPlanner=" + ContextPlannerVisible +
                ",ContextChartDevice=" + ContextChartDeviceVisible +
                ",ContextCapsule=" + ContextCapsuleVisible +
                ",ReadComplete=" + IsReadComplete +
                ",ReadFailures=" + ReadFailures +
                ",FailureNames=" +
                (ReadFailureNames ?? String.Empty);
        }
    }

    internal static class DpbNativeApi
    {
        private static bool _capabilitiesLogged;

        public static void LogCapabilities()
        {
            if (_capabilitiesLogged)
            {
                return;
            }
            _capabilitiesLogged = true;

            Version assemblyVersion = null;
            string assemblyName = "Unknown";
            try
            {
                Assembly assembly = typeof(LokiPoe).Assembly;
                assemblyVersion = assembly.GetName().Version;
                assemblyName = assembly.FullName ?? "Unknown";
            }
            catch
            {
            }

            AllflameLog.Info(
                "DPB_API_CAPABILITIES" +
                "|Assembly=" +
                Sanitize(assemblyName) +
                "|Version=" +
                (assemblyVersion == null
                    ? "Unknown"
                    : assemblyVersion.ToString()) +
                "|WorldTravel=Compiled" +
                "|Capsule=Compiled" +
                "|Loot=Compiled" +
                "|BottleReveal=Compiled" +
                "|ChartDevice=Compiled" +
                "|VoyagePlanner=Compiled" +
                "|DeepwaterCounters=Compiled" +
                "|NativeTravel=" +
                AllflameSettings.Instance.UseNativeSovereignTravel +
                "|NativeCapsule=" +
                AllflameSettings.Instance.UseNativeCapsuleActions +
                "|NativeLoot=" +
                AllflameSettings.Instance.UseNativeLootUi +
                "|NativeBottle=" +
                AllflameSettings.Instance.UseNativeBottleReveal +
                "|NativeChartDevice=" +
                AllflameSettings.Instance.UseNativeChartDevice +
                "|NativeCounters=" +
                AllflameSettings.Instance.UseNativeDeepwaterCounters +
                "|NativeVoyageRead=" +
                AllflameSettings.Instance.UseNativeVoyagePlannerRead +
                "|NativeVoyageActions=" +
                AllflameSettings.Instance.UseNativeVoyagePlannerActions +
                "|NativeVoyageStash=StableStashUi" +
                "|BlockingWindowCleanup=Compiled");
        }

        public static async Task<DpbActionResult>
            CloseBlockingWindowsAsync()
        {
            try
            {
                await DreamPoeBot.Loki.Bot.Coroutines
                    .CloseBlockingWindows();
                return new DpbActionResult
                {
                    Mode = "Native",
                    ApiAvailable = true,
                    InputSent = true,
                    PostconditionVerified = false,
                    Reason = "Invoked",
                    Exception = String.Empty
                };
            }
            catch (Exception exception)
            {
                return DpbActionResult.Failure(
                    "CloseBlockingWindowsException",
                    exception);
            }
        }

        public static DpbBlockingUiSnapshot CaptureBlockingUi()
        {
            var snapshot = new DpbBlockingUiSnapshot
            {
                IsReadComplete = true,
                ReadFailureNames = String.Empty,
                ContextPlannerVisible = AllflameContext.PlanVoyageVisible,
                ContextChartDeviceVisible =
                    AllflameContext.ChartAreaOptionVisible,
                ContextCapsuleVisible = AllflameContext.CapsuleUiVisible
            };

            snapshot.VoyagePlannerOpened = ReadUiOpened(
                "VoyagePlanner",
                delegate
                {
                    return LokiPoe.InGameState.DeepwaterVoyageUi.IsOpened;
                },
                snapshot);
            snapshot.DeepwaterChartDeviceOpened = ReadUiOpened(
                "DeepwaterChartDevice",
                delegate
                {
                    return LokiPoe.InGameState.DeepwaterChartUi.IsOpened;
                },
                snapshot);
            snapshot.MapDeviceOpened = ReadUiOpened(
                "MapDevice",
                delegate
                {
                    return LokiPoe.InGameState.MapDeviceUi.IsOpened;
                },
                snapshot);
            snapshot.LootOpened = ReadUiOpened(
                "Loot",
                delegate
                {
                    return LokiPoe.InGameState.AllflameLootUi.IsOpened;
                },
                snapshot);
            snapshot.BottleRevealOpened = ReadUiOpened(
                "BottleReveal",
                delegate
                {
                    return LokiPoe.InGameState.BottledItemRevealUi.IsOpened;
                },
                snapshot);
            snapshot.CapsuleOpened = ReadUiOpened(
                "Capsule",
                delegate
                {
                    return LokiPoe.InGameState.AllflameCapsuleUi.IsOpened;
                },
                snapshot);
            snapshot.StashOpened = ReadUiOpened(
                "Stash",
                delegate
                {
                    return LokiPoe.InGameState.StashUi.IsOpened;
                },
                snapshot);
            snapshot.GuildStashOpened = ReadUiOpened(
                "GuildStash",
                delegate
                {
                    return LokiPoe.InGameState.GuildStashUi.IsOpened;
                },
                snapshot);
            snapshot.NpcDialogOpened = ReadUiOpened(
                "NpcDialog",
                delegate
                {
                    return LokiPoe.InGameState.NpcDialogUi.IsOpened;
                },
                snapshot);
            snapshot.SellOpened = ReadUiOpened(
                "Sell",
                delegate
                {
                    return LokiPoe.InGameState.SellUi.IsOpened;
                },
                snapshot);
            snapshot.RewardOpened = ReadUiOpened(
                "Reward",
                delegate
                {
                    return LokiPoe.InGameState.RewardUi.IsOpened;
                },
                snapshot);
            return snapshot;
        }

        public static DpbActionResult GoToTheSovereign()
        {
            try
            {
                var result =
                    LokiPoe.InGameState.WorldUi.GoToTheSovereign();
                string resultText = result.ToString();
                return new DpbActionResult
                {
                    Mode = "Native",
                    ApiAvailable = true,
                    InputSent = String.Equals(
                        resultText,
                        "None",
                        StringComparison.OrdinalIgnoreCase),
                    PostconditionVerified = false,
                    Reason = resultText,
                    Exception = String.Empty
                };
            }
            catch (Exception exception)
            {
                return DpbActionResult.Failure(
                    "GoToTheSovereignException",
                    exception);
            }
        }

        public static bool TryCaptureCapsule(
            out DpbCapsuleSnapshot snapshot,
            out string reason)
        {
            snapshot = null;
            reason = "Unknown";
            try
            {
                snapshot = new DpbCapsuleSnapshot
                {
                    IsOpened =
                        LokiPoe.InGameState.AllflameCapsuleUi.IsOpened,
                    SendToSurfaceEnabled =
                        LokiPoe.InGameState.AllflameCapsuleUi
                            .IsSendToSurfaceEnabled,
                    CallBathysphereEnabled =
                        LokiPoe.InGameState.AllflameCapsuleUi
                            .IsCaalBathysphereEnabled,
                    InventoryControl =
                        LokiPoe.InGameState.AllflameCapsuleUi.InventoryControl
                };
                reason = snapshot.IsOpened
                    ? "Opened"
                    : "Closed";
                return true;
            }
            catch (Exception exception)
            {
                reason = exception.GetType().Name;
                LogFailure("CapsuleSnapshot", exception);
                return false;
            }
        }

        public static DpbActionResult SendToSurface()
        {
            try
            {
                if (!LokiPoe.InGameState.AllflameCapsuleUi.IsOpened)
                {
                    return DpbActionResult.Failure("CapsuleClosed");
                }
                if (!LokiPoe.InGameState.AllflameCapsuleUi
                        .IsSendToSurfaceEnabled)
                {
                    return DpbActionResult.Failure(
                        "SendToSurfaceDisabled");
                }
                LokiPoe.InGameState.AllflameCapsuleUi.SendToSurface();
                return new DpbActionResult
                {
                    Mode = "Native",
                    ApiAvailable = true,
                    InputSent = true,
                    PostconditionVerified = false,
                    Reason = "Invoked",
                    Exception = String.Empty
                };
            }
            catch (Exception exception)
            {
                return DpbActionResult.Failure(
                    "SendToSurfaceException",
                    exception);
            }
        }

        public static DpbActionResult CallBathysphere()
        {
            try
            {
                if (!LokiPoe.InGameState.AllflameCapsuleUi.IsOpened)
                {
                    return DpbActionResult.Failure("CapsuleClosed");
                }
                if (!LokiPoe.InGameState.AllflameCapsuleUi
                        .IsCaalBathysphereEnabled)
                {
                    return DpbActionResult.Failure(
                        "CallBathysphereDisabled");
                }
                LokiPoe.InGameState.AllflameCapsuleUi.CaalBathysphere();
                return new DpbActionResult
                {
                    Mode = "Native",
                    ApiAvailable = true,
                    InputSent = true,
                    PostconditionVerified = false,
                    Reason = "Invoked",
                    Exception = String.Empty
                };
            }
            catch (Exception exception)
            {
                return DpbActionResult.Failure(
                    "CallBathysphereException",
                    exception);
            }
        }

        public static bool TryCaptureLoot(
            out bool isOpened,
            out InventoryControlWrapper inventoryControl,
            out string reason)
        {
            isOpened = false;
            inventoryControl = null;
            reason = "Unknown";
            try
            {
                isOpened = LokiPoe.InGameState.AllflameLootUi.IsOpened;
                inventoryControl =
                    LokiPoe.InGameState.AllflameLootUi.InventoryControl;
                reason = isOpened ? "Opened" : "Closed";
                return true;
            }
            catch (Exception exception)
            {
                reason = exception.GetType().Name;
                LogFailure("LootSnapshot", exception);
                return false;
            }
        }

        public static bool TryCaptureBottleReveal(
            out bool isOpened,
            out InventoryControlWrapper inventoryControl,
            out string reason)
        {
            isOpened = false;
            inventoryControl = null;
            reason = "Unknown";
            try
            {
                isOpened = LokiPoe.InGameState.BottledItemRevealUi.IsOpened;
                inventoryControl =
                    LokiPoe.InGameState.BottledItemRevealUi.InventoryControl;
                reason = isOpened ? "Opened" : "Closed";
                return true;
            }
            catch (Exception exception)
            {
                reason = exception.GetType().Name;
                LogFailure("BottleRevealSnapshot", exception);
                return false;
            }
        }

        public static DpbActionResult AcceptBottleReveal()
        {
            try
            {
                if (!LokiPoe.InGameState.BottledItemRevealUi.IsOpened)
                {
                    return DpbActionResult.Failure(
                        "BottleRevealClosed");
                }
                LokiPoe.InGameState.BottledItemRevealUi.Accept();
                return new DpbActionResult
                {
                    Mode = "Native",
                    ApiAvailable = true,
                    InputSent = true,
                    PostconditionVerified = false,
                    Reason = "Invoked",
                    Exception = String.Empty
                };
            }
            catch (Exception exception)
            {
                return DpbActionResult.Failure(
                    "BottleRevealAcceptException",
                    exception);
            }
        }

        public static bool TryCaptureDeepwaterCounters(
            out DpbDeepwaterCounterSnapshot snapshot,
            out string reason)
        {
            snapshot = null;
            reason = "Unknown";
            try
            {
                snapshot = new DpbDeepwaterCounterSnapshot
                {
                    MaxLanterns = Convert.ToInt32(
                        LokiPoe.InstanceInfo.Deepwater.MaxLanterns),
                    RemainingLanterns = Convert.ToInt32(
                        LokiPoe.InstanceInfo.Deepwater.RemainingLanterns),
                    CollectedSulphur = Convert.ToInt32(
                        LokiPoe.InstanceInfo.Deepwater.CollectedSulphur)
                };
                reason = "Available";
                return true;
            }
            catch (Exception exception)
            {
                reason = exception.GetType().Name;
                LogFailure("DeepwaterCounters", exception);
                return false;
            }
        }

        private static void LogFailure(
            string operation,
            Exception exception)
        {
            AllflameLog.Throttled(
                "dpb_native_" + operation,
                TimeSpan.FromSeconds(5),
                "DPB_NATIVE_FAILED" +
                "|Operation=" + operation +
                "|Exception=" + exception.GetType().Name +
                "|Message=" + Sanitize(exception.Message));
        }

        private static bool ReadUiOpened(
            string name,
            Func<bool> read,
            DpbBlockingUiSnapshot snapshot)
        {
            try
            {
                return read();
            }
            catch (Exception exception)
            {
                snapshot.IsReadComplete = false;
                snapshot.ReadFailures++;
                if (!String.IsNullOrEmpty(snapshot.ReadFailureNames))
                {
                    snapshot.ReadFailureNames += ",";
                }
                snapshot.ReadFailureNames +=
                    name + ":" + exception.GetType().Name;
                return false;
            }
        }

        private static string Sanitize(string value)
        {
            return (value ?? String.Empty)
                .Replace("|", "/")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }
    }

    internal static class DpbUiCleanupCoordinator
    {
        private static readonly TimeSpan VerificationTimeout =
            TimeSpan.FromSeconds(3);
        private static readonly TimeSpan RetryCooldown =
            TimeSpan.FromSeconds(1);
        private static bool _verificationPending;
        private static uint _issuedAreaHash;
        private static DateTime _issuedUtc = DateTime.MinValue;
        private static DateTime _nextAttemptUtc = DateTime.MinValue;
        private static int _attempts;

        public static void Reset()
        {
            _verificationPending = false;
            _issuedAreaHash = 0;
            _issuedUtc = DateTime.MinValue;
            _nextAttemptUtc = DateTime.MinValue;
            _attempts = 0;
        }

        public static async Task<bool> EnsureClosedAsync(
            string taskName,
            bool requirePostcondition)
        {
            DateTime now = DateTime.UtcNow;
            uint currentAreaHash = AllflameContext.AreaHash;
            DpbBlockingUiSnapshot snapshot =
                DpbNativeApi.CaptureBlockingUi();

            if (_verificationPending &&
                _issuedAreaHash != 0 &&
                currentAreaHash != 0 &&
                _issuedAreaHash != currentAreaHash)
            {
                AllflameLog.Info(
                    "DPB_UI_CLEANUP" +
                    "|Task=" + taskName +
                    "|State=AreaChanged" +
                    "|Postcondition=Unavailable" +
                    "|PreviousAreaHash=" + _issuedAreaHash +
                    "|CurrentAreaHash=" + currentAreaHash +
                    "|Reason=AreaTransition");
                _verificationPending = false;
                _issuedAreaHash = 0;
                _issuedUtc = DateTime.MinValue;
                _nextAttemptUtc = DateTime.MinValue;
                _attempts = 0;
            }

            if (_verificationPending)
            {
                if (snapshot.IsReadComplete &&
                    !snapshot.HasBlockingSignal)
                {
                    AllflameLog.Info(
                        "DPB_UI_CLEANUP" +
                        "|Task=" + taskName +
                        "|State=Verified" +
                        "|Postcondition=True" +
                        "|Attempt=" + _attempts +
                        "|AreaHash=" + currentAreaHash +
                        "|After=" + snapshot.Describe());
                    _verificationPending = false;
                    _issuedAreaHash = 0;
                    _issuedUtc = DateTime.MinValue;
                    _nextAttemptUtc = now.AddMilliseconds(500);
                    _attempts = 0;
                    return true;
                }

                if (now - _issuedUtc < VerificationTimeout)
                {
                    AllflameLog.Throttled(
                        "dpb_ui_cleanup_pending_" + taskName,
                        TimeSpan.FromSeconds(1),
                        "DPB_UI_CLEANUP" +
                        "|Task=" + taskName +
                        "|State=Waiting" +
                        "|Postcondition=False" +
                        "|Attempt=" + _attempts +
                        "|AreaHash=" + currentAreaHash +
                        "|After=" + snapshot.Describe());
                    return !requirePostcondition;
                }

                AllflameLog.Info(
                    "DPB_UI_CLEANUP" +
                    "|Task=" + taskName +
                    "|State=Timeout" +
                    "|Postcondition=False" +
                    "|Attempt=" + _attempts +
                    "|AreaHash=" + currentAreaHash +
                    "|After=" + snapshot.Describe() +
                    "|Action=RetryAfterCooldown");
                _verificationPending = false;
            }

            if (snapshot.IsReadComplete &&
                !snapshot.HasBlockingSignal)
            {
                AllflameLog.Throttled(
                    "dpb_ui_cleanup_not_needed_" + taskName,
                    TimeSpan.FromSeconds(2),
                    "DPB_UI_CLEANUP" +
                    "|Task=" + taskName +
                    "|State=Skipped" +
                    "|Postcondition=True" +
                    "|Reason=NoBlockingWindows" +
                    "|AreaHash=" + currentAreaHash +
                    "|Snapshot=" + snapshot.Describe());
                return true;
            }

            if (now < _nextAttemptUtc)
            {
                AllflameLog.Throttled(
                    "dpb_ui_cleanup_cooldown_" + taskName,
                    TimeSpan.FromSeconds(1),
                    "DPB_UI_CLEANUP" +
                    "|Task=" + taskName +
                    "|State=Cooldown" +
                    "|Postcondition=False" +
                    "|Attempt=" + _attempts +
                    "|AreaHash=" + currentAreaHash +
                    "|Snapshot=" + snapshot.Describe());
                return !requirePostcondition;
            }

            DpbActionResult result =
                await DpbNativeApi.CloseBlockingWindowsAsync();
            _attempts++;
            _issuedAreaHash = currentAreaHash;
            _issuedUtc = DateTime.UtcNow;
            _nextAttemptUtc = _issuedUtc + RetryCooldown;
            _verificationPending = true;
            AllflameContext.RequestManualSnapshot();
            AllflameLog.Info(
                "DPB_UI_CLEANUP" +
                "|Task=" + taskName +
                "|State=Issued" +
                "|Mode=" + result.Mode +
                "|ApiAvailable=" + result.ApiAvailable +
                "|InputSent=" + result.InputSent +
                "|Postcondition=False" +
                "|Attempt=" + _attempts +
                "|AreaHash=" + currentAreaHash +
                "|Reason=" + result.Reason +
                "|Exception=" + result.Exception +
                "|Before=" + snapshot.Describe());
            return !requirePostcondition;
        }
    }
}
