using System;

using Kate.Types;

namespace Kate.Maps
{
    [Serializable()]
    public class Tile
    {
        #region Public attributes
        public int X { get; private set; }
        public int Y { get; private set; }
        public Owner Owner { get; set; }
        public int Population { get; set; }
        public int GetDistance(Tile other)
        {
            return Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
        }
        public override int GetHashCode()
        {
            return hash;
        }
        #endregion

        #region Private attributes
        private readonly int hash;
        #endregion

        #region Constructors
        public Tile(): this(-1, -1) { }

        public Tile(int x, int y) : this(x, y, Owner.Neutral, 0) { }

        public Tile(int x, int y, Owner owner, int population)
        {
            X = x;
            Y = y;
            Owner = owner;
            Population = population;
            hash = int(owner) + population * 10 + x * 10000 + y * 10000000;
        }

        public Tile(Tile oT) : this(oT.X, oT.Y, oT.Owner,oT.Population) { }
        
        #endregion
    }
}
