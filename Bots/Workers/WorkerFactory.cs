using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots.Workers
{
    public enum Worker
    {
        DefaultWorker
    }

    public static class WorkerFactory
    {
        private static readonly IDictionary<Worker, Func<IMap, Owner, IWorker>> WorkerReference = new Dictionary<Worker, Func<IMap, Owner, IWorker>>()
        {
            {Worker.DefaultWorker, (IMap map, Owner turn) => new DefaultWorker(map, turn)},
        };

        public static IWorker Build(Worker worker, IMap map, Owner turn)
        {
            return WorkerReference[worker](map, turn);
        }
    }
}
