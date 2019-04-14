using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Navimesh")]
public class MSB1NavimeshEvent : MSB1Event
{
    /// <summary>
    /// Region for navimesh
    /// </summary>
    public string RegionName;

    public void SetEvent(MsbEventNavimesh evt)
    {
        setBaseEvent(evt);
        RegionName = evt.NvmRegion;
    }

    public MsbEventNavimesh Serialize(GameObject parent)
    {
        var evt = new MsbEventNavimesh();
        _Serialize(evt, parent);
        evt.NvmRegion = (RegionName == "") ? null : RegionName;
        return evt;
    }
}
