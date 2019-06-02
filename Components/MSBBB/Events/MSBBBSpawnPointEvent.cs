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

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.SpawnPoint)bevt;
        SpawnRegionName = evt.SpawnRegionName;
    }

    public override MSBBB.Event Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.SpawnPoint(parent.name);
        _Serialize(evt, parent);
        evt.SpawnRegionName = SpawnRegionName;
        return evt;
    }
}
