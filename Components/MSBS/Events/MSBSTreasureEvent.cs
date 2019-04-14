using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Treasure")]
public class MSBSTreasureEvent : MSBSEvent
{

    public string TreasurePartName;
    public int ItemLotID;
    public int ActionButtonID;
    public int PickupAnimID;
    public bool InChest;
    public bool StartDisabled;

    public void SetEvent(MSBS.Event.Treasure evt)
    {
        setBaseEvent(evt);
        TreasurePartName = evt.TreasurePartName;
        ItemLotID = evt.ItemLotID;
        ActionButtonID = evt.ActionButtonID;
        PickupAnimID = evt.PickupAnimID;
        InChest = evt.InChest;
        StartDisabled = evt.StartDisabled;
    }

    public MSBS.Event.Treasure Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Treasure();
        _Serialize(evt, parent);
        evt.TreasurePartName = TreasurePartName;
        evt.ItemLotID = ItemLotID;
        evt.ActionButtonID = ActionButtonID;
        evt.PickupAnimID = PickupAnimID;
        evt.InChest = InChest;
        evt.StartDisabled = StartDisabled;
        return evt;
    }
}
