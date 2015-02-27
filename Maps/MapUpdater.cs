using System;
using Kate.Types;

namespace Kate.Maps
{
    public class MapUpdater: IMapUpdater
    {
        //Map Updater properties to be used
        private int xCoord;
        private int yCoord;
        private int deltaHumans;
        private int deltaMe;
        private int deltaOpponent;

        //Variables locales
        private int humanPop = 0;
        private int myPop = 0;
        private int opponentPop = 0;

        public void execute (IMap target)
        {
            Tile destTile = new Tile(target.getTile(xCoord, yCoord));
            
            if ( destTile.Owner.Equals(Owner.Me))
            {
                myPop = destTile.Population;
            }
            else if (destTile.Owner.Equals(Owner.Humans))
            {
                humanPop = destTile.Population;
            }
            else if (destTile.Owner.Equals(Owner.Opponent))
            {
                opponentPop = destTile.Population;
            }

            humanPop += deltaHumans;
            myPop += deltaMe;
            opponentPop += deltaOpponent;

            if (humanPop != 0)
            {
                destTile.Owner = Owner.Humans;
                destTile.Population = humanPop;
            }
            else if (opponentPop != 0)
            {
                destTile.Owner = Owner.Opponent;
                destTile.Population = opponentPop;
            }
            else if (myPop != 0)
            {
                destTile.Owner = Owner.Me;
                destTile.Population = myPop;
            }
            else
            {
                destTile.Owner = Owner.Neutral;
                destTile.Population = 0;
            }

            //The destination Tile processed during the function is then set
            //in the map.
            target.setTile(destTile);
        }

        public MapUpdater (int x, int y, int deltaH = 0, int deltaM = 0, int deltaO = 0)
        {
            this.xCoord = x;
            this.yCoord = y;
            this.deltaHumans = deltaH;
            this.deltaMe = deltaM;
            this.deltaOpponent = deltaO;

        }   
    }
}

