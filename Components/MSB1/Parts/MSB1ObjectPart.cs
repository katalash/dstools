using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/Object")]
public class MSB1ObjectPart : MSB1Part
{
    public string CollisionName;
    public byte Unk1, Unk2;
    public short Unk3, Unk4;
    public int Unk5;

    public void SetPart(MsbPartsObject part)
    {
        setBasePart(part);
        CollisionName = part.PartName;
        Unk1 = part.SubUnk1;
        Unk2 = part.SubUnk2;
        Unk3 = part.SubUnk3;
        Unk4 = part.SubUnk4;
        Unk5 = part.SubUnk5;
    }

    public MsbPartsObject Serialize(GameObject parent)
    {
        var part = new MsbPartsObject();
        _Serialize(part, parent);
        part.PartName = (CollisionName == "") ? null : CollisionName;
        part.SubUnk1 = Unk1;
        part.SubUnk2 = Unk2;
        part.SubUnk3 = Unk3;
        part.SubUnk4 = Unk4;
        part.SubUnk5 = Unk5;
        return part;
    }
}
