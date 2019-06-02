using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public abstract class MSBBBModel : MonoBehaviour
{
    /// <summary>
    /// The placeholder used for this model in MapStudio.
    /// </summary>
    public string Placeholder;

    public void setBaseModel(MSBBB.Model model)
    {
        Placeholder = model.Placeholder;
    }

    internal void _Serialize(MSBBB.Model model, GameObject parent)
    {
        model.Name = parent.name;
        model.Placeholder = Placeholder;
    }

    public abstract void SetModel(MSBBB.Model model);
}
