using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBSGParamConfig : MonoBehaviour
{
    public int EnvMapID;
    public int FogParamID;
    public int LightSetID;
    public int Unk08;


    public void setStruct(MSBS.Part.GparamConfig part)
    {
        EnvMapID = part.EnvMapID;
        FogParamID = part.FogParamID;
        LightSetID = part.LightSetID;
        Unk08 = part.Unk08;
    }

    public MSBS.Part.GparamConfig Serialize()
    {
        MSBS.Part.GparamConfig part = new MSBS.Part.GparamConfig();
        part.EnvMapID = EnvMapID;
        part.FogParamID = FogParamID;
        part.LightSetID = LightSetID;
        part.Unk08 = Unk08;
        return part;
    }
}
