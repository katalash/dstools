using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1NPCPart : MSB1Part
{
    public void SetPart(MsbPartsNPC part)
    {
        setBasePart(part);
    }

    public MsbPartsNPC Serialize(GameObject parent)
    {
        var part = new MsbPartsNPC();
        _Serialize(part, parent);
        return part;
    }
}
