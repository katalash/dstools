using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Events/Cheap Mode")]
public class MSB2CheapModeEvent : MSB2Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    public override void SetEvent(MSB2.Event bevt)
    {
        var evt = (MSB2.Event.CheapMode)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
    }

    public override MSB2.Event Serialize(GameObject parent)
    {
        var evt = new MSB2.Event.CheapMode();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        return evt;
    }
}
