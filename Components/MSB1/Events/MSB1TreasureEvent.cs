using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Treasure")]
public class MSB1TreasureEvent : MSB1Event
{
    /// <summary>
    /// The part the treasure is attached to.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// IDs in the item lot param given by this treasure.
    /// </summary>
    public int ItemLot1, ItemLot2, ItemLot3, ItemLot4, ItemLot5;

    public int Unk1;

    public void SetEvent(MsbEventTreasure evt)
    {
        setBaseEvent(evt);
        PartName2 = evt.AttachObj;
        ItemLot1 = evt.ItemLot1;
        ItemLot2 = evt.ItemLot2;
        ItemLot3 = evt.ItemLot3;
        ItemLot4 = evt.ItemLot4;
        ItemLot5 = evt.ItemLot5;
        Unk1 = evt.SubUnk2;
    }

    public MsbEventTreasure Serialize(GameObject parent)
    {
        var evt = new MsbEventTreasure();
        _Serialize(evt, parent);
        evt.AttachObj = (PartName2 == "") ? null : PartName2;
        evt.ItemLot1 = ItemLot1;
        evt.ItemLot2 = ItemLot2;
        evt.ItemLot3 = ItemLot3;
        evt.ItemLot4 = ItemLot4;
        evt.ItemLot5 = ItemLot5;
        evt.SubUnk2 = Unk1;
        return evt;
    }
}
