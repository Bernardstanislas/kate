using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots.Workers
{
    public interface IWorker
    {
        IMap Map { get; }
        Owner Turn { get; }
        int NodeHash { get; }
        event WorkerEndEvent WorkerEnd;
    }

    public delegate void WorkerEndEvent(object sender, WorkerEndEventArgs e);

    public class WorkerEndEventArgs : EventArgs
    {
        public ICollection<IMap> Maps { get; private set; }
        public long NodeHash { get; private set; }

        public WorkerEndEventArgs(ICollection<IMap> maps, long nodeHash)
        {
            Maps = maps;
            NodeHash = nodeHash;
        }
    }
}
