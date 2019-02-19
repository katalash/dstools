using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSBBBOtherModel : MSBBBModel
{
    public void SetModel(MSBBB.Model.Other model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.Other Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Other(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
