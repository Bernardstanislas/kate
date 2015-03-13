using System;
using System.Collections.Generic;

using Kate.Types;
using Kate.Commands;

namespace Kate.Maps
{
    public interface IMap
    {
        IEnumerable<Tile> GetGrid();
        int[] GetMapDimension();
        Tile GetTile (int xCoordinate, int yCoordinate);
        IEnumerable<Tile> GetPlayerTiles(Owner owner);
        void SetTile (Tile newTile);
        void UpdateMap(IMapUpdater mapUpdater);
        IEnumerable<Tile> GetSurroundingTiles(Tile tile);
        bool HasGameEnded();
        List<Move[]> GenerateMovesLists(Owner owner);
    }
}
