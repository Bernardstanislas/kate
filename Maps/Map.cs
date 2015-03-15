﻿using System;
using System.Linq;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    public class Map : AbstractMap
    {
        private Tile[,] grid;
        public Tile[,] Grid { get { return grid; } }

        private int[, , ,] hashArray;
        public int[, , ,] HashArray { get { return hashArray; } }

        public Map(int xSize, int ySize)
        {
            generateHashArray(xSize, ySize);

            grid = new Tile[xSize, ySize];
            for (int i = 0; i < grid.GetLength (0); i++)
                for (int j = 0; j < grid.GetLength (1); j++) 
                {
                    grid [i, j] = new Tile (i, j);
                    hash ^= hashArray [i, j, (int)Owner.Neutral, 0];
                }
        }

        public Map(Map map)
        {
            var size = map.GetMapDimension();
            grid = new Tile[size[0], size[1]];
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++) 
                    grid[i, j] = new Tile(map.Grid[i, j]);

            hash = map.GetHashCode();
            hashArray = map.HashArray;
        }

        public override bool HasGameEnded()
        {
            int meCount = 0;
            int opCount = 0;
            foreach (var tile in grid)
            {
                if (tile.Owner == Owner.Me)
                    meCount++;
                else if (tile.Owner == Owner.Opponent)
                    opCount++; 
            }
            if (meCount == 0 || opCount == 0)
                return true;
            else
                return false;
        }

        #region Grid
        public override IEnumerable<Tile> GetGrid()
        {
            foreach (Tile tile in grid)
                yield return tile;
        }

        public override IEnumerable<Tile> GetPlayerTiles(Owner owner)
        {
            foreach (Tile tile in grid)
                if (tile.Owner.Equals(owner))
                    yield return tile;
        }

        public override int[] GetMapDimension()
        {
            return new int[2] {grid.GetLength(0), grid.GetLength(1)};
        }

        protected override void UpdateTile(Tile newTile) 
        {
            grid[newTile.X, newTile.Y] = newTile;
        }

        public override Tile GetTile(int xCoordinate, int yCoordinate) 
        {
            return grid[xCoordinate, yCoordinate];
        }
        #endregion

        #region Hash
        private void generateHashArray(int xSize, int ySize)
        {
            Random random = new Random();
            hashArray = new int[xSize, ySize, Enum.GetNames(typeof(Owner)).Length, 256];

            for (int index0 = 0; index0 < hashArray.GetLength(0); index0++)
                for (int index1 = 0; index1 < hashArray.GetLength(1); index1++)
                    for (int index2 = 0; index2 < hashArray.GetLength(2); index2++)
                        for (int index3 = 0; index3 < hashArray.GetLength(3); index3++)
                            hashArray[index0, index1, index2, index3] = random.Next();
        }

        protected override void UpdateHash(Tile newTile)
        {
            var oldTile = GetTile(newTile.X, newTile.Y);
            hash = hash 
                ^ hashArray[oldTile.X, oldTile.Y, (int)oldTile.Owner, oldTile.Population] 
                ^ hashArray[newTile.X, newTile.Y, (int)newTile.Owner, newTile.Population];
        }
        #endregion
    }
}
