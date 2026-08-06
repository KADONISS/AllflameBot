using System;
using System.Reflection;
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
                "|NativeVoyageStash=StableStashUi");
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

        private static string Sanitize(string value)
        {
            return (value ?? String.Empty)
                .Replace("|", "/")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }
    }
}
