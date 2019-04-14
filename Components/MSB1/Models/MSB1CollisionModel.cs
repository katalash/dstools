using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Model Declarations/Collision")]
public class MSB1CollisionModel : MSB1Model
{
    public void SetModel(MsbModelCollision model)
    {
        setBaseModel(model);
    }

    public MsbModelCollision Serialize(GameObject parent)
    {
        var model = new MsbModelCollision();
        _Serialize(model, parent);
        return model;
    }
}
