using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Model Declarations/Other")]
public class MSB3OtherModel : MSB3Model
{
    public void SetModel(MSB3.Model.Other model)
    {
        setBaseModel(model);
    }

    public MSB3.Model.Other Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Other(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
