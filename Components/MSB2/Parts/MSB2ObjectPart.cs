using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Parts/Object")]
public class MSB2ObjectPart : MSB2Part
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04;

    public override void SetPart(MSB2.Part bpart)
    {
        var part = (MSB2.Part.Object)bpart;
        setBasePart(part);
        UnkT00 = part.UnkT00;
        UnkT04 = part.UnkT04;
    }

    public override MSB2.Part Serialize(GameObject parent)
    {
        var part = new MSB2.Part.Object();
        _Serialize(part, parent);
        part.UnkT00 = UnkT00;
        part.UnkT04 = UnkT04;
        return part;
    }
}
