using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Player")]
public class MSBSPlayerPart : MSBSPart
{
    public void SetPart(MSBS.Part.Player part)
    {
        setBasePart(part);
    }

    public MSBS.Part.Player Serialize(GameObject parent)
    {
        var part = new MSBS.Part.Player();
        _Serialize(part, parent);
        return part;
    }
}
