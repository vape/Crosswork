using UnityEngine;

namespace Crosswork.View.Tiling
{
    [CreateAssetMenu(fileName = "Tiling Grid Config", menuName = "Crosswork/View/Tiling Grid Config")]
    public class TilingGridConfig : ScriptableObject
    {
        [SerializeField]
        private Sprite[] sprites;

        public Sprite this[int x, int y]
        {
            get
            {
                return sprites[((7 - y) * 8) + x];
            }
        }
    }
}
