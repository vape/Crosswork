namespace Crosswork.View.Tiling.Pattern
{
    using Rule = QuadPatternMatchingRule;

    internal static class QuadPatterns
    {
        private const Rule any = Rule.Irrelevant;
        private const Rule yes = Rule.Active;
        private const Rule not = Rule.Inactive;

        private static QuadPattern[] leftBottom;
        private static QuadPattern[] leftTop;
        private static QuadPattern[] rightBottom;
        private static QuadPattern[] rightTop;

        static QuadPatterns()
        {
            leftBottom = GenerateLeftBottomPatterns();
            leftTop = GenerateLeftTopPatterns();
            rightBottom = GenerateRightBottomPatterns();
            rightTop = GenerateRightTopPatterns();
        }

        public static bool TryFindSpritePosition(Quad quad, out int spriteX, out int spriteY)
        {
            switch (quad.Position)
            {
                case QuadPosition.LeftBottom:
                    return TryFindSpritePosition(ref leftBottom, quad, out spriteX, out spriteY);
                case QuadPosition.LeftTop:
                    return TryFindSpritePosition(ref leftTop, quad, out spriteX, out spriteY);
                case QuadPosition.RightBottom:
                    return TryFindSpritePosition(ref rightBottom, quad, out spriteX, out spriteY);
                case QuadPosition.RightTop:
                    return TryFindSpritePosition(ref rightTop, quad, out spriteX, out spriteY);
            }

            spriteX = default;
            spriteY = default;
            return false;
        }

        private static bool TryFindSpritePosition(ref QuadPattern[] patterns, Quad quad, out int spriteX, out int spriteY)
        {
            for (int i = 0; i < patterns.Length; i++)
            {
                if (patterns[i].Test(quad))
                {
                    spriteX = patterns[i].SpriteX;
                    spriteY = patterns[i].SpriteY;
                    return true;
                }
            }

            spriteX = default;
            spriteY = default;
            return false;
        }

        private static QuadPattern[] GenerateLeftBottomPatterns()
        {
            var p = new QuadPattern[17];

            p[0] = new QuadPattern(1, 1, yes, not, not, not);
            p[1] = new QuadPattern(3, 1, yes, not, not, yes);
            p[2] = new QuadPattern(5, 1, yes, not, not, yes);
            p[3] = new QuadPattern(7, 1, not, not, not, yes);

            p[4] = new QuadPattern(1, 3, yes, yes, not, not);
            p[5] = new QuadPattern(3, 3, not, yes, any, yes);
            p[6] = new QuadPattern(5, 3, yes, yes, yes, not);
            p[7] = new QuadPattern(7, 3, not, not, yes, yes);

            p[8] = new QuadPattern(1, 5, yes, yes, not, not);
            p[9] = new QuadPattern(3, 5, yes, not, yes, yes);
            p[10] = new QuadPattern(5, 5, yes, any, yes, any);
            p[11] = new QuadPattern(5, 5, yes, yes, any, yes);
            p[12] = new QuadPattern(7, 5, not, not, yes, yes);

            p[13] = new QuadPattern(1, 7, not, yes, not, not);
            p[14] = new QuadPattern(3, 7, not, yes, yes, not);
            p[15] = new QuadPattern(5, 7, not, yes, yes, not);
            p[16] = new QuadPattern(7, 7, not, not, yes, not);

            return p;
        }

        private static QuadPattern[] GenerateLeftTopPatterns()
        {
            var p = new QuadPattern[17];

            p[0] = new QuadPattern(1, 0, not, not, not, yes);
            p[1] = new QuadPattern(3, 0, not, not, yes, yes);
            p[2] = new QuadPattern(5, 0, not, not, yes, yes);
            p[3] = new QuadPattern(7, 0, not, not, yes, not);

            p[4] = new QuadPattern(1, 2, yes, not, not, yes);
            p[5] = new QuadPattern(3, 2, yes, yes, yes, not);
            p[6] = new QuadPattern(5, 2, yes, any, yes, any);
            p[7] = new QuadPattern(5, 2, yes, yes, any, yes);
            p[8] = new QuadPattern(7, 2, not, yes, yes, not);

            p[9] = new QuadPattern(1, 4, yes, not, not, yes);
            p[10] = new QuadPattern(3, 4, not, yes, any, yes);
            p[11] = new QuadPattern(5, 4, yes, not, yes, yes);
            p[12] = new QuadPattern(7, 4, not, yes, yes, not);

            p[13] = new QuadPattern(1, 6, yes, not, not, not);
            p[14] = new QuadPattern(3, 6, yes, yes, not, not);
            p[15] = new QuadPattern(5, 6, yes, yes, not, not);
            p[16] = new QuadPattern(7, 6, not, yes, not, not);

            return p;
        }

        private static QuadPattern[] GenerateRightTopPatterns()
        {
            var p = new QuadPattern[16];

            p[0] = new QuadPattern(0, 0, not, not, yes, not);
            p[1] = new QuadPattern(2, 0, not, yes, yes, not);
            p[2] = new QuadPattern(4, 0, not, yes, yes, not);
            p[3] = new QuadPattern(6, 0, not, yes, not, not);

            p[4] = new QuadPattern(0, 2, not, not, yes, yes);
            p[5] = new QuadPattern(2, 2, yes, any, yes, any);
            p[6] = new QuadPattern(2, 2, yes, yes, any, yes);
            p[7] = new QuadPattern(4, 2, yes, not, yes, yes);
            p[8] = new QuadPattern(6, 2, yes, yes, not, not);

            p[9] = new QuadPattern(0, 4, not, not, yes, yes);
            p[10] = new QuadPattern(2, 4, yes, yes, yes, not);
            p[11] = new QuadPattern(4, 4, not, yes, any, yes);
            p[12] = new QuadPattern(0, 6, not, not, not, yes);

            p[13] = new QuadPattern(2, 6, yes, not, not, yes);
            p[14] = new QuadPattern(4, 6, yes, not, not, yes);
            p[15] = new QuadPattern(6, 6, yes, not, not, not);

            return p;
        }

        private static QuadPattern[] GenerateRightBottomPatterns()
        {
            var p = new QuadPattern[17];

            p[0] = new QuadPattern(0, 1, not, yes, not, not);
            p[1] = new QuadPattern(2, 1, yes, yes, not, not);
            p[2] = new QuadPattern(4, 1, yes, yes, not, not);
            p[3] = new QuadPattern(6, 1, yes, not, not, not);

            p[4] = new QuadPattern(0, 3, not, yes, yes, not);
            p[5] = new QuadPattern(2, 3, yes, not, yes, yes);
            p[6] = new QuadPattern(4, 3, not, yes, any, yes);
            p[7] = new QuadPattern(6, 3, yes, not, not, yes);

            p[8] = new QuadPattern(0, 5, not, yes, yes, not);
            p[9] = new QuadPattern(2, 5, yes, any, yes, any);
            p[10] = new QuadPattern(2, 5, yes, yes, any, yes);
            p[11] = new QuadPattern(4, 5, yes, yes, yes, not);
            p[12] = new QuadPattern(6, 5, yes, not, not, yes);

            p[13] = new QuadPattern(0, 7, not, not, yes, not);
            p[14]= new QuadPattern(2, 7, not, not, yes, yes);
            p[15] = new QuadPattern(4, 7, not, not, yes, yes);
            p[16] = new QuadPattern(6, 7, not, not, not, yes);

            return p;
        }
    }
}