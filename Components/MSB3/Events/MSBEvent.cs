using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for an event
public class MSB3Event : MonoBehaviour
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

    public void setBaseEvent(MSB3.Event evt)
    {
        EventIndex = evt.EventIndex;
        ID = evt.ID;
        PartName = evt.PartName;
        PointName = evt.PointName;
        EventEntityID = evt.EventEntityID;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
