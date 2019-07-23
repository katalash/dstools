using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

public abstract class MSB1Event : MonoBehaviour
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
    public string RegionName;

    /// <summary>
    /// Used to identify the event in event scripts.
    /// </summary>
    public int EventEntityID;

    public void setBaseEvent(MSB1.Event evt)
    {
        EventID = evt.EventID;
        PartName = evt.PartName;
        RegionName = evt.RegionName;
        EventEntityID = evt.EntityID;
    }


    internal void _Serialize(MSB1.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventID = EventID;
        evt.PartName = (PartName == "") ? null : PartName;
        evt.RegionName = (RegionName == "") ? null : RegionName;
        evt.EntityID = EventEntityID;
    }

    public abstract void SetEvent(MSB1.Event evt);
    public abstract MSB1.Event Serialize(GameObject obj);
}
