using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Object")]
public class MSBBBObjectPart : MSBBBPart
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04, UnkT06, UnkT07, UnkT08, UnkT09, UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT02a, UnkT02b, UnkT03a, UnkT03b, UnkT05a, UnkT05b;

    public void SetPart(MSBBB.Part.Object part)
    {
        setBasePart(part);
        CollisionName = part.CollisionName;
        UnkT04 = part.UnkT04;
        UnkT06 = part.UnkT06;
        UnkT07 = part.UnkT07;
        UnkT08 = part.UnkT08;
        UnkT09 = part.UnkT09;
        UnkT10 = part.UnkT10;
        UnkT02a = part.UnkT02a;
        UnkT02b = part.UnkT02b;
        UnkT03a = part.UnkT03a;
        UnkT03b = part.UnkT03b;
        UnkT05a = part.UnkT05a;
        UnkT05b = part.UnkT05b;
    }

    public MSBBB.Part.Object Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Object(ID, parent.name);
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.UnkT04 = UnkT04;
        part.UnkT06 = UnkT06;
        part.UnkT07 = UnkT07;
        part.UnkT08 = UnkT08;
        part.UnkT09 = UnkT09;
        part.UnkT10 = UnkT10;
        part.UnkT02a = UnkT02a;
        part.UnkT02b = UnkT02b;
        part.UnkT03a = UnkT03a;
        part.UnkT03b = UnkT03b;
        part.UnkT05a = UnkT05a;
        part.UnkT05b = UnkT05b;
        return part;
    }
}
