using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3InvasionEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int HostEventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int InvasionEventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int InvasionRegionIndex;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int SoundIDMaybe;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int MapEventIDMaybe;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int FlagsMaybe;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT18;

    public void SetEvent(MSB3.Event.Invasion evt)
    {
        setBaseEvent(evt);
        HostEventEntityID = evt.HostEventEntityID;
        InvasionEventEntityID = evt.InvasionEventEntityID;
        InvasionRegionIndex = evt.InvasionRegionIndex;
        SoundIDMaybe = evt.SoundIDMaybe;
        MapEventIDMaybe = evt.MapEventIDMaybe;
        FlagsMaybe = evt.FlagsMaybe;
        UnkT18 = evt.UnkT18;
    }
}
