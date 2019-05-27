using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSB3UnkStruct4 : MonoBehaviour
{
    public int Unk3C;
    public float Unk40;

    public void setStruct(MSB3.Part.UnkStruct4 part)
    {
        Unk3C = part.Unk3C;
        Unk40 = part.Unk40;
    }

    public MSB3.Part.UnkStruct4 Serialize()
    {
        MSB3.Part.UnkStruct4 part = new MSB3.Part.UnkStruct4();
        part.Unk3C = Unk3C;
        part.Unk40 = Unk40;
        return part;
    }
}
