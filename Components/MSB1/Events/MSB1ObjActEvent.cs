using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Object Action")]
public class MSB1ObjActEvent : MSB1Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int ObjActEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short ParameterID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventFlagID;

    public void SetEvent(MsbEventObjAct evt)
    {
        setBaseEvent(evt);
        ObjActEntityID = evt.ObjActEntityID;
        PartName2 = evt.ObjName;
        ParameterID = evt.ParameterID;
        UnkT10 = evt.SubUnk1;
        EventFlagID = evt.EventFlagID;
    }

    public MsbEventObjAct Serialize(GameObject parent)
    {
        var evt = new MsbEventObjAct();
        _Serialize(evt, parent);
        evt.ObjActEntityID = ObjActEntityID;
        evt.ObjName = (PartName2 == "") ? null : PartName2;
        evt.ParameterID = ParameterID;
        evt.SubUnk1 = UnkT10;
        evt.EventFlagID = EventFlagID;
        return evt;
    }
}
