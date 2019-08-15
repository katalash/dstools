using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

// Stores all the MSB specific fields for a part
public abstract class MSB2Part : MonoBehaviour
{
    /// <summary>
    /// The name of this part's model.
    /// </summary>
    public string ModelName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint DispGroup1, DispGroup2, DispGroup3, DispGroup4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint NvmGroup1, NvmGroup2, NvmGroup3, NvmGroup4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk64;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk68;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk6C;

    //public UnityEngine.Vector3 Rotation;

    public void setBasePart(MSB2.Part part)
    {
        ModelName = part.ModelName;
        DrawGroup1 = part.DrawGroups[0];
        DrawGroup2 = part.DrawGroups[1];
        DrawGroup3 = part.DrawGroups[2];
        DrawGroup4 = part.DrawGroups[3];
        DispGroup1 = part.DispGroups[0];
        DispGroup2 = part.DispGroups[1];
        DispGroup3 = part.DispGroups[2];
        DispGroup4 = part.DispGroups[3];
        NvmGroup1 = part.Unk44;
        NvmGroup2 = part.Unk48;
        NvmGroup3 = part.Unk4C;
        NvmGroup4 = part.Unk50;
        Unk64 = part.Unk64;
        Unk68 = part.Unk68;
        Unk6C = part.Unk6C;

        //Rotation = new UnityEngine.Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
    }

    static System.Numerics.Vector3 ConvertEuler(UnityEngine.Vector3 r)
    {
        // ZXY Euler to rot matrix

        var x = (r.x > 180.0f ? r.x - 360.0f : r.x) * Mathf.Deg2Rad;
        var y = (r.y > 180.0f ? r.y - 360.0f : r.y) * Mathf.Deg2Rad;
        var z = (r.z > 180.0f ? r.z - 360.0f : r.z) * Mathf.Deg2Rad;

        //System.Numerics.Matrix4x4 mat2 = System.Numerics.Matrix4x4.CreateRotationY(y)
        //    * System.Numerics.Matrix4x4.CreateRotationX(x) * System.Numerics.Matrix4x4.CreateRotationZ(z);
        double M11 = Math.Cos(z) * Math.Cos(y) - Math.Sin(z) * Math.Sin(x) * Math.Sin(y);
        double M13 = Math.Cos(z) * Math.Sin(y) + Math.Cos(y) * Math.Sin(x) * Math.Sin(x);
        double M21 = Math.Cos(y) * Math.Sin(z) + Math.Cos(z) * Math.Sin(x) * Math.Sin(y);
        double M22 = Math.Cos(z) * Math.Cos(x);
        double M23 = Math.Sin(z) * Math.Sin(y) - Math.Cos(z) * Math.Cos(y) * Math.Sin(x);
        double M31 = -Math.Cos(x) * Math.Sin(y);
        double M33 = Math.Cos(x) * Math.Cos(y);

        // YZX
        if (Math.Abs(M21) < 0.99999f)
        {
            /*z = (float)((r.z >= 90.0f && r.z < 270.0f) ? Math.PI + Math.Asin(Math.Max(Math.Min((double)mat2.M21, 1.0), -1.0)) : -Math.Asin(Math.Max(Math.Min((double)mat2.M21, 1.0), -1.0)));
            x = -(float)Math.Atan2(-mat2.M23 / Math.Cos(z), mat2.M22 / Math.Cos(z));
            y = -(float)Math.Atan2(-mat2.M31 / Math.Cos(z), mat2.M11 / Math.Cos(z));*/
            z = (float)Math.Asin(Math.Max(Math.Min(M21, 1.0), -1.0));
            x = (float)Math.Atan2(-M23, M22);
            y = (float)Math.Atan2(-M31, M11);
        }
        else
        {
            if (M21 > 0)
            {
                z = Mathf.PI / 2.0f;
                y = (float)Math.Atan2(M13, M33);
                x = 0.0f;
            }
            else
            {
                z = -Mathf.PI / 2.0f;
                y = (float)Math.Atan2(M13, M33);
                x = 0.0f;
            }
        }

        return new System.Numerics.Vector3(Mathf.Rad2Deg * x, Mathf.Rad2Deg * y, Mathf.Rad2Deg * z);
    }

    internal void _Serialize(MSB2.Part part, GameObject parent)
    {
        part.Name = parent.name;

        var pos = new System.Numerics.Vector3();
        pos.X = parent.transform.position.x;
        pos.Y = parent.transform.position.y;
        pos.Z = parent.transform.position.z;
        part.Position = pos;

        //part.Rotation = ConvertEuler(parent.transform.eulerAngles);
        part.Rotation = EulerUtils.quaternion2EulerDeg(parent.transform.rotation.normalized, EulerUtils.RotSeq.yzx);
        //Debug.Log($@"{part.Name}: {Rotation}, {parent.transform.eulerAngles} -> {part.Rotation}");
        /*if (Mathf.Abs(Rotation.x - part.Rotation.X) > 0.01f ||
            Mathf.Abs(Rotation.y - part.Rotation.Y) > 0.01f ||
            Mathf.Abs(Rotation.z - part.Rotation.Z) > 0.01f)
        {
            Debug.Log($@"{part.Name}: {Rotation}, {parent.transform.eulerAngles} -> {part.Rotation} Q: {parent.transform.rotation.normalized}");
        }*/
        //var rotation = new System.Numerics.Vector3();
        //rotation.X = parent.transform.eulerAngles.x;
        //rotation.Y = parent.transform.eulerAngles.y;
        //rotation.Z = parent.transform.eulerAngles.z;
        //part.Rotation = rotation;

        var scale = new System.Numerics.Vector3();
        scale.X = parent.transform.localScale.x;
        scale.Y = parent.transform.localScale.y;
        scale.Z = parent.transform.localScale.z;
        part.Scale = scale;

        part.ModelName = (ModelName == "") ? null : ModelName;
        part.DrawGroups[0] = DrawGroup1;
        part.DrawGroups[1] = DrawGroup2;
        part.DrawGroups[2] = DrawGroup3;
        part.DrawGroups[3] = DrawGroup4;
        part.DispGroups[0] = DispGroup1;
        part.DispGroups[1] = DispGroup2;
        part.DispGroups[2] = DispGroup3;
        part.DispGroups[3] = DispGroup4;
        part.Unk44 = NvmGroup1;
        part.Unk48 = NvmGroup2;
        part.Unk4C = NvmGroup3;
        part.Unk50 = NvmGroup4;
        part.Unk64 = Unk64;
        part.Unk68 = Unk68;
        part.Unk6C = Unk6C;
    }

    public abstract void SetPart(MSB2.Part part);
    public abstract MSB2.Part Serialize(GameObject obj);
}
