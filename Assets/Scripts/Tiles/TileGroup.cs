using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tiles
{
    public abstract class TileGroup : SerializedMonoBehaviour
    {
        protected List<Tile> _tiles = new();
    }
}