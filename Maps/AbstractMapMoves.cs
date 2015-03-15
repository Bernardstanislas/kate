using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Kate.Utils;
using Kate.Types;
using Kate.Commands;

/**
 * Convention: a MultipleMove is a list of moves with the same origin
 */
namespace Kate.Maps
{
    public abstract partial class AbstractMap : IMap
    {
        /**
         * Generate all the moves associated with an owner
         */
        public List<Move[]> GenerateMovesLists(Owner owner)
        {
            var movesLists = new List<Move[]>();

            var tiles = GetPlayerTiles(owner).ToArray();
            var tilesLength = tiles.Length;
            var multipleMovesByTile = new Move[tilesLength][][];

            for (int firstTileIndex = 0; firstTileIndex < tilesLength; firstTileIndex++) 
            {
                if (multipleMovesByTile[firstTileIndex] == null)
                    multipleMovesByTile[firstTileIndex] = GenerateMultipleMoves(tiles[firstTileIndex]).ToArray();

                movesLists.AddRange(multipleMovesByTile[firstTileIndex]);

                for (int secondTileIndex = firstTileIndex + 1; secondTileIndex < tiles.Length; secondTileIndex++) 
                {
                    if (multipleMovesByTile[secondTileIndex] == null)
                        multipleMovesByTile[secondTileIndex] = GenerateMultipleMoves(tiles[secondTileIndex]).ToArray();

                    var multipleMovesByTilePair = new List<Move[][]>();
                    for 
                    (
                        int firstMultipleMoveIndex = 0; 
                        firstMultipleMoveIndex < multipleMovesByTile[firstTileIndex].Length; 
                        firstMultipleMoveIndex++
                    )
                        for 
                        (
                            int secondMultipleMoveIndex = 0; 
                            secondMultipleMoveIndex < multipleMovesByTile[secondTileIndex].Length; 
                            secondMultipleMoveIndex++
                        )
                            if (AreMultipleMoveCoherent(
                                multipleMovesByTile[firstTileIndex][firstMultipleMoveIndex], 
                                multipleMovesByTile[secondTileIndex][secondMultipleMoveIndex]
                            )) 
                            {
                                var firstLength = multipleMovesByTile[firstTileIndex][firstMultipleMoveIndex].Length;
                                var mergedMultipleMoves = new Move[
                                    firstLength + multipleMovesByTile[secondTileIndex][secondMultipleMoveIndex].Length
                                ];
                                multipleMovesByTile[firstTileIndex][firstMultipleMoveIndex].CopyTo(mergedMultipleMoves, 0);
                                multipleMovesByTile[secondTileIndex][secondMultipleMoveIndex].CopyTo(mergedMultipleMoves, firstLength);
                                movesLists.Add(mergedMultipleMoves);

                                multipleMovesByTilePair.Add(new Move[][] {
                                    multipleMovesByTile[firstTileIndex][firstMultipleMoveIndex], 
                                    multipleMovesByTile[secondTileIndex][secondMultipleMoveIndex]
                                });
                            }

                    for (int thirdTileIndex = secondTileIndex + 1; thirdTileIndex < tiles.Length; thirdTileIndex++) 
                    {
                        if (multipleMovesByTile[thirdTileIndex] == null)
                            multipleMovesByTile[thirdTileIndex] = GenerateMultipleMoves(tiles[thirdTileIndex]).ToArray();
                            
                        for (int pairIndex = 0; pairIndex < multipleMovesByTilePair.Count; pairIndex++) 
                        {
                            var movePair = multipleMovesByTilePair[pairIndex];

                            for
                            (
                                int multipleMoveIndex = 0; 
                                multipleMoveIndex < multipleMovesByTile[thirdTileIndex].Length; 
                                multipleMoveIndex++
                            ) 
                            {
                                if (IsMultipleMoveCoherentWithPair(multipleMovesByTile[thirdTileIndex][multipleMoveIndex], movePair)) {
                                    var firstLength = movePair[0].Length;
                                    var secondLength = movePair[1].Length + firstLength;

                                    var mergedMultipleMoves = new Move[
                                        secondLength + multipleMovesByTile[thirdTileIndex][multipleMoveIndex].Length
                                    ];
                                        
                                    movePair[0].CopyTo(mergedMultipleMoves, 0);
                                    movePair[1].CopyTo(mergedMultipleMoves, firstLength);
                                    multipleMovesByTile[thirdTileIndex][multipleMoveIndex].CopyTo(mergedMultipleMoves, secondLength);

                                    movesLists.Add(mergedMultipleMoves);
                                }
                            }
                        }
                    }
                }
            }

            return movesLists;
        }

        /**
         * Generate MultipleMoves associated with a tile
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<Move[]> GenerateMultipleMoves(Tile tile) 
        {
            return new List<Move[]>();
        }

        /**
         * Check if the MultipleMoves are coherent (origin different from target)
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private Boolean AreMultipleMoveCoherent(Move[] firstMultipleMove, Move[] secondMultipleMove) 
        {
            for (int moveIndex = 0; moveIndex < secondMultipleMove.Length; moveIndex++) 
            {
                if 
                (
                    secondMultipleMove[moveIndex].Dest.X == firstMultipleMove[0].Origin.X &&
                    secondMultipleMove[moveIndex].Dest.Y == firstMultipleMove[0].Origin.Y
                ) 
                    return false;
            }
            for (int moveIndex = 0; moveIndex < firstMultipleMove.Length; moveIndex++) 
            {
                if
                (
                    firstMultipleMove[moveIndex].Dest.X == secondMultipleMove[0].Origin.X &&
                    firstMultipleMove[moveIndex].Dest.Y == secondMultipleMove[0].Origin.Y
                ) 
                    return false;
            }

            return true;
        }

        /**
         * Check if the MultipleMove is coherent with a pair 
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private Boolean IsMultipleMoveCoherentWithPair(Move[] multipleMove, Move[][] multipleMovePair)
        {
            return 
            (
                AreMultipleMoveCoherent(multipleMove, multipleMovePair[0]) && 
                AreMultipleMoveCoherent(multipleMove, multipleMovePair[1])
            );
        }


        // Return all possible moves where all the units from a tile move towards an other tile
        private static List<List<Move>> GetMissionMoves(Tile tile)
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

    }
}
