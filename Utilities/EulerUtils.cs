using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class EulerUtils
{
    public enum RotSeq
    {
        zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy, xzx
    };

    static Vector3 twoaxisrot(float r11, float r12, float r21, float r31, float r32)
    {
        Vector3 ret = new Vector3();
        ret.x = Mathf.Atan2(r11, r12);
        ret.y = Mathf.Acos(r21);
        ret.z = Mathf.Atan2(r31, r32);
        return ret;
    }

    static Vector3 threeaxisrot(float r11, float r12, float r21, float r31, float r32)
    {
        Vector3 ret = new Vector3();
        ret.x = Mathf.Atan2(r31, r32);
        ret.y = Mathf.Asin(r21);
        ret.z = Mathf.Atan2(r11, r12);
        return ret;
    }

    static Vector3 quaternion2Euler(Quaternion q, RotSeq rotSeq)
    {
        switch (rotSeq)
        {
            case RotSeq.zyx:
                return threeaxisrot(2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    -2 * (q.x * q.z - q.w * q.y),
                    2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


            case RotSeq.zyz:
                return twoaxisrot(2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.z - q.w * q.y));


            case RotSeq.zxy:
                return threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


            case RotSeq.zxz:
                return twoaxisrot(2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.x * q.z - q.w * q.y),
                    2 * (q.y * q.z + q.w * q.x));


            case RotSeq.yxz:
                return threeaxisrot(2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    -2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);

            case RotSeq.yxy:
                return twoaxisrot(2 * (q.x * q.y - q.w * q.z),
                    2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.y * q.z - q.w * q.x));


            case RotSeq.yzx:
                return threeaxisrot(-2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);


            case RotSeq.yzy:
                return twoaxisrot(2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.y + q.w * q.z));


            case RotSeq.xyz:
                return threeaxisrot(-2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


            case RotSeq.xyx:
                return twoaxisrot(2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y - q.w * q.z),
                    2 * (q.x * q.z + q.w * q.y));


            case RotSeq.xzy:
                return threeaxisrot(2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    -2 * (q.x * q.y - q.w * q.z),
                    2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


            case RotSeq.xzx:
                return twoaxisrot(2 * (q.x * q.z - q.w * q.y),
                    2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.x * q.y - q.w * q.z));

            default:
                Debug.LogError("No good sequence");
                return Vector3.zero;

        }
    }

    public static System.Numerics.Vector3 quaternion2EulerDeg(Quaternion q, RotSeq rotSeq)
    {
        Vector3 res = quaternion2Euler(q, rotSeq);
        var result = new System.Numerics.Vector3();
        float test = q.w * q.z + q.x * q.y;
        float unit = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
        switch (rotSeq)
        {
            case RotSeq.zyx:
                result.X = res.x * Mathf.Rad2Deg;
                result.Y = res.y * Mathf.Rad2Deg;
                result.Z = res.z * Mathf.Rad2Deg;
                break;

            case RotSeq.zxy:
                result.X = res.y * Mathf.Rad2Deg;
                result.Y = res.x * Mathf.Rad2Deg;
                result.Z = res.z * Mathf.Rad2Deg;
                break;

            case RotSeq.yxz:
                result.X = res.z * Mathf.Rad2Deg;
                result.Y = res.x * Mathf.Rad2Deg;
                result.Z = res.y * Mathf.Rad2Deg;
                break;

            case RotSeq.yzx:
                result.X = res.x * Mathf.Rad2Deg;
                result.Y = res.z * Mathf.Rad2Deg;
                result.Z = res.y * Mathf.Rad2Deg;
                // Handle poles
                if (test > 0.4995f * unit)
                {
                    result.X = 0.0f;
                    result.Y = 2.0f * Mathf.Atan2(q.y, q.z) * Mathf.Rad2Deg;
                    result.Z = 90.0f;
                }
                if (test < -0.4995f * unit)
                {
                    result.X = 0.0f;
                    result.Y = -2.0f * Mathf.Atan2(q.y, q.z) * Mathf.Rad2Deg;
                    result.Z = -90.0f;
                }
                break;

            case RotSeq.xyz:
                result.X = res.z * Mathf.Rad2Deg;
                result.Y = res.y * Mathf.Rad2Deg;
                result.Z = res.x * Mathf.Rad2Deg;
                break;

            case RotSeq.xzy:
                result.X = res.y * Mathf.Rad2Deg;
                result.Y = res.z * Mathf.Rad2Deg;
                result.Z = res.x * Mathf.Rad2Deg;
                break;

            default:
                Debug.LogError("No good sequence");
                return System.Numerics.Vector3.Zero;
        }
        result.X = (result.X <= -180.0f) ? result.X + 360.0f : result.X;
        result.Y = (result.Y <= -180.0f) ? result.Y + 360.0f : result.Y;
        result.Z = (result.Z <= -180.0f) ? result.Z + 360.0f : result.Z;
        return result;
    }
}