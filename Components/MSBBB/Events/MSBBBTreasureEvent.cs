using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Treasure")]
public class MSBBBTreasureEvent : MSBBBEvent
{
    /// <summary>
    /// The part the treasure is attached to.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// IDs in the item lot param given by this treasure.
    /// </summary>
    public int ItemLot1, ItemLot2, ItemLot3;

    public int Unk1;
    public int Unk2;
    public int Unk3;

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

    public void SetEvent(MSBBB.Event.Treasure evt)
    {
        setBaseEvent(evt);
        PartName2 = evt.PartName2;
        ItemLot1 = evt.ItemLot1;
        ItemLot2 = evt.ItemLot2;
        ItemLot3 = evt.ItemLot3;
        Unk1 = evt.Unk1;
        Unk2 = evt.Unk2;
        Unk3 = evt.Unk3;
        PickupAnimID = evt.PickupAnimID;
        InChest = evt.InChest;
        StartDisabled = evt.StartDisabled;
    }

    public MSBBB.Event.Treasure Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Treasure(ID, parent.name);
        _Serialize(evt, parent);
        evt.PartName2 = (PartName2 == "") ? null : PartName2;
        evt.ItemLot1 = ItemLot1;
        evt.ItemLot2 = ItemLot2;
        evt.ItemLot3 = ItemLot3;
        evt.Unk1 = Unk1;
        evt.Unk2 = Unk2;
        evt.Unk3 = Unk3;
        evt.PickupAnimID = PickupAnimID;
        evt.InChest = InChest;
        evt.StartDisabled = StartDisabled;
        return evt;
    }
}
