using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Player")]
public class MSB1PlayerPart : MSB1Part
{
    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.Player)bpart;
        setBasePart(part);
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.Player();
        _Serialize(part, parent);
        return part;
    }
}
