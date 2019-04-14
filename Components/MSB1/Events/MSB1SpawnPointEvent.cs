using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Spawn Point")]
public class MSB1SpawnPointEvent : MSB1Event
{
    public string SpawnPoint;

    public void SetEvent(MsbEventSpawnPoint evt)
    {
        setBaseEvent(evt);
        SpawnPoint = evt.SpawnPoint;
    }

    public MsbEventSpawnPoint Serialize(GameObject parent)
    {
        var evt = new MsbEventSpawnPoint();
        _Serialize(evt, parent);
        evt.SpawnPoint = (SpawnPoint == "") ? null : SpawnPoint;
        return evt;
    }
}
