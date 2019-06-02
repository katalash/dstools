using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

// Stores all the MSB specific fields for a part
public abstract class MSBBBPart : MonoBehaviour
{
    /// <summary>
    /// The placeholder model for this part.
    /// </summary>
    public string Placeholder;

    /// <summary>
    /// The name of this part's model.
    /// </summary>
    public string ModelName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4, DrawGroup5, DrawGroup6, DrawGroup7, DrawGroup8;

    public uint DispGroup1, DispGroup2, DispGroup3, DispGroup4, DispGroup5, DispGroup6, DispGroup7, DispGroup8;

    public uint BackreadGroup1, BackreadGroup2, BackreadGroup3, BackreadGroup4, BackreadGroup5, BackreadGroup6, BackreadGroup7, BackreadGroup8;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkFA4;

    /// <summary>
    /// Used to identify the part in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public sbyte OldLightID, OldFogID, OldScatterID, OldLensFlareID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public sbyte OldLanternID, OldLodParamID, UnkB0E;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool OldIsShadowDest;

    public void setBasePart(MSBBB.Part part)
    {
        Placeholder = part.Placeholder;
        ModelName = part.ModelName;
        DrawGroup1 = part.DrawGroups[0];
        DrawGroup2 = part.DrawGroups[1];
        DrawGroup3 = part.DrawGroups[2];
        DrawGroup4 = part.DrawGroups[3];
        DrawGroup5 = part.DrawGroups[4];
        DrawGroup6 = part.DrawGroups[5];
        DrawGroup7 = part.DrawGroups[6];
        DrawGroup8 = part.DrawGroups[7];
        DispGroup1 = part.DispGroups[0];
        DispGroup2 = part.DispGroups[1];
        DispGroup3 = part.DispGroups[2];
        DispGroup4 = part.DispGroups[3];
        DispGroup5 = part.DispGroups[4];
        DispGroup6 = part.DispGroups[5];
        DispGroup7 = part.DispGroups[6];
        DispGroup8 = part.DispGroups[7];
        BackreadGroup1 = part.BackreadGroups[0];
        BackreadGroup2 = part.BackreadGroups[1];
        BackreadGroup3 = part.BackreadGroups[2];
        BackreadGroup4 = part.BackreadGroups[3];
        BackreadGroup5 = part.BackreadGroups[4];
        BackreadGroup6 = part.BackreadGroups[5];
        BackreadGroup7 = part.BackreadGroups[6];
        BackreadGroup8 = part.BackreadGroups[7];
        UnkFA4 = part.UnkFA4;

        EventEntityID = part.EventEntityID;
        OldLightID = part.OldLightID;
        OldFogID = part.OldFogID;
        OldScatterID = part.OldScatterID;
        OldLensFlareID = part.OldLensFlareID;
        OldLanternID = part.OldLanternID;
        OldLodParamID = part.OldLodParamID;
        UnkB0E = part.UnkB0E;
        OldIsShadowDest = part.OldIsShadowDest;
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

    internal void _Serialize(MSBBB.Part part, GameObject parent)
    {
        part.Name = parent.name;
        part.Placeholder = Placeholder;

        part.Position.X = parent.transform.position.x;
        part.Position.Y = parent.transform.position.y;
        part.Position.Z = parent.transform.position.z;
        // :trashcat:
        //part.Rotation.X = parent.transform.rotation.eulerAngles.x;
        //part.Rotation.Y = parent.transform.rotation.eulerAngles.z;
        //part.Rotation.Z = -parent.transform.rotation.eulerAngles.y;
        part.Rotation = ConvertEuler(parent.transform.eulerAngles);
        //print($@"{part.Name}: {parent.transform.eulerAngles}, {parent.transform.localEulerAngles} -> {part.Rotation}");
        part.Scale.X = parent.transform.localScale.x;
        part.Scale.Y = parent.transform.localScale.y;
        part.Scale.Z = parent.transform.localScale.z;

        part.ModelName = ModelName;
        part.DrawGroups[0] = DrawGroup1;
        part.DrawGroups[1] = DrawGroup2;
        part.DrawGroups[2] = DrawGroup3;
        part.DrawGroups[3] = DrawGroup4;
        part.DrawGroups[4] = DrawGroup5;
        part.DrawGroups[5] = DrawGroup6;
        part.DrawGroups[6] = DrawGroup7;
        part.DrawGroups[7] = DrawGroup8;

        part.DispGroups[0] = DispGroup1;
        part.DispGroups[1] = DispGroup2;
        part.DispGroups[2] = DispGroup3;
        part.DispGroups[3] = DispGroup4;
        part.DispGroups[4] = DispGroup5;
        part.DispGroups[5] = DispGroup6;
        part.DispGroups[6] = DispGroup7;
        part.DispGroups[7] = DispGroup8;

        part.BackreadGroups[0] = BackreadGroup1;
        part.BackreadGroups[1] = BackreadGroup2;
        part.BackreadGroups[2] = BackreadGroup3;
        part.BackreadGroups[3] = BackreadGroup4;
        part.BackreadGroups[4] = BackreadGroup5;
        part.BackreadGroups[5] = BackreadGroup6;
        part.BackreadGroups[6] = BackreadGroup7;
        part.BackreadGroups[7] = BackreadGroup8;
        part.UnkFA4 = UnkFA4;

        part.EventEntityID = EventEntityID;
        part.OldLightID = OldLightID;
        part.OldFogID = OldFogID;
        part.OldScatterID = OldScatterID;
        part.OldLensFlareID = OldLensFlareID;
        part.OldLanternID = OldLanternID;
        part.OldLodParamID = OldLodParamID;
        part.UnkB0E = UnkB0E;
        part.OldIsShadowDest = OldIsShadowDest;
    }

    public abstract void SetPart(MSBBB.Part part);
    public abstract MSBBB.Part Serialize(GameObject obj);
}
