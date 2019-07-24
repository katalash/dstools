using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Player")]
public class MSB3PlayerPart : MSB3Part
{
    public override void SetPart(MSB3.Part part)
    {
        setBasePart(part);
    }

    public override MSB3.Part Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Player(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
