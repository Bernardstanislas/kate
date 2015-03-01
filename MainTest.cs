using System;
using Kate.Types;
using Kate.Bots;
using Kate.IO;
using Kate.Utils;
using Kate.Maps;
using System.Linq;
using Kate.Commands;
using System.Collections.Generic;

namespace Kate
{
    class MainTest
    {
        public static void TestMain()
        {
            Console.WriteLine("KATE is starting...");
			var map = new Map(5,5);
			map.setTile (new Tile (1, 2, Kate.Types.Owner.Me, 5));
			map.setTile (new Tile (3, 2, Kate.Types.Owner.Me, 3));
			map.setTile (new Tile (2, 3, Kate.Types.Owner.Me, 2));
            map.setTile (new Tile (1, 1, Kate.Types.Owner.Me, 15));
            map.setTile (new Tile (2, 2, Kate.Types.Owner.Me, 8));
           
            var moves  = MoveGenerator.GenerateMoves(map, Kate.Types.Owner.Me );

            //MoveGenerator.PrintMove (AllMoves);
            MoveGenerator.PrintListStats (moves);
        }
    }
}
