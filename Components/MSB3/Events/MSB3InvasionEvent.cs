using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Invasion")]
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

    public void SetEvent(MSB3.Event.PseudoMultiplayer evt)
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

    public MSB3.Event.PseudoMultiplayer Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.PseudoMultiplayer(parent.name);
        _Serialize(evt, parent);
        evt.HostEventEntityID = HostEventEntityID;
        evt.InvasionEventEntityID = InvasionEventEntityID;
        evt.InvasionRegionIndex = InvasionRegionIndex;
        evt.SoundIDMaybe = SoundIDMaybe;
        evt.MapEventIDMaybe = MapEventIDMaybe;
        evt.FlagsMaybe = FlagsMaybe;
        evt.UnkT18 = UnkT18;
        return evt;
    }
}
