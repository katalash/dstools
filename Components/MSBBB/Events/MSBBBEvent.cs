using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for an event
public abstract class MSBBBEvent : MonoBehaviour
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventIndex;

    /// <summary>
    /// The ID of this event.
    /// </summary>
    public int ID;

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

    public int Unk01;

    public void setBaseEvent(MSBBB.Event evt)
    {
        EventIndex = evt.EventIndex;
        ID = evt.ID;
        PartName = evt.PartName;
        PointName = evt.PointName;
        EventEntityID = evt.EventEntityID;
        Unk01 = evt.Unk01;
    }

    internal void _Serialize(MSBBB.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventIndex = EventIndex;
        evt.ID = ID;
        evt.PartName = (PartName == "") ? null : PartName;
        evt.PointName = (PointName == "") ? null : PointName;
        evt.EventEntityID = EventEntityID;
        evt.Unk01 = Unk01;
    }
}
