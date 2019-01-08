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
}
