using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Collision")]
public class MSBSCollisionPart : MSBSPart
{
    public MSBSUnkStruct1Part Unk1;
    public MSBSUnkStruct2Part Unk2;
    public MSBSUnkStruct5Part Unk5;

    public byte HitFilterID;
    public byte SoundSpaceType;
    public float ReflectPlaneHeight;
    public short MapNameID;
    public bool DisableStart;
    public byte UnkT17;
    public int DisableBonfireEntityID;
    public byte UnkT24;
    public byte UnkT25;
    public byte UnkT26;
    public byte MapVisibility;
    public int PlayRegionID;
    public short LockCamParamID;
    public int UnkT3C;
    public int UnkT40;
    public float UnkT44;
    public float UnkT48;
    public int UnkT4C;
    public float UnkT50;
    public float UnkT54;
    public int Unk3C;
    public float Unk40;

    public void SetPart(MSBS.Part.Collision part)
    {
        setBasePart(part);
        Unk1 = gameObject.AddComponent<MSBSUnkStruct1Part>();
        Unk1.setStruct(part.Unk1);
        Unk2 = gameObject.AddComponent<MSBSUnkStruct2Part>();
        Unk2.setStruct(part.Unk2);
        Unk5 = gameObject.AddComponent<MSBSUnkStruct5Part>();
        Unk5.setStruct(part.Unk5);
        HitFilterID = part.HitFilterID;
        SoundSpaceType = part.SoundSpaceType;
        ReflectPlaneHeight = part.ReflectPlaneHeight;
        MapNameID = part.MapNameID;
        DisableStart = part.DisableStart;
        UnkT17 = part.UnkT17;
        DisableBonfireEntityID = part.DisableBonfireEntityID;
        UnkT24 = part.UnkT24;
        UnkT25 = part.UnkT25;
        UnkT26 = part.UnkT26;
        MapVisibility = part.MapVisibility;
        PlayRegionID = part.PlayRegionID;
        LockCamParamID = part.LockCamParamID;
        UnkT3C = part.UnkT3C;
        UnkT40 = part.UnkT40;
        UnkT44 = part.UnkT44;
        UnkT48 = part.UnkT48;
        UnkT4C = part.UnkT4C;
        UnkT50 = part.UnkT50;
        UnkT54 = part.UnkT54;
        Unk3C = part.Unk6.Unk3C;
        Unk40 = part.Unk6.Unk40;
    }

    public MSBS.Part.Collision Serialize(GameObject parent)
    {
        var part = new MSBS.Part.Collision();
        _Serialize(part, parent);
        part.Unk1 = Unk1.Serialize();
        part.Unk2 = Unk2.Serialize();
        part.Unk5 = Unk5.Serialize();
        part.Unk6 = new MSBS.Part.UnkStruct6();
        part.Unk6.Unk3C = Unk3C;
        part.Unk6.Unk40 = Unk40;
        part.HitFilterID = HitFilterID;
        part.SoundSpaceType = SoundSpaceType;
        part.ReflectPlaneHeight = ReflectPlaneHeight;
        part.MapNameID = MapNameID;
        part.DisableStart = DisableStart;
        part.UnkT17 = UnkT17;
        part.DisableBonfireEntityID = DisableBonfireEntityID;
        part.UnkT24 = UnkT24;
        part.UnkT25 = UnkT25;
        part.UnkT26 = UnkT26;
        part.MapVisibility = MapVisibility;
        part.PlayRegionID = PlayRegionID;
        part.LockCamParamID = LockCamParamID;
        part.UnkT3C = UnkT3C;
        part.UnkT40 = UnkT40;
        part.UnkT44 = UnkT44;
        part.UnkT48 = UnkT48;
        part.UnkT4C = UnkT4C;
        part.UnkT50 = UnkT50;
        part.UnkT54 = UnkT54;
        return part;
    }
}
