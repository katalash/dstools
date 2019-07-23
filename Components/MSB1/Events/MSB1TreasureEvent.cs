using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Treasure")]
public class MSB1TreasureEvent : MSB1Event
{
    /// <summary>
    /// The part the treasure is attached to.
    /// </summary>
    public string TreasurePartName;

    /// <summary>
    /// IDs in the item lot param given by this treasure.
    /// </summary>
    public int ItemLot1, ItemLot2, ItemLot3, ItemLot4, ItemLot5;

    public bool InChest;
    public bool StartDisabled;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Treasure)bevt;
        setBaseEvent(evt);
        TreasurePartName = evt.TreasurePartName;
        ItemLot1 = evt.ItemLots[0];
        ItemLot2 = evt.ItemLots[1];
        ItemLot3 = evt.ItemLots[2];
        ItemLot4 = evt.ItemLots[3];
        ItemLot5 = evt.ItemLots[4];
        InChest = evt.InChest;
        StartDisabled = evt.StartDisabled;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Treasure();
        _Serialize(evt, parent);
        evt.TreasurePartName = (TreasurePartName == "") ? null : TreasurePartName;
        evt.ItemLots[0] = ItemLot1;
        evt.ItemLots[1] = ItemLot2;
        evt.ItemLots[2] = ItemLot3;
        evt.ItemLots[3] = ItemLot4;
        evt.ItemLots[4] = ItemLot5;
        evt.InChest = InChest;
        evt.StartDisabled = StartDisabled;
        return evt;
    }
}
