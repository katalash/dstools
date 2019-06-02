using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Other")]
public class MSBBBOtherModel : MSBBBModel
{
    public override void SetModel(MSBBB.Model model)
    {
        setBaseModel(model);
    }

    public override MSBBB.Model Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Other(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
