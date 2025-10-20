using System;

namespace Core
{
    public static class RandomUtil
    {
        public static float Range(float minInclusive, float maxExclusive)
        {
            var random = new Random();
            return (float)random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
        }
    }
}