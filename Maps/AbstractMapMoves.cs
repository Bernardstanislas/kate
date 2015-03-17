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
            var possibleMoves = new List<Move[]>();

            var humansDistances = GetDistances(tile, Owner.Humans);
            var opponentDistances = GetDistances(tile, Owner.Opponent);

            var humansLength = humansDistances.Length;

            var targetDirections = new HashSet<Direction>();
            for (var distanceIndex = 0; distanceIndex < opponentDistances.Length; distanceIndex++) 
            {
                targetDirections.Add(opponentDistances[distanceIndex].Item1);

                var oppositeDirection = Directions.Opposite(opponentDistances[distanceIndex].Item1);
                var mapDimensions = GetMapDimension();
                int order = 0;
                bool foundOpposite = false;
                while (!foundOpposite)
                {
                    var directions = oppositeDirection.Neighbours(order);
                    for (var directionIndex = 0; directionIndex < directions.Length; directionIndex++) 
                    {
                        var coordinates = directions[directionIndex].GetTileCoordinates(tile);
                        if 
                        (
                            coordinates[0] >= 0 && coordinates[0] < mapDimensions[0] &&
                            coordinates[1] >= 0 && coordinates[1] < mapDimensions[1]
                        ) 
                        {
                            foundOpposite = true;
                            targetDirections.Add(directions[directionIndex]);
                            break;
                        }
                    }

                    if (!foundOpposite) 
                        order++;
                }
            }
            for (var distanceIndex = 0; distanceIndex < humansLength; distanceIndex++) 
            {
                targetDirections.Add(humansDistances[distanceIndex].Item1);
            }

            // Generate fullForce moves
            foreach (var targetDirection in targetDirections)
            {
                var coordinates = Directions.GetTileCoordinates(targetDirection, tile);
                possibleMoves.Add(new Move[]{new Move(tile, GetTile(coordinates[0], coordinates[1]), tile.Population)});
            }
           
            if (humansLength > 1 && tile.Population > 1) 
            {
                if (humansLength > 2 && tile.Population > 2) 
                {
                    var firstCoordinates = Directions.GetTileCoordinates(humansDistances[0].Item1, tile);
                    var secondCoordinates = Directions.GetTileCoordinates(humansDistances[1].Item1, tile);
                    var thirdCoordinates = Directions.GetTileCoordinates(humansDistances[2].Item1, tile);

                    var halfPopulation = tile.Population / 2;
                    possibleMoves.Add(new Move[]{
                        new Move(tile, GetTile(firstCoordinates[0], firstCoordinates[1]), tile.Population - halfPopulation),
                        new Move(tile, GetTile(secondCoordinates[0], secondCoordinates[1]), halfPopulation),
                    });

                    var thirdPopulation = tile.Population / 3;
                    possibleMoves.Add(new Move[]{
                        new Move(tile, GetTile(firstCoordinates[0], firstCoordinates[1]), tile.Population - 2 * thirdPopulation),
                        new Move(tile, GetTile(secondCoordinates[0], secondCoordinates[1]), thirdPopulation),
                        new Move(tile, GetTile(thirdCoordinates[0], thirdCoordinates[1]), thirdPopulation),
                    });
                } 
                else 
                {
                    var firstCoordinates = Directions.GetTileCoordinates(humansDistances[0].Item1, tile);
                    var secondCoordinates = Directions.GetTileCoordinates(humansDistances[1].Item1, tile);

                    var halfPopulation = tile.Population / 2;
                    possibleMoves.Add(new Move[]{
                        new Move(tile, GetTile(firstCoordinates[0], firstCoordinates[1]), tile.Population - halfPopulation),
                        new Move(tile, GetTile(secondCoordinates[0], secondCoordinates[1]), halfPopulation),
                    });
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
