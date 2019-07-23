using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Navimesh")]
public class MSB1NavimeshEvent : MSB1Event
{
    /// <summary>
    /// Region for navimesh
    /// </summary>
    public string NavmeshRegionName;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Navmesh)bevt;
        setBaseEvent(evt);
        NavmeshRegionName = evt.NavmeshRegionName;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Navmesh();
        _Serialize(evt, parent);
        evt.NavmeshRegionName = (NavmeshRegionName == "") ? null : NavmeshRegionName;
        return evt;
    }
}
