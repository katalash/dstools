using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Object")]
public class MSBBBObjectPart : MSBBBPart
{
    public MSBBBGParamConfig GParamConfig;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04, UnkT06;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT02a, UnkT02b, UnkT03a, UnkT03b, UnkT05a, UnkT05b;

    public override void SetPart(MSBBB.Part bpart)
    {
        var part = (MSBBB.Part.Object)bpart;
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSBBBGParamConfig>();
        GParamConfig.setStruct(part.Gparam);
        CollisionName = part.CollisionName;
        UnkT04 = part.UnkT04;
        UnkT06 = part.UnkT06;
        UnkT02a = part.UnkT02a;
        UnkT02b = part.UnkT02b;
        UnkT03a = part.UnkT03a;
        UnkT03b = part.UnkT03b;
        UnkT05a = part.UnkT05a;
        UnkT05b = part.UnkT05b;
    }

    public override MSBBB.Part Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Object(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.UnkT04 = UnkT04;
        part.UnkT06 = UnkT06;
        part.UnkT02a = UnkT02a;
        part.UnkT02b = UnkT02b;
        part.UnkT03a = UnkT03a;
        part.UnkT03b = UnkT03b;
        part.UnkT05a = UnkT05a;
        part.UnkT05b = UnkT05b;
        return part;
    }
}
