using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Object")]
public class MSB1ObjectPart : MSB1Part
{
    public string CollisionName;
    public int UnkT08;
    public short UnkT0C, UnkT0E;
    public int UnkT10;

    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.Object)bpart;
        setBasePart(part);
        CollisionName = part.CollisionName;
        UnkT08 = part.UnkT08;
        UnkT0C = part.UnkT0C;
        UnkT0E = part.UnkT0E;
        UnkT10 = part.UnkT10;
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.Object();
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.UnkT08 = UnkT08;
        part.UnkT0C = UnkT0C;
        part.UnkT0E = UnkT0E;
        part.UnkT10 = UnkT10;
        return part;
    }
}
