using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Utils
{
    public static class MoveUtils
    {
        public static void PrintMove(List<List<Move>> moveListList)
        {
            foreach (List<Move> moveList in moveListList)
            {
                foreach (Move element in moveList)
                    Console.WriteLine ("Or: " + element.Origin.X + element.Origin.Y + " pop :" + element.PopToMove + " Dest: " + element.Dest.X + element.Dest.Y);
                
                Console.WriteLine ("*******************************");
            }
        }

        public static void PrintListStats(List<List<Move>> moveListList)
        {
            var count1 = 0;
            var count2 = 0;
            var count3 = 0;
            var count4 = 0;
            var countElse = 0;

            foreach (List<Move> moveList in moveListList)
                foreach (Move element in moveList) 
                {
                    if (moveList.Count == 1)
                        count1++;

                    if (moveList.Count == 2)
                        count2++;

                    if (moveList.Count == 3)
                        count3++;

                    if (moveList.Count == 4)
                        count4++;

                    if (moveList.Count > 4)
                        countElse++;
                }

            Console.WriteLine("Nombre de moves : unique : "+ count1 + " double : " + count2 + " triple : " + count3 + " quad : " + count4 + " et plus : " + countElse);
        }
    }
}
