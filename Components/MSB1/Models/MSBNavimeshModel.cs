using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Model Declarations/Navimesh")]
public class MSB1NavimeshModel : MSB1Model
{
    public void SetModel(MsbModelNavimesh model)
    {
        setBaseModel(model);
    }

    public MsbModelNavimesh Serialize(GameObject parent)
    {
        var model = new MsbModelNavimesh();
        _Serialize(model, parent);
        return model;
    }
}
