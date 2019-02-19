using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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
    public int UnkT2C, UnkT34, UnkT50, UnkT54, UnkT58, UnkT5C, UnkT74;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT78;

    public void SetPart(MSBBB.Part.Collision part)
    {
        setBasePart(part);
        HitFilterID = part.HitFilterID;
        SoundSpaceType = part.SoundSpaceType;
        EnvLightMapSpotIndex = part.EnvLightMapSpotIndex;
        ReflectPlaneHeight = part.ReflectPlaneHeight;
        MapNameID = part.MapNameID;
        //DisableStart = part.DisableStart;
        DisableBonfireEntityID = part.DisableBonfireEntityID;
        PlayRegionID = part.PlayRegionID;
        LockCamID1 = part.LockCamID1;
        LockCamID2 = part.LockCamID2;
        UnkHitName = part.UnkHitName;
        UnkT2C = part.UnkT2C;
        UnkT34 = part.UnkT34;
        UnkT50 = part.UnkT50;
        UnkT54 = part.UnkT54;
        UnkT58 = part.UnkT58;
        UnkT5C = part.UnkT5C;
        UnkT74 = part.UnkT74;
        UnkT78 = part.UnkT78;
    }

    public MSBBB.Part.Collision Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Collision(ID, parent.name);
        _Serialize(part, parent);
        part.HitFilterID = HitFilterID;
        part.SoundSpaceType = SoundSpaceType;
        part.EnvLightMapSpotIndex = EnvLightMapSpotIndex;
        part.ReflectPlaneHeight = ReflectPlaneHeight;
        part.MapNameID = MapNameID;
        //part.DisableStart = DisableStart;
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
        part.UnkT50 = UnkT50;
        part.UnkT54 = UnkT54;
        part.UnkT58 = UnkT58;
        part.UnkT5C = UnkT5C;
        part.UnkT74 = UnkT74;
        part.UnkT78 = UnkT78;
        return part;
    }
}
