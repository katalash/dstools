using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Collision")]
public class MSBBBCollisionPart : MSBBBPart
{
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
    public int UnkT14, UnkT18, UnkT1C, UnkT20, UnkT24, UnkT38, UnkT3A, UnkT40, UnkT44, UnkT48, UnkT4C, UnkT70, UnkT74;

    public void SetPart(MSBBB.Part.Collision part)
    {
        setBasePart(part);
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
        UnkT18 = part.UnkT18;
        UnkT1C = part.UnkT1C;
        UnkT20 = part.UnkT20;
        UnkT24 = part.UnkT24;
        UnkT38 = part.UnkT38;
        UnkT3A = part.UnkT3A;
        UnkT40 = part.UnkT40;
        UnkT44 = part.UnkT44;
        UnkT48 = part.UnkT48;
        UnkT4C = part.UnkT4C;
        UnkT70 = part.UnkT70;
        UnkT74 = part.UnkT74;
    }

    public MSBBB.Part.Collision Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Collision(parent.name);
        _Serialize(part, parent);
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
        part.UnkT18 = UnkT18;
        part.UnkT1C = UnkT1C;
        part.UnkT20 = UnkT20;
        part.UnkT24 = UnkT24;
        part.UnkT38 = UnkT38;
        part.UnkT3A = UnkT3A;
        part.UnkT40 = UnkT40;
        part.UnkT44 = UnkT44;
        part.UnkT48 = UnkT48;
        part.UnkT4C = UnkT4C;
        part.UnkT70 = UnkT70;
        part.UnkT74 = UnkT74;
        return part;
    }
}
