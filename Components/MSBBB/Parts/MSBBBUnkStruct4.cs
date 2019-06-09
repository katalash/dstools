using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBBBUnkStruct4 : MonoBehaviour
{
    public int Unk3C;
    public float Unk40;

    public void setStruct(MSBBB.Part.UnkStruct4 part)
    {
        Unk3C = part.Unk3C;
        Unk40 = part.Unk40;
    }

    public MSBBB.Part.UnkStruct4 Serialize()
    {
        MSBBB.Part.UnkStruct4 part = new MSBBB.Part.UnkStruct4();
        part.Unk3C = Unk3C;
        part.Unk40 = Unk40;
        return part;
    }
}
