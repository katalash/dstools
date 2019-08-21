using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

// Enemy Generator regist param
public class EnemyGeneratorRegistParam : MonoBehaviour
{
    public long ID;

    public uint EnemyParam;
    public uint Unk04;
    public uint Unk08;
    public ushort Unk0C;
    public byte Unk0E;
    public byte Unk0F;

    public void SetFromGeneratorParam(long id, PARAM.Row regist)
    {
        ID = id;
        EnemyParam = (uint)regist["EnemyParam"].Value;
        Unk04 = (uint)regist["Unk04"].Value;
        Unk08 = (uint)regist["Unk08"].Value;
        Unk0C = (ushort)regist["Unk0C"].Value;
        Unk0E = (byte)regist["Unk0E"].Value;
        Unk0F = (byte)regist["Unk0F"].Value;
    }


    public void Serialize(PARAM.Row regist, GameObject parent)
    {
        regist["EnemyParam"].Value = EnemyParam;
        regist["Unk04"].Value = Unk04;
        regist["Unk08"].Value = Unk08;
        regist["Unk0C"].Value = Unk0C;
        regist["Unk0E"].Value = Unk0E;
        regist["Unk0F"].Value = Unk0F;
    }
}
