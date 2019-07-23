using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/SFX")]
public class MSB1SFXEvent : MSB1Event
{
    /// <summary>
    /// The ID of the .fxr file to play in this region.
    /// </summary>
    public int FFXID;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.SFX)bevt;
        setBaseEvent(evt);
        FFXID = evt.FFXID;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.SFX();
        _Serialize(evt, parent);
        evt.FFXID = FFXID;
        return evt;
    }
}
