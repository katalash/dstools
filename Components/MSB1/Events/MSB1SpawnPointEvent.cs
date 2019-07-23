using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Spawn Point")]
public class MSB1SpawnPointEvent : MSB1Event
{
    public string SpawnPoint;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.SpawnPoint)bevt;
        setBaseEvent(evt);
        SpawnPoint = evt.SpawnPointName;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.SpawnPoint();
        _Serialize(evt, parent);
        evt.SpawnPointName = (SpawnPoint == "") ? null : SpawnPoint;
        return evt;
    }
}
