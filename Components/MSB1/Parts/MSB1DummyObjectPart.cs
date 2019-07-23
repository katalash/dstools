using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB1DummyObjectPart : MSB1ObjectPart
{
    [AddComponentMenu("Dark Souls 1/Parts/Dummy Object")]
    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.DummyObject();
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.UnkT08 = UnkT08;
        part.UnkT0C = UnkT0C;
        part.UnkT0E = UnkT0E;
        part.UnkT10 = UnkT10;
        return part;
    }
}
