using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Region")]
public class MSB1Region : MonoBehaviour
{
    public UnityEngine.Vector3 Rotation;

    /// <summary>
    /// The ID of this region.
    /// </summary>
    public int ID;

    /// <summary>
    /// An ID used to identify this region in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Used to disambiguate a point from a sphere
    /// </summary>
    public bool IsPoint = false;

    public void setBaseRegion(MsbRegionBase region)
    {
        ID = region.Index;
        if (region is MsbRegionBox)
        {
            EventEntityID = ((MsbRegionBox)region).EntityID;
        }
        if (region is MsbRegionCylinder)
        {
            EventEntityID = ((MsbRegionCylinder)region).EntityID;
        }
        if (region is MsbRegionPoint)
        {
            EventEntityID = ((MsbRegionPoint)region).EntityID;
            IsPoint = true;
        }
        if (region is MsbRegionSphere)
        {
            EventEntityID = ((MsbRegionSphere)region).EntityID;
        }
        Rotation = new UnityEngine.Vector3(region.RotX, region.RotY, region.RotZ);
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

    public MsbRegionBase Serialize(MsbRegionBase region, GameObject parent)
    {
        region.Name = parent.name;
        region.Index = ID;

        region.PosX = parent.transform.position.x;
        region.PosY = parent.transform.position.y;
        region.PosZ = parent.transform.position.z;
        var rot = ConvertEuler(parent.transform.rotation.eulerAngles);
        //region.RotX = rot.X;
        //region.RotY = rot.Y;
        //region.RotZ = rot.Z;
        region.RotX = Rotation.x;
        region.RotY = Rotation.y;
        region.RotZ = Rotation.z;

        if (region is MsbRegionBox)
        {
            var shape = (MsbRegionBox)region;
            var col = parent.GetComponent<BoxCollider>();
            shape.WidthX = col.size.x;
            shape.HeightY = col.size.y;
            shape.DepthZ = col.size.z;
            shape.EntityID = EventEntityID;
        }
        else if (region is MsbRegionCylinder)
        {
            var shape = (MsbRegionCylinder)region;
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            shape.Height = col.height;
            shape.EntityID = EventEntityID;
        }
        else if (region is MsbRegionSphere)
        {
            var shape = (MsbRegionSphere)region;
            var col = parent.GetComponent<SphereCollider>();
            shape.Radius = col.radius;
            shape.EntityID = EventEntityID;
        }
        else if (region is MsbRegionPoint)
        {
            var shape = (MsbRegionPoint)region;
            shape.EntityID = EventEntityID;
        }
        return region;
    }
}
