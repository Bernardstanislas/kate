using System;
using Kate.Commands;
using System.Collections.Generic;

namespace Kate.IO
{
	public interface IClient
	{
		event MapSetEventHandler MapSet;
		event MapInitEventHandler MapInit;
		event MapUpdateEventHandler MapUpdate;
		event EventHandler GameEnd;

		void open();
		void declareName(DeclareName declareName);
		void executeMoves(ICollection<Move> moves);
		void close();
	}
}

