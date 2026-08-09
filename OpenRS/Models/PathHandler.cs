using System;

using NuciLog.Core;

using NuciXNA.Primitives;

using OpenRS.Logging;

namespace OpenRS.Models
{
    public sealed class PathHandler
    {
        [Flags]
        private enum TileWallFlag : byte
        {
            None = 0,
            North = 1,
            East = 2,
            South = 4,
            West = 8,
            DiagonalBackslash = 16,
            DiagonalForwardSlash = 32,
            Unwalkable = 64
        }

        private static int NoWaypointIndex => -1;

        public WalkPath Path { get; private set; }

        private int currentWaypoint;
        private readonly MobInstance mobInstance;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<PathHandler>();

        public PathHandler(MobInstance mobInstance)
        {
            this.mobInstance = mobInstance;
            ResetPath();
        }

        public void SetPath(WalkPath path)
        {
            currentWaypoint = NoWaypointIndex;
            Path = path;
        }

        public void UpdateLocation()
        {
            if (!FinishedPath())
            {
                SetNextPosition();
            }
        }

        public bool FinishedPath()
        {
            if (Path is null)
            {
                return true;
            }

            if (Path.Length > 0)
            {
                return AtWaypoint(Path.Length - 1);
            }

            return AtStart();
        }

        public void ResetPath()
        {
            Path = null;
            currentWaypoint = NoWaypointIndex;
        }

        private bool AtStart() => Path.StartLocation == mobInstance.Location;

        private bool AtWaypoint(int waypoint) => Path.GetWaypoint(waypoint) == mobInstance.Location;

        private Point2D? GetNextPosition(Point2D start, Point2D destination)
        {
            try
            {
                Point2D location = start;

                bool startXBlocked = false;
                bool startYBlocked = false;

                if (start.X > destination.X)
                {
                    // Check the right tile's left wall.
                    startXBlocked = IsBlocking(new Point2D(start.X - 1, start.Y), TileWallFlag.West);
                    location.X = start.X - 1;
                }
                else if (start.X < destination.X)
                {
                    // Check the left tile's right wall.
                    startXBlocked = IsBlocking(new Point2D(start.X + 1, start.Y), TileWallFlag.East);
                    location.X = start.X + 1;
                }

                if (start.Y > destination.Y)
                {
                    // Check the top tile's bottom wall.
                    startYBlocked = IsBlocking(new Point2D(start.X, start.Y - 1), TileWallFlag.South);
                    location.Y = start.Y - 1;
                }
                else if (start.Y < destination.Y)
                {
                    // Check the bottom tile's top wall.
                    startYBlocked = IsBlocking(new Point2D(start.X, start.Y + 1), TileWallFlag.North);
                    location.Y = start.Y + 1;
                }

                // If both directions are blocked, or moving straight and the direction is blocked.
                if ((startXBlocked && startYBlocked) ||
                    (startXBlocked && start.Y == destination.Y) ||
                    (startYBlocked && start.X == destination.X))
                {
                    return null;
                }

                bool destinationXBlocked = false;
                bool destinationYBlocked = false;

                if (location.X > start.X)
                {
                    // Check the destination tile's right wall.
                    destinationXBlocked = IsBlocking(location, TileWallFlag.East);
                }
                else if (location.X < start.X)
                {
                    // Check the destination tile's left wall.
                    destinationXBlocked = IsBlocking(location, TileWallFlag.West);
                }

                if (location.Y > start.Y)
                {
                    // Check the destination tile's top wall.
                    destinationYBlocked = IsBlocking(location, TileWallFlag.North);
                }
                else if (location.Y < start.Y)
                {
                    // Check the destination tile's bottom wall.
                    destinationYBlocked = IsBlocking(location, TileWallFlag.South);
                }

                // If both directions are blocked, or moving straight and the direction is blocked.
                if ((destinationXBlocked && destinationYBlocked) ||
                    (destinationXBlocked && start.Y == location.Y) ||
                    (startYBlocked && destinationYBlocked))
                {
                    return null;
                }

                // If only one direction is blocked, but it blocks both tiles.
                if ((startXBlocked && destinationXBlocked) ||
                    (startYBlocked && destinationYBlocked))
                {
                    return null;
                }

                return location;
            }
            catch (Exception ex)
            {
                logger.Error(
                    GameOperation.CalculatePath,
                    "Failed to calculate the next path location.",
                    ex);
            }

            return start;
        }

        private void SetNextPosition()
        {
            if (currentWaypoint == NoWaypointIndex)
            {
                if (!AtStart())
                {
                    Point2D? newLocation = GetNextPosition(mobInstance.Location, Path.StartLocation);

                    if (newLocation is null)
                    {
                        ResetPath();
                        return;
                    }

                    mobInstance.SetLocation(newLocation.Value);
                    return;
                }

                currentWaypoint = 0;
            }

            if (AtWaypoint(currentWaypoint))
            {
                currentWaypoint += 1;
            }

            if (currentWaypoint < Path.Length)
            {
                Point2D? newLocation = GetNextPosition(
                    mobInstance.Location,
                    Path.GetWaypoint(currentWaypoint));

                if (newLocation is null)
                {
                    ResetPath();
                    return;
                }

                mobInstance.SetLocation(newLocation.Value);
            }
            else
            {
                ResetPath();
            }
        }

        private bool IsBlocking(Point2D location, TileWallFlag wallFlag)
        {
            throw new NotImplementedException();
        }

        private static bool IsBlocking(byte wallData, TileWallFlag wallFlag)
        {
            if ((wallData & (byte)wallFlag) != 0)
            {
                // There is a wall in the way.
                return true;
            }

            if ((wallData & (byte)TileWallFlag.DiagonalBackslash) != 0)
            {
                // There is a diagonal wall here: \.
                return true;
            }

            if ((wallData & (byte)TileWallFlag.DiagonalForwardSlash) != 0)
            {
                // There is a diagonal wall here: /.
                return true;
            }

            if ((wallData & (byte)TileWallFlag.Unwalkable) != 0)
            {
                // This tile is unwalkable.
                return true;
            }

            return false;
        }
    }
}
