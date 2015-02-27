using System;
using System.Collections.Generic;
using System.Linq;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Utils
{
    public static class MoveGenerator
    {
        // Get all possible Move sequences for a player on a given map
        public static List<List<Move>> GenerateMoves(IMap map, Owner owner)
        {
            var multipleMoveListList = GetCombinations(GenerateMoveLists(map, owner));

            var output = new List<List<Move>>();
            foreach (var multipleMoveList in multipleMoveListList)
            {
                var moveList = new List<Move>();
                foreach (var multipleMove in multipleMoveList)
                    foreach (var move in multipleMove.GetMoves())
                        moveList.Add(move);

                output.Add(moveList);
            }
            return output;
        }

        // Generate all possible MultipleMove sequences from all MultipleMove lists from each Tile
        private static List<List<MultipleMove>> GetCombinations(List<List<MultipleMove>> moveListList)
        {
            if (moveListList.Count == 1)
            {
                var result = new List<List<MultipleMove>>();
                foreach (var element in moveListList[0])
                    result.Add(new List<MultipleMove>() { element });

                result.Add(new List<MultipleMove>());
                return result;
            }

            var moves = new List<List<MultipleMove>>();
            var firstElement = moveListList[0];
            moveListList.RemoveAt(0);

            foreach (var moveList in GetCombinations(moveListList))
            {
                foreach (var move in firstElement)
                {
                    var localList = new List<MultipleMove>();
                    localList.Add(move);
                    localList.AddRange(moveList);
                    if (IsLegalMoveList(localList))
                        moves.Add(localList);
                }
                moves.Add(moveList);
            }
            return moves;
        }

        private static List<List<MultipleMove>> GenerateMoveLists(IMap map, Owner owner)
        {
            // Create a list of move list
            // Each sub-list is a list of move from one tile
            var splitListList = GetAllSplitMoves(map, owner);
            var possibleMoves = GetAllFullForceMoves(map, owner);

            foreach (var splitList in splitListList)
                for (int i = 0; i < possibleMoves.Count; i++)
                    if (splitList[0].Origin.X == possibleMoves[i][0].Origin.X && splitList[0].Origin.Y == possibleMoves[i][0].Origin.Y) 
                        possibleMoves[i].AddRange(splitList);

            return possibleMoves;
        }

        // Return all possible moves where all the units from a tile move towards an other tile
        private static List<List<MultipleMove>> GetAllFullForceMoves(IMap map, Owner owner)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getPlayerTiles(owner).ToList(); // Get all the tiles with my units
            int[] gridDim = map.getMapDimension();

            var possibleMoves = new List<List<MultipleMove>>();

            foreach (Tile tile in myTiles)
            {
                var tileMoves = new List<MultipleMove>();

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
                                    var move = new MultipleMove(tile, new Dictionary<Tile, int>() { { destTile, tile.Population } });
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
        private static List<List<MultipleMove>> GetAllSplitMoves(IMap map, Owner owner)
        {
            var myTiles = new List<Tile>();
            myTiles = map.getPlayerTiles(owner).ToList(); // Get all the tiles with our units
            int[] gridDim = map.getMapDimension();

            var possibleMoves = new List<List<MultipleMove>>();

            foreach (Tile tile in myTiles)
            {
                var tileMoves = new List<MultipleMove>();

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
                                tileMoves.Add(new MultipleMove(tile, new Dictionary<Tile, int>() { { destTile1, pop1 }, { destTile2, pop2 } }));
                            }
                    }
                }
                if (tileMoves.Count != 0)
                    possibleMoves.Add(tileMoves);
            }
            return possibleMoves;
        }

        // Return true is a list of move is legal (each move is compatible with every other move)
        private static bool IsLegalMoveList(List<MultipleMove> moveList)
        {
            foreach (var move in moveList)
                foreach (var otherMove in moveList)
                    if (!IsCompatibleMove(move, otherMove))
                        return false;
            return true;
        }

        // Return true is a move is compatible with an other move
        private static bool IsCompatibleMove(MultipleMove move, MultipleMove otherMove)
        {
            foreach (var dest in move.Dests)
                foreach (var otherDest in otherMove.Dests)
                    if (dest.Key == otherMove.Origin || move.Origin == otherDest.Key)
                        return false;
            return true;
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
            var count4 = 0;
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

                    if (moveList.Count == 4)
                        count4++;

                    if (moveList.Count > 4)
                        countElse++;
                }

            Console.WriteLine("Nombre de moves : unique : "+ count1 + " double : " + count2 + " triple : " + count3 + " quad : " + count4 + " et plus : " + countElse);
        }
    }
}
