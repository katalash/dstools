using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for an event
public abstract class MSB3Event : MonoBehaviour
{
    /// <summary>
    /// The ID of this event.
    /// </summary>
    public int EventID;

    /// <summary>
    /// The name of a part the event is attached to.
    /// </summary>
    public string PartName;

    /// <summary>
    /// The name of a region the event is attached to.
    /// </summary>
    public string PointName;

    /// <summary>
    /// Used to identify the event in event scripts.
    /// </summary>
    public int EventEntityID;

    public void setBaseEvent(MSB3.Event evt)
    {
        EventID = evt.EventID;
        PartName = evt.PartName;
        PointName = evt.PointName;
        EventEntityID = evt.EventEntityID;
    }

    internal void _Serialize(MSB3.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventID = EventID;
        evt.PartName = (PartName == "") ? null : PartName;
        evt.PointName = (PointName == "") ? null : PointName;
        evt.EventEntityID = EventEntityID;
    }

    public abstract void SetEvent(MSB3.Event evt);
    public abstract MSB3.Event Serialize(GameObject obj);
}
