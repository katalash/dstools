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

    // Mostly chalice related
    public int UnkT1C;
    public int UnkT20;
    public int UnkT24;
    public int UnkT28;
    public int UnkT2C;
    public int UnkT30;
    public int UnkT34;
    public int UnkT38;

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

    public short UnkT42;
    public int UnkT44;
    public int UnkT48;
    public int UnkT4C;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.Treasure)bevt;
        PartName2 = evt.PartName2;
        ItemLot1 = evt.ItemLot1;
        ItemLot2 = evt.ItemLot2;
        ItemLot3 = evt.ItemLot3;
        UnkT1C = evt.UnkT1C;
        UnkT20 = evt.UnkT20;
        UnkT24 = evt.UnkT24;
        UnkT28 = evt.UnkT28;
        UnkT2C = evt.UnkT2C;
        UnkT30 = evt.UnkT30;
        UnkT34 = evt.UnkT34;
        UnkT38 = evt.UnkT38;
        PickupAnimID = evt.PickupAnimID;
        InChest = evt.InChest;
        StartDisabled = evt.StartDisabled;
        UnkT42 = evt.UnkT42;
        UnkT44 = evt.UnkT44;
        UnkT48 = evt.UnkT48;
        UnkT4C = evt.UnkT4C;
    }

    public MSBBB.Event.Treasure Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Treasure(parent.name);
        _Serialize(evt, parent);
        evt.PartName2 = (PartName2 == "") ? null : PartName2;
        evt.ItemLot1 = ItemLot1;
        evt.ItemLot2 = ItemLot2;
        evt.ItemLot3 = ItemLot3;
        evt.UnkT1C = UnkT1C;
        evt.UnkT20 = UnkT20;
        evt.UnkT24 = UnkT24;
        evt.UnkT28 = UnkT28;
        evt.UnkT2C = UnkT2C;
        evt.UnkT30 = UnkT30;
        evt.UnkT34 = UnkT34;
        evt.UnkT38 = UnkT38;
        evt.PickupAnimID = PickupAnimID;
        evt.InChest = InChest;
        evt.StartDisabled = StartDisabled;
        evt.UnkT42 = UnkT42;
        evt.UnkT44 = UnkT44;
        evt.UnkT48 = UnkT48;
        evt.UnkT4C = UnkT4C;
        return evt;
    }
}
