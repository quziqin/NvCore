using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public struct IntersectResult2D
{
    public Vector2  pos;
    public Vector2  normal;
    public float    depth;
};

public class SweepResult2D
{
    public bool hit;
    public Vector2 pos;
    public Vector2 normal;
    public float time;
    public Circle2D c;
    public void Clear()
    {
        hit = false;
        pos = Vector2.zero;
        normal = Vector2.zero;
        time = float.PositiveInfinity;
        c = null;
    }
};

public enum TimeRegion
{
    Utc,
    American
}

public class Circle2D
{
    public Vector2 Center;
    public float R;
    public float SlideFactor;

    public Circle2D()
    {
        SlideFactor = 1.0f;
    }
    public bool Overlap(Circle2D c)
    {
        return (c.Center - Center).SqrMagnitude() < ((c.R + R) * (c.R + R));

    }

    public bool IntersectLine (Vector2 RayPoint , Vector2 RayDir,  IntersectResult2D[] result)
    {
        bool hit;            /* True if ray intersects sphere*/
        float bsq, u, disc;
        float root;
        float inp  , outp;

        Vector2 d = RayPoint - Center;
        bsq = Vector2.Dot( d,RayDir);
        u = d.sqrMagnitude - R * R;
        disc = bsq * bsq - u;
        hit = (disc >= 0.0);
        if (hit)
        {                               /* If ray hits sphere	*/
            root = Mathf.Sqrt(disc);
            inp  = -bsq - root;		    /*    entering distance	*/
            outp = -bsq + root;         /*    leaving distance	*/
            result[0].depth = inp;
            result[0].pos = RayPoint+RayDir* inp;
            result[0].normal = result[0].pos - Center;
            result[0].normal /= R;

            result[1].depth = outp;
            result[1].pos = RayPoint+ RayDir * outp;
            result[1].normal = result[1].pos - Center;
            result[1].normal /= R;
        }

        if (hit) return true;
        else return false;
    }

    public bool IntersectLine2(Vector2 RayPoint, Vector2 RayDir, IntersectResult2D[] result)
    {
        float projection;
        float root;
        float inp, outp;

        Vector2 d =  Center- RayPoint;
        projection = Vector2.Dot(d, RayDir);
        float dist_sq=d.sqrMagnitude - projection * projection;
        if (dist_sq<=R*R)
        {                               /* If ray hits sphere	*/
            root = Mathf.Sqrt(R*R-dist_sq);
            inp = projection - root;          /*    entering distance    */
            outp = projection + root;         /*    leaving distance     */
            result[0].depth = inp;
            result[0].pos = RayPoint + RayDir * inp;
            result[0].normal = result[0].pos - Center;
            result[0].normal /= R;

            result[1].depth = outp;
            result[1].pos = RayPoint + RayDir * outp;
            result[1].normal = result[1].pos - Center;
            result[1].normal /= R;
            return true; 
        }
        return false;
    }

    /// Calculate the time of closest approach of two moving circles.  Also determine if the circles collide.
    /// 
    /// Input:
    /// Pa - Position of circle A.
    /// Pb - Position of circle B.
    /// Va - Velocity of circle A.
    /// Vb - Velocity of circle B.
    /// Ra - Radius of circle A.
    /// Rb - Radius of circle B.
    /// 
    /// Returns:
    /// collision - Returns True if a collision occured, else False.
    /// The method returns the time to impact if collision=true, else it returns the time of closest approach.
    /// 
    /// Notes:
    /// This algorithm will work in any dimension.  Simply change the Vector2's to Vector3's to make this work
    /// for spheres.  You can also set the radii to 0 to work with points/rays.
    /// 
    public float TimeOfClosestApproach(Vector2 Pa, Vector2 Pb, Vector2 Va, Vector2 Vb, float Ra, float Rb, out bool collision)
    {
        Vector2 Pab = Pa - Pb;
        Vector2 Vab = Va - Vb;
        float a = Vector2.Dot(Vab, Vab);
        float b = 2 * Vector2.Dot(Pab, Vab);
        float c = Vector2.Dot(Pab, Pab) - (Ra + Rb) * (Ra + Rb);

        // The quadratic discriminant.
        float discriminant = b * b - 4 * a * c;

        // Case 1:
        // If the discriminant is negative, then there are no real roots, so there is no collision.  The time of
        // closest approach is then given by the average of the imaginary roots, which is:  t = -b / 2a
        float t;
        if (discriminant < 0)
        {
            t = -b / (2 * a);
            collision = false;
        }
        else
        {
            // Case 2 and 3:
            // If the discriminant is zero, then there is exactly one real root, meaning that the circles just grazed each other.  If the 
            // discriminant is positive, then there are two real roots, meaning that the circles penetrate each other.  In that case, the
            // smallest of the two roots is the initial time of impact.  We handle these two cases identically.
            float t0 = (-b + (float)Mathf.Sqrt(discriminant)) / (2 * a);
            float t1 = (-b - (float)Mathf.Sqrt(discriminant)) / (2 * a);
            t = Mathf.Min(t0, t1);

            // We also have to check if the time to impact is negative.  If it is negative, then that means that the collision
            // occured in the past.  Since we're only concerned about future events, we say that no collision occurs if t < 0.
            if (t < 0)
                collision = false;
            else
                collision = true;
        }

        // Finally, if the time is negative, then set it to zero, because, again, we want this function to respond only to future events.

        return t;
    }
    static Circle2D TestCircle=new Circle2D();
    public bool Sweep2(Vector2 speed, Circle2D c, ref SweepResult2D ret)
    {
        bool r=false;
        float time = TimeOfClosestApproach(Center, c.Center, speed, Vector2.zero, R, c.R,out r);
        if (time < 0 || time > 1)
        {
            return false;
        }
        if (r)
        {

            Circle2D c2 = TestCircle;
            c2.R=this.R;
            c2.Center = this.Center;
            c2.Center += time * speed;
            if (c2.Overlap(c))
            {
                //DebugUtil.Log("sweep error ");
                time = 0;// TimeOfClosestApproach(Center, c.Center, speed, Vector2.zero, R, c.R, out r);
                c2 = this;
            }

            ret.time = time;
            ret.normal = (c2.Center - c.Center).normalized;
            ret.hit = true;
        }
        return r; 
    }


    public bool	Sweep(Vector2  speed, Circle2D c	 , SweepResult2D  ret)
    {
        //create the collision volume for o
        Circle2D b=new Circle2D();
        b.Center = c.Center;
        b.R = R + c.R;
        float v = speed.magnitude;

        IntersectResult2D[] lcr=new IntersectResult2D[2];
        lcr[0] = new IntersectResult2D();
        lcr[1] = new IntersectResult2D();

        Vector2 rayDir = speed.normalized;


        if (b.IntersectLine2(Center, rayDir, lcr))
        {
            float time = Mathf.Min( lcr[0].depth , lcr[1].depth);
            time /= v;
            if (time < 0 || time > 1)
            {
                return false;
            }
            Circle2D c2 = new Circle2D();
            c2.R = this.R;
            c2.Center = this.Center;
            c2.Center += time * speed;
            if (c2.Overlap(c))
            {
                //DebugUtil.Log("sweep error ");
                time = 0;// TimeOfClosestApproach(Center, c.Center, speed, Vector2.zero, R, c.R, out r);
            }

            ret.pos = lcr[0].pos + rayDir * R;
            ret.normal = lcr[0].normal;
            ret.time = time;
            return true;
        }
        return false;
    }

};
/// <summary>
/// segment is s+t*d, t in [0,1]
/// </summary>
public class Segment2D
{
    public Segment2D(Vector2 start,Vector2 end)
    {
        s = start;
        d = end - start;
    }
    public Vector2 s;
    public Vector2 d;
}
public static class Utils
{
    public static DateTime DtStart
    {
        get
        {
            return new DateTime(1970, 1, 1);
        }
    }

    public static DateTime LocalDtStart
    {
        get
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        }
    }

    public static long UtcNow
    {
        get
        {
            return ConvertToTimestamp(DateTime.UtcNow);
        }
    }

    public static T Duplicate<T>(T t)
    {
        object ret = null;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, t);
            ms.Seek(0, SeekOrigin.Begin);
            ret = bf.Deserialize(ms);
            ms.Close();
        }
        return (T)ret;
    }

    public static T CopyComponent<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T[] Concate<T>(T[] x, T[] y)
    {
        if (x == null) throw new ArgumentNullException("x");
        if (y == null) throw new ArgumentNullException("y");
        int oldLen = x.Length;
        Array.Resize<T>(ref x, x.Length + y.Length);
        Array.Copy(y, 0, x, oldLen, y.Length);
        return x;
    }


    public static void Swap<T>(ref T v1, ref T v2)
    {
        T tmp = v1;
        v1 = v2;
        v2 = tmp;
    }

    public static bool IsMeaningful(ref string ss)
    {
        return ss != null && ss != "" && ss != " " && ss != "null" && ss != "#";
    }

    public static bool IsEmpty(ref string ss)
    {
        return ss == "#";
    }

    public static Color32 HexToColor(int HexVal)
    {
        byte R = (byte)((HexVal >> 24) & 0xFF);
        byte G = (byte)((HexVal >> 16) & 0xFF);
        byte B = (byte)((HexVal >> 8) & 0xFF);
        byte A = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, A);
    }

    public static Vector3 To2D(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public static Vector3 To3D(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static List<int> GetIntsByString(string ss)
    {
        List<int> ret = new List<int>();
        if (Utils.IsMeaningful(ref ss))
        {
            string[] sss = ss.Split(':');
            int slen = sss.Length;
            for (int i = 0; i < slen; i++)
                ret.Add(int.Parse(sss[i]));
        }
        return ret;
    }

    public static List<string> ParseStringList(string ss)
    {
        List<string> ret = new List<string>();
        if (Utils.IsMeaningful(ref ss))
        {
            string[] sss = ss.Split(':');
            ret.AddRange(sss);
        }
        return ret;
    }

    public static void GetCDString(StringBuilder sb, int timeLeft)
    {
        int day = timeLeft / 86400;
        int dl = timeLeft % 86400;
        int hour = dl / 3600;
        int hl = dl % 3600;
        int minutes = hl / 60;
        int second = hl % 60;

        sb.Length = 0;
        if (day > 0)
            sb.Append(day.ToString("D2")).Append(":");
        if (hour > 0)
            sb.Append(hour.ToString("D2")).Append(":");
        sb.Append(minutes.ToString("D2")).Append(":").Append(second.ToString("D2"));
    }

    public static IEnumerator WaitForRealSeconds(float delay, System.Action callback = null)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay)
            yield return null;
        callback.Invoke();
    }

    public static IEnumerator WaitForFrame(int frame, System.Action callback = null)
    {
        int count = 0;
        while (count < frame)
        {
            yield return null;
            count++;
        }
        callback.Invoke();
    }

    public static float GetRadianBetween(float rad1, float rad2)
    {
        float twoPi = Mathf.PI * 2;
        return Mathf.Min(Mathf.Abs(rad1 - rad2 + twoPi) % twoPi, Mathf.Abs((rad2 - rad1 + twoPi) % twoPi));
    }
    public static bool OverlapCircles(Circle2D target, List<Circle2D> Circles)
    {
        for (int i = 0; i < Circles.Count; i++)
        {
            Vector3 diff = target.Center - Circles[i].Center;
            if (diff.magnitude < Circles[i].R + target.R)
            {
                return true;
            }
        }
        return false;
    }
    public static bool SweepCircles(Circle2D target, List<Circle2D> Circles, Vector2 speed, ref SweepResult2D result)
    {
        float sweep_time = 1;
        bool hit = false;
        result.hit = false;
        if (speed == Vector2.zero) return false;
        
        for (int i = 0; i < Circles.Count; i++)
        {
            if (target.Sweep2(speed, Circles[i], ref result))
            {
                if (result.time < sweep_time)
                {
                    hit = true;
                    sweep_time = result.time;
                    result.c = Circles[i];
                    result.hit = true;

                }
            }
        }
        //for test

        return hit;
    }

    public static float DistanceToLineSeg(Vector3 v, Vector3 start, Vector3 end)
    {
        float r = 0;
        Vector3 dir = (end - start).normalized;
        float length = (end - start).magnitude;
        if (length == 0)
        {
            return (v - start).magnitude;
        }
        float proj = Vector3.Dot((v - start), dir);
        if (proj > length)
        {
            return (v - end).magnitude;
        }
        if (proj < 0)
        {
            return (v - start).magnitude;
        }
        r = Mathf.Sqrt((v - start).sqrMagnitude - proj * proj);
        return r;
    }

    public static string trim(string inputstr, int maxLenth)
    {
        if (maxLenth > 0 && maxLenth < inputstr.Length)
        {
            inputstr = inputstr.Remove(maxLenth - 3);
            inputstr = inputstr.Insert(maxLenth - 3, "...");
        }
        return inputstr;
    }


    public static DateTime GetDateTime(TimeRegion region)
    {
        if (region == TimeRegion.American)
            return DateTime.UtcNow.AddHours(-5);

        return DateTime.UtcNow;
    }

    public static DateTime ConvertToDatetime(long timestamp)
    {
        return ConvertToDatetime(timestamp.ToString());
    }

    public static DateTime ConvertToDatetime(string timestamp)
    {
        long lTime = long.Parse(timestamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);

        return DtStart.Add(toNow);
    }

    public static long ConvertToTimestamp(DateTime time)
    {
        return (long)(time - DtStart).TotalSeconds;
    }

    public static DateTime LastDayInMonth(int year, int month)
    {
        DateTime resource = new DateTime(year, month, 1);
        return resource.AddMonths(1).AddDays(-1);
    }

    public static float GetTapThreshold()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            return 0;

        float ret = 5.0f;
        return ret * UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold;
    }
    /// <summary>
    /// return 0 means not intersect
    /// return 1 means only have one intersection
    /// return 2 means two segments overlap
    /// </summary>
    /// <param name="s0">segment 0</param>
    /// <param name="s1">segment 1</param>
    /// <param name="result">return value indicate how many valid result exit
    /// function caller should pass result array length at least 2</param>
    /// <returns></returns>
    public static int FindIntersection(Segment2D s0,Segment2D s1,ref Vector2[] result)
    {
        return FindIntersection(s0.s, s0.d, s1.s, s1.d, ref result);
    }
    /// <summary>
    /// segments p0+s*d0 for s in [0,1],p1+t*d1 for t in [0,1]
    /// return 0 means not intersect
    /// return 1 means only have one intersection
    /// return 2 means two segments overlap
    /// </summary>
    /// <param name="p0">start point of segmemnt0</param>
    /// <param name="d0">end point of segment0</param>
    /// <param name="p1">start point of segment1</param>
    /// <param name="d1">end point of segment1</param>
    /// <param name="result">return value indicate how many valid result exit
    /// function caller should pass result array length at least 2</param>
    /// <returns></returns>
    static int FindIntersection(Vector2 p0, Vector2 d0, Vector2 p1, Vector2 d1, ref Vector2[] result)
    {
        Vector2 e = p1 - p0;
        float kross = d0.x * d1.y - d0.y * d1.x;
        float sqrKross = kross * kross;
        float sqrLen0 = d0.x * d0.x + d0.y * d0.y;
        float sqrLen1 = d1.x * d1.x + d1.y * d1.y;

        if ((sqrKross - sqrLen0 * sqrLen1) > 1.0e-3f)
        {
            // line of the segments are not parallel
            float s = (e.x * d1.y - e.y * d1.x) / kross;
            if (s < 0 || s > 1)
            {
                // intersection of lines is not a point on segment p0+s*d0
                return 0;
            }

            float t = (e.x * d0.y - e.y * d0.x) / kross;
            if (t < 0 || t > 1)
            {
                // intersection of lines is not a point on segment p1+t*d1
                return 0;
            }

            // intersection of lines is a point on each segment
            result[0] = p0 + s * d0;
            return 1;
        }	

        // lines of the segments are parallel
        float sqrLenE = e.x * e.x + e.y * e.y;
        kross = e.x * d0.y - e.y * d0.x;
        sqrKross = kross * kross;
        if((sqrKross-sqrLen0*sqrLenE)>1.0e-3f)
        {
            // lines of the segments are different
            return 0;
        }

        // lines of the segments are the same. need to test for overlap of segments
        float s0 = Vector2.Dot(d0, e) / sqrLen0;
        float s1 = s0 + Vector2.Dot(d0, d1) / sqrLen0;
        float[] w= { 0.0f, 1.0f };
        float smin = Mathf.Min(s0, s1);
        float smax = Mathf.Max(s0, s1);
        int imax = FindIntersection(0.0f, 1.0f, smin, smax, ref w);
        for(int i = 0; i < imax;++i)
        {
            result[i] = p0 + w[i] * d0;
        }
        return imax;
    }
    /// <summary>
    /// calculate overlap of two range [u0,u1] and [v0,v1]
    /// u0 < u1,v0<v1.
    /// return 0 if without overlap
    /// return 1 if only one intersection
    /// return 2 if there are overlap
    /// </summary>
    /// <param name="u0">start of range0</param>
    /// <param name="u1">end of range0</param>
    /// <param name="v0">start of range1</param>
    /// <param name="v1">end of range1</param>
    /// <param name="w">w[0] conntains section point if return 1
    /// w[0] and w[1] contain overlap start and end if return 2 </param>
    /// <returns></returns>
    static int FindIntersection(float u0, float u1, float v0, float v1, ref float[] w)
    {
        if (u1 < v0 || u0 > v1)
            return 0;

        if(u1>v0)
        {
            if(u0<v1)
            {
                if (u0 < v0)
                    w[0] = v0;
                else
                    w[0] = u0;
                if (u1 > v1)
                    w[1] = v1;
                else
                    w[1] = u1;
                return 2;
            }
            else
            {
                // u0 == v1
                w[0] = u0;
                return 1;
            }
        }
        else
        {
            // u1 == v0
            w[0] = u1;
            return 1;
        }

    }

    public static int TryParse(string param)
    {
        int id = 0;
        if (!int.TryParse(param, out id))
        {
            //DebugUtil.Log("Param exception. param = " + param);
            return 0;
        }
        return id;
    }

    public static void Log(string msg, Transform trans)
    {
        //DebugUtil.Log("Error: " + msg + ". [" + trans.parent.gameObject.name + "/" + trans.gameObject.name + "]");
    }
}

public class CircularBuffer<T>
{
    private T[] _buffer;
    private int _head;
    private int _tail;

    public CircularBuffer(int capacity)
    {
        _buffer = new T[capacity];
        _head = capacity - 1;
    }

    public int Count { get; private set; }

    public int Capacity
    {
        get { return _buffer.Length; }
    }

    public T Enqueue(T item)
    {
        _head = (_head + 1) % Capacity;
        var overwritten = _buffer[_head];
        _buffer[_head] = item;
        if (Count == Capacity)
            _tail = (_tail + 1) % Capacity;
        else
            ++Count;
        return overwritten;
    }

    public void Clear()
    {
        _head = Capacity - 1;
        _tail = 0;
        Count = 0;
    }

    public T this[int index]
    {
        get
        {
            return _buffer[(_tail + index) % Capacity];
        }
        set
        {
            _buffer[(_tail + index) % Capacity] = value;
        }
    }
}
