using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBSUnkStruct2Part : MonoBehaviour
{

    public int Condition;

    public int DispGroups1;
    public int DispGroups2;
    public int DispGroups3;
    public int DispGroups4;
    public int DispGroups5;
    public int DispGroups6;
    public int DispGroups7;
    public int DispGroups8;

    public short Unk24;
    public short Unk26;

    public void setStruct(MSBS.Part.UnkStruct2 part)
    {
        Condition = part.Condition;
        DispGroups1 = part.DispGroups[0];
        DispGroups2 = part.DispGroups[1];
        DispGroups3 = part.DispGroups[2];
        DispGroups4 = part.DispGroups[3];
        DispGroups5 = part.DispGroups[4];
        DispGroups6 = part.DispGroups[5];
        DispGroups7 = part.DispGroups[6];
        DispGroups8 = part.DispGroups[7];
        Unk24 = part.Unk24;
        Unk26 = part.Unk26;
    }

    public MSBS.Part.UnkStruct2 Serialize()
    {
        MSBS.Part.UnkStruct2 part = new MSBS.Part.UnkStruct2();
        part.Condition = Condition;
        part.DispGroups[0] = DispGroups1;
        part.DispGroups[1] = DispGroups2;
        part.DispGroups[2] = DispGroups3;
        part.DispGroups[3] = DispGroups4;
        part.DispGroups[4] = DispGroups5;
        part.DispGroups[5] = DispGroups6;
        part.DispGroups[6] = DispGroups7;
        part.DispGroups[7] = DispGroups8;
        part.Unk24 = Unk24;
        part.Unk26 = Unk26;
        return part;
    }
}
