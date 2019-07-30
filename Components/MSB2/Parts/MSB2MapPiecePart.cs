using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB2MapPiecePart : MSB2Part
{
    [AddComponentMenu("Dark Souls 2/Parts/Map Piece")]

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT02;

    public override void SetPart(MSB2.Part bpart)
    {
        var part = (MSB2.Part.MapPiece)bpart;
        setBasePart(part);
        UnkT00 = part.UnkT00;
        UnkT02 = part.UnkT02;
    }

    public override MSB2.Part Serialize(GameObject parent)
    {
        var part = new MSB2.Part.MapPiece();
        _Serialize(part, parent);
        part.UnkT00 = UnkT00;
        part.UnkT02 = UnkT02;
        return part;
    }
}
