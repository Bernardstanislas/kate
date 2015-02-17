using System;

using Kate.Maps;

namespace Kate.Commands
{
    public class Move : ICommand
    {
        public Tile Origin { get; private set; }
        public Tile Dest { get; private set; }
        public int PopToMove { get; private set; }

        public Move (Tile origin, Tile dest, int popToMove)
        {
            Origin = origin;
            Dest = dest;
            PopToMove = popToMove;
        }
    }
}

