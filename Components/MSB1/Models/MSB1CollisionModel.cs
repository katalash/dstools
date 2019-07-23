using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Model Declarations/Collision")]
public class MSB1CollisionModel : MSB1Model
{
    public override void SetModel(MSB1.Model model)
    {
        setBaseModel(model);
    }

    public override MSB1.Model Serialize(GameObject parent)
    {
        var model = new MSB1.Model();
        _Serialize(model, parent);
        return model;
    }
}
