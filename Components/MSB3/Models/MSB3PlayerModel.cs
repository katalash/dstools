using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Model Declarations/Player")]
public class MSB3PlayerModel : MSB3Model
{
    public override void SetModel(MSB3.Model model)
    {
        setBaseModel(model);
    }

    public override MSB3.Model Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Player(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
