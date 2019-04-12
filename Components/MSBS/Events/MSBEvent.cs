using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for an event
public abstract class MSBSEvent : MonoBehaviour
{

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventIndex;

    /// <summary>
    /// The name of a part the event is attached to.
    /// </summary>
    public string PartName;

    /// <summary>
    /// The name of a region the event is attached to.
    /// </summary>
    public string RegionName;

    /// <summary>
    /// Used to identify the event in event scripts.
    /// </summary>
    public int EntityID;

    public void setBaseEvent(MSBS.Event evt)
    {
        EventIndex = evt.EventIndex;
        PartName = evt.PartName;
        RegionName = evt.RegionName;
        EntityID = evt.EntityID;
    }

    internal void _Serialize(MSBS.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventIndex = EventIndex;
        evt.PartName = (PartName == "") ? null : PartName;
        evt.RegionName = (RegionName == "") ? null : RegionName;
        evt.EntityID = EntityID;
    }
}
