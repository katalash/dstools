using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Model Declarations/Object")]
public class MSB1ObjectModel : MSB1Model
{
    public void SetModel(MsbModelObject model)
    {
        setBaseModel(model);
    }

    public MsbModelObject Serialize(GameObject parent)
    {
        var model = new MsbModelObject();
        _Serialize(model, parent);
        return model;
    }
}
