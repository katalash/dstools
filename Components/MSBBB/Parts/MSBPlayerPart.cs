using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSBBBPlayerPart : MSBBBPart
{
    public void SetPart(MSBBB.Part.Player part)
    {
        setBasePart(part);
    }

    public MSBBB.Part.Player Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Player(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
