using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Walk Route")]
public class MSBSWalkRouteEvent : MSBSEvent
{

    public int UnkT00;
    public string[] WalkRegionNames;

    public string RegionName1;
    public int Unk041;
    public int Unk081;
    public string RegionName2;
    public int Unk042;
    public int Unk082;
    public string RegionName3;
    public int Unk043;
    public int Unk083;
    public string RegionName4;
    public int Unk044;
    public int Unk084;
    public string RegionName5;
    public int Unk045;
    public int Unk085;



    public void SetEvent(MSBS.Event.WalkRoute evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        WalkRegionNames = evt.WalkRegionNames;
        RegionName1 = evt.WREntries[0].RegionName;
        Unk041 = evt.WREntries[0].Unk04;
        Unk081 = evt.WREntries[0].Unk08;
        RegionName2 = evt.WREntries[1].RegionName;
        Unk042 = evt.WREntries[1].Unk04;
        Unk082 = evt.WREntries[1].Unk08;
        RegionName3 = evt.WREntries[2].RegionName;
        Unk043 = evt.WREntries[2].Unk04;
        Unk083 = evt.WREntries[2].Unk08;
        RegionName4 = evt.WREntries[3].RegionName;
        Unk044 = evt.WREntries[3].Unk04;
        Unk084 = evt.WREntries[3].Unk08;
        RegionName5 = evt.WREntries[4].RegionName;
        Unk045 = evt.WREntries[4].Unk04;
        Unk085 = evt.WREntries[4].Unk08;
    }

    public MSBS.Event.WalkRoute Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.WalkRoute();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        for (int i = 0; i < 32; i++)
        {
            if (i >= WalkRegionNames.Length)
                break;
            evt.WalkRegionNames[i] = (WalkRegionNames[i] == "") ? null : WalkRegionNames[i];
        }
        evt.WREntries[0].RegionName = RegionName1;
        evt.WREntries[0].Unk04 = Unk041;
        evt.WREntries[0].Unk08 = Unk081;
        evt.WREntries[1].RegionName = RegionName2;
        evt.WREntries[1].Unk04 = Unk042;
        evt.WREntries[1].Unk08 = Unk082;
        evt.WREntries[2].RegionName = RegionName3;
        evt.WREntries[2].Unk04 = Unk043;
        evt.WREntries[2].Unk08 = Unk083;
        evt.WREntries[3].RegionName = RegionName4;
        evt.WREntries[3].Unk04 = Unk044;
        evt.WREntries[3].Unk08 = Unk084;
        evt.WREntries[4].RegionName = RegionName5;
        evt.WREntries[4].Unk04 = Unk045;
        evt.WREntries[4].Unk08 = Unk085;
        return evt;
    }
}
