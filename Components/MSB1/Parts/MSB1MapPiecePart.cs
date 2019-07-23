using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB1MapPiecePart : MSB1Part
{
    [AddComponentMenu("Dark Souls 1/Parts/Map Piece")]
    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.MapPiece)bpart;
        setBasePart(part);
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.MapPiece();
        _Serialize(part, parent);
        return part;
    }
}
