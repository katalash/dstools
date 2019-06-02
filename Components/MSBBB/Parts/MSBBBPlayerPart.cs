using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Player")]
public class MSBBBPlayerPart : MSBBBPart
{
    public override void SetPart(MSBBB.Part part)
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
