using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSBBBCollisionModel : MSBBBModel
{
    public void SetModel(MSBBB.Model.Collision model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.Collision Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Collision(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
