using UnityEngine;
using System.Collections;

public static class VectorExtended 
{
    static public bool AlmostEqual(this float self, float other, float dis = float.Epsilon)
    {
         return Mathf.Abs(self - other) <= dis;
    }

    static public bool AlmostEqual(this Vector3 self, Vector3 other, float dis = float.Epsilon)
    {
        return (self.x.AlmostEqual(other.x, dis) && self.y.AlmostEqual(other.y, dis) && self.z.AlmostEqual(other.z, dis));
    }

    static public bool AlmostEqualXZ(this Vector3 self, Vector3 other, float dis = float.Epsilon)
    {
        return (self.x.AlmostEqual(other.x, dis) && self.z.AlmostEqual(other.z, dis));
    }

    static public int CompareTo(this float x, float y)
    {
        return x.CompareTo(y);
    }

    static public Vector2 Divide(this Vector2 x, Vector2 y)
    {
        return new Vector2(x.x / y.x, x.y / y.y);
    }

    static public Vector2 Multiply(this Vector2 self, Vector2 other)
    {
        return new Vector2(self.x * other.x, self.y * other.y);
    }

    // distance without y affecting
    static public float Distance2D(this Vector3 self, Vector3 other)
    {
        Vector3 disVec = self - other;
        disVec.y = 0.0f;
        return disVec.magnitude;
    }

    // distance without y affecting
    static public Vector3 DistanceVector2D(this Vector3 self, Vector3 other)
    {
        Vector3 disVec = other - self;
        disVec.y = 0.0f;
        return disVec;
    }

    // just for sorting.
    static public int CompareTo(this Vector3 self, Vector3 other)
    {
        if (self.x == other.x)
        {
            if (self.y == other.y)
                return self.z.CompareTo(other.z);
            else
                return self.y.CompareTo(other.y);
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
    static public float AngleWithSign(this Vector3 from,Vector3 to)
    {
        float ret = 0.0f;
        ret = Vector3.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);
        ret *= Mathf.Sign(cross.y);
        return ret;
    }
}
