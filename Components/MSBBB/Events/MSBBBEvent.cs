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

    public int Unk01;

    public void setBaseEvent(MSBBB.Event evt)
    {
        EventID = evt.EventID;
        PartName = evt.PartName;
        PointName = evt.PointName;
        EventEntityID = evt.EventEntityID;
        Unk01 = evt.Unk01;
    }

    internal void _Serialize(MSBBB.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventID = EventID;
        evt.PartName = (PartName == "") ? null : PartName;
        evt.PointName = (PointName == "") ? null : PointName;
        evt.EventEntityID = EventEntityID;
        evt.Unk01 = Unk01;
    }
}
