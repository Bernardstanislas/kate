using System;
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

        private Dictionary<Tile, Dictionary<Owner, Tuple<Direction, int>[]>> distances = 
            new Dictionary<Tile, Dictionary<Owner, Tuple<Direction, int>[]>>();

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

        public override Tuple<Direction, int>[] GetDistances(Tile tile, Owner target)
        {
            if (distances.Count == 0) 
                GenerateDistances();

            return distances[tile][target];
        }

        private void GenerateDistances()
        {
            var data = new Dictionary<Owner, Tuple<Tile[], int>>();
            foreach (var owner in new Owner[]{Owner.Me, Owner.Opponent, Owner.Humans})
            {
                var tiles = GetPlayerTiles(owner).ToArray();
                data[owner] = Tuple.Create(tiles, tiles.Length);
            }

            foreach (var owner in new Owner[]{Owner.Me, Owner.Opponent})
            {
                for (var ownerIndex = 0; ownerIndex < data[owner].Item2; ownerIndex++) 
                {
                    distances[data[owner].Item1[ownerIndex]] = new Dictionary<Owner, Tuple<Direction, int>[]>();

                    foreach (var otherOwner in new Owner[]{owner.Opposite(), Owner.Humans}) 
                    {
                        distances[data[owner].Item1[ownerIndex]][otherOwner] = new Tuple<Direction, int>[data[otherOwner].Item2];
                        for (var otherTileIndex = 0; otherTileIndex < data[otherOwner].Item2; otherTileIndex++) 
                        { 
                            distances[data[owner].Item1[ownerIndex]][otherOwner][otherTileIndex] = Tuple.Create
                            (
                                Directions.Get(data[owner].Item1[ownerIndex], data[otherOwner].Item1[otherTileIndex]),
                                data[owner].Item1[ownerIndex].GetDistance(data[otherOwner].Item1[otherTileIndex])
                            );
                        }
                        Array.Sort(distances[data[owner].Item1[ownerIndex]][otherOwner], (first, second) => first.Item2 - second.Item2);
                    }
                }
            }
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
