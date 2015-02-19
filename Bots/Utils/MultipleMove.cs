using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;

namespace Kate.Bots.Utils
{
    class MultipleMove
    {
        public Tile Origin { get; private set; }
        public Dictionary<Tile, int> Dests { get; private set; }

        public MultipleMove(Tile origin, Dictionary<Tile, int> dests)
        {
            Origin = origin;
            Dests = dests;

            // Checking that the total population to move doesn't exceed the population of the origin tile
            int totalPopToMove = 0;
            foreach (var dest in Dests)
                totalPopToMove += dest.Value;

            if (totalPopToMove > Origin.Population)
                throw new ArgumentException("Trying to move more people than there are on the origin Tile");
        }

        public IEnumerable<Move> GetMoves()
        {
            foreach(var dest in Dests)
                yield return new Move(Origin, dest.Key, dest.Value);
        }
    }
}
