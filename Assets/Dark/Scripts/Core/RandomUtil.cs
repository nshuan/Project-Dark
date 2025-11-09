using System;

public static class RandomUtil
{
    public static float Range(float minInclusive, float maxExclusive)
    {
        var random = new Random();
        return (float)random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        var random = new Random();
        return random.Next(minInclusive, maxExclusive);
    }
}