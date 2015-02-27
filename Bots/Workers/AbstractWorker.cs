using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots.Workers
{
    public abstract class AbstractWorker : IWorker
    {
        public IMap Map { get; protected set; }
        public Owner Turn { get; protected set; }
        public int NodeHash { get; protected set; }
        public event WorkerEndEvent WorkerEnd;

        protected virtual void OnWorkerEnd(WorkerEndEventArgs args)
        {
            if (WorkerEnd != null)
                WorkerEnd(this, args);
        }

        public AbstractWorker(IMap map, Owner turn, int nodeHash)
        {
            Map = map;
            Turn = turn;
            NodeHash = nodeHash;

            var outputMaps = ComputeNode();
            OnWorkerEnd(new WorkerEndEventArgs(outputMaps, NodeHash));
        }

        protected abstract ICollection<IMap> ComputeNode();
    }
}
