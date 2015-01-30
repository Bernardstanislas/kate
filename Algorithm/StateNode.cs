using System;
using Models.Map;

namespace Algorithm
{
    public abstract class StateNode
    {
        #region Private fields
        private readonly Map map;
        #endregion

        #region Public attributes
		public IMap Map {get;}
		#endregion

		#region Constructors
		public StateNode(IMap map)
		{
			this.map = map;
		}
		#endregion
    }
}
