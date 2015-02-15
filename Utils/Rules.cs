using System;
using Kate.Maps;
using System.Collections.Generic;
using System.Linq;
using Kate.Commands;

namespace kate
{
	public static class Rules
	{
		#region generateMoves
		// Return all possible unit moves from a map (different from a combination of moves)
		//
		public static List<Move> getpossibleMoves(Map map, string param = "all")
		{
			List<Tile> myTiles = new List<Tile>();
			myTiles = map.getMyTiles();
			int[] gridDim = map.getMapDimension();

			List<Move> possibleMoves = new List<Move>();

			foreach (Tile tile in myTiles)
			{
				int xPos = tile.XCoordinate;
				int yPos = tile.YCoordinate;

				for (int i =-1; i <= 1; i++)
				{
					if (0 < xPos < gridDim[0] - 1)
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

					for (int j =-1; i <= 1; j++)
					{
						if (0 < yPos < gridDim[1] - 1)
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

						Tile destTile = map.getTile (xPos, yPos);

						if (param == "fullForce") // Assume that the move will use all the population on the tile
						{
							Move move = new Move (tile, destTile, tile.Population);
							possibleMoves.Add(move);
						}
						else if (param == "all") // Generate different moves for different populatio to move
							for (int pop = 1; pop <= tile.Population; pop++) 
							{
								Move move = new Move (tile, destTile, pop);
								possibleMoves.Add(move);
							}
					}
				}
			}

			return possibleMoves;
		}


		// Return true is a move is compatible with a list of other moves
		public static bool isLegalMove(Move move, List<Move> moveList)
		{
			foreach (Move otherMove in moveList)
			{
				if (move.Dest == otherMove.Origin)
				{
					return false;
				}
			}
		return true;
		}

		public static List<Move> getLegalFullForceMoves(Map map)
		{
			List<Move> possibleFullForceMoves = getpossibleMoves (map, "fullForce");
			List<Move> legalMoves = new List<Move>();
			foreach (Move currentMove in possibleFullForceMoves) 
			{
				if (isLegalMove(currentMove, legalMoves))
				{
					legalMoves.Add(currentMove);
				}
			}

			return legalMoves;
		}
		#endregion


	}
}
