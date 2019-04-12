using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public abstract class MSBSModel : MonoBehaviour
{
    /// <summary>
    /// The placeholder used for this model in MapStudio.
    /// </summary>
    public string Placeholder;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk1C;

    public void setBaseModel(MSBS.Model model)
    {
        Placeholder = model.Placeholder;
        Unk1C = model.Unk1C;
    }

    internal void _Serialize(MSBS.Model model, GameObject parent)
    {
        model.Name = parent.name;
        model.Placeholder = Placeholder;
        model.Unk1C = Unk1C;
    }

}
