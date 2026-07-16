using UnityEngine;

public static class MathExtra
{
    public static Vector3 RandomVector3(float max) => new Vector3(Random.Range(-max, max), Random.Range(-max, max), Random.Range(-max, max));

    public static Vector2 RandomVector2(float max) => new Vector2(Random.Range(-max, max), Random.Range(-max, max));

    public static float Rand1(float max) => Random.Range(-max, max);

    public static Vector3 SetVector3(float a) => new Vector3(a, a, a);

    public static Vector2 SetVector2(float val) => new Vector2(val, val);

    public static float FromLerp(float a, float b, float c) => (c - a) / (b - a);

    public static float ClampFromLerp(float a, float b, float c) => Mathf.Clamp01((c - a) / (b - a));

    public static double FromLerpDouble(double a, double b, double c) => (c - a) / (b - a);

    public static Color SetColor(byte r, byte g, byte b, byte a) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    public static Color SetColor(byte r, byte g, byte b) => new Color(r / 255f, g / 255f, b / 255f, 1f);

    public static Color FillColor(byte c) => new Color(c / 255f, c / 255f, c / 255f, 0f);

    public static Vector2 FromLerpVector2(Vector2 a, Vector2 b, Vector2 c) => new Vector2(FromLerp(a.x, b.x, c.x), FromLerp(a.y, b.y, c.y));

    public static float DistanceFloat(float a, float b) => Mathf.Abs(a - b);

    public static Vector3 SmoothSteepVector3(Vector3 a, Vector3 b, float t) => new Vector3(Mathf.SmoothStep(a.x, b.x, t), Mathf.SmoothStep(a.y, b.y, t), Mathf.SmoothStep(a.z, b.z, t));

    public static Vector3 ColorToVector3(Color a) => new Vector3(a.r, a.g, a.b);

    public static Color LerpColor(Color a, Color b, float t) => new Color(Mathf.Lerp(a.r, b.r, t), Mathf.Lerp(a.g, b.g, t), Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));

    public static Color Vector3ToColor(Vector3 a) => new Color(a.x, a.y, a.z);

    public static Vector2 ToPlaneVector2(Vector3 a) => new Vector2(a.x, a.z);

    public static float ToPlaneDistanceVector3(Vector3 a, Vector3 b) => Vector2.Distance(ToPlaneVector2(a), ToPlaneVector2(b));

    public static Vector3 Y0(Vector3 a) => new Vector3(a.x, 0f, a.z);

    public static Vector2 RotateVectorRight(Vector2 a) => new Vector2(a.y, -a.x);

    public static Vector2 RotateVectorLeft(Vector2 a) => new Vector2(-a.y, a.x);

    public static bool Vector3AxisIsInfinity(Vector3 a) => float.IsInfinity(a.x) || float.IsInfinity(a.y) || float.IsInfinity(a.z);

    public static float R2(float min, float max) => Random.Range(min, max);

    public static int IR2(int min, int max) => Random.Range(min, max);

    public static float SinLerp(float a, float b, float t)
    {
        float funcT = Mathf.Sin((t + 2) * Mathf.PI);
        return Mathf.Lerp(a, b, funcT);
    }

    public static float SmoothSinLerp(float a, float b, float t)
    {
        float funcT = (Mathf.Sin((t - 0.25f) * 2 * Mathf.PI) + 1) / 2f;
        return Mathf.Lerp(a, b, funcT);
    }

    public static Vector3 ExclusionRotationTo(Vector3 from, Vector3 to)
    {
        Vector3 delta = from - to;

        if (delta.x > 180f)
            to.x += 360f;
        else if (delta.x < -180f)
            to.x -= 360f;

        if (delta.y > 180f)
            to.y += 360f;
        else if (delta.y < -180f)
            to.y -= 360f;

        if (delta.z > 180f)
            to.z += 360f;
        else if (delta.z < -180f)
            to.z -= 360f;

        return to;
    }

    public static Vector2 RotationLimitXY(Vector2 rot, Vector2 max, Vector2 min)
    {
        if (rot.x > max.x && rot.x < min.x)
        {
            if (rot.x > 180f)
                rot.x = min.x;
            else
                rot.x = max.x;
        }
        if (rot.y > max.y && rot.y < min.y)
        {
            if (rot.y > 180f)
                rot.y = min.y;
            else
                rot.y = max.y;
        }

        return rot;
    }

    public static float RotationLimitX(float rot, float max, float min)
    {
        if (rot > max && rot < min)
        {
            if (rot > 180f)
                rot = min;
            else
                rot = max;
        }

        return rot;
    }

    public static float DirToAngle(Vector2 dir)
    {
        return Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;
    }

    public static Vector2 AngleToDir(float angle)
    {
        float radians = Mathf.PI / 180 * angle;
        float x = Mathf.Sin(radians);
        float y = Mathf.Cos(radians);

        return new Vector2(x, y);
    }

    public static float AngelBetweenTwoPoints(Vector2 p1, Vector2 p2)
    {
        return DirToAngle(p1 - p2);
    }

    public static bool BoolRandom()
    {
        return Random.Range(0, 10) < 5;
    }

    public static bool BoolRandom(float trueChance) //trueChance - range(0; 1)
    {
        return Random.Range(0f, 1f) <= trueChance;
    }

    public static double DoubleClamp01(double value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 1)
        {
            return 1;
        }

        return value;
    }

    public static string SecondsToTime(float seconds)
    {
        int roundSec = (int)seconds;

        int ms = Mathf.RoundToInt(seconds % 1f * 100f);
        int sec = roundSec % 60;
        int min = ((roundSec - sec) % (60 * 60)) / 60;
        int hour = roundSec / (60 * 60);

        string msStr = ms < 10 ? ("0" + ms) : ms.ToString();
        string secStr = sec < 10 ? ("0" + sec) : sec.ToString();
        string minStr = min < 10 && hour != 0 ? ("0" + min) : min.ToString();
        string hourStr = hour.ToString();

        string res = "";

        if (hour != 0)
        {
            res = res.Insert(res.Length, hourStr + ":");
            res = res.Insert(res.Length, minStr + ":");
        }
        else if (min != 0)
        {
            res = res.Insert(res.Length, minStr + ":");
        }

        res = res.Insert(res.Length, secStr + ".");
        res = res.Insert(res.Length, msStr);

        return res;
    }
}
