using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

// Enemy Generator param
public class EnemyGeneratorParam : MonoBehaviour
{
    public long ID;

    public uint LocationUnk0C;
    public uint LocationUnk14;
    public uint LocationUnk18;
    public uint LocationUnk1C;

    public void SetFromGeneratorParam(long id, PARAM.Row generator, PARAM.Row regist, PARAM.Row location)
    {
        ID = id;
        LocationUnk0C = (uint)location["Unk0C"].Value;
        LocationUnk14 = (uint)location["Unk14"].Value;
        LocationUnk18 = (uint)location["Unk18"].Value;
        LocationUnk1C = (uint)location["Unk1C"].Value;
    }


    public void Serialize(PARAM.Row generator, PARAM.Row genLocation, GameObject parent)
    {
    }
}
