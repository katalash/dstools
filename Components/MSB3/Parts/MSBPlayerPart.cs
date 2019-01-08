using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3PlayerPart : MSB3Part
{
    public void SetPart(MSB3.Part.Player part)
    {
        setBasePart(part);
    }

    public MSB3.Part.Player Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Player(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
