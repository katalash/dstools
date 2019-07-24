using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Dummy Object")]
public class MSB3DummyObjectPart : MSB3ObjectPart
{
    public override MSB3.Part Serialize(GameObject parent)
    {
        var part = new MSB3.Part.DummyObject(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.UnkT0C = UnkT0C;
        part.UnkT0E = UnkT0E;
        part.UnkT10 = StartAnimID;
        part.UnkT12 = UnkT12;
        part.UnkT14 = UnkT14;
        part.UnkT16 = UnkT16;
        part.UnkT18 = UnkT18;
        part.UnkT1A = UnkT1A;
        part.UnkT1C = UnkT1C;
        part.UnkT1E = UnkT1E;
        return part;
    }
}
