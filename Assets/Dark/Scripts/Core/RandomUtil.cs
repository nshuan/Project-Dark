using System;
using UnityEngine;
using Random = System.Random;

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
    
    public static Vector2 InsideUnitSpan(Vector2 spanDirection, float spanAngle)
    {
        // Normalize the direction to get a unit vector
        spanDirection = spanDirection.normalized;
        
        // Generate a random angle offset within the span (in radians)
        var randomAngleRad = Range(-spanAngle / 2f, spanAngle / 2f) * Mathf.Deg2Rad;
        
        // Precompute sin and cos of the rotation angle
        var sin = Mathf.Sin(randomAngleRad);
        var cos = Mathf.Cos(randomAngleRad);
        
        // Rotate the normalized direction vector using 2D rotation matrix
        // [cos -sin] [x]   = [x*cos - y*sin]
        // [sin  cos] [y]     [x*sin + y*cos]
        var direction = new Vector2(
            spanDirection.x * cos - spanDirection.y * sin,
            spanDirection.x * sin + spanDirection.y * cos
        );
        
        // Use sqrt of random for uniform distribution inside unit circle
        var magnitude = Mathf.Sqrt(Range(0f, 1f));
        
        return direction * magnitude;
    }

    public static Vector3 InsideUnitSpan(Vector3 spanDirection, float spanAngle)
    {
        return InsideUnitSpan((Vector2)spanDirection, spanAngle);
    }

    public static void InsideUnitSpanSpacedNonAlloc(Vector2 spanDirection, float spanAngle, int count, ref Vector2[] result, float randomness = 0.3f)
    {
        if (result == null) return;
        if (count > result.Length) count = result.Length;
        
        float startAngle = -spanAngle * 0.5f;
        float step = spanAngle / count;

        for (int i = 0; i < count; i++)
        {
            float baseAngle = startAngle + step * (i + 0.5f);
            float offset = Range(-step, step) * randomness * 0.5f;
            float angle = baseAngle + offset;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * spanDirection;
            result[i] = dir.normalized;
        }
        
        // 2. Shuffle (break left→right feeling)
        for (int i = count - 1; i > 0; i--)
        {
            int j = Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }
    }
    
    // Shouldn't spam
    public static int RangeWithOwnRate(params float[] rates)
    {
        if (rates == null || rates.Length == 0)
            return 0;
        
        // Calculate total sum of all rates
        float total = 0f;
        foreach (var rate in rates)
        {
            total += rate;
        }
        
        // If total is zero or negative, return 0
        if (total <= 0f)
            return 0;
        
        // Generate a random number between 0 and total
        var random = new Random();
        float randomValue = (float)random.NextDouble() * total;
        
        // Find the index where accumulated rate exceeds random value
        float accumulated = 0f;
        for (int i = 0; i < rates.Length; i++)
        {
            accumulated += rates[i];
            if (randomValue < accumulated)
                return i;
        }
        
        // Fallback: return last index (shouldn't happen, but safety check)
        return rates.Length - 1;
    }

    public static int[] ShuffleIndex(int start, int end)
    {
        if (end < start)
            return Array.Empty<int>();

        if (end == start)
            return new int[] { start };
        
        int length = end - start + 1;
        int[] indices = new int[length];
        
        // Initialize array with sequential indices
        for (int i = 0; i < length; i++)
        {
            indices[i] = start + i;
        }
        
        // Fisher-Yates shuffle algorithm
        var random = new Random();
        for (int i = length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        
        return indices;
    }
    
    public static void ShuffleIndexNonAlloc(int[] buffer, int start)
    {
        if (buffer == null || buffer.Length < 1)
        {
            return;
        }

        int length = buffer.Length;

        // Initialize buffer with sequential indices
        for (int i = 0; i < length; i++)
        {
            buffer[i] = start + i;
        }

        // Fisher-Yates shuffle algorithm
        var random = new Random();
        for (int i = length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }
    }
    
    public static T[] Shuffle<T>(ReadOnlySpan<T> span)
    {
        if (span.IsEmpty)
            return Array.Empty<T>();
        
        // Create a copy of the span as an array
        T[] result = new T[span.Length];
        span.CopyTo(result);
        
        // Fisher-Yates shuffle algorithm
        var random = new Random();
        for (int i = result.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }
        
        return result;
    }
}