using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBBBGParamConfig : MonoBehaviour
{
    public int LightSetID;
    public int FogParamID;
    public int Unk08;
    public int EnvMapID;


    public void setStruct(MSBBB.Part.GparamConfig part)
    {
        LightSetID = part.LightSetID;
        FogParamID = part.FogParamID;
        Unk08 = part.Unk08;
        EnvMapID = part.EnvMapID;
    }

    public MSBBB.Part.GparamConfig Serialize()
    {
        MSBBB.Part.GparamConfig part = new MSBBB.Part.GparamConfig();
        part.LightSetID = LightSetID;
        part.FogParamID = FogParamID;
        part.Unk08 = Unk08;
        part.EnvMapID = EnvMapID;
        return part;
    }
}
