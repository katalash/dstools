using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

public abstract class MSB2Region : MonoBehaviour
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public short Unk08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short Unk0E;

    /// <summary>
    /// Used to disambiguate a point from a sphere
    /// </summary>
    public bool IsPointOrCircle = false;

    public void setBaseRegion(MSB2.Region region)
    {
        Unk08 = region.Unk08;
        Unk0E = region.Unk0E;
        if (region.Shape is MSB2.Shape.Point || region.Shape is MSB2.Shape.Circle)
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

    internal void _Serialize(MSB2.Region region, GameObject parent)
    {
        region.Name = parent.name;

        var pos = new System.Numerics.Vector3();
        pos.X = parent.transform.position.x;
        pos.Y = parent.transform.position.y;
        pos.Z = parent.transform.position.z;
        region.Position = pos;

        var rot = ConvertEuler(parent.transform.rotation.eulerAngles);
        region.Rotation = rot;

        region.Unk08 = Unk08;
        region.Unk0E = Unk0E;

        if (parent.GetComponent<SphereCollider>() != null && IsPointOrCircle)
        {
            region.Shape = new MSB2.Shape.Point();
        }
        else if (parent.GetComponent<CapsuleCollider>() != null && IsPointOrCircle)
        {
            var shape = new MSB2.Shape.Circle();
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            region.Shape = shape;
        }
        else if (parent.GetComponent<BoxCollider>() != null)
        {
            var shape = new MSB2.Shape.Box();
            var col = parent.GetComponent<BoxCollider>();
            shape.Width = col.size.x;
            shape.Height = col.size.y;
            shape.Depth = col.size.z;
            region.Shape = shape;
        }
        else if (parent.GetComponent<CapsuleCollider>() != null)
        {
            var shape = new MSB2.Shape.Cylinder();
            var col = parent.GetComponent<CapsuleCollider>();
            shape.Radius = col.radius;
            shape.Height = col.height;
            region.Shape = shape;
        }
        else if (parent.GetComponent<SphereCollider>() != null)
        {
            var shape = new MSB2.Shape.Sphere();
            var col = parent.GetComponent<SphereCollider>();
            shape.Radius = col.radius;
            region.Shape = shape;
        }
        else
        {
            throw new Exception($@"Region {parent.name} has no shape. Attach a spherical, box, or capsule collider.");
        }
    }

    public abstract void SetRegion(MSB2.Region region);
    public abstract MSB2.Region Serialize(GameObject obj);
}
