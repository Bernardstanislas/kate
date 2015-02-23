using System;
using System.Collections.Generic;
using System.Linq;

using Kate.Commands;
using Kate.Maps;

namespace Kate.Utils
{
    public static class MoveGenerator
    {
        // Return all possible moves where all the units from a tile move towards an other tile
        //
        public static List<List<Move>> GetAllFullForceMoves(IMap map)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getMyTiles().ToList(); // Get all the tiles with my units
            int[] gridDim = map.getMapDimension();

            var possibleMoves = new List<List<Move>>();

            foreach (Tile tile in myTiles)
            {
                List<Move> tileMoves = new List<Move>();

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
                                    Tile destTile = map.getTile(xPos, yPos);
                                    Move move = new Move(tile, destTile, tile.Population);
                                    tileMoves.Add(move);
                                }
                            }
                        }
                    }
                }
                possibleMoves.Add(tileMoves);
            }
            return possibleMoves;
        }

        // Return all possible moves where the units from a tile split in two equal groups in two opposite directions
        //
        public static List<List<Move>> GetAllSplitMoves(IMap map)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getMyTiles().ToList(); // Get all the tiles with our units
            int[] gridDim = map.getMapDimension();

            List<List<Move>> possibleMoves = new List<List<Move>>();

            foreach (Tile tile in myTiles)
            {
                List<Move> tileMoves = new List<Move>();

                for (int i = 0; i <= 1; i++)
                {
                    int xPos1 = tile.X;
                    int xPos2 = tile.X;
                    xPos1 += i;
                    xPos2 += -i;

                    for (int j = 0; j <= 1; j++)
                    {
                        int yPos1 = tile.Y;
                        int yPos2 = tile.Y;
                        yPos1 += j;
                        yPos2 += -j;

                        // Generate moves that fits the grid, necessary for the sides of the grid
                        if ((xPos1 <= gridDim[0] - 1) && (0 <= xPos2) && (yPos1 <= gridDim[1] - 1) && (0 <= yPos2))
                            // Never generate null moves
                            if (!(xPos1 == tile.X && yPos1 == tile.Y) || !(xPos2 == tile.X && yPos2 == tile.Y)) 
                            {
                                Tile destTile1 = map.getTile(xPos1, yPos1);
                                Tile destTile2 = map.getTile(xPos2, yPos2);

                                // Split units in two groups equivalent in number
                                int pop1 = (int)(tile.Population / 2);
                                int pop2 = tile.Population - pop1;

                                // Create two moves with half of the initial population
                                Move move1 = new Move(tile, destTile1, pop1);
                                Move move2 = new Move(tile, destTile2, pop2);
                                tileMoves.Add(move1);
                                tileMoves.Add(move2);
                            }
                    }
                }
                if (tileMoves.Count != 0)
                    possibleMoves.Add(tileMoves);
            }
            return possibleMoves;
        }

        public static List<List<Move>> GenerateMoves(IMap map)
        {
            // Create a list of move list
            // Each sub-list is a list of move from one tile
            List<List<Move>> splitListList = GetAllSplitMoves(map);

            var possibleMoves = GetAllFullForceMoves(map);

            foreach (var splitList in splitListList)
                for (int i = 0; i < possibleMoves.Count; i++)
                    if (splitList[0].Origin.X == possibleMoves[i][0].Origin.X && splitList[0].Origin.Y == possibleMoves[i][0].Origin.Y) 
                        possibleMoves[i].AddRange(splitList);

            return possibleMoves;
        }

        // Return true is a move is compatible with an other move
        public static bool IsCompatibleMove(Move move, Move otherMove)
        {
            if (move.Dest == otherMove.Origin || move.Origin == otherMove.Dest || move.Equals(otherMove))
                return false;
            return true;
        }

        public static List<List<Move>> Combinations(List<List<Move>> moveListList)
        {
            if (moveListList.Count == 1)
            {
                var result = new List<List<Move>>();
                foreach (var element in moveListList[0])
                    result.Add(new List<Move>(){ element });

                result.Add(new List<Move>());
                return result;
            }

            var moves = new List<List<Move>>();
            var firstElement = moveListList[0];
            moveListList.RemoveAt(0);

            foreach (List<Move> moveList in Combinations(moveListList))
            {
                foreach(Move move in firstElement)
                {
                    var localList = new List<Move>();
                    localList.Add(move);
                    localList.AddRange(moveList);
                    moves.Add(localList);
                }
                moves.Add(moveList);
            }
            return moves;
        }

        public static void PrintMove(List<List<Move>> moveListList)
        {
            foreach (List<Move> moveList in moveListList)
            {
                foreach (Move element in moveList)
                    Console.WriteLine ("Or: " + element.Origin.X + element.Origin.Y + " pop :" + element.PopToMove + " Dest: " + element.Dest.X + element.Dest.Y);
                
                Console.WriteLine ("*******************************");
            }
        }

        public static void PrintListStats(List<List<Move>> moveListList)
        {
            var count1 = 0;
            var count2 = 0;
            var count3 = 0;
            var countElse = 0;

            foreach (List<Move> moveList in moveListList)
                foreach (Move element in moveList) 
                {
                    if (moveList.Count == 1)
                        count1++;

                    if (moveList.Count == 2)
                        count2++;

                    if (moveList.Count == 3)
                        count3++;

                    if (moveList.Count > 3)
                        countElse++;
                }

            Console.WriteLine("Nombre de moves : unique : "+ count1 + " double : " + count2 + " triple : " + count3 + " et plus : " + countElse);
        }
    }
}
