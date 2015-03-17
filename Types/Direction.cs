using System;
using Kate.Maps;

namespace Kate.Types

{
    public enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    public static class Directions
    {
        public static Direction Get(Tile origin, Tile target)
        {
            var deltaX = target.X - origin.X;
            var deltaY = target.Y - origin.Y;
     
            return Get
            (
                ((deltaX == 0) ? 0 : deltaX / Math.Abs(deltaX)), 
                ((deltaY == 0) ? 0 : deltaY / Math.Abs(deltaY))
            );
        }

        public static Direction Get(int x, int y)
        {
            if (x == 1)
            {
                if (y == -1)
                    return Direction.NE;
                else if (y == 0)
                    return Direction.E;
                else
                    return Direction.SE;
            }

            else if (x == 0)
            {
                if (y == -1)
                    return Direction.N;
                else
                    return Direction.S;
            }
            else
            {
                if (y == -1)
                    return Direction.NW;
                else if (y == 0)
                    return Direction.W;
                else
                    return Direction.SW;
            }
        }

        public static int[] GetDirectionCoordinates(this Direction direction)
        {
            switch (direction)
            {
                case Direction.E:  return new int[2]{ 1,  0};
                case Direction.N:  return new int[2]{ 0, -1};
                case Direction.NE: return new int[2]{ 1, -1};
                case Direction.NW: return new int[2]{-1, -1};
                case Direction.S:  return new int[2]{ 0,  1};
                case Direction.SE: return new int[2]{ 1,  1};
                case Direction.SW: return new int[2]{-1,  1};
                default:           return new int[2]{-1,  0};
            }
        }

        public static int[] GetTileCoordinates(this Direction direction, Tile tile)
        {
            var coordinates = direction.GetDirectionCoordinates();

            return new int[2]{tile.X + coordinates[0], tile.Y + coordinates[1]};
        }

        public static Direction[] Neighbours(this Direction direction, int order)
        {
            switch (order)
            {
                case 0:
                    return new Direction[]{direction};
                case 1:
                    switch (direction)
                    {
                        case Direction.E:  return new Direction[]{Direction.NE, Direction.SE};
                        case Direction.N:  return new Direction[]{Direction.NW, Direction.NE};
                        case Direction.NE: return new Direction[]{Direction.N,  Direction.E};
                        case Direction.NW: return new Direction[]{Direction.N,  Direction.W};
                        case Direction.S:  return new Direction[]{Direction.SE, Direction.SW};
                        case Direction.SE: return new Direction[]{Direction.S,  Direction.E};
                        case Direction.SW: return new Direction[]{Direction.S,  Direction.W};
                        default:           return new Direction[]{Direction.NW, Direction.SW};
                    }
                case 2:
                    switch (direction)
                    {
                        case Direction.E:  return new Direction[]{Direction.N, Direction.S};
                        case Direction.N:  return new Direction[]{Direction.E, Direction.W};
                        case Direction.NE: return new Direction[]{Direction.NW,  Direction.SE};
                        case Direction.NW: return new Direction[]{Direction.NE,  Direction.SW};
                        case Direction.S:  return new Direction[]{Direction.E, Direction.W};
                        case Direction.SE: return new Direction[]{Direction.NE,  Direction.SW};
                        case Direction.SW: return new Direction[]{Direction.SE,  Direction.NW};
                        default:           return new Direction[]{Direction.N, Direction.S};
                    }
                case 3:
                    switch (direction)
                    {
                        case Direction.E:  return new Direction[]{Direction.NW, Direction.SW};
                        case Direction.N:  return new Direction[]{Direction.SE, Direction.SW};
                        case Direction.NE: return new Direction[]{Direction.W,  Direction.S};
                        case Direction.NW: return new Direction[]{Direction.E,  Direction.S};
                        case Direction.S:  return new Direction[]{Direction.NE, Direction.NW};
                        case Direction.SE: return new Direction[]{Direction.N,  Direction.W};
                        case Direction.SW: return new Direction[]{Direction.N,  Direction.E};
                        default:           return new Direction[]{Direction.NE, Direction.SE};
                    }
                default:
                    return new Direction[]
                    {
                        Direction.N, 
                        Direction.S, 
                        Direction.E, 
                        Direction.W, 
                        Direction.NE, 
                        Direction.NW, 
                        Direction.SE, 
                        Direction.SW
                    };
            }
        }

        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.E:
                    return Direction.W;
                case Direction.N:
                    return Direction.S;
                case Direction.NE:
                    return Direction.SW;
                case Direction.NW:
                    return Direction.SE;
                case Direction.S:
                    return Direction.N;
                case Direction.SE:
                    return Direction.NW;
                case Direction.SW:
                    return Direction.NE;
                default:
                    return Direction.E;
            }
        }
    }
}
