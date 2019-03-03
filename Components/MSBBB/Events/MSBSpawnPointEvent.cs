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
    public int UnkT00;

    public void SetEvent(MSBBB.Event.SpawnPoint evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
    }

    public MSBBB.Event.SpawnPoint Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.SpawnPoint(ID, parent.name);
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        return evt;
    }
}
