using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Wind")]
public class MSBBBWindEvent : MSBBBEvent
{
    /// <summary>
    /// The ID of the .fxr file to play in this region.
    /// </summary>
    public int FFXID;

    /// <summary>
    /// Name of a corresponding WindArea region.
    /// </summary>
    public string WindAreaName;

    public float UnkF0C;

    public void SetEvent(MSBBB.Event.Wind evt)
    {
        setBaseEvent(evt);
        FFXID = evt.FFXID;
        WindAreaName = evt.WindAreaName;
        UnkF0C = evt.UnkF0C;
    }

    public MSBBB.Event.Wind Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Wind(ID, parent.name);
        _Serialize(evt, parent);
        evt.FFXID = FFXID;
        evt.WindAreaName = (WindAreaName == "") ? null : WindAreaName;
        evt.UnkF0C = UnkF0C;
        return evt;
    }
}
