using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Utils
{
    public static class MoveUtils
    {
        public static void PrintMove(List<Move[]> moveListList)
        {
            foreach (Move[] moveList in moveListList)
            {
                foreach (Move element in moveList)
                    Console.WriteLine ("Or: " + element.Origin.X + element.Origin.Y + " pop :" + element.PopToMove + " Dest: " + element.Dest.X + element.Dest.Y);
                
                Console.WriteLine ("*******************************");
            }
        }

        public static void PrintListStats(List<Move[]> moveListList)
        {
            var count1 = 0;
            var count2 = 0;
            var count3 = 0;
            var count4 = 0;
            var countElse = 0;

            foreach (Move[] moveList in moveListList)
                foreach (Move element in moveList) 
                {
                    if (moveList.Length == 1)
                        count1++;

                    if (moveList.Length == 2)
                        count2++;

                    if (moveList.Length == 3)
                        count3++;

                    if (moveList.Length == 4)
                        count4++;

                    if (moveList.Length > 4)
                        countElse++;
                }

            Console.WriteLine("Nombre de moves : unique : "+ count1 + " double : " + count2 + " triple : " + count3 + " quad : " + count4 + " et plus : " + countElse);
        }
    }
}
