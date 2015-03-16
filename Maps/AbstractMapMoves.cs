using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Kate.Commands;
using Kate.Types;
using Kate.Utils;

// Convention: a MultipleMove is a list of moves with the same origin

namespace Kate.Maps
{
    public abstract partial class AbstractMap : IMap
    {
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

            Utils.MoveUtils.PrintListStats(movesLists);
            Utils.MoveUtils.PrintMove(movesLists);

            return movesLists;
        }

        // Return all possible moves where all the units from a tile move towards an other tile
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<Move[]> GenerateMultipleMoves(Tile tile)
        {
            // Find enemy tiles
            var opponentTiles = new List<Tile>();
            Owner opponent = tile.Owner.Opposite();
            opponentTiles = this.GetPlayerTiles(opponent).ToList();

            // Find human tiles
            var humanTiles = new List<Tile>();
            humanTiles = GetPlayerTiles(Kate.Types.Owner.Humans).ToList();

            var possibleMoves = new List<Move[]>();

            // Find directions for both rush to ennemies and humans, and fleeing from enemies
            var targetDirections = new HashSet<Direction>();
            var humanTargetDirections = new HashSet<Direction>();

            var mapDimension = GetMapDimension();

            foreach (var opponentTile in opponentTiles)
            {
                var direction = getMissionDirection(tile, opponentTile);
                targetDirections.Add(direction);

                if (tile.X == mapDimension [0] || tile.Y == mapDimension [1])
                {
                    targetDirections.Add (Direction.E);
                    targetDirections.Add (Direction.W);
                    targetDirections.Add (Direction.N);
                    targetDirections.Add (Direction.S);
                }
                else
                    targetDirections.Add(direction.Opposite());
            }

            foreach (var humanTile in humanTiles)
            {
                var humanDirection = getMissionDirection(tile, humanTile);
                humanTargetDirections.Add(humanDirection);
                targetDirections.Add (humanDirection);
            }
                

            var splitDestTiles = new List<Tile>();


            // Generate fullForce moves
            foreach (var targetDirection in targetDirections)
            {
                var coords = Directions.GetTileCoordinates(targetDirection, tile);
                if (coords[0] < mapDimension[0] && coords[0] >= 0 && coords[1] < mapDimension[1] && coords[1] >= 0)
                {
                    var surroundingTile = GetTile(coords[0], coords[1]);
                    possibleMoves.Add(new Move[1] { new Move(tile, surroundingTile, tile.Population) });
                }
            }

            // Generate split moves
            int splitCount = 0;
            if (humanTargetDirections.Count > 1)
            {
                foreach (var humanDirection in humanTargetDirections) {
                    var coords = Directions.GetTileCoordinates (humanDirection, tile);
                    // Store tiles that are candidates to split moves

                    if (coords [0] < mapDimension [0] && coords [0] >= 0 && coords [1] < mapDimension [1] && coords [1] >= 0)
                    if (splitCount < 4) {
                        var surroundingHumanTile = GetTile (coords [0], coords [1]);
                        splitDestTiles.Add (surroundingHumanTile);
                        splitCount++;
                    }
                }
            }

            // Split haves multiples destination tiles
            if (splitDestTiles.Count > 1)
            {
                // Create two cases: either we want to split in two groups, either in three groups.
                int totalPop = tile.Population;
                int totalPopDividedBy2 = (int)(Math.Floor((double)(tile.Population / 2)));
                int totalPopDividedBy3 = (int)(Math.Floor((double)(tile.Population / 3)));

                var pop2 = new int[] { totalPopDividedBy2, totalPop - totalPopDividedBy2 };
                var pop3 = new int[] { totalPopDividedBy3, totalPopDividedBy3, totalPop - 2 * totalPopDividedBy3 };

                // Generate splits in two groups
                var split2Moves = new Move[2];
                for (int i = 0; i < 2; i++)
                    split2Moves[i] = new Move(tile, splitDestTiles[i], pop2[i]);

                possibleMoves.Add(split2Moves);

                // Generate splits in 3 groups
                if (splitDestTiles.Count > 2)
                {
                    var split3Moves = new Move[3];
                    for (int i = 0; i < 3; i++)
                        split3Moves[i] = new Move(tile, splitDestTiles[i], pop3[i]);

                    possibleMoves.Add(split3Moves);
                }
            }
            return possibleMoves;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private bool AreMultipleMoveCoherent(Move[] firstMultipleMove, Move[] secondMultipleMove) 
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private bool IsMultipleMoveCoherentWithPair(Move[] multipleMove, Move[][] multipleMovePair)
        {
            return 
            (
                AreMultipleMoveCoherent(multipleMove, multipleMovePair[0]) && 
                AreMultipleMoveCoherent(multipleMove, multipleMovePair[1])
            );
        }

        // Return a list with the coordinates of the surrounding tile of the origin tile that is in the direction of targetTile
        private Direction getMissionDirection(Tile originTile, Tile targetTile)
        {
            int xPos = 0;
            int yPos = 0;

            if (targetTile.X > originTile.X)
                xPos = 1;
            else if (targetTile.X < originTile.X)
                xPos = -1;

            if (targetTile.Y > originTile.Y)
                yPos = 1;
            else if (targetTile.Y < originTile.Y)
                yPos = -1;

            return Directions.Get(xPos, yPos);
        }
    }
}
