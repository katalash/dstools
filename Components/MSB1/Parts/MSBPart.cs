using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

// Stores all the MSB specific fields for a part
public class MSB1Part : MonoBehaviour
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
    public int DrawGroup1, DrawGroup2, DrawGroup3, DrawGroup4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int DispGroup1, DispGroup2, DispGroup3, DispGroup4;

    /// <summary>
    /// Used to identify the part in event scripts.
    /// </summary>
    public int EventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public sbyte LightID, FogID, ScatterID, LensFlareID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public sbyte LanternID, LodParamID;

    public bool IsShadowSrc;

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

    public void setBasePart(MsbPartsBase part)
    {
        Placeholder = part.PlaceholderModel;
        ID = part.Index;
        ModelName = part.ModelName;
        DrawGroup1 = part.DrawGroup1;
        DrawGroup2 = part.DrawGroup2;
        DrawGroup3 = part.DrawGroup3;
        DrawGroup4 = part.DrawGroup4;
        DispGroup1 = part.DispGroup1;
        DispGroup2 = part.DispGroup2;
        DispGroup3 = part.DispGroup3;
        DispGroup4 = part.DispGroup4;

        EventEntityID = part.EntityID;
        LightID = part.LightID;
        FogID = part.FogID;
        ScatterID = part.ScatterID;
        LensFlareID = part.LensFlareID;
        LanternID = part.LanternID;
        LodParamID = part.LodParamID;
        IsShadowSrc = part.IsShadowSrc;
        IsShadowDest = part.IsShadowDest;
        IsShadowOnly = part.IsShadowOnly;
        DrawByReflectCam = part.DrawByReflectCam;
        DrawOnlyReflectCam = part.DrawOnlyReflectCam;
        UseDepthBiasFloat = part.IsUseDepthBiasFloat;
        DisablePointLightEffect = part.DisablePointLightEffect;
    }

    internal void _Serialize(MsbPartsBase part, GameObject parent)
    {
        part.Name = parent.name;
        part.PlaceholderModel = Placeholder;
        part.Index = ID;

        part.PosX = parent.transform.position.x;
        part.PosY = parent.transform.position.y;
        part.PosZ = parent.transform.position.z;
        part.RotX = parent.transform.eulerAngles.x;
        part.RotY = parent.transform.eulerAngles.y;
        part.RotZ = parent.transform.eulerAngles.z;
        part.ScaleX = parent.transform.localScale.x;
        part.ScaleY = parent.transform.localScale.y;
        part.ScaleZ = parent.transform.localScale.z;

        part.ModelName = ModelName;
        part.DrawGroup1 = DrawGroup1;
        part.DrawGroup2 = DrawGroup2;
        part.DrawGroup3 = DrawGroup3;
        part.DrawGroup4 = DrawGroup4;
        part.DispGroup1 = DispGroup1;
        part.DispGroup2 = DispGroup2;
        part.DispGroup3 = DispGroup3;
        part.DispGroup4 = DispGroup4;

        part.EntityID = EventEntityID;
        part.LightID = LightID;
        part.FogID = FogID;
        part.ScatterID = ScatterID;
        part.LensFlareID = LensFlareID;
        part.LanternID = LanternID;
        part.LodParamID = LodParamID;
        part.IsShadowSrc = IsShadowSrc;
        part.IsShadowDest = IsShadowDest;
        part.IsShadowOnly = IsShadowOnly;
        part.DrawByReflectCam = DrawByReflectCam;
        part.DrawOnlyReflectCam = DrawOnlyReflectCam;
        part.IsUseDepthBiasFloat = UseDepthBiasFloat;
        part.DisablePointLightEffect = DisablePointLightEffect;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
