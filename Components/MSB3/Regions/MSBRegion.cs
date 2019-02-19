using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoulsFormats;

public class MSB3Region : MonoBehaviour
{
    /// <summary>
    /// Whether this region has additional type data. The only region type where this actually varies is Sound.
    /// </summary>
    public bool HasTypeData;

    /// <summary>
    /// The ID of this region.
    /// </summary>
    public int ID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk2, Unk3, Unk4;

    /// <summary>
    /// Not sure if this is exactly a drawgroup, but it's what makes messages not appear in dark Firelink.
    /// </summary>
    public uint DrawGroup;

    /// <summary>
    /// Region is inactive unless this part is drawn; null for always active.
    /// </summary>
    public string ActivationPartName;

    /// <summary>
    /// An ID used to identify this region in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Used to disambiguate a point from a sphere
    /// </summary>
    public bool IsPoint = false;

    public void setBaseRegion(MSB3.Region region)
    {
        HasTypeData = region.HasTypeData;
        ID = region.ID;
        Unk2 = region.Unk2;
        Unk3 = region.Unk3;
        Unk4 = region.Unk4;
        DrawGroup = region.DrawGroup;
        ActivationPartName = region.ActivationPartName;
        EventEntityID = region.EventEntityID;

        if (region.Shape is MSB3.Shape.Point)
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

    internal void _Serialize(MSB3.Region region, GameObject parent)
    {
        region.Name = parent.name;
        region.ID = ID;

        region.Position.X = parent.transform.position.x;
        region.Position.Y = parent.transform.position.y;
        region.Position.Z = parent.transform.position.z;
        //region.Rotation.X = parent.transform.eulerAngles.x;
        //region.Rotation.Y = parent.transform.eulerAngles.y;
        //region.Rotation.Z = parent.transform.eulerAngles.z;
        region.Rotation = ConvertEuler(parent.transform.rotation.eulerAngles);

        region.HasTypeData = HasTypeData;
        region.Unk2 = Unk2;
        region.Unk3 = Unk3;
        region.Unk4 = Unk4;
        region.DrawGroup = DrawGroup;
        region.ActivationPartName = (ActivationPartName == "") ? null : ActivationPartName;
        region.EventEntityID = EventEntityID;

        if (parent.GetComponent<SphereCollider>() != null)
        {
            var col = parent.GetComponent<SphereCollider>();
            if (IsPoint)
            {
                region.Shape = new MSB3.Shape.Point();
            }
            else
            {
                region.Shape = new MSB3.Shape.Sphere(col.radius);
            }
        }
        else if (parent.GetComponent<BoxCollider>() != null)
        {
            var col = parent.GetComponent<BoxCollider>();
            region.Shape = new MSB3.Shape.Box(col.size.x, col.size.z, col.size.y);
        }
        else if (parent.GetComponent<CapsuleCollider>() != null)
        {
            var col = parent.GetComponent<CapsuleCollider>();
            region.Shape = new MSB3.Shape.Cylinder(col.radius, col.height);
        }
        else
        {
            throw new Exception($@"Region {parent.name} has no shape. Attach a spherical, box, or capsule collider.");
        }
    }
}
