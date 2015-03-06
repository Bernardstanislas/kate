using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Utils
{
    public static class FightUtil
    {
        // Simple method. Please note oriPop is the ATTACKING pop, destPop is the DEFENDING one.
        public static bool IsWon(int oriPop, Owner oriOwner, int destPop, Owner destOwner)
        {
            bool res = false;
            int victoryProba = oriPop / (oriPop + destPop);
            switch (destOwner)
            {
            case Owner.Humans:
                if (oriPop >= destPop || victoryProba > 0.5)
                    res = true;
                break;

            case Owner.Me:
                if (oriOwner.Equals(Owner.Me) || (oriOwner.Equals(Owner.Opponent) && ((oriPop > 1.5 * destPop) || victoryProba > 0.5)))
                    res = true;
                break;

            case Owner.Opponent:
                if (oriOwner.Equals(Owner.Opponent) || (oriOwner.Equals(Owner.Me) && ((oriPop > 1.5 * destPop) || victoryProba > 0.5)))
                    res = true;
                break;
            }
            return res;
        }

        public static Tile FightResult(Owner oriOwner, int attackingPop, Owner destOwner, int destPop)
        {
            float victoryProba = (float)attackingPop / (float)(attackingPop + destPop);
            Tile result = new Tile();

            switch (destOwner) 
            {
            case Owner.Humans:
                // Victory is possible only if attacking pop > human pop
                if (attackingPop >= destPop)
                {
                    result.Owner = oriOwner;
                    result.Population = destPop + attackingPop;
                }
                // Defeat case, defending human pop is partly killed.
                else
                {
                    //Console.WriteLine("topkek");
                    int finalPop = (int)(destPop*(1 - victoryProba));
                    //Console.WriteLine(destPop + " <- Pop destination .. Proba de victoire ->"+victoryProba+ "lel jabba le zgeg " + finalPop);

                    result.Owner = Owner.Humans;
                    result.Population = finalPop;
                }
                break;

            case Owner.Me:
                // We only treat attacks. So it is assumed the oriOwner is Opponent.
                if (attackingPop >= destPop*1.5)
                {
                    result.Owner = oriOwner;
                    result.Population = attackingPop;
                }
                // Random battle attackers victory
                else if (victoryProba > 0.5)
                {
                    int finalPop = (int)(attackingPop * victoryProba);
                    result.Owner = Owner.Opponent;
                    result.Population = finalPop;
                }
                // Random battle defenders victory
                else
                {
                    int finalPop = (int)(destPop * (1-victoryProba));
                    result.Owner = Owner.Me;
                    result.Population = finalPop;
                }
                break;

            case Owner.Opponent:
                // We only treat attacks. So it is assumed the oriOwner is Me.
                if (attackingPop >= destPop * 1.5)
                {
                    result.Owner = oriOwner;
                    result.Population = attackingPop;
                }
                // Random battle attackers victory (Me)
                else if (victoryProba > 0.5)
                {
                    int finalPop = (int)(attackingPop * victoryProba);
                    result.Owner = oriOwner;
                    result.Population = finalPop;
                }
                // Random battle defenders victory
                else
                {
                    int finalPop = (int)(destPop * (1 - victoryProba));
                    result.Owner = Owner.Opponent;
                    result.Population = finalPop;
                }
                break;
            }
            return result;
        }    
    }
}
