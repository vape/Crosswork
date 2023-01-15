using Crosswork.Core;
using UnityEngine;

namespace Crosswork.Tests.Core.Models
{
    public class TestElementModel : IElementModel
    { }

    public class TestElementBigModel : IElementModel
    {
        public int Width;
        public int Height;
    }

    public class TestElement : Element
    {
        private TestElementModel model;

        public TestElement(TestElementModel model) 
            : base(model)
        {
            this.model = model;
        }

        public override ulong GetCollisionMask()
        {
            return 0x1;
        }
    }

    public class TestElementBig : Element
    {
        public static Vector2Int[] CreatePattern(int width, int height)
        {
            var result = new Vector2Int[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; ++x)
                {
                    result[x + (y * width)] = new Vector2Int(x, y);
                }
            }

            return result;
        }

        private Vector2Int[] pattern;

        public TestElementBig(TestElementBigModel model)
            : base(model)
        {
            pattern = CreatePattern(model.Width, model.Height);
        }

        public override ulong GetCollisionMask()
        {
            return 0x1;
        }

        public override Vector2Int[] GetPattern()
        {
            return pattern;
        }
    }
}
