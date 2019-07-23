using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Invasion")]
public class MSB1InvasionEvent : MSB1Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int HostEventEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventFlagID;

    public string SpawnPoint;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.PseudoMultiplayer)bevt;
        setBaseEvent(evt);
        HostEventEntityID = evt.HostEntityID;
        EventFlagID = evt.EventFlagID;
        SpawnPoint = evt.SpawnPointName;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.PseudoMultiplayer();
        _Serialize(evt, parent);
        evt.HostEntityID = HostEventEntityID;
        evt.EventFlagID = EventFlagID;
        evt.SpawnPointName = (SpawnPoint == "") ? null : SpawnPoint;
        return evt;
    }
}
