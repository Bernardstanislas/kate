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

        private Dictionary<Tile, Dictionary<Owner, Tuple<Direction, int>[]>> distances = null;

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
            if (distances == null) 
                GenerateDistances();

            return distances[tile][target];
        }

        private void GenerateDistances()
        {
            var humansTiles = GetPlayerTiles (Owner.Humans).ToArray ();
            var myTiles = GetPlayerTiles (Owner.Me).ToArray ();
            var opponentTiles = GetPlayerTiles (Owner.Opponent).ToArray ();

            var humansLength = humansTiles.Length;
            var myLength = myTiles.Length;
            var opponentLength = opponentTiles.Length;

            for (var myTileIndex = 0; myTileIndex < myTiles.Length; myTileIndex++) {
                distances [myTiles [myTileIndex]] [Owner.Humans] = Tuple<Direction, int> [humansLength];
                for (var humansTileIndex = 0; humansTileIndex < humansLength; humansTileIndex++) {
                    distances [myTiles [myTileIndex]] [Owner.Humans] [humansTileIndex] = Tuple.Create
                    (
                        Directions.Get (myTiles [myTileIndex], humansTiles [humansTileIndex]),
                        myTiles [myTileIndex].GetDistance (humansTiles [humansTileIndex])
                    );
                }

                for (var opponentTileIndex = 0; opponentTileIndex < opponentTiles.Length; opponentTileIndex++) {
                    var meToOpponentDirection = Directions.Get (myTiles [myTileIndex], opponentTiles [opponentTileIndex]);
                    var distance = myTiles [myTileIndex].GetDistance (opponentTiles [opponentTileIndex]);

                    distances [Owner.Me] [Owner.Opponent] [myTileIndex, opponentTileIndex] = Tuple.Create
                    (
                        meToOpponentDirection,
                        distance
                    );
                    distances [Owner.Opponent] [Owner.Me] [myTileIndex, opponentTileIndex] = Tuple.Create
                    (
                        Directions.Opposite(meToOpponentDirection),
                        distance
                    );
                }
            }

            for (var opponentTileIndex = 0; opponentTileIndex < opponentTiles.Length; opponentTileIndex++) {
                distances [opponentTiles [opponentTileIndex]] [Owner.Humans] = Tuple<Direction, int> [humansLength];
                for (var humansTileIndex = 0; humansTileIndex < humansLength; humansTileIndex++) {
                    distances[opponentTiles[opponentTileIndex]][Owner.Humans][humansTileIndex] = Tuple.Create
                    (
                        Directions.Get(opponentTiles[opponentTileIndex], humansTiles[humansTileIndex]),
                        opponentTiles[opponentTileIndex].GetDistance(humansTiles[humansTileIndex])
                    );
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


        public override IEnumerable<Tile> GetSurroundingTiles(Tile tile)
        {
            int[] gridDim = this.GetMapDimension();
            var surroundingTiles = new List<Tile> ();

            for (int i = -1; i <= 1; i++)
            {
                int xPos = tile.X;

                if (
                    0 < tile.X && tile.X < gridDim[0] - 1
                    || tile.X == 0 && i != -1               // Tile on left edge
                    || tile.X == gridDim[0] - 1 && i != 1   // Tile on right edge
                    )
                {
                    xPos += i;

                    for (int j = -1; j <= 1; j++)
                    {
                        int yPos = tile.Y;
                        if (
                            0 < tile.Y && tile.Y < gridDim[1] - 1
                            || tile.Y == 0 && j != -1              // Tile on top edge
                            || tile.Y == gridDim[1] - 1 && j != 1  // Tile on bottom edge
                            )
                        {
                            yPos += j;

                            // The null move is not generated
                            if (!(xPos == tile.X && yPos == tile.Y))
                            {
                                Tile destTile = this.GetTile(xPos, yPos);
                                surroundingTiles.Add(destTile);
                            }
                        }
                    }
                }
            }
            return surroundingTiles;
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
