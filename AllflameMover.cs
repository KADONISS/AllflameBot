using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;

namespace AllflameBot
{
    /// <summary>
    /// Self-contained mover so Allflame Bot does not depend on TheThief's mover.
    /// </summary>
    public sealed class AllflameMover : IPlayerMover
    {
        private PathfindingCommand _currentCommand;
        private DateTime _nextMoveUtc = DateTime.MinValue;
        private DateTime _nextPathWarningUtc = DateTime.MinValue;
        private bool _hasStableApproach;
        private Vector2i _stableRequestedPosition;
        private Vector2i _stableResolvedPosition;
        private DateTime _stableApproachExpiresUtc = DateTime.MinValue;
        private float _stableBestDistance = Single.MaxValue;
        private DateTime _stableLastProgressUtc = DateTime.MinValue;
        private string _lastLoggedSkill = String.Empty;
        private bool _heldMoveActive;
        private string _lastApproachProfile = String.Empty;
        private bool _fallbackSearchActive;
        private Vector2i _fallbackRequestedPosition;
        private int _fallbackRadiusIndex;
        private int _fallbackAngleIndex;

        private static readonly int[] FallbackRadii =
            { 14, 20, 28, 36, 46, 58, 70 };
        private const int FallbackAngleSamples = 16;
        private const int FallbackChecksPerCycle = 6;

        private enum ApproachProfile
        {
            Traversal,
            Loot,
            Chest,
            Object,
            Capsule
        }

        public string Name => "Allflame Mover";
        public string Description => "Pathfinding mover for Allflame Chart Areas.";
        public string Author => "Codex";
        public string Version => "0.23.153";
        public UserControl Control => null;
        public JsonSettings Settings => AllflameSettings.Instance;
        public PathfindingCommand CurrentCommand => _currentCommand;

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
        }

        public void Start()
        {
            _currentCommand = null;
            _nextMoveUtc = DateTime.MinValue;
            ClearStableApproach();
            _lastLoggedSkill = String.Empty;
            _heldMoveActive = false;
            _lastApproachProfile = String.Empty;
            ResetFallbackSearch();
        }

        public void Stop()
        {
            _currentCommand = null;
            ClearStableApproach();
            _lastLoggedSkill = String.Empty;
            _heldMoveActive = false;
            _lastApproachProfile = String.Empty;
            ResetFallbackSearch();
        }

        public void Tick()
        {
        }

        public Task<LogicResult> Logic(Logic logic)
        {
            return Task.FromResult(LogicResult.Unprovided);
        }

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool MoveTowards(Vector2i position, params dynamic[] user)
        {
            return MoveTowardsCore(
                position,
                ApproachProfile.Traversal);
        }

        public bool MoveTowardsPrecise(Vector2i position, params dynamic[] user)
        {
            return MoveTowardsCore(
                position,
                ApproachProfile.Object);
        }

        public bool MoveTowardsInteraction(
            Vector2i position,
            string context)
        {
            return MoveTowardsCore(
                position,
                ResolveApproachProfile(context));
        }

        private bool MoveTowardsCore(
            Vector2i position,
            ApproachProfile profile)
        {
            if (!LokiPoe.IsInGame || LokiPoe.Me == null)
                return false;

            Vector2i playerPosition = LokiPoe.Me.Position;
            float requestedDistance = playerPosition.DistanceF(position);
            if (requestedDistance <= 8f)
            {
                ClearStableApproach();
                return true;
            }

            Vector2i resolvedPosition;
            if (!TryBuildPath(
                playerPosition,
                position,
                out resolvedPosition))
            {
                if (DateTime.UtcNow >= _nextPathWarningUtc)
                {
                    AllflameLog.Info($"MOVER_PATH_FAILED|Destination={position}");
                    _nextPathWarningUtc = DateTime.UtcNow.AddSeconds(2);
                }

                return false;
            }

            if (resolvedPosition != position)
            {
                AllflameLog.Throttled(
                    "mover_approach_resolved",
                    TimeSpan.FromSeconds(2),
                    "MOVER_APPROACH_RESOLVED|Requested=" +
                    position +
                    "|Resolved=" + resolvedPosition +
                    "|RequestedBlocked=True");
            }

            float remainingDistance =
                playerPosition.DistanceF(resolvedPosition);
            Vector2i waypoint =
                SelectWaypoint(
                    _currentCommand.Path,
                    playerPosition,
                    remainingDistance);
            if (DateTime.UtcNow < _nextMoveUtc)
                return true;

            var moveSkill = LokiPoe.InGameState.SkillBarHud.LastBoundMoveSkill;
            if (moveSkill == null || moveSkill.Slots == null || !moveSkill.Slots.Any())
            {
                AllflameLog.Info("MOVER_SKILL_MISSING|No bound move skill was found.");
                _nextMoveUtc = DateTime.UtcNow.AddSeconds(1);
                return false;
            }

            int moveSlot = moveSkill.Slots.Last();
            LogSelectedMovementSkill(false);
            LogApproachProfile(profile, remainingDistance);
            if (!LokiPoe.InGameState.SkillBarHud.CanUse(moveSlot))
            {
                AllflameLog.Throttled(
                    "mover_skill_not_ready",
                    TimeSpan.FromSeconds(2),
                    "MOVER_SKILL_NOT_READY|Slot=" + moveSlot +
                    "|Skill=" + moveSkill.Name);
                _nextMoveUtc = DateTime.UtcNow.AddMilliseconds(125);
                return true;
            }

            float precisionDistance = PrecisionDistance(profile);
            bool singleClick =
                profile != ApproachProfile.Traversal &&
                remainingDistance < precisionDistance;
            if (singleClick)
            {
                // Near an interaction target, use a single click so a held
                // Move input cannot carry the player through the target ring.
                LokiPoe.InGameState.SkillBarHud.UseAt(
                    moveSlot,
                    false,
                    waypoint,
                    true);
                _heldMoveActive = false;
            }
            else
            {
                LokiPoe.InGameState.SkillBarHud.BeginUseAt(
                    moveSlot,
                    false,
                    waypoint,
                    true);
                _heldMoveActive = true;
            }

            int moveIntervalMs = MoveIntervalMilliseconds(
                profile,
                remainingDistance,
                singleClick);
            _nextMoveUtc =
                DateTime.UtcNow.AddMilliseconds(moveIntervalMs);
            AllflameLog.Throttled(
                "mover_approach_phase_" + profile,
                TimeSpan.FromSeconds(1),
                "MOVER_APPROACH_PHASE" +
                "|Profile=" + profile +
                "|Phase=" +
                (singleClick ? "PrecisionClick" : "FastTraversal") +
                "|Remaining=" + remainingDistance.ToString("0.0") +
                "|PrecisionDistance=" +
                precisionDistance.ToString("0") +
                "|IntervalMs=" + moveIntervalMs);
            return true;
        }

        public bool ReleaseMovementInput()
        {
            bool wasHeld = _heldMoveActive;
            LokiPoe.ProcessHookManager.ClearAllKeyStates();
            _heldMoveActive = false;
            return wasHeld;
        }

        public void ResetApproachState()
        {
            _currentCommand = null;
            _nextMoveUtc = DateTime.MinValue;
            ClearStableApproach();
            _heldMoveActive = false;
            ResetFallbackSearch();
        }

        public void LogSelectedMovementSkill(bool force)
        {
            var moveSkill = LokiPoe.InGameState.SkillBarHud.LastBoundMoveSkill;
            if (moveSkill == null ||
                moveSkill.Slots == null ||
                !moveSkill.Slots.Any())
            {
                return;
            }

            int moveSlot = moveSkill.Slots.Last();
            string name = moveSkill.Name ?? String.Empty;
            string internalName = moveSkill.InternalName ?? String.Empty;
            string signature = name + "|" + internalName + "|" + moveSlot;
            if (!force &&
                String.Equals(
                    _lastLoggedSkill,
                    signature,
                    StringComparison.Ordinal))
            {
                return;
            }

            _lastLoggedSkill = signature;
            AllflameLog.Info(
                "MOVER_SKILL_SELECTED" +
                "|Name=" + name +
                "|InternalName=" + internalName +
                "|Slot=" + moveSlot);
        }

        public static string DescribeSelectedMovementSkill()
        {
            try
            {
                var moveSkill =
                    LokiPoe.InGameState.SkillBarHud.LastBoundMoveSkill;
                if (moveSkill == null ||
                    moveSkill.Slots == null ||
                    !moveSkill.Slots.Any())
                {
                    return "None";
                }

                return
                    (moveSkill.Name ?? String.Empty) + "/" +
                    (moveSkill.InternalName ?? String.Empty) + "/" +
                    moveSkill.Slots.Last();
            }
            catch
            {
                return "Unavailable";
            }
        }

        private bool TryBuildPath(
            Vector2i playerPosition,
            Vector2i requestedPosition,
            out Vector2i resolvedPosition)
        {
            resolvedPosition = requestedPosition;
            bool sameStableTarget =
                _hasStableApproach &&
                _stableRequestedPosition.DistanceF(
                    requestedPosition) <= 10f;
            if (sameStableTarget &&
                DateTime.UtcNow < _stableApproachExpiresUtc)
            {
                float stableDistance =
                    playerPosition.DistanceF(
                        _stableResolvedPosition);
                if (stableDistance + 3f < _stableBestDistance)
                {
                    float improvement =
                        _stableBestDistance - stableDistance;
                    _stableBestDistance = stableDistance;
                    _stableLastProgressUtc = DateTime.UtcNow;
                    AllflameLog.Throttled(
                        "mover_approach_convergence",
                        TimeSpan.FromSeconds(1),
                        "MOVER_APPROACH_CONVERGENCE" +
                        "|Target=" + requestedPosition +
                        "|Distance=" + stableDistance.ToString("0.0") +
                        "|Improvement=" + improvement.ToString("0.0"));
                }

                bool stableStale =
                    stableDistance <= 12f ||
                    DateTime.UtcNow - _stableLastProgressUtc >
                    TimeSpan.FromSeconds(3);
                if (!stableStale &&
                    _currentCommand != null &&
                    _currentCommand.Path != null &&
                    _currentCommand.Path.Count > 0)
                {
                    resolvedPosition =
                        _stableResolvedPosition;
                    return true;
                }

                ClearStableApproach();
            }
            else if (_hasStableApproach)
            {
                ClearStableApproach();
            }

            bool continueFallback =
                _fallbackSearchActive &&
                _fallbackRequestedPosition.DistanceF(
                    requestedPosition) <= 10f;
            if (!continueFallback &&
                TryPath(playerPosition, requestedPosition))
            {
                ResetFallbackSearch();
                RememberStableApproach(
                    playerPosition,
                    requestedPosition,
                    requestedPosition,
                    TimeSpan.FromSeconds(2));
                return true;
            }

            if (!continueFallback)
            {
                _fallbackSearchActive = true;
                _fallbackRequestedPosition = requestedPosition;
                _fallbackRadiusIndex = 0;
                _fallbackAngleIndex = 0;
            }

            double baseAngle = Math.Atan2(
                playerPosition.Y - requestedPosition.Y,
                playerPosition.X - requestedPosition.X);
            int checks = 0;
            while (_fallbackRadiusIndex < FallbackRadii.Length &&
                   checks < FallbackChecksPerCycle)
            {
                int radius = FallbackRadii[_fallbackRadiusIndex];
                int angleIndex = _fallbackAngleIndex++;
                if (_fallbackAngleIndex >= FallbackAngleSamples)
                {
                    _fallbackAngleIndex = 0;
                    _fallbackRadiusIndex++;
                }
                checks++;
                double angle =
                    baseAngle +
                    angleIndex * Math.PI * 2.0 /
                    FallbackAngleSamples;
                Vector2i candidate = new Vector2i(
                    requestedPosition.X +
                    (int)Math.Round(Math.Cos(angle) * radius),
                    requestedPosition.Y +
                    (int)Math.Round(Math.Sin(angle) * radius));
                PathfindingCommand candidateCommand =
                    new PathfindingCommand(
                        playerPosition,
                        candidate,
                        3f);
                if (!ExilePather.FindPath(ref candidateCommand) ||
                    candidateCommand.Path == null ||
                    candidateCommand.Path.Count == 0)
                {
                    continue;
                }

                _currentCommand = candidateCommand;
                resolvedPosition = candidate;
                ResetFallbackSearch();
                RememberStableApproach(
                    playerPosition,
                    requestedPosition,
                    candidate,
                    TimeSpan.FromSeconds(6));
                return true;
            }

            if (_fallbackRadiusIndex >= FallbackRadii.Length)
            {
                ResetFallbackSearch();
            }
            AllflameLog.Throttled(
                "mover_fallback_search",
                TimeSpan.FromSeconds(1),
                "MOVER_FALLBACK_SEARCH" +
                "|Target=" + requestedPosition +
                "|ChecksThisCycle=" + checks +
                "|RadiusIndex=" + _fallbackRadiusIndex +
                "|AngleIndex=" + _fallbackAngleIndex +
                "|OneActionPerCycle=True");
            return false;
        }

        private void ResetFallbackSearch()
        {
            _fallbackSearchActive = false;
            _fallbackRadiusIndex = 0;
            _fallbackAngleIndex = 0;
        }

        private bool TryPath(
            Vector2i playerPosition,
            Vector2i destination)
        {
            PathfindingCommand command =
                new PathfindingCommand(
                    playerPosition,
                    destination,
                    3f);
            if (!ExilePather.FindPath(ref command) ||
                command.Path == null ||
                command.Path.Count == 0)
            {
                return false;
            }

            _currentCommand = command;
            return true;
        }

        private void ClearStableApproach()
        {
            _hasStableApproach = false;
            _stableApproachExpiresUtc = DateTime.MinValue;
            _stableBestDistance = Single.MaxValue;
            _stableLastProgressUtc = DateTime.MinValue;
        }

        private void RememberStableApproach(
            Vector2i playerPosition,
            Vector2i requestedPosition,
            Vector2i resolvedPosition,
            TimeSpan lifetime)
        {
            _hasStableApproach = true;
            _stableRequestedPosition = requestedPosition;
            _stableResolvedPosition = resolvedPosition;
            _stableApproachExpiresUtc = DateTime.UtcNow.Add(lifetime);
            _stableBestDistance =
                playerPosition.DistanceF(resolvedPosition);
            _stableLastProgressUtc = DateTime.UtcNow;
        }

        private static Vector2i SelectWaypoint(
            IndexedList<Vector2i> path,
            Vector2i playerPosition,
            float remainingDistance)
        {
            float targetDistance =
                remainingDistance < 65f
                    ? 18f
                    : remainingDistance < 130f
                        ? 30f
                        : 42f;
            int closestIndex = 0;
            float closestDistance =
                playerPosition.DistanceF(path[0]);
            for (int i = 1; i < path.Count; i++)
            {
                float candidateDistance =
                    playerPosition.DistanceF(path[i]);
                if (candidateDistance < closestDistance)
                {
                    closestDistance = candidateDistance;
                    closestIndex = i;
                }
            }

            Vector2i waypoint = path[closestIndex];
            float distance = 0f;

            for (int i = closestIndex + 1; i < path.Count; i++)
            {
                float segment = path[i - 1].DistanceF(path[i]);
                if (distance + segment > targetDistance)
                    break;

                distance += segment;
                waypoint = path[i];
            }

            return waypoint;
        }

        private static ApproachProfile ResolveApproachProfile(
            string context)
        {
            context = context ?? String.Empty;
            if (context.IndexOf(
                    "capsule",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ApproachProfile.Capsule;
            }
            if (context.IndexOf(
                    "loot",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ApproachProfile.Loot;
            }
            if (context.IndexOf(
                    "chest",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ApproachProfile.Chest;
            }
            return ApproachProfile.Object;
        }

        private static float PrecisionDistance(ApproachProfile profile)
        {
            switch (profile)
            {
                case ApproachProfile.Loot:
                    return 78f;
                case ApproachProfile.Chest:
                    return 95f;
                case ApproachProfile.Object:
                    return 90f;
                case ApproachProfile.Capsule:
                    return 130f;
                default:
                    return 0f;
            }
        }

        private static int MoveIntervalMilliseconds(
            ApproachProfile profile,
            float remainingDistance,
            bool singleClick)
        {
            if (!singleClick)
            {
                return remainingDistance < 130f ? 220 : 250;
            }
            if (profile == ApproachProfile.Capsule)
            {
                return remainingDistance < 65f ? 425 : 330;
            }
            if (profile == ApproachProfile.Loot)
            {
                return remainingDistance < 42f ? 110 : 165;
            }
            return remainingDistance < 50f ? 130 : 185;
        }

        private void LogApproachProfile(
            ApproachProfile profile,
            float remainingDistance)
        {
            string signature = profile.ToString();
            if (String.Equals(
                    _lastApproachProfile,
                    signature,
                    StringComparison.Ordinal))
            {
                return;
            }
            _lastApproachProfile = signature;
            AllflameLog.Info(
                "MOVER_APPROACH_PROFILE" +
                "|Profile=" + profile +
                "|PrecisionDistance=" +
                PrecisionDistance(profile).ToString("0") +
                "|Remaining=" + remainingDistance.ToString("0.0"));
        }
    }
}
