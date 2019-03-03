using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/Collision")]
public class MSB1CollisionPart : MSB1Part
{
    public byte HitFilterID;
    public string EnvLightMapSpot;
    public float ReflectPlaneHeight;

    public int NvmGroup1;
    public int NvmGroup2;
    public int NvmGroup3;
    public int NvmGroup4;

    public int VagrantID1;
    public int VagrantID2;
    public int VagrantID3;

    public short MapNameID;
    public bool DisableStart;
    public int DisableBonfireEntityID;

    public int PlayRegionID;

    public short LockCamID1, LockCamID2;

    public void SetPart(MsbPartsHit part)
    {
        setBasePart(part);
        HitFilterID = part.HitFilterID;
        EnvLightMapSpot = part.EnvLightMapSpot;
        ReflectPlaneHeight = part.ReflectPlaneHeight;

        NvmGroup1 = part.NvmGroup1;
        NvmGroup2 = part.NvmGroup2;
        NvmGroup3 = part.NvmGroup3;
        NvmGroup4 = part.NvmGroup4;

        VagrantID1 = part.VagrantID1;
        VagrantID2 = part.VagrantID2;
        VagrantID3 = part.VagrantID3;

        MapNameID = part.MapNameID;
        DisableStart = (part.DisableStart > 0);
        DisableBonfireEntityID = part.DisableBonfireID;

        PlayRegionID = part.PlayRegionID;

        LockCamID1 = part.LockCamID1;
        LockCamID2 = part.LockCamID2;
    }

    public MsbPartsHit Serialize(GameObject parent)
    {
        var part = new MsbPartsHit();
        part.HitFilterID = HitFilterID;
        part.EnvLightMapSpot = (EnvLightMapSpot == "") ? null : EnvLightMapSpot;
        part.ReflectPlaneHeight = ReflectPlaneHeight;

        _Serialize(part, parent);
        part.NvmGroup1 = NvmGroup1;
        part.NvmGroup2 = NvmGroup2;
        part.NvmGroup3 = NvmGroup3;
        part.NvmGroup4 = NvmGroup4;

        part.VagrantID1 = VagrantID1;
        part.VagrantID2 = VagrantID2;
        part.VagrantID3 = VagrantID3;

        part.MapNameID = MapNameID;
        part.DisableStart = (short)(DisableStart ? 1 : 0);
        part.DisableBonfireID = DisableBonfireEntityID;

        part.PlayRegionID = PlayRegionID;

        part.LockCamID1 = LockCamID1;
        part.LockCamID2 = LockCamID2;
        return part;
    }
}
