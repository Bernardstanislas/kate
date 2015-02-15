using System;
using Kate.Maps;
using System.Collections.Generic;
using System.Linq;
using Kate.Commands;


namespace Kate.Utils
{
    public static class MoveGenerator
    {
        #region generateMoves
        // Return all possible moves where all the units from a tile move toward an other tile
        //
        public static List<List<Move>> getAllFullForceMoves(Map map)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getMyTiles().ToList(); // Get all the tiles with my units
            int[] gridDim = map.getMapDimension();

            var possibleMoves = new List<List<Move>>();

            foreach (Tile tile in myTiles)
            {
                int xPos = tile.X;
                int yPos = tile.Y;
                List<Move> tileMoves = new List<Move>();

                for (int i =-1; i <= 1; i++)
                {
                    if (0 < xPos && xPos < gridDim[0] - 1)
                    { 
                        xPos += i;
                    }

                    else if (xPos == 0)
                    {
                        if (i != -1)
                        {
                            xPos += i;
                        }
                    }

                    else if (xPos == gridDim[0] - 1)
                    {
                        if (i != 1)
                        {
                            xPos += i;
                        }
                    }

                    for (int j =-1; j <= 1; j++)
                    {
                        if (0 < yPos && yPos< gridDim[1] - 1)
                        {
                            yPos += j;
                        }

                        else if (yPos == 0)
                        {
                            if (j != -1)
                            {
                                yPos += j;
                            }
                        }

                        else if (yPos == gridDim[1] - 1)
                        {
                            if (j != 1)
                            {
                                yPos += i;
                            }
                        }

                        // The null move is not generated
                        if (!(xPos == 0 && yPos == 0)) 
                        {
                            Tile destTile = map.getTile (xPos, yPos);
                            Move move = new Move (tile, destTile, tile.Population);
                            tileMoves.Add (move);
                        }
                    }
                }
                possibleMoves.Add(tileMoves);
            }

            return possibleMoves;
        }

        // Return all possible moves where the units from a tile split in two equal groups in two opposite directions
        //
        public static List<List<Move>> getAllSplitMoves(Map map)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getMyTiles().ToList(); // Get all the tiles with my units
            int[] gridDim = map.getMapDimension();

        List<List<Move>> possibleMoves = new List<List<Move>> ();

            foreach (Tile tile in myTiles)
            {
                int xPos1 = tile.X;
                int yPos1 = tile.Y;
                int xPos2 = tile.X;
                int yPos2 = tile.Y;

                List<Move> tileMoves = new List<Move>();

                for (int i = 0; i <= 1; i++)
                {
                    xPos1 += i;
                    xPos2 += -i;

                    for (int j = 0; j <= 1; j++)
                    {
                        yPos1 += j;
                        yPos2 += -j;

                        // Generate moves that fits the grid, necessary for the sides of the grid
                        if ((xPos1 <= gridDim[0] - 1) && (0 <= xPos2 ) && (yPos1 <= gridDim[1] - 1) && (0 <= yPos2 ))
                        {
                            // Never generate null moves
                            if (!(xPos1 == 0 && yPos1 == 0) || !(xPos2 == 0 && yPos2 == 0)) 
                            {
                                Tile destTile1 = map.getTile (xPos1, yPos1);
                                Tile destTile2 = map.getTile (xPos2, yPos2);

                                // Split units in two groups equivalent in number
                                int pop1 = (int)(tile.Population / 2);
                                int pop2 = 1 - pop1;

                                // Create two moves with half of the initial population
                                Move move1 = new Move (tile, destTile1, pop1);
                                Move move2 = new Move (tile, destTile2, pop2);
                                tileMoves.Add (move1);
                                tileMoves.Add (move2);
                            }
                        }
                    }
                }
                possibleMoves.Add (tileMoves);
            }

            return possibleMoves;
        }

        public static List<List<Move>> generateMoves(Map map)
        {
            // Create a list of move list
            // Each sub-list is a list of move from one tile
            List<List<Move>> fullForce = getAllFullForceMoves (map);
            List<List<Move>> splitForce = getAllSplitMoves (map);
            List<List<Move>> possibleMoves = new List<List<Move>> ();

            foreach (List<Move> ffMoveList in fullForce)
            {
                List<Move> bufferMove = new List<Move> ();
                foreach (List<Move> splitMoveList in splitForce)
                {
                    if (ffMoveList [0].Origin == splitMoveList [0].Origin)
                    {
                        bufferMove.Concat(ffMoveList).Concat(splitMoveList);
                    }
                }
                possibleMoves.Add(bufferMove);
            }

            return Combinations(possibleMoves);
        }
        #endregion

        // Return true is a move is compatible with an other move
        public static bool isCompatibleMove(Move move, Move otherMove)
        {
            if (move.Dest == otherMove.Origin || move.Origin == otherMove.Dest || move.Equals(otherMove))
            {
                return false;
            }

            return true;
        }


        public static List<List<Move>> Combinations(List<List<Move>> moveListList)
        {
            if (moveListList.Count == 1)
            {
                var result = new List<List<Move>>();
                foreach (var element in moveListList[0])
                {
                    result.Add (new List<Move>(){element});
                }
                result.Add (new List<Move> ());
                return result;
            }

            var moves = new List<List<Move>>();
            moveListList.RemoveAt (0);

            foreach (List<Move> moveList in Combinations(moveListList))
            {
                foreach( Move move in (moveListList[0]))
                {
                    var localList = new List<Move>();
                    localList.Add(move);
                    localList.Concat (moveList);
                    moves.Add(localList);
                    moves.Add(moveList);
                }
            }
            return moves;

        }
    }
}
