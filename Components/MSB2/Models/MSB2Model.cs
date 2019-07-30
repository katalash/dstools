using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public abstract class MSB2Model : MonoBehaviour
{
    public void setBaseModel(MSB2.Model model)
    {
    }

    internal void _Serialize(MSB2.Model model, GameObject parent)
    {
        model.Name = parent.name;
    }

    public abstract void SetModel(MSB2.Model model);
    public abstract MSB2.Model Serialize(GameObject obj);
}
