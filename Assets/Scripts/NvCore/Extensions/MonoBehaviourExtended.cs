using UnityEngine;
using System.Collections.Generic;

public static class MonoBehaviourExtension
{
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    public static void SetComponentEnabled<T>(this Transform tr, bool f) where T : MonoBehaviour
    {
        T result = tr.GetComponent<T>();
        if (result != null)
            result.enabled = f;
    }

    public static List<T> GetComponentsRecursively<T>(this Transform tr) where T : Component
    {
        List<T> ret = new List<T>();
        GetComponentsOf<T>(tr, ref ret);
        return ret;
    }

    static void GetComponentsOf<T>(Transform tr, ref List<T> ret) where T : Component
    {
        ret.AddRange(tr.GetComponents<T>());
        for (int i = 0; i < tr.childCount; i++)
        {
            GetComponentsOf<T>(tr.GetChild(i), ref ret);
        }
    }

    public static Transform FindChildRecursively(this Transform tr, string ss)
    {
        Transform ret = null;
        FindChildOf(tr, ss, ref ret);
        return ret;
    }

    static void FindChildOf(Transform tr, string ss, ref Transform ret)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform ci = tr.GetChild(i);
            if (ci.gameObject.name == ss)
            {
                ret = ci;
                return;
            }
            FindChildOf(ci, ss, ref ret);
            if (ret != null)
                return;
        }
    }

    public static float GetRadianTo(this Transform self, Vector3 target)
    {
        Vector3 ldir = self.InverseTransformPoint(target);
        if (ldir.x == 0)
        {
            if (ldir.z >= 0) return 0;
        }

        Vector3 toTarget = target - self.position;
        toTarget.y = 0;
        if (toTarget.magnitude > 1.0e-3)
        {
            float cos = Vector3.Dot(toTarget.normalized, self.forward);
            cos = Mathf.Clamp(cos, -1.0f, 1.0f);
            float arccos = Mathf.Acos(cos);
            if (arccos == float.NaN) arccos = 0;
            if (ldir.x < 0)
                arccos *= -1;
            return arccos;
        }
        else
        // too close to rotate
        {
            return 0.0f;
        }
    }

    public static float GetRadianTo(this Vector3 orig, Vector3 from, Vector3 target)
    {
        Vector3 direction = from - orig;
        Vector3 toTarget = target - orig;
        direction.y = toTarget.y = 0;
        toTarget.Normalize();
        direction.Normalize();
        float cos = Vector3.Dot(toTarget, direction);
        cos = Mathf.Clamp(cos, -1.0f, 1.0f);
        float arccos = Mathf.Acos(cos);
        if (arccos == float.NaN) arccos = 0;
        Vector3 crs = Vector3.Cross(direction, toTarget);
        if (crs.y < 0)
            arccos *= -1;
        return arccos;
    }

    public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, float angleY)
    {
        Vector3 dir = point - pivot;
        dir.y = 0;
        dir = Quaternion.Euler(0, angleY, 0) * dir;
        point = dir + pivot;
        return point;
    }

    /// <summary>
    /// return the shortest distance form the dot to the ray from self to des.
    /// return negative if on left, positive value if on right
    /// </summary>
    /// <param name="self"> the origin position </param>
    /// <param name="des"> the destination of the direction </param>
    /// <param name="pos"> the position that need to calculate the distance </param>
    /// <returns></returns>
    public static float GetDistanceToDir(this Transform self, Vector3 des, Vector3 pos)
    {
        Vector3 dir = des - self.position;
        dir.y = 0;
        dir.Normalize();
        Vector3 div = pos - self.position;
        div.y = 0;
        Vector3 crs = Vector3.Cross(dir, div);
        return crs.magnitude * (crs.y >= 0 ? 1 : -1);
    }

    public static float GetDistanceToDir(this Vector3 self, Vector3 des, Vector3 pos)
    {
        Vector3 dir = (des - self).normalized;
        Vector3 div = pos - self;
        Vector3 crs = Vector3.Cross(dir, div);
        return crs.magnitude;
    }

    public static void LookAtXZ(this Transform tr, Transform t)
    {
        LookAtXZ(tr, t.position);
    }

    public static void LookAtXZ(this Transform tr, Vector3 t)
    {
        Vector3 tp = t;
        tp.y = tr.position.y;
        tr.LookAt(tp);
    }
    /// <summary>
    /// return the normalized value between s and e
    /// auto swap s and e if e less than s
    /// </summary>
    /// <param name="f">to normalized value</param>
    /// <param name="s">start value</param>
    /// <param name="e">end value</param>
    /// <returns>return the normalized value between s and e
    /// return 0 if f less than s
    /// return 1 if f great than e
    /// return 0 if s and e too close
    /// </returns>
    public static float Normalized(this float f, float s, float e)
    {
        float ret = 0;
        // swap if s greater than e
        if (s > e)
        {
            ret = s;
            s = e;
            e = ret;
        }
        ret = 0;
        if (f < s)
        {
            ret = 0;
        }
        else if (f >= e)
        {
            ret = 1.0f;
        }
        else
        {
            if (e - s > 1.0e-4)
            {
                ret = (f - s) / (e - s);
            }
        }
        return ret;
    }
    /// <summary>
    /// return tangent point that come from p if it exist
    /// return c if the distance between p and c less equal than r
    /// </summary>
    /// <param name="p">from point</param>
    /// <param name="c">circle center </param>
    /// <param name="r">circle radius</param>
    /// <param name="CW">the point at CW direction base on point to center direction</param>
    /// <returns></returns>
    public static Vector2 TangentLine(this Vector2 p, Vector2 c, float r, bool CW = true)
    {
        Vector2 ret = c;
        Vector2 pc = c - p;
        float pcL = pc.magnitude;
        if (pcL > r)
        {
            float sinA = r / pcL;
            float cosA = Mathf.Sqrt(1 - sinA * sinA);
            float ptL = pcL * cosA;
            float tx = pc.x;
            float ty = pc.y;
            float sign = CW ? 1.0f : -1.0f;
            ret.x = cosA * tx + sign * sinA * ty;
            ret.y = -sign * sinA * tx + cosA * ty;
            ret = ret.normalized;
            ret = p + ptL * ret;
        }
        return ret;
    }
    /// <summary>
    /// from start to end define a segment 
    /// return true while the nearest point between start and end
    /// else return false 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="nearestpoint"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static bool NearestPointToSegment(this Vector3 point, Vector3 start, Vector3 end, out Vector3 nearestpoint, out float distance,float eps = float.Epsilon)
    //static public bool NearestPointToSegment(this Vector3 point, Vector3 start, Vector3 end, float eps = float.Epsilon,out Vector3 nearestpoint, out float distance)
    {
        bool ret = false;
        Vector3 se = end - start, sp = point - start;
        if (se.sqrMagnitude > eps)
        {
            /// based on triangle area calculation formula
            /// assume Vector se is from end to start 
            /// Vector sp is from point to start
            /// Angle alpha is angle between sp and se
            /// so 
            /// |se|*|sp|*Sin(alpha) = |se|*distance
            /// and
            /// Dot e of sp and se is
            /// |se|*|sp|*Cos(alpha)
            /// and
            /// Cos(alpha)*Cos(alpha)+Sin(alpha)*Sin(alpha) = 1
            /// then
            /// |se|*|se|*|sp|*|sp|*(1-Cos(alpha)*Cos(alpha))=|se|*|se|*distance*distance
            /// |se|*|se|*|sp|*|sp|1-e*e=|se|*|se|*distance*distance
            /// distance*distance = |sp|*|sp|- e* e/|se|*|se|

            float e = Vector3.Dot(sp, se);
            float SqProjPointDistance = e * e / se.sqrMagnitude;
            float SqDistance = sp.sqrMagnitude - SqProjPointDistance;
            distance = Mathf.Sqrt(SqDistance);
            float dir = Mathf.Sign(e);
            float ProjPointDistance = Mathf.Sqrt(SqProjPointDistance);
            nearestpoint = start + dir * se.normalized * ProjPointDistance;
            /// behind start point
            if(dir<0)
            {
                ret = false;
            }
            else
            {
                /// ahead end point
                if(ProjPointDistance>se.magnitude)
                {
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            } 
        }
        else
        {
            nearestpoint = start;
            distance = sp.magnitude;
            ret = true;
        }
        return ret;
    }
    /// <summary>
    /// from start to end define a segment 
    /// return true while the nearest point between start and end
    /// else return false 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="nearestpoint"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static bool NearestPointToSegment(this Vector2 point, Vector2 start, Vector2 end, out Vector2 nearestPoint, out float distance)
    {
        return NearestPointToSegment(point, start, end, out nearestPoint, out distance);
    }
    /// <summary>
    /// from start to end define a segment 
    /// return sign is positive while the nearest point between start and end
    /// else return sign is negative 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static float PointToSegmentDistance(this Vector3 point, Vector3 start, Vector3 end)
    {
        float dis;
        Vector3 np;
        bool inside = point.NearestPointToSegment(start, end, out np, out dis);
        if (!inside)
        {
            dis *= -1.0f;
        }
        return dis;
    }	
}
