using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Kate.Bots.Workers;
using Kate.Commands;
using Kate.IO;
using Kate.Types;

namespace Kate.Bots
{
    public abstract class TurnByTurnBot : AbstractBot
    {
        protected Stopwatch elapsedTime;

        // Tree Structure

        public TurnByTurnBot(SocketClient socket, string name) : base(socket, name) { }
        
        public void OnWorkerEnd(object sender, WorkerEndEventArgs args)
        {
            // bla bla bla
            // Update tree
            // Launch new workers
        }

        public override ICollection<Move> playTurn()
        {
            elapsedTime.Start();

            new DumbWorker(map, Owner.Me, 0 /*nodeHash*/);

            while (elapsedTime.ElapsedMilliseconds < 1000) { }

            elapsedTime.Stop();

            // return best node on the tree
            return new List<Move>();
        }
    }
}
