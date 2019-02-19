using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1ObjectPart : MSB1Part
{
    public void SetPart(MsbPartsObject part)
    {
        setBasePart(part);
    }

    public MsbPartsObject Serialize(GameObject parent)
    {
        var part = new MsbPartsObject();
        _Serialize(part, parent);
        return part;
    }
}
