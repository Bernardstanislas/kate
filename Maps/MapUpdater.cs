using System;

namespace Kate.Maps
{
    public class MapUpdater: IMapUpdater
    {
        private readonly Tile replacementTile;

        public void execute (IMap target)
        {
            target.setTile(replacementTile);
        }

        public MapUpdater (Tile replacementTile)
        {
            this.replacementTile = replacementTile;
        }   
    }
}
