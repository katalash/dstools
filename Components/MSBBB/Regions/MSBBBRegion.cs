using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;
using MeowDSIO.DataTypes.MSB;

[AddComponentMenu("Bloodborne/Region")]
public class MSBBBRegion : MonoBehaviour
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk2, Unk3, Unk4;

    /// <summary>
    /// An ID used to identify this region in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Used to disambiguate a point from a sphere
    /// </summary>
    public bool IsPointOrCircle = false;

    public void setBaseRegion(MSBBB.Region region)
    {
        Unk2 = region.Unk2;
        Unk3 = region.Unk3;
        Unk4 = region.Unk4;
        EventEntityID = region.EventEntityID;
        if (region.Shape is MSBBB.Shape.Point || region.Shape is MSBBB.Shape.Circle)
        {
            IsPointOrCircle = true;
        }
    }

    static System.Numerics.Vector3 ConvertEuler(UnityEngine.Vector3 r)
    {
        // ZXY Euler to rot matrix

        var x = (r.x > 180.0f ? r.x - 360.0f : r.x) * Mathf.Deg2Rad;
        var y = (r.y > 180.0f ? r.y - 360.0f : r.y) * Mathf.Deg2Rad;
        var z = (r.z > 180.0f ? r.z - 360.0f : r.z) * Mathf.Deg2Rad;

        System.Numerics.Matrix4x4 mat2 = System.Numerics.Matrix4x4.CreateRotationZ(z)
            * System.Numerics.Matrix4x4.CreateRotationX(x) * System.Numerics.Matrix4x4.CreateRotationY(y);

        // YZX
        if (Mathf.Abs(mat2.M21) < 0.99999f)
        {
            z = (float)((r.z >= 90.0f && r.z < 270.0f) ? Math.PI + Math.Asin(Math.Max(Math.Min((double)mat2.M21, 1.0), -1.0)) : -Math.Asin(Math.Max(Math.Min((double)mat2.M21, 1.0), -1.0)));
            x = (float)Math.Atan2(mat2.M23 / Math.Cos(z), mat2.M22 / Math.Cos(z));
            y = (float)Math.Atan2(mat2.M31 / Math.Cos(z), mat2.M11 / Math.Cos(z));
        }
        else
        {
            if (mat2.M12 > 0)
            {
                z = -Mathf.PI / 2.0f;
                y = (float)Math.Atan2(-mat2.M13, -mat2.M33);
                x = 0.0f;
            }
            else
            {
                z = Mathf.PI / 2.0f;
                y = (float)Math.Atan2(mat2.M13, mat2.M33);
                x = 0.0f;
            }
        }

        return new System.Numerics.Vector3(Mathf.Rad2Deg * x, Mathf.Rad2Deg * y, Mathf.Rad2Deg * z);
    }

    public MSBBB.Region Serialize(MSBBB.Region region, GameObject parent)
    {
        region.Name = parent.name;

        region.Position.X = parent.transform.position.x;
        region.Position.Y = parent.transform.position.y;
        region.Position.Z = parent.transform.position.z;
        var rot = ConvertEuler(parent.transform.rotation.eulerAngles);
        region.Rotation.X = rot.X;
        region.Rotation.Y = rot.Y;
        region.Rotation.Z = rot.Z;

        region.Unk2 = Unk2;
        region.Unk3 = Unk3;
        region.Unk4 = Unk4;
        region.EventEntityID = EventEntityID;

        if (parent.GetComponent<SphereCollider>() != null && IsPointOrCircle)
        {
            region.Shape = new MSBBB.Shape.Point();
        }
        else if (parent.GetComponent<CapsuleCollider>() != null && IsPointOrCircle)
        {
            var shape = new MSBBB.Shape.Circle();
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            region.Shape = shape;
        }
        else if (parent.GetComponent<BoxCollider>() != null)
        {
            var shape = new MSBBB.Shape.Box();
            var col = parent.GetComponent<BoxCollider>();
            shape.Width = col.size.x;
            shape.Height = col.size.y;
            shape.Depth = col.size.z;
            region.Shape = shape;
        }
        else if (parent.GetComponent<CapsuleCollider>() != null)
        {
            var shape = new MSBBB.Shape.Cylinder();
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            shape.Height = col.height;
            region.Shape = shape;
        }
        else if (parent.GetComponent<SphereCollider>() != null)
        {
            var shape = new MSBBB.Shape.Sphere();
            var col = parent.GetComponent<SphereCollider>();
            shape.Radius = col.radius;
            region.Shape = shape;
        }
        return region;
    }
}
