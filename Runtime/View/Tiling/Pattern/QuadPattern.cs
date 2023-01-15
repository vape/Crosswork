namespace Crosswork.View.Tiling.Pattern
{
    internal enum QuadPatternMatchingRule
    {
        Irrelevant = 0,
        Active = 1,
        Inactive = 2,
    };

    internal struct QuadPattern
    {
        public readonly int SpriteX;
        public readonly int SpriteY;

        public readonly QuadPatternMatchingRule Self;
        public readonly QuadPatternMatchingRule Adjacent0;
        public readonly QuadPatternMatchingRule Adjacent1;
        public readonly QuadPatternMatchingRule Adjacent2;

        public QuadPattern(
            int spriteX, int spriteY, 
            QuadPatternMatchingRule self, 
            QuadPatternMatchingRule adjacent0, 
            QuadPatternMatchingRule adjacent1, 
            QuadPatternMatchingRule adjacent2)
        {
            SpriteX = spriteX;
            SpriteY = spriteY;

            Self = self;
            Adjacent0 = adjacent0;
            Adjacent1 = adjacent1;
            Adjacent2 = adjacent2;
        }

        public bool Test(Quad tile)
        {
            return 
                Test(Self, tile.Self) &&
                Test(Adjacent0, tile.Adjacent0) &&
                Test(Adjacent1, tile.Adjacent1) &&
                Test(Adjacent2, tile.Adjacent2);
        }

        private static bool Test(QuadPatternMatchingRule rule, bool value)
        {
            switch (rule)
            {
                case QuadPatternMatchingRule.Active:
                    return value;
                case QuadPatternMatchingRule.Inactive:
                    return !value;
                default:
                    return true;
            }
        }
    }
}