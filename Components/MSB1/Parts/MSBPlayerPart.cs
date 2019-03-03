using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/Player")]
public class MSB1PlayerPart : MSB1Part
{
    public void SetPart(MsbPartsPlayer part)
    {
        setBasePart(part);
    }

    public MsbPartsPlayer Serialize(GameObject parent)
    {
        var part = new MsbPartsPlayer();
        _Serialize(part, parent);
        return part;
    }
}
