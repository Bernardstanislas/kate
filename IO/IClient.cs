using System;
using System.Collections.Generic;

using Kate.Commands;

namespace Kate.IO
{
	public interface IClient
	{
		event MapSetEventHandler MapSet;
		event MapInitEventHandler MapInit;
		event MapUpdateEventHandler MapUpdate;
        event EventHandler GameEnd;
		event EventHandler GameDisconnection;

		void open();
		void declareName(DeclareName declareName);
		void executeMoves(ICollection<Move> moves);
		void close();
	}
}

