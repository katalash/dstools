using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Generator")]
public class MSB1GeneratorEvent : MSB1Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public short MaxNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short LimitNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short MinGenNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short MaxGenNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float MinInterval;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float MaxInterval;

    /// <summary>
    /// Regions that enemies can be spawned at.
    /// </summary>
    public string SpawnPointName1;
    public string SpawnPointName2;
    public string SpawnPointName3;
    public string SpawnPointName4;

    /// <summary>
    /// Enemies spawned by this generator.
    /// </summary>
    public string SpawnPartName1;
    public string SpawnPartName2;
    public string SpawnPartName3;
    public string SpawnPartName4;
    public string SpawnPartName5;
    public string SpawnPartName6;
    public string SpawnPartName7;
    public string SpawnPartName8;
    public string SpawnPartName9;
    public string SpawnPartName10;
    public string SpawnPartName11;
    public string SpawnPartName12;
    public string SpawnPartName13;
    public string SpawnPartName14;
    public string SpawnPartName15;
    public string SpawnPartName16;
    public string SpawnPartName17;
    public string SpawnPartName18;
    public string SpawnPartName19;
    public string SpawnPartName20;
    public string SpawnPartName21;
    public string SpawnPartName22;
    public string SpawnPartName23;
    public string SpawnPartName24;
    public string SpawnPartName25;
    public string SpawnPartName26;
    public string SpawnPartName27;
    public string SpawnPartName28;
    public string SpawnPartName29;
    public string SpawnPartName30;
    public string SpawnPartName31;
    public string SpawnPartName32;

    public void SetEvent(MsbEventGenerator evt)
    {
        setBaseEvent(evt);
        MaxNum = evt.MaxNum;
        LimitNum = evt.LimitNum;
        MinGenNum = evt.MinGenNum;
        MaxGenNum = evt.MaxGenNum;
        MinInterval = evt.MinInterval;
        MaxInterval = evt.MaxInterval;
        SpawnPointName1 = evt.SpawnPoint1;
        SpawnPointName2 = evt.SpawnPoint2;
        SpawnPointName3 = evt.SpawnPoint3;
        SpawnPointName4 = evt.SpawnPoint4;
        SpawnPartName1 = evt.SpawnPart1;
        SpawnPartName2 = evt.SpawnPart2;
        SpawnPartName3 = evt.SpawnPart3;
        SpawnPartName4 = evt.SpawnPart4;
        SpawnPartName5 = evt.SpawnPart5;
        SpawnPartName6 = evt.SpawnPart6;
        SpawnPartName7 = evt.SpawnPart7;
        SpawnPartName8 = evt.SpawnPart8;
        SpawnPartName9 = evt.SpawnPart9;
        SpawnPartName10 = evt.SpawnPart10;
        SpawnPartName11 = evt.SpawnPart11;
        SpawnPartName12 = evt.SpawnPart12;
        SpawnPartName13 = evt.SpawnPart13;
        SpawnPartName14 = evt.SpawnPart14;
        SpawnPartName15 = evt.SpawnPart15;
        SpawnPartName16 = evt.SpawnPart16;
        SpawnPartName17 = evt.SpawnPart17;
        SpawnPartName18 = evt.SpawnPart18;
        SpawnPartName19 = evt.SpawnPart19;
        SpawnPartName20 = evt.SpawnPart20;
        SpawnPartName21 = evt.SpawnPart21;
        SpawnPartName22 = evt.SpawnPart22;
        SpawnPartName23 = evt.SpawnPart23;
        SpawnPartName24 = evt.SpawnPart24;
        SpawnPartName25 = evt.SpawnPart25;
        SpawnPartName26 = evt.SpawnPart26;
        SpawnPartName27 = evt.SpawnPart27;
        SpawnPartName28 = evt.SpawnPart28;
        SpawnPartName29 = evt.SpawnPart29;
        SpawnPartName30 = evt.SpawnPart30;
        SpawnPartName31 = evt.SpawnPart31;
        SpawnPartName32 = evt.SpawnPart32;
    }

    public MsbEventGenerator Serialize(GameObject parent)
    {
        var evt = new MsbEventGenerator();
        _Serialize(evt, parent);
        evt.MaxNum = MaxNum;
        evt.LimitNum = LimitNum;
        evt.MinGenNum = MinGenNum;
        evt.MaxGenNum = MaxGenNum;
        evt.MinInterval = MinInterval;
        evt.MaxInterval = MaxInterval;
        evt.SpawnPoint1 = (SpawnPointName1 == "") ? null : SpawnPointName1;
        evt.SpawnPoint2 = (SpawnPointName2 == "") ? null : SpawnPointName2;
        evt.SpawnPoint3 = (SpawnPointName3 == "") ? null : SpawnPointName3;
        evt.SpawnPoint4 = (SpawnPointName4 == "") ? null : SpawnPointName4;

        evt.SpawnPart1 = (SpawnPartName1 == "") ? null : SpawnPartName1;
        evt.SpawnPart2 = (SpawnPartName2 == "") ? null : SpawnPartName2;
        evt.SpawnPart3 = (SpawnPartName3 == "") ? null : SpawnPartName3;
        evt.SpawnPart4 = (SpawnPartName4 == "") ? null : SpawnPartName4;
        evt.SpawnPart5 = (SpawnPartName5 == "") ? null : SpawnPartName5;
        evt.SpawnPart6 = (SpawnPartName6 == "") ? null : SpawnPartName6;
        evt.SpawnPart7 = (SpawnPartName7 == "") ? null : SpawnPartName7;
        evt.SpawnPart8 = (SpawnPartName8 == "") ? null : SpawnPartName8;
        evt.SpawnPart9 = (SpawnPartName9 == "") ? null : SpawnPartName9;
        evt.SpawnPart10 = (SpawnPartName10 == "") ? null : SpawnPartName10;
        evt.SpawnPart11 = (SpawnPartName11 == "") ? null : SpawnPartName11;
        evt.SpawnPart12 = (SpawnPartName12 == "") ? null : SpawnPartName12;
        evt.SpawnPart13 = (SpawnPartName13 == "") ? null : SpawnPartName13;
        evt.SpawnPart14 = (SpawnPartName14 == "") ? null : SpawnPartName14;
        evt.SpawnPart15 = (SpawnPartName15 == "") ? null : SpawnPartName15;
        evt.SpawnPart16 = (SpawnPartName16 == "") ? null : SpawnPartName16;
        evt.SpawnPart17 = (SpawnPartName17 == "") ? null : SpawnPartName17;
        evt.SpawnPart18 = (SpawnPartName18 == "") ? null : SpawnPartName18;
        evt.SpawnPart19 = (SpawnPartName19 == "") ? null : SpawnPartName19;
        evt.SpawnPart20 = (SpawnPartName20 == "") ? null : SpawnPartName20;
        evt.SpawnPart21 = (SpawnPartName21 == "") ? null : SpawnPartName21;
        evt.SpawnPart22 = (SpawnPartName22 == "") ? null : SpawnPartName22;
        evt.SpawnPart23 = (SpawnPartName23 == "") ? null : SpawnPartName23;
        evt.SpawnPart24 = (SpawnPartName24 == "") ? null : SpawnPartName24;
        evt.SpawnPart25 = (SpawnPartName25 == "") ? null : SpawnPartName25;
        evt.SpawnPart26 = (SpawnPartName26 == "") ? null : SpawnPartName26;
        evt.SpawnPart27 = (SpawnPartName27 == "") ? null : SpawnPartName27;
        evt.SpawnPart28 = (SpawnPartName28 == "") ? null : SpawnPartName28;
        evt.SpawnPart29 = (SpawnPartName29 == "") ? null : SpawnPartName29;
        evt.SpawnPart30 = (SpawnPartName30 == "") ? null : SpawnPartName30;
        evt.SpawnPart31 = (SpawnPartName31 == "") ? null : SpawnPartName31;
        evt.SpawnPart32 = (SpawnPartName32 == "") ? null : SpawnPartName32;
        return evt;
    }
}
