using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1CollisionPart : MSB1Part
{
    public void SetPart(MsbPartsHit part)
    {
        setBasePart(part);
    }

    public MsbPartsHit Serialize(GameObject parent)
    {
        var part = new MsbPartsHit();
        _Serialize(part, parent);
        return part;
    }
}
