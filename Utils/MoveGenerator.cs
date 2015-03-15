using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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
                    if (IsLegalMoveList(localList) && localList.Count < 4)
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
            var possibleMoves = GetAllFullForceMissionMoves(map, owner);
            /*
            foreach (var splitList in splitListList)
                for (int i = 0; i < possibleMoves.Count; i++)
                    if (splitList[0].Origin.X == possibleMoves[i][0].Origin.X && splitList[0].Origin.Y == possibleMoves[i][0].Origin.Y) 
                        possibleMoves[i].AddRange(splitList);*/

            return possibleMoves;
        }


        // Return all possible moves where all the units from a tile move towards an other tile
        private static List<List<Move>> GetAllFullForceMissionMoves(Tile tile)
        {
            var opponentTiles = new List<Tile>();
            Owner opponent = tile.Owner.Opposite();

            opponentTiles = getPlayerTiles(opponent).ToList();

            var humanTiles = new List<Tile>();
            humanTiles = getPlayerTiles(Kate.Types.Owner.Humans).ToList();

            var possibleMoves = new List<List<Move>>();

            var tileMoves = new List<Move>();
            var targetDirections = new HashSet<Direction>();

            foreach (var opponentTile in opponentTiles)
            {
                targetDirections.Add (getMissionDirection (tile, opponentTile));
                targetDirections.Add (getMissionOppositeDirection (tile, opponentTile));
            }
            foreach (var humanTile in humanTiles)
            {
                targetDirections.Add (getMissionDirection (tile, humanTile));
            }

            var surroundinTiles = getSurroundingTiles (tile);

            foreach (var surroundingTile in surroundinTiles)
            {
                var currentDirection = getMissionDirection(tile, surroundingTile);

                if (targetDirections.Contains(targetDirections))
                    tileMoves.Add (new Move{tile, surroundingTile, tile.Population});

            possibleMoves.Add(tileMoves);
            }
            return possibleMoves;
        }

        // Return all possible moves where the units from a tile split in two equal groups in two opposite directions
        private static List<List<MultipleMove>> GetAllSplitMoves(IMap map, Owner owner)
        {
            var myTiles = new List<Tile>();
            myTiles = map.GetPlayerTiles(owner).ToList(); // Get all the tiles with our units
            int[] gridDim = map.GetMapDimension();

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
                                Tile destTile1 = map.GetTile(xPos1, yPos1);
                                Tile destTile2 = map.GetTile(xPos2, yPos2);

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


        // Generate split moves in only North-South and Est-West direction for each tile
        private static List<List<MultipleMove>> GetHumanTargetedSplitMoves(IMap map, Owner owner)
        {
            List<Tile> myTiles = new List<Tile>();
            myTiles = map.getPlayerTiles(owner).ToList(); // Get all the tiles with my units
            var humanTiles = new List<Tile>();
            humanTiles = map.getPlayerTiles(Kate.Types.Owner.Humans).ToList();


            var possibleMoves = new List<List<MultipleMove>>();

            foreach (Tile tile in myTiles)
            {
                var targetDirections = new List<List<int>> ();

                foreach (var humanTile in humanTiles)
                {
                    targetDirections.Add (getMissionDirection (tile, humanTile));
                }

                var tileMoves = new List<MultipleMove>();
                var surroundinTiles = map.getSurroundingTiles (tile);
                var destTiles = new List<Tile>();
                foreach (var surroundingTile in surroundinTiles)
                {
                    for (int i = 0; i < targetDirections.Count; i++)
                    {
                        if (surroundingTile.X == targetDirections [i] [0] && surroundingTile.Y == targetDirections [i] [1]) {

                            destTiles.Add (surroundingTile);
                            break;
                        }
                    }
                
                int totalPop = tile.Population;
                int pop = (int)(Math.Floor((double)(tile.Population / destTiles.Count)));
                var dictDestTile = new Dictionary<Tile, int> ();
                foreach (var destTile in destTiles)
                    {
                        dictDestTile.Add(destTile, pop);
                    }
                tileMoves.Add(new MultipleMove(tile, dictDestTile));
                }
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


        // Compute a squared distance
        public static double squaredDistance(Tile tile1, Tile tile2)
        {

            return Math.Pow(tile1.X - tile2.X, 2) + Math.Pow(tile1.Y - tile2.Y, 2);
        }

        // Return a list with the coordinates of the surrounding tile of the origin tile that is in the direction of targetTile
        public static Direction getMissionDirection(Tile originTile, Tile targetTile)
        {
            Direction direction = new Direction ();
            int xPos = 0;
            int yPos = 0;
            if (targetTile.X > originTile.X) {
                xPos = 1;
            }
            else if (targetTile.X < originTile.X) {
                xPos = - 1;
            }

            if (targetTile.Y > originTile.Y) {
                yPos = 1;
            }
            else if (targetTile.Y < originTile.Y) {
                yPos = - 1;
            }
            return DirectionExt.getDirection(xPos, yPos);
        }

        public static Direction getMissionOppositeDirection(Tile originTile, Tile targetTile)
        {
            return DirectionExt.getOppositeDirection(originTile, targetTile);
        }

        //Check if a tile is in the direction of an other tile, according to the surrounding origin tile
        // Used to rush in a direction
        public static bool inDirection()
        {
            return true;
        }


        // Used to split in a direction
        public static bool inDirectionSplit()
        {
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
