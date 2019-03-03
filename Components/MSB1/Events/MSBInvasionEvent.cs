using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

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

    public void SetEvent(MsbEventNpcWorldInvitation evt)
    {
        setBaseEvent(evt);
        HostEventEntityID = evt.NPCHostEntityID;
        EventFlagID = evt.EventFlagID;
        SpawnPoint = evt.SpawnPoint;
    }

    public MsbEventNpcWorldInvitation Serialize(GameObject parent)
    {
        var evt = new MsbEventNpcWorldInvitation();
        _Serialize(evt, parent);
        evt.NPCHostEntityID = HostEventEntityID;
        evt.EventFlagID = EventFlagID;
        evt.SpawnPoint = (SpawnPoint == "") ? null : SpawnPoint;
        return evt;
    }
}
