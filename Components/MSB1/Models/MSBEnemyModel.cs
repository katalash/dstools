using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

public class MSB1EnemyModel : MSB1Model
{
    public void SetModel(MsbModelCharacter model)
    {
        setBaseModel(model);
    }

    public MsbModelCharacter Serialize(GameObject parent)
    {
        var model = new MsbModelCharacter();
        _Serialize(model, parent);
        return model;
    }
}
