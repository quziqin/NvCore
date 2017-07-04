using System;
using UnityEngine;
using System.Collections;

public static class VectorExtended 
{
    private const float Tolerance = 0.01f;

    public static bool AlmostEqual(this float self, float other, float dis = float.Epsilon)
    {
         return Mathf.Abs(self - other) <= dis;
    }

    public static bool AlmostEqual(this Vector3 self, Vector3 other, float dis = float.Epsilon)
    {
        return (self.x.AlmostEqual(other.x, dis) && self.y.AlmostEqual(other.y, dis) && self.z.AlmostEqual(other.z, dis));
    }

    public static bool AlmostEqualXZ(this Vector3 self, Vector3 other, float dis = float.Epsilon)
    {
        return (self.x.AlmostEqual(other.x, dis) && self.z.AlmostEqual(other.z, dis));
    }

    public static int CompareTo(this float x, float y)
    {
        return x.CompareTo(y);
    }

    public static Vector2 Divide(this Vector2 x, Vector2 y)
    {
        return new Vector2(x.x / y.x, x.y / y.y);
    }

    public static Vector2 Multiply(this Vector2 self, Vector2 other)
    {
        return new Vector2(self.x * other.x, self.y * other.y);
    }

    // distance without y affecting
    public static float Distance2D(this Vector3 self, Vector3 other)
    {
        Vector3 disVec = self - other;
        disVec.y = 0.0f;
        return disVec.magnitude;
    }

    // distance without y affecting
    public static Vector3 DistanceVector2D(this Vector3 self, Vector3 other)
    {
        Vector3 disVec = other - self;
        disVec.y = 0.0f;
        return disVec;
    }

    // just for sorting.
    public static int CompareTo(this Vector3 self, Vector3 other)
    {
        if (Math.Abs(self.x - other.x) < Tolerance)
        {
            return Math.Abs(self.y - other.y) < Tolerance ? self.z.CompareTo(other.z) : self.y.CompareTo(other.y);
        }
        else
            return self.x.CompareTo(other.x);
    }
    /// <summary>
    /// return angle between from and to based on left handness
    /// extend engine Vector3.Angle method that only return
    /// absolute between angle value.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float AngleWithSign(this Vector3 from,Vector3 to)
    {
        float ret = 0.0f;
        ret = Vector3.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);
        ret *= Mathf.Sign(cross.y);
        return ret;
    }
}
