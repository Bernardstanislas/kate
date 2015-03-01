using Kate.Maps;

namespace Kate.Bots.Algorithms
{
    public abstract class StateNode
    {
        private readonly IMap map;
        public IMap Map;

        public StateNode(IMap map)
        {
            this.map = map;
        }
    }
}
