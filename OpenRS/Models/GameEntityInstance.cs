using System;

using NuciXNA.Primitives;

namespace OpenRS.Models
{
    public class GameEntityInstance
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public Point2D Location { get; private set; }

        public virtual void SetLocation(Point2D location)
        {
            Location = location;
        }

        public bool IsWithinRange(GameEntityInstance other, int radius) => IsWithinRange(other.Location, radius);

        public bool IsWithinRange(Point2D location, int radius)
        {
            Point2D difference = new(
                Math.Abs(Location.X - location.X),
                Math.Abs(Location.Y - location.Y));

            return difference.X <= radius && difference.Y <= radius;
        }

        public Point2D? NextStep(Point2D myLocation, GameEntityInstance other)
        {
            if (myLocation == other.Location)
            {
                return myLocation;
            }

            Point2D newLocation = myLocation;
            bool myXblocked = false;
            bool myYblocked = false;

            if (myLocation.X > other.Location.X)
            {
                // Check right tile's left wall.
                myXblocked = IsBlocking(new Point2D(myLocation.X - 1, myLocation.Y), 8);
                newLocation.X = myLocation.X - 1;
            }
            else if (myLocation.X < other.Location.X)
            {
                // Check left tile's right wall.
                myXblocked = IsBlocking(new Point2D(myLocation.X + 1, myLocation.Y), 2);
                newLocation.X = myLocation.X + 1;
            }

            if (myLocation.Y > other.Location.Y)
            {
                // Check top tile's bottom wall.
                myYblocked = IsBlocking(new Point2D(myLocation.X, myLocation.Y - 1), 4);
                newLocation.Y = myLocation.Y - 1;
            }
            else if (myLocation.Y < other.Location.Y)
            {
                // Check bottom tile's top wall.
                myYblocked = IsBlocking(new Point2D(myLocation.X, myLocation.Y + 1), 1);
                newLocation.Y = myLocation.Y + 1;
            }

            // If both directions are blocked OR we are going straight and the direction is blocked.
            if ((myXblocked && myYblocked) ||
                (myXblocked && myLocation.Y == newLocation.Y) ||
                (myYblocked && myLocation.X == newLocation.X))
            {
                return null;
            }

            bool newXblocked = false;
            bool newYblocked = false;

            if (newLocation.X > myLocation.X)
            {
                // Check destination tile's right wall.
                newXblocked = IsBlocking(newLocation, 2);
            }
            else if (newLocation.X < myLocation.X)
            {
                // Check destination tile's left wall.
                newXblocked = IsBlocking(newLocation, 8);
            }

            if (newLocation.Y > myLocation.Y)
            {
                // Check destination tile's top wall.
                newYblocked = IsBlocking(newLocation, 1);
            }
            else if (newLocation.Y < myLocation.Y)
            {
                // Check destination tile's bottom wall.
                newYblocked = IsBlocking(newLocation, 4);
            }

            // If both directions are blocked OR we are going straight and the direction is blocked.
            if ((newXblocked && newYblocked) ||
                (newXblocked && myLocation.Y == newLocation.Y) ||
                (newYblocked && myLocation.X == newLocation.X))
            {
                return null;
            }

            // If only one direction is blocked, but it blocks both tiles.
            if ((myXblocked && newXblocked) ||
                (myYblocked && newYblocked))
            {
                return null;
            }

            return newLocation;
        }

        private bool IsBlocking(Point2D location, int bit) => IsMapBlocking(location, (byte)bit);

        private bool IsMapBlocking(Point2D location, byte bit)
        {
            throw new NotImplementedException();
        }
    }
}
