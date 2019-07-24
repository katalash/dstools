using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Object")]
public class MSB3ObjectPart : MSB3Part
{
    public MSB3GParamConfig GParamConfig;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT0C, UnkT0E, StartAnimID, UnkT12, UnkT14, UnkT16, UnkT18, UnkT1A, UnkT1C, UnkT1E;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT20, UnkT24, UnkT28, UnkT2C;

    public override void SetPart(MSB3.Part bpart)
    {
        var part = (MSB3.Part.Object)bpart;
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSB3GParamConfig>();
        GParamConfig.setStruct(part.Gparam);
        CollisionName = part.CollisionName;
        UnkT0C = part.UnkT0C;
        UnkT0E = part.UnkT0E;
        StartAnimID = part.UnkT10;
        UnkT12 = part.UnkT12;
        UnkT14 = part.UnkT14;
        UnkT16 = part.UnkT16;
        UnkT18 = part.UnkT18;
        UnkT1A = part.UnkT1A;
        UnkT1C = part.UnkT1C;
        UnkT1E = part.UnkT1E;
    }

    public override MSB3.Part Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Object(parent.name);
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
