using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1DummyObjectPart : MSB1ObjectPart
{
    [AddComponentMenu("Dark Souls 1/Parts/Dummy Object")]
    public new MsbPartsObjectDummy Serialize(GameObject parent)
    {
        var part = new MsbPartsObjectDummy();
        _Serialize(part, parent);
        part.PartName = CollisionName;
        part.SubUnk1 = Unk1;
        part.SubUnk2 = Unk2;
        part.SubUnk3 = Unk3;
        part.SubUnk4 = Unk4;
        part.SubUnk5 = Unk5;
        return part;
    }
}
