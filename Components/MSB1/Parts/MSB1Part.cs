using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

// Stores all the MSB specific fields for a part
public abstract class MSB1Part : MonoBehaviour
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
    public uint DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public uint DispGroup1, DispGroup2, DispGroup3, DispGroup4;

    /// <summary>
    /// Used to identify the part in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte LightID, FogID, ScatterID, LensFlareID;

    public byte ShadowID, DepthOfFieldID, ToneMapID, ToneCorrectID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte LanternID, LodParamID;

    public byte IsShadowSrc;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool IsShadowDest;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool IsShadowOnly, DrawByReflectCam, DrawOnlyReflectCam, UseDepthBiasFloat;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool DisablePointLightEffect;

    public void setBasePart(MSB1.Part part)
    {
        Placeholder = part.Placeholder;
        ModelName = part.ModelName;
        DrawGroup1 = part.DrawGroups[0];
        DrawGroup2 = part.DrawGroups[1];
        DrawGroup3 = part.DrawGroups[2];
        DrawGroup4 = part.DrawGroups[3];
        DispGroup1 = part.DispGroups[0];
        DispGroup2 = part.DispGroups[1];
        DispGroup3 = part.DispGroups[2];
        DispGroup4 = part.DispGroups[3];

        Rotation = new UnityEngine.Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);

        EventEntityID = part.EntityID;
        LightID = part.LightID;
        FogID = part.FogID;
        ScatterID = part.ScatterID;
        LensFlareID = part.LensFlareID;
        ShadowID = part.ShadowID;
        DepthOfFieldID = part.DofID;
        ToneMapID = part.ToneMapID;
        ToneCorrectID = part.ToneCorrectID;
        LanternID = part.LanternID;
        LodParamID = part.LodParamID;
        IsShadowSrc = part.IsShadowSrc;
        IsShadowDest = part.IsShadowDest >= 1;
        IsShadowOnly = part.IsShadowOnly >= 1;
        DrawByReflectCam = part.DrawByReflectCam >= 1;
        DrawOnlyReflectCam = part.DrawOnlyReflectCam >= 1;
        UseDepthBiasFloat = part.UseDepthBiasFloat >= 1;
        DisablePointLightEffect = part.DisablePointLightEffect >= 1;
    }

    internal void _Serialize(MSB1.Part part, GameObject parent)
    {
        part.Name = parent.name;
        part.Placeholder = Placeholder;

        var pos = new System.Numerics.Vector3();
        pos.X = parent.transform.position.x;
        pos.Y = parent.transform.position.y;
        pos.Z = parent.transform.position.z;
        part.Position = pos;

        var rot = new System.Numerics.Vector3();
        rot.X = Rotation.x;
        rot.Y = Rotation.y;
        rot.Z = Rotation.z;
        part.Rotation = rot;

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

        part.EntityID = EventEntityID;
        part.LightID = LightID;
        part.FogID = FogID;
        part.ScatterID = ScatterID;
        part.LensFlareID = LensFlareID;
        part.ShadowID = ShadowID;
        part.DofID = DepthOfFieldID;
        part.ToneMapID = ToneMapID;
        part.ToneCorrectID = ToneCorrectID;
        part.LanternID = LanternID;
        part.LodParamID = LodParamID;
        part.IsShadowSrc = IsShadowSrc;
        part.IsShadowDest = (byte)(IsShadowDest ? 1 : 0);
        part.IsShadowOnly = (byte)(IsShadowOnly ? 1 : 0);
        part.DrawByReflectCam = (byte)(DrawByReflectCam ? 1 : 0);
        part.DrawOnlyReflectCam = (byte)(DrawOnlyReflectCam ? 1 : 0);
        part.UseDepthBiasFloat = (byte)(UseDepthBiasFloat ? 1 : 0);
        part.DisablePointLightEffect = (byte)(DisablePointLightEffect ? 1 : 0);
    }

    public abstract void SetPart(MSB1.Part part);
    public abstract MSB1.Part Serialize(GameObject obj);
}
