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

        // Return all possible moves where all the units from a tile move towards an other tile
        private static List<List<Move>> GetAllFullForceMissionMoves(Tile tile)
        {
            var opponentTiles = new List<Tile>();
            Owner opponent = tile.Owner.Opposite();

            opponentTiles = GetPlayerTiles(opponent).ToList();

            var humanTiles = new List<Tile>();
            humanTiles = GetPlayerTiles(Kate.Types.Owner.Humans).ToList();

            var possibleMoves = new List<List<Move>>();

            var tileMoves = new List<Move>();
            var targetDirections = new HashSet<Direction>();

            foreach (var opponentTile in opponentTiles)
            {
                targetDirections.Add (GetMissionDirection (tile, opponentTile));
                targetDirections.Add (GetMissionOppositeDirection (tile, opponentTile));
            }
            foreach (var humanTile in humanTiles)
            {
                targetDirections.Add (GetMissionDirection (tile, humanTile));
            }

            var surroundinTiles = GetSurroundingTiles (tile);

            foreach (var surroundingTile in surroundinTiles)
            {
                var currentDirection = GetMissionDirection(tile, surroundingTile);

                if (targetDirections.Contains(targetDirections))
                    tileMoves.Add (new Move{tile, surroundingTile, tile.Population});

            possibleMoves.Add(tileMoves);
            }
            return possibleMoves;
        }

        // Generate split moves in only North-South and Est-West direction for each tile
        private static List<List<Move>> GetHumanTargetedSplitMoves(Tile tile)
        {
            var humanTiles = new List<Tile>();
            humanTiles = GetPlayerTiles(Kate.Types.Owner.Humans).ToList();

            var possibleMoves = new List<List<Move>>();


            var targetDirections = new HashSet<Direction>();
            foreach (var humanTile in humanTiles)
            {
                targetDirections.Add (GetMissionDirection (tile, humanTile));
            }

            var tileMoves = new List<Move>();

            var surroundinTiles = GetSurroundingTiles (tile);
            var destTiles = new List<Tile>();

            foreach (var surroundingTile in surroundinTiles)
            {
                var currentDirection = GetMissionDirection(tile, surroundingTile);
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
            
            return possibleMoves;
        }



        // Return a list with the coordinates of the surrounding tile of the origin tile that is in the direction of targetTile
        public static Direction GetMissionDirection(Tile originTile, Tile targetTile)
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
            return DirectionExt.GetDirection(xPos, yPos);
        }

        public static Direction GetMissionOppositeDirection(Tile originTile, Tile targetTile)
        {
            return DirectionExt.GetOppositeDirection(originTile, targetTile);
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
