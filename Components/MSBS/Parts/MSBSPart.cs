using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

// Stores all the MSB specific fields for a part
public abstract class MSBSPart : MonoBehaviour
{

    /// <summary>
    /// Workaround field until I figure out how DS3 rotations actually work :trashcat:
    /// </summary>
    public UnityEngine.Vector3 Rotation;

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
    public int EntityID;
    public int UnkE18, UnkE3C, UnkE40;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EntityGroupID1;
    public int EntityGroupID2;
    public int EntityGroupID3;
    public int EntityGroupID4;
    public int EntityGroupID5;
    public int EntityGroupID6;
    public int EntityGroupID7;
    public int EntityGroupID8;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte LanternID, LodParamID, EnableOnAboveShadow, IsStaticShadowSrc, IsCascade3ShadowSrc;
    public byte UnkE04, UnkE05, UnkE06, UnkE09, UnkE0B, UnkE0F, UnkE10, UnkE17;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool IsPointLightShadowSrc, IsShadowSrc, IsShadowDest, IsShadowOnly, DrawByReflectCam, DrawOnlyReflectCam, DisablePointLightEffect;


    public void setBasePart(MSBS.Part part)
    {
        Placeholder = part.Placeholder;
        ModelName = part.ModelName;
        Rotation = new UnityEngine.Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
        EntityID = part.EntityID;
        UnkE18 = part.UnkE18;
        UnkE3C = part.UnkE3C;
        UnkE40 = part.UnkE40;
        EntityGroupID1 = part.EntityGroupIDs[0];
        EntityGroupID2 = part.EntityGroupIDs[1];
        EntityGroupID3 = part.EntityGroupIDs[2];
        EntityGroupID4 = part.EntityGroupIDs[3];
        EntityGroupID5 = part.EntityGroupIDs[4];
        EntityGroupID6 = part.EntityGroupIDs[5];
        EntityGroupID7 = part.EntityGroupIDs[6];
        EntityGroupID8 = part.EntityGroupIDs[7];
        LanternID = part.LanternID;
        LodParamID = part.LodParamID;
        EnableOnAboveShadow = part.EnableOnAboveShadow;
        IsStaticShadowSrc = part.IsStaticShadowSrc;
        IsCascade3ShadowSrc = part.IsCascade3ShadowSrc;
        UnkE04 = part.UnkE04;
        UnkE05 = part.UnkE05;
        UnkE06 = part.UnkE06;
        UnkE09 = part.UnkE09;
        UnkE0B = part.UnkE0B;
        UnkE0F = part.UnkE0F;
        UnkE10 = part.UnkE10;
        UnkE17 = part.UnkE17;
        IsPointLightShadowSrc = part.IsPointLightShadowSrc;
        IsShadowSrc = part.IsShadowSrc;
        IsShadowDest = part.IsShadowDest;
        IsShadowOnly = part.IsShadowOnly;
        DrawByReflectCam = part.DrawByReflectCam;
        DrawOnlyReflectCam = part.DrawOnlyReflectCam;
        DisablePointLightEffect = part.DisablePointLightEffect;
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

    internal void _Serialize(MSBS.Part part, GameObject parent)
    {
        part.Name = parent.name;
        part.Placeholder = Placeholder;
        part.ModelName = ModelName;
        part.Position = new System.Numerics.Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
        // :trashcat:
        //part.Rotation.X = parent.transform.rotation.eulerAngles.x;
        //part.Rotation.Y = parent.transform.rotation.eulerAngles.z;
        //part.Rotation.Z = -parent.transform.rotation.eulerAngles.y;
        //part.Rotation = ConvertEuler(parent.transform.eulerAngles);
        part.Rotation = new System.Numerics.Vector3(Rotation.x, Rotation.y, Rotation.z);
        //print($@"{part.Name}: {parent.transform.eulerAngles}, {parent.transform.localEulerAngles} -> {part.Rotation}");
        part.Scale = new System.Numerics.Vector3(parent.transform.localScale.x, parent.transform.localScale.y, parent.transform.localScale.z);

        part.EntityID = EntityID;
        part.UnkE18 = UnkE18;
        part.UnkE3C = UnkE3C;
        part.UnkE40 = UnkE40;
        part.EntityGroupIDs[0] = EntityGroupID1;
        part.EntityGroupIDs[1] = EntityGroupID2;
        part.EntityGroupIDs[2] = EntityGroupID3;
        part.EntityGroupIDs[3] = EntityGroupID4;
        part.EntityGroupIDs[4] = EntityGroupID5;
        part.EntityGroupIDs[5] = EntityGroupID6;
        part.EntityGroupIDs[6] = EntityGroupID7;
        part.EntityGroupIDs[7] = EntityGroupID8;
        part.LanternID = LanternID;
        part.LodParamID = LodParamID;
        part.EnableOnAboveShadow = EnableOnAboveShadow;
        part.IsStaticShadowSrc = IsStaticShadowSrc;
        part.IsCascade3ShadowSrc = IsCascade3ShadowSrc;
        part.UnkE04 = UnkE04;
        part.UnkE05 = UnkE05;
        part.UnkE06 = UnkE06;
        part.UnkE09 = UnkE09;
        part.UnkE0B = UnkE0B;
        part.UnkE0F = UnkE0F;
        part.UnkE10 = UnkE10;
        part.UnkE17 = UnkE17;
        part.IsPointLightShadowSrc = IsPointLightShadowSrc;
        part.IsShadowSrc = IsShadowSrc;
        part.IsShadowDest = IsShadowDest;
        part.IsShadowOnly = IsShadowOnly;
        part.DrawByReflectCam = DrawByReflectCam;
        part.DrawOnlyReflectCam = DrawOnlyReflectCam;
        part.DisablePointLightEffect = DisablePointLightEffect;
    }
}
