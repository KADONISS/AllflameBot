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

        public string Name => "Allflame Mover";
        public string Description => "Pathfinding mover for Allflame Chart Areas.";
        public string Author => "Codex";
        public string Version => "0.23.118";
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
        }

        public void Stop()
        {
            _currentCommand = null;
            ClearStableApproach();
            _lastLoggedSkill = String.Empty;
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
            return MoveTowardsCore(position, false);
        }

        public bool MoveTowardsPrecise(Vector2i position, params dynamic[] user)
        {
            return MoveTowardsCore(position, true);
        }

        private bool MoveTowardsCore(
            Vector2i position,
            bool preciseInteraction)
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

            if (preciseInteraction && remainingDistance < 130f)
            {
                // Near an interaction target, use a single click so a held
                // Move input cannot carry the player through the target ring.
                LokiPoe.InGameState.SkillBarHud.UseAt(
                    moveSlot,
                    false,
                    waypoint,
                    true);
            }
            else
            {
                LokiPoe.InGameState.SkillBarHud.BeginUseAt(
                    moveSlot,
                    false,
                    waypoint,
                    true);
            }

            int moveIntervalMs =
                remainingDistance < 65f
                    ? 425
                    : remainingDistance < 130f
                        ? 330
                        : 275;
            _nextMoveUtc =
                DateTime.UtcNow.AddMilliseconds(moveIntervalMs);
            return true;
        }

        public void ResetApproachState()
        {
            _currentCommand = null;
            _nextMoveUtc = DateTime.MinValue;
            ClearStableApproach();
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
                    _stableBestDistance = stableDistance;
                    _stableLastProgressUtc = DateTime.UtcNow;
                }

                bool stableStale =
                    stableDistance <= 12f ||
                    DateTime.UtcNow - _stableLastProgressUtc >
                    TimeSpan.FromSeconds(3);
                if (!stableStale &&
                    TryPath(
                        playerPosition,
                        _stableResolvedPosition))
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

            if (TryPath(playerPosition, requestedPosition))
            {
                ClearStableApproach();
                return true;
            }

            double baseAngle = Math.Atan2(
                playerPosition.Y - requestedPosition.Y,
                playerPosition.X - requestedPosition.X);
            int[] radii = { 14, 20, 28, 36, 46, 58, 70 };
            const int angleSamples = 16;

            foreach (int radius in radii)
            {
                for (int index = 0;
                     index < angleSamples;
                     index++)
                {
                    double angle =
                        baseAngle +
                        index * Math.PI * 2.0 / angleSamples;
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
                    if (!ExilePather.FindPath(
                            ref candidateCommand) ||
                        candidateCommand.Path == null ||
                        candidateCommand.Path.Count == 0)
                    {
                        continue;
                    }

                    _currentCommand = candidateCommand;
                    resolvedPosition = candidate;
                    _hasStableApproach = true;
                    _stableRequestedPosition =
                        requestedPosition;
                    _stableResolvedPosition = candidate;
                    _stableApproachExpiresUtc =
                        DateTime.UtcNow.AddSeconds(6);
                    _stableBestDistance =
                        playerPosition.DistanceF(candidate);
                    _stableLastProgressUtc = DateTime.UtcNow;
                    return true;
                }
            }

            return false;
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

        private static Vector2i SelectWaypoint(
            IndexedList<Vector2i> path,
            float remainingDistance)
        {
            float targetDistance =
                remainingDistance < 65f
                    ? 18f
                    : remainingDistance < 130f
                        ? 30f
                        : 42f;
            Vector2i waypoint = path[0];
            float distance = 0f;

            for (int i = 1; i < path.Count; i++)
            {
                float segment = path[i - 1].DistanceF(path[i]);
                if (distance + segment > targetDistance)
                    break;

                distance += segment;
                waypoint = path[i];
            }

            return waypoint;
        }
    }
}
