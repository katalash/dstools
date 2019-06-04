using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Map Piece")]
public class MSBBBMapPiecePart : MSBBBPart
{

    public override void SetPart(MSBBB.Part bpart)
    {
        var part = (MSBBB.Part.MapPiece)bpart;
        setBasePart(part);
    }

    public override MSBBB.Part Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.MapPiece(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
