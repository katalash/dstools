using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Generator")]
public class MSBSGeneratorEvent : MSBSEvent
{

    public short MaxNum;
    public short LimitNum;
    public short MinGenNum;
    public short MaxGenNum;
    public float MinInterval;
    public float MaxInterval;
    public int SessionCondition;
    public float UnkT14;
    public float UnkT18;

    public string SpawnRegionName1;
    public string SpawnRegionName2;
    public string SpawnRegionName3;
    public string SpawnRegionName4;
    public string SpawnRegionName5;
    public string SpawnRegionName6;
    public string SpawnRegionName7;
    public string SpawnRegionName8;

    public string[] SpawnPartNames;

    public void SetEvent(MSBS.Event.Generator evt)
    {
        setBaseEvent(evt);
        MaxNum = evt.MaxNum;
        LimitNum = evt.LimitNum;
        MinGenNum = evt.MinGenNum;
        MaxGenNum = evt.MaxGenNum;
        MinInterval = evt.MinInterval;
        MaxInterval = evt.MaxInterval;
        SessionCondition = evt.SessionCondition;
        UnkT14 = evt.UnkT14;
        UnkT18 = evt.UnkT18;

        SpawnRegionName1 = evt.SpawnRegionNames[0];
        SpawnRegionName2 = evt.SpawnRegionNames[1];
        SpawnRegionName3 = evt.SpawnRegionNames[2];
        SpawnRegionName4 = evt.SpawnRegionNames[3];
        SpawnRegionName5 = evt.SpawnRegionNames[4];
        SpawnRegionName6 = evt.SpawnRegionNames[5];
        SpawnRegionName7 = evt.SpawnRegionNames[6];
        SpawnRegionName8 = evt.SpawnRegionNames[7];

        SpawnPartNames = evt.SpawnPartNames;
    }

    public MSBS.Event.Generator Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Generator();
        _Serialize(evt, parent);
        evt.MaxNum = MaxNum;
        evt.LimitNum = LimitNum;
        evt.MinGenNum = MinGenNum;
        evt.MaxGenNum = MaxGenNum;
        evt.MinInterval = MinInterval;
        evt.MaxInterval = MaxInterval;
        evt.SessionCondition = SessionCondition;
        evt.UnkT14 = UnkT14;
        evt.UnkT18 = UnkT18;

        evt.SpawnRegionNames[0] = SpawnRegionName1;
        evt.SpawnRegionNames[1] = SpawnRegionName2;
        evt.SpawnRegionNames[2] = SpawnRegionName3;
        evt.SpawnRegionNames[3] = SpawnRegionName4;
        evt.SpawnRegionNames[4] = SpawnRegionName5;
        evt.SpawnRegionNames[5] = SpawnRegionName6;
        evt.SpawnRegionNames[6] = SpawnRegionName7;
        evt.SpawnRegionNames[7] = SpawnRegionName8;

        for (int i = 0; i < 32; i++)
        {
            if (i >= SpawnPartNames.Length)
            {
                evt.SpawnPartNames[i] = null;
                continue;
            }
            evt.SpawnPartNames[i] = (SpawnPartNames[i] == "") ? null : SpawnPartNames[i];
        }
        return evt;
    }
}
