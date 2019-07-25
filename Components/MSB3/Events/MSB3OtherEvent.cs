using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Other")]
public class MSB3OtherEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int SoundTypeMaybe;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int SoundIDMaybe;

    public override void SetEvent(MSB3.Event bevt)
    {
        var evt = (MSB3.Event.Other)bevt;
        setBaseEvent(evt);
        SoundTypeMaybe = evt.SoundTypeMaybe;
        SoundIDMaybe = evt.SoundIDMaybe;
    }

    public override MSB3.Event Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.Other(parent.name);
        _Serialize(evt, parent);
        evt.SoundTypeMaybe = SoundTypeMaybe;
        evt.SoundIDMaybe = SoundIDMaybe;
        return evt;
    }
}
