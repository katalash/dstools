using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Player")]
public class MSBBBPlayerPart : MSBBBPart
{
    public void SetPart(MSBBB.Part.Player part)
    {
        setBasePart(part);
    }

    public MSBBB.Part.Player Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.Player(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
