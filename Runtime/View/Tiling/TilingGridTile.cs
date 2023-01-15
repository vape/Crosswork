using UnityEngine;
using UnityEngine.Tilemaps;

namespace Crosswork.View.Tiling
{
    public class TilingGridTile : TileBase
    {
        public Sprite sprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
        }
    }
}
