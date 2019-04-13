using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Model Declarations/Collision")]
public class MSBSCollisionModel : MSBSModel
{

    public void SetModel(MSBS.Model.Collision model)
    {
        setBaseModel(model);
    }

    public MSBS.Model.Collision Serialize(GameObject parent)
    {
        var model = new MSBS.Model.Collision();
        _Serialize(model, parent);
        return model;
    }
}
