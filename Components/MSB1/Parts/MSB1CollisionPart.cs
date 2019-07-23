using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Collision")]
public class MSB1CollisionPart : MSB1Part
{
    public byte HitFilterID;
    public byte SoundSpace;
    public short EnvLightMapSpotIndex;
    public float ReflectPlaneHeight;

    public uint NvmGroup1;
    public uint NvmGroup2;
    public uint NvmGroup3;
    public uint NvmGroup4;

    public int VagrantID1;
    public int VagrantID2;
    public int VagrantID3;

    public short MapNameID;
    public bool DisableStart;
    public int DisableBonfireEntityID;

    public int PlayRegionID;

    public short LockCamID1, LockCamID2;

    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.Collision)bpart;
        setBasePart(part);
        HitFilterID = part.HitFilterID;
        SoundSpace = part.SoundSpaceType;
        EnvLightMapSpotIndex = part.EnvLightMapSpotIndex;
        ReflectPlaneHeight = part.ReflectPlaneHeight;

        NvmGroup1 = part.NvmGroups[0];
        NvmGroup2 = part.NvmGroups[1];
        NvmGroup3 = part.NvmGroups[2];
        NvmGroup4 = part.NvmGroups[3];

        VagrantID1 = part.VagrantEntityIDs[0];
        VagrantID2 = part.VagrantEntityIDs[1];
        VagrantID3 = part.VagrantEntityIDs[2];

        MapNameID = part.MapNameID;
        DisableStart = (part.DisableStart > 0);
        DisableBonfireEntityID = part.DisableBonfireEntityID;

        PlayRegionID = part.PlayRegionID;

        LockCamID1 = part.LockCamParamID1;
        LockCamID2 = part.LockCamParamID1;
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.Collision();
        part.HitFilterID = HitFilterID;
        part.SoundSpaceType = SoundSpace;
        part.EnvLightMapSpotIndex = EnvLightMapSpotIndex;
        part.ReflectPlaneHeight = ReflectPlaneHeight;

        _Serialize(part, parent);
        part.NvmGroups[0] = NvmGroup1;
        part.NvmGroups[1] = NvmGroup2;
        part.NvmGroups[2] = NvmGroup3;
        part.NvmGroups[3] = NvmGroup4;

        part.VagrantEntityIDs[0] = VagrantID1;
        part.VagrantEntityIDs[1] = VagrantID2;
        part.VagrantEntityIDs[2] = VagrantID3;

        part.MapNameID = MapNameID;
        part.DisableStart = (short)(DisableStart ? 1 : 0);
        part.DisableBonfireEntityID = DisableBonfireEntityID;

        part.PlayRegionID = PlayRegionID;

        part.LockCamParamID1 = LockCamID1;
        part.LockCamParamID2 = LockCamID2;
        return part;
    }
}
