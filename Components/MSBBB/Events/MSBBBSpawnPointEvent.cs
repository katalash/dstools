using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Spawn Point")]
public class MSBBBSpawnPointEvent : MSBBBEvent
{
    /// <summary>
    /// Unknown; seems kind of like a region index, but also kind of doesn't.
    /// </summary>
    public string SpawnRegionName;

    public void SetEvent(MSBBB.Event.SpawnPoint evt)
    {
        setBaseEvent(evt);
        SpawnRegionName = evt.SpawnRegionName;
    }

    public MSBBB.Event.SpawnPoint Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.SpawnPoint(parent.name);
        _Serialize(evt, parent);
        evt.SpawnRegionName = SpawnRegionName;
        return evt;
    }
}
