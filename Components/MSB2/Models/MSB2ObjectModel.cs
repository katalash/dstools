using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Model Declarations/Object")]
public class MSB2ObjectModel : MSB2Model
{
    public override void SetModel(MSB2.Model model)
    {
        setBaseModel(model);
    }

    public override MSB2.Model Serialize(GameObject parent)
    {
        var model = new MSB2.Model();
        _Serialize(model, parent);
        return model;
    }
}
