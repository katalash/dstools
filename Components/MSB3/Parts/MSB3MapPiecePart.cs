using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Map Piece")]
public class MSB3MapPiecePart : MSB3Part
{
    public MSB3GParamConfig GParamConfig;

    public override void SetPart(MSB3.Part bpart)
    {
        var part = (MSB3.Part.MapPiece)bpart;
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSB3GParamConfig>();
        GParamConfig.setStruct(part.Gparam);
    }

    public override MSB3.Part Serialize(GameObject parent)
    {
        var part = new MSB3.Part.MapPiece(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        return part;
    }
}
