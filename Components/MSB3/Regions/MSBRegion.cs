using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3Region : MonoBehaviour
{
    /// <summary>
    /// Whether this region has additional type data. The only region type where this actually varies is Sound.
    /// </summary>
    public bool HasTypeData;

    /// <summary>
    /// The ID of this region.
    /// </summary>
    public int ID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk2, Unk3, Unk4;

    /// <summary>
    /// Not sure if this is exactly a drawgroup, but it's what makes messages not appear in dark Firelink.
    /// </summary>
    public uint DrawGroup;

    /// <summary>
    /// Region is inactive unless this part is drawn; null for always active.
    /// </summary>
    public string ActivationPartName;

    /// <summary>
    /// An ID used to identify this region in event scripts.
    /// </summary>
    public int EventEntityID;

    public void setBaseRegion(MSB3.Region region)
    {
        HasTypeData = region.HasTypeData;
        ID = region.ID;
        Unk2 = region.Unk2;
        Unk3 = region.Unk3;
        Unk4 = region.Unk4;
        DrawGroup = region.DrawGroup;
        ActivationPartName = region.ActivationPartName;
        EventEntityID = region.EventEntityID;
    }

    internal void _Serialize(MSB3.Region region, GameObject parent)
    {
        region.Name = parent.name;
        region.ID = ID;

        region.HasTypeData = HasTypeData;
        region.Unk2 = Unk2;
        region.Unk3 = Unk3;
        region.Unk4 = Unk4;
        region.DrawGroup = DrawGroup;
        region.ActivationPartName = ActivationPartName;
        region.EventEntityID = EventEntityID;
    }
}
