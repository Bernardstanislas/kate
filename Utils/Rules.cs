using System;
using Models.Map;
using System.Collections.Generic;
using System.Linq;
using Models.Commands;
using Models;

namespace kate
{
	public static class Rules
	{
		public static List<Move> getpossibleMoves(Map map)
		{
			List<Tile> myTiles = new List<Tile>();
			int[] gridDim = map.getMapDimension();

			foreach (Tile tile in map.getGrid())
			{
				if (tile.Owner.Equals(Models.Player.Me))
				{
					myTiles.Add(tile);
				}
			}

			List<Move> possibleMoves = new List<Move>();

			foreach (Tile tile in myTiles.Where(tile => tile.Population > 0))
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
					}
						Tile destTile = map.getTile(xPos, yPos);
						Move move = new Move(tile, destTile, tile.Population);
						possibleMoves.Add(move);
				}
			}

			return possibleMoves;
		}


		public static List<List<Move>> getLegalMovesLists(Map map, List<Move> moveList)
		{
			List<List<Move>> legalMoves = new List<List<Move>>();

			foreach (Move move1 in moveList)
			{
				foreach (Move move2 in moveList.Where( move2 => move1 != move2))
				{
					if (move1.Dest != move2.Origin)
					{
						legalMoves.Add(move1);
					}
				}
			}
		}



	}
}
