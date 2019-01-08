using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

public class MSB1PlayerModel : MSB1Model
{
    public void SetModel(MsbModelPlayer model)
    {
        setBaseModel(model);
    }

    public MsbModelPlayer Serialize(GameObject parent)
    {
        var model = new MsbModelPlayer();
        _Serialize(model, parent);
        return model;
    }
}
