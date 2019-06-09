using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Collision")]
public class MSBBBCollisionPart : MSBBBPart
{
    public MSBBBGParamConfig GParamConfig;
    public MSBBBUnkStruct4 UnkStruct4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte HitFilterID;

    /// <summary>
    /// Modifies sounds while the player is touching this collision.
    /// </summary>
    public MSBBB.Part.Collision.SoundSpace SoundSpaceType;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short EnvLightMapSpotIndex;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float ReflectPlaneHeight;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short MapNameID;

    public short UnkT08b;

    /// <summary>
    /// Disables a bonfire with this entity ID when an enemy is touching this collision.
    /// </summary>
    public int DisableBonfireEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int PlayRegionID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short LockCamID1, LockCamID2;

    /// <summary>
    /// Unknown. Always refers to another collision part.
    /// </summary>
    public string UnkHitName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT14;

    public override void SetPart(MSBBB.Part bpart)
    {
        var part = (MSBBB.Part.Collision)bpart;
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSBBBGParamConfig>();
        GParamConfig.setStruct(part.Gparam);
        UnkStruct4 = gameObject.AddComponent<MSBBBUnkStruct4>();
        UnkStruct4.setStruct(part.Unk4);
        HitFilterID = part.HitFilterID;
        SoundSpaceType = part.SoundSpaceType;
        EnvLightMapSpotIndex = part.EnvLightMapSpotIndex;
        ReflectPlaneHeight = part.ReflectPlaneHeight;
        MapNameID = part.MapNameID;
        UnkT08b = part.UnkT08b;
        DisableBonfireEntityID = part.DisableBonfireEntityID;
        PlayRegionID = part.PlayRegionID;
        LockCamID1 = part.LockCamID1;
        LockCamID2 = part.LockCamID2;
        UnkHitName = part.UnkHitName;
        UnkT14 = part.UnkT14;
    }

    public override MSBBB.Part Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Collision(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.Unk4 = UnkStruct4.Serialize();
        part.HitFilterID = HitFilterID;
        part.SoundSpaceType = SoundSpaceType;
        part.EnvLightMapSpotIndex = EnvLightMapSpotIndex;
        part.ReflectPlaneHeight = ReflectPlaneHeight;
        part.MapNameID = MapNameID;
        part.UnkT08b = UnkT08b;
        part.DisableBonfireEntityID = DisableBonfireEntityID;
        part.PlayRegionID = PlayRegionID;
        part.LockCamID1 = LockCamID1;
        part.LockCamID2 = LockCamID2;
        if (UnkHitName == "")
            part.UnkHitName = null;
        else
            part.UnkHitName = UnkHitName;
        part.UnkT14 = UnkT14;
        return part;
    }
}
