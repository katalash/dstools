using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3TreasureEvent : MSB3Event
{
    /// <summary>
    /// The part the treasure is attached to.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// IDs in the item lot param given by this treasure.
    /// </summary>
    public int ItemLot1, ItemLot2;

    /// <summary>
    /// Animation to play when taking this treasure.
    /// </summary>
    public int PickupAnimID;

    /// <summary>
    /// Used for treasures inside chests, exact significance unknown.
    /// </summary>
    public bool InChest;

    /// <summary>
    /// Used only for Yoel's ashes treasure; in DS1, used for corpses in barrels.
    /// </summary>
    public bool StartDisabled;

    public void SetEvent(MSB3.Event.Treasure evt)
    {
        setBaseEvent(evt);
        PartName2 = evt.PartName2;
        ItemLot1 = evt.ItemLot1;
        ItemLot2 = evt.ItemLot2;
        PickupAnimID = evt.PickupAnimID;
        InChest = evt.InChest;
        StartDisabled = evt.StartDisabled;
    }

    public MSB3.Event.Treasure Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.Treasure(ID, parent.name);
        _Serialize(evt, parent);
        evt.PartName2 = (PartName2 == "") ? null : PartName2;
        evt.ItemLot1 = ItemLot1;
        evt.ItemLot2 = ItemLot2;
        evt.PickupAnimID = PickupAnimID;
        evt.InChest = InChest;
        evt.StartDisabled = StartDisabled;
        return evt;
    }
}
