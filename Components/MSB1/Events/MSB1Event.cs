using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

public abstract class MSB1Event : MonoBehaviour
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

    public void setBaseEvent(MsbEventBase evt)
    {
        EventIndex = evt.EventIndex;
        ID = evt.Index;
        PartName = evt.Part;
        PointName = evt.Region;
        EventEntityID = evt.EntityID;
    }


    internal void _Serialize(MsbEventBase evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.Index = ID;
        evt.Part = PartName;
        evt.Region = PointName;
        evt.EntityID = EventEntityID;
    }
}
