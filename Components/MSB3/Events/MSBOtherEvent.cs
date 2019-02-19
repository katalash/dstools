using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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

    public void SetEvent(MSB3.Event.Other evt)
    {
        setBaseEvent(evt);
        SoundTypeMaybe = evt.SoundTypeMaybe;
        SoundIDMaybe = evt.SoundIDMaybe;
    }

    public MSB3.Event.Other Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.Other(ID, parent.name);
        _Serialize(evt, parent);
        evt.SoundTypeMaybe = SoundTypeMaybe;
        evt.SoundIDMaybe = SoundIDMaybe;
        return evt;
    }
}
