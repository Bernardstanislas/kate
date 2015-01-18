using System;
using Models;
using System.IO;

namespace Models.Map
{
	[Serializable()]
	public class Tile
	{
		#region Private fields
		private int xCoordinate;
		private int yCoordinate;
		private Player owner;
		private int population;
		#endregion

		#region Public attributes
		public int XCoordinate
		{
			get
			{
				return xCoordinate;
			}
			set
			{
				if (xCoordinate != -1)
				{
					throw new UnauthorizedAccessException("XCoordinate is already set.");
				}
				else
				{
					xCoordinate = value;
				}
			}
		}

		public int YCoordinate
		{
			get
			{
				return yCoordinate;
			}
			set
			{
				if (yCoordinate != -1)
				{
					throw new UnauthorizedAccessException("YCoordinate is already set.");
				}
				else
				{
					yCoordinate = value;
				}
			}
		}

		public Player Owner
		{
			get
			{
				return owner;
			}
			set
			{
				if (value == Player.NEUTRAL)
				{
					Population = 0;
				}
				owner = value;
			}
		}

		public int Population {get;set;}
		#endregion

		#region Constructors
		public Tile(): this(-1, -1) {}

		public Tile(int xCoordinate, int yCoordinate): this(xCoordinate, yCoordinate, Player.NEUTRAL, 0) {}

		public Tile(int xCoordinate, int yCoordinate, Player owner, int population)
		{
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
			this.owner = owner;
			this.Population = population;
		}
		#endregion

	}
}

