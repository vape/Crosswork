using Crosswork.View.Tiling.Pattern;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Crosswork.View.Tiling
{
    public class TilingGridController : MonoBehaviour
    {
        [SerializeField]
        private Tilemap tilemap;
        [SerializeField]
        private TilingGridConfig config;

        public void Clear()
        {
            if (tilemap != null)
            {
                tilemap.ClearAllTiles();
            }
        }

        public void Render(Vector2 pivot, Vector2 tileSize, TilingGrid grid)
        {
            var fromX = -1;
            var fromY = -1;
            var toX = grid.Width + 1;
            var toY = grid.Height + 1;

            for (int y = fromY; y < toY; y++)
            {
                for (int x = fromX; x < toX; x++)
                {
                    for (byte i = 0; i < 4; i++)
                    {
                        var quad = new Quad(grid, x, y, (QuadPosition)i);
                        var position = new Vector3Int(quad.TileX, quad.TileY);

                        if (quad.AnyActive && QuadPatterns.TryFindSpritePosition(quad, out var spriteX, out var spriteY))
                        {
                            var tile = tilemap.GetTile<TilingGridTile>(position) ?? ScriptableObject.CreateInstance<TilingGridTile>();
                            tile.sprite = config[spriteX, spriteY];
                            tilemap.SetTile(position, tile);
                        }
                        else
                        {
                            tilemap.SetTile(position, null);
                        }
                    }
                }
            }

            transform.localScale = new Vector2(0.5f * tileSize.x, 0.5f * tileSize.y);
            transform.localPosition = new Vector2((toX - fromX), (toY - fromY)) * tileSize * pivot * -1;
        }
    }
}
