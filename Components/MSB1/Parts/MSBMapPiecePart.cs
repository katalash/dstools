using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1MapPiecePart : MSB1Part
{
    public void SetPart(MsbPartsMapPiece part)
    {
        setBasePart(part);
    }

    public MsbPartsMapPiece Serialize(GameObject parent)
    {
        var part = new MsbPartsMapPiece();
        _Serialize(part, parent);
        return part;
    }
}
