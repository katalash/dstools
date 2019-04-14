using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBSUnkStruct5Part : MonoBehaviour
{
    public int Unk00;
    public int Unk04;
    public int Unk08;
    public int Unk0C;


    public void setStruct(MSBS.Part.UnkStruct5 part)
    {
        Unk00 = part.Unk00;
        Unk04 = part.Unk04;
        Unk08 = part.Unk08;
        Unk0C = part.Unk0C;
    }

    public MSBS.Part.UnkStruct5 Serialize()
    {
        MSBS.Part.UnkStruct5 part = new MSBS.Part.UnkStruct5();
        part.Unk00 = Unk00;
        part.Unk04 = Unk04;
        part.Unk08 = Unk08;
        part.Unk0C = Unk0C;
        return part;
    }
}
