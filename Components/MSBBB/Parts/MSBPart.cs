using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

// Stores all the MSB specific fields for a part
public class MSBBBPart : MonoBehaviour
{
    /// <summary>
    /// The placeholder model for this part.
    /// </summary>
    public string Placeholder;

    /// <summary>
    /// The ID of this part, which should be unique but does not appear to be used otherwise.
    /// </summary>
    public int ID;

    /// <summary>
    /// The name of this part's model.
    /// </summary>
    public string ModelName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkF64, UnkF68, UnkF6C, UnkF70, UnkF74, UnkF78, UnkF7C, UnkF80, UnkF84, UnkF88, UnkF8C, UnkF90, UnkF94, UnkF98, UnkF9C, UnkFA0, UnkFA4, UnkFA8;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4, DispGroup1, DispGroup2, DispGroup3, DispGroup4;

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
        ID = part.ID;
        ModelName = part.ModelName;
        DrawGroup1 = part.DrawGroup1;
        DrawGroup2 = part.DrawGroup2;
        DrawGroup3 = part.DrawGroup3;
        DrawGroup4 = part.DrawGroup4;
        DispGroup1 = part.DispGroup1;
        DispGroup2 = part.DispGroup2;
        DispGroup3 = part.DispGroup3;
        DispGroup4 = part.DispGroup4;
        UnkF64 = part.UnkF64;
        UnkF68 = part.UnkF68;
        UnkF6C = part.UnkF6C;
        UnkF70 = part.UnkF70;
        UnkF74 = part.UnkF74;
        UnkF78 = part.UnkF78;
        UnkF7C = part.UnkF7C;
        UnkF80 = part.UnkF80;
        UnkF84 = part.UnkF84;
        UnkF88 = part.UnkF88;
        UnkF8C = part.UnkF8C;
        UnkF90 = part.UnkF90;
        UnkF94 = part.UnkF94;
        UnkF98 = part.UnkF98;
        UnkF9C = part.UnkF9C;
        UnkFA0 = part.UnkFA0;
        UnkFA4 = part.UnkFA4;
        UnkFA8 = part.UnkFA8;

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
        part.ID = ID;

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
        part.UnkF64 = UnkF64;
        part.UnkF68 = UnkF68;
        part.UnkF8C = UnkF8C;
        part.UnkF90 = UnkF90;
        part.UnkF94 = UnkF94;
        part.UnkF98 = UnkF98;
        part.UnkF9C = UnkF9C;
        part.UnkFA0 = UnkFA0;
        part.UnkFA4 = UnkFA4;
        part.UnkFA8 = UnkFA8;

        part.DrawGroup1 = DrawGroup1;
        part.DrawGroup2 = DrawGroup2;
        part.DrawGroup3 = DrawGroup3;
        part.DrawGroup4 = DrawGroup4;
        part.DispGroup1 = DispGroup1;
        part.DispGroup2 = DispGroup2;
        part.DispGroup3 = DispGroup3;
        part.DispGroup4 = DispGroup4;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
