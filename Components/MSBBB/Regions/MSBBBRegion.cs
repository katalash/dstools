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
    public bool IsPoint = false;

    public void setBaseRegion(MSBBB.Region region)
    {
        Unk2 = region.Unk2;
        Unk3 = region.Unk3;
        Unk4 = region.Unk4;
        EventEntityID = region.EventEntityID;
        if (region is MSBBB.Region.Point)
        {
            IsPoint = true;
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

        // XYZ
        if (Mathf.Abs(mat2.M13) < 0.99999f)
        {
            y = ((r.y >= 90.0f && r.y < 270.0f) ? Mathf.PI + Mathf.Asin(Mathf.Clamp(mat2.M13, -1.0f, 1.0f)) : -Mathf.Asin(Mathf.Clamp(mat2.M13, -1.0f, 1.0f)));
            x = Mathf.Atan2(mat2.M23 / Mathf.Cos(y), mat2.M33 / Mathf.Cos(y));
            z = Mathf.Atan2(mat2.M12 / Mathf.Cos(y), mat2.M11 / Mathf.Cos(y));
        }
        else
        {
            if (mat2.M31 > 0)
            {
                y = -Mathf.PI / 2.0f;
                x = Mathf.Atan2(-mat2.M21, -mat2.M31);
                z = 0.0f;
            }
            else
            {
                y = Mathf.PI / 2.0f;
                x = Mathf.Atan2(mat2.M21, mat2.M31);
                z = 0.0f;
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

        if (region is MSBBB.Region.Box)
        {
            var shape = (MSBBB.Region.Box)region;
            var col = parent.GetComponent<BoxCollider>();
            shape.Width = col.size.x;
            shape.Height = col.size.y;
            shape.Length = col.size.z;
        }
        else if (region is MSBBB.Region.Cylinder)
        {
            var shape = (MSBBB.Region.Cylinder)region;
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            shape.Height = col.height;
        }
        else if (region is MSBBB.Region.Sphere)
        {
            var shape = (MSBBB.Region.Sphere)region;
            var col = parent.GetComponent<SphereCollider>();
            shape.Radius = col.radius;
        }
        else if (region is MSBBB.Region.Point)
        {
        }

        return region;
    }
}
