using System;
using System.Collections.Generic;
using System.Threading;

using Kate.Commands;
using Kate.IO;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Bots
{
    public class RandomBot : Bot
    {
        public RandomBot(SocketClient socket, string name) : base(socket, name) { }
        
        protected override ICollection<Move> playTurn()
        {
            Random rnd = new Random();

            var possibleMoves = MoveGenerator.GenerateMoves(map, Owner.Me);
            possibleMoves.RemoveAt(possibleMoves.Count - 1); // Last move is empty
            Thread.Sleep(500); // Let us see what's happening on the game

            return possibleMoves[rnd.Next(possibleMoves.Count)];
        }
    }
}
