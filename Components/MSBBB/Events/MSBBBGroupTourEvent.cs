using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Group Tour")]
public class MSBBBGroupTourEvent : MSBBBEvent
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00, UnkT04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string[] GroupPartsNames;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.GroupTour)bevt;
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        GroupPartsNames = evt.GroupPartsNames;
    }

    public override MSBBB.Event Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.GroupTour(parent.name);
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        for (int i = 0; i < 32; i++)
        {
            if (i >= GroupPartsNames.Length)
                break;
            evt.GroupPartsNames[i] = (GroupPartsNames[i] == "") ? null : GroupPartsNames[i];
        }

        return evt;
    }
}
