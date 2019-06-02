using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Object")]
public class MSBBBObjectModel : MSBBBModel
{
    public void SetModel(MSBBB.Model.Object model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.Object Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Object(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
