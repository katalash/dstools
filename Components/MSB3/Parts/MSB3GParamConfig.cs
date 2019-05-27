using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSB3GParamConfig : MonoBehaviour
{
    public int LightSetID;
    public int FogParamID;
    public int Unk08;
    public int EnvMapID;


    public void setStruct(MSB3.Part.GparamConfig part)
    {
        LightSetID = part.LightSetID;
        FogParamID = part.FogParamID;
        Unk08 = part.Unk08;
        EnvMapID = part.EnvMapID;
    }

    public MSB3.Part.GparamConfig Serialize()
    {
        MSB3.Part.GparamConfig part = new MSB3.Part.GparamConfig();
        part.LightSetID = LightSetID;
        part.FogParamID = FogParamID;
        part.Unk08 = Unk08;
        part.EnvMapID = EnvMapID;
        return part;
    }
}
