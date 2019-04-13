using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoulsFormats;

public abstract class MSBSRegion : MonoBehaviour
{

    /// <summary>
    /// Workaround field until I figure out how DS3 rotations actually work :trashcat:
    /// </summary>
    public UnityEngine.Vector3 Rotation;

    /// <summary>
    /// Not sure if this is exactly a drawgroup, but it's what makes messages not appear in dark Firelink.
    /// </summary>
    public int MapStudioLayer;


    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk2C;
    public short[] UnkA;
    public short[] UnkB;
    public int UnkC00;
    public int UnkC04;


    /// <summary>
    /// Used to disambiguate a point from a sphere
    /// </summary>
    public bool IsPoint = false;

    public void setBaseRegion(MSBS.Region region)
    {
        Rotation = new UnityEngine.Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        MapStudioLayer = region.MapStudioLayer;
        Unk2C = region.Unk2C;
        UnkA = new short[region.UnkA.Count];
        for(int i = 0; i < UnkA.Length; i++)
        {
            UnkA[i] = region.UnkA[i];
        }
        UnkB = new short[region.UnkB.Count];
        for (int i = 0; i < UnkB.Length; i++)
        {
            UnkB[i] = region.UnkB[i];
        }
        UnkC00 = region.UnkC00;
        UnkC04 = region.UnkC04;

        if (region.Shape is MSBS.Shape.Point)
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
    
    internal void _Serialize(MSBS.Region region, GameObject parent)
    {
        region.Name = parent.name;

        region.Position = new System.Numerics.Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
        //region.Rotation.X = parent.transform.eulerAngles.x;
        //region.Rotation.Y = parent.transform.eulerAngles.y;
        //region.Rotation.Z = parent.transform.eulerAngles.z;
        region.Rotation = ConvertEuler(parent.transform.rotation.eulerAngles);

        region.MapStudioLayer = MapStudioLayer;
        region.Unk2C = Unk2C;
        for (int i = 0; i < UnkA.Length; i++)
        {
            region.UnkA.Add(UnkA[i]);
        }
        for (int i = 0; i < UnkB.Length; i++)
        {
            region.UnkB.Add(UnkB[i]);
        }
        region.UnkC00 = UnkC00;
        region.UnkC04 = UnkC04;

        if (parent.GetComponent<SphereCollider>() != null)
        {
            var col = parent.GetComponent<SphereCollider>();
            if (IsPoint)
            {
                region.Shape = new MSBS.Shape.Point();
            }
            else
            {
                MSBS.Shape.Sphere shape = new MSBS.Shape.Sphere();
                shape.Radius = col.radius;
                region.Shape = shape;
            }
        }
        else if (parent.GetComponent<BoxCollider>() != null)
        {
            var col = parent.GetComponent<BoxCollider>();
            MSBS.Shape.Box shape = new MSBS.Shape.Box();
            shape.Width = col.size.x;
            shape.Height = col.size.y;
            shape.Depth = col.size.z;
            region.Shape = shape;
        }
        else if (parent.GetComponent<CapsuleCollider>() != null)
        {
            var col = parent.GetComponent<CapsuleCollider>();
            MSBS.Shape.Cylinder shape = new MSBS.Shape.Cylinder();
            shape.Radius = col.radius;
            shape.Height = col.height;
            region.Shape = shape;
        }
        else if (parent.GetComponent<MSBSCompositeShape>() != null)
        {
            var col = parent.GetComponent<MSBSCompositeShape>();
            MSBS.Shape.Composite shape = col.Serialize();
            region.Shape = shape;
        }
        else
        {
            throw new Exception($@"Region {parent.name} has no shape. Attach a spherical, box, or capsule collider.");
        }
    }
}
