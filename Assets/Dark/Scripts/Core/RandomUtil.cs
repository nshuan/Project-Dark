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
}