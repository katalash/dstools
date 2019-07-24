using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Collision")]
public class MSB3CollisionPart : MSB3Part
{
    public MSB3GParamConfig GParamConfig;
    public MSB3UnkStruct4 UnkStruct4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte HitFilterID;

    /// <summary>
    /// Modifies sounds while the player is touching this collision.
    /// </summary>
    public MSB3.Part.Collision.SoundSpace SoundSpaceType;

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

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool DisableStart;

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
    public int UnkT2C;

    public byte UnkT34, UnkT35, UnkT36;

    public MSB3.Part.Collision.MapVisiblity MapVisType;

    public override void SetPart(MSB3.Part bpart)
    {
        var part = (MSB3.Part.Collision)bpart;
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSB3GParamConfig>();
        GParamConfig.setStruct(part.Gparam);
        UnkStruct4 = gameObject.AddComponent<MSB3UnkStruct4>();
        UnkStruct4.setStruct(part.Unk4);
        HitFilterID = part.HitFilterID;
        SoundSpaceType = part.SoundSpaceType;
        EnvLightMapSpotIndex = part.EnvLightMapSpotIndex;
        ReflectPlaneHeight = part.ReflectPlaneHeight;
        MapNameID = part.MapNameID;
        DisableStart = part.DisableStart;
        DisableBonfireEntityID = part.DisableBonfireEntityID;
        PlayRegionID = part.PlayRegionID;
        LockCamID1 = part.LockCamID1;
        LockCamID2 = part.LockCamID2;
        UnkHitName = part.UnkHitName;
        UnkT2C = part.UnkT2C;
        UnkT34 = part.UnkT34;
        UnkT35 = part.UnkT35;
        UnkT36 = part.UnkT36;
        MapVisType = part.MapVisType;
    }

    public override MSB3.Part Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Collision(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.Unk4 = UnkStruct4.Serialize();
        part.HitFilterID = HitFilterID;
        part.SoundSpaceType = SoundSpaceType;
        part.EnvLightMapSpotIndex = EnvLightMapSpotIndex;
        part.ReflectPlaneHeight = ReflectPlaneHeight;
        part.MapNameID = MapNameID;
        part.DisableStart = DisableStart;
        part.DisableBonfireEntityID = DisableBonfireEntityID;
        part.PlayRegionID = PlayRegionID;
        part.LockCamID1 = LockCamID1;
        part.LockCamID2 = LockCamID2;
        if (UnkHitName == "")
            part.UnkHitName = null;
        else
            part.UnkHitName = UnkHitName;
        part.UnkT2C = UnkT2C;
        part.UnkT34 = UnkT34;
        part.UnkT35 = UnkT35;
        part.UnkT36 = UnkT36;
        part.MapVisType = MapVisType;
        return part;
    }
}
