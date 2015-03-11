using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;

namespace Kate.IO
{
    class TestClient : AbstractClient
    {
        private int mapWidth;
        private int mapHeight;
        private ICollection<Tile> tiles;

        public TestClient(int mapWidth, int mapHeight, ICollection<Tile> tiles)
        {
            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;
            this.tiles = tiles;
        }

        public override void DeclareName(DeclareName name) 
        {
            onMapSet(new MapSetEventArgs(mapWidth, mapHeight));
            onMapInit(new MapUpdateEventArgs(tiles));
            onMapUpdate(new MapUpdateEventArgs(new List<Tile>()));
        }

        public override void ExecuteMoves(ICollection<Move> moves)
        {
            Console.WriteLine("First series of moves:");
            foreach (var move in moves)
            {
                Console.Write("Origin: ");
                Console.Write(move.Origin.X);
                Console.Write(", ");
                Console.Write(move.Origin.Y);
                Console.Write(" Dest: ");
                Console.Write(move.Dest.X);
                Console.Write(", ");
                Console.Write(move.Dest.Y);
                Console.Write(" Pop moving: ");
                Console.Write(move.PopToMove);
                Console.WriteLine();
            }
        }
    }
}
