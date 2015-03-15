<<<<<<< HEAD
﻿using Kate.Maps;

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


        public static int[] GetTileCoordinates(this Direction direction, Tile tile)
        {
            switch (direction)
            {
                case Direction.E:
                    return new int[2] { tile.X + 1, tile.Y };
                case Direction.N:
                    return new int[2] { tile.X, tile.Y - 1};
                case Direction.NE:
                    return new int[2] { tile.X + 1, tile.Y - 1 };
                case Direction.NW:
                    return new int[2] { tile.X - 1, tile.Y - 1 };
                case Direction.S:
                    return new int[2] { tile.X, tile.Y + 1 };
                case Direction.SE:
                    return new int[2] { tile.X + 1, tile.Y + 1 };
                case Direction.SW:
                    return new int[2] { tile.X - 1, tile.Y + 1 };
                default:
                    return new int[2] { tile.X - 1, tile.Y };
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
