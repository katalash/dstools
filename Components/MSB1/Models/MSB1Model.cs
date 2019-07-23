using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public abstract class MSB1Model : MonoBehaviour
{
    /// <summary>
    /// The placeholder used for this model in MapStudio.
    /// </summary>
    public string Placeholder;

    public void setBaseModel(MSB1.Model model)
    {
        Placeholder = model.Placeholder;
    }

    internal void _Serialize(MSB1.Model model, GameObject parent)
    {
        model.Name = parent.name;
        model.Placeholder = Placeholder;
    }

    public abstract void SetModel(MSB1.Model model);
    public abstract MSB1.Model Serialize(GameObject obj);
}
