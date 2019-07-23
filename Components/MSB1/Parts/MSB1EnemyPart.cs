using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Enemy")]
public class MSB1EnemyPart : MSB1Part
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// Controls enemy AI.
    /// </summary>
    public int ThinkParamID;

    /// <summary>
    /// Controls enemy stats.
    /// </summary>
    public int NPCParamID;

    /// <summary>
    /// Controls enemy speech.
    /// </summary>
    public int TalkID;

    public float UnkT14;

    /// <summary>
    /// Controls enemy equipment.
    /// </summary>
    public int CharaInitID;

    public string MovePoint1;
    public string MovePoint2;
    public string MovePoint3;
    public string MovePoint4;

    public int InitAnimID;
    public int ButterflyAnimUnk;

    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.Enemy)bpart;
        setBasePart(part);
        CollisionName = part.CollisionName;
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        TalkID = part.TalkID;
        UnkT14 = part.UnkT14;
        CharaInitID = part.CharaInitID;
        MovePoint1 = part.MovePointNames[0];
        MovePoint2 = part.MovePointNames[1];
        MovePoint3 = part.MovePointNames[2];
        MovePoint4 = part.MovePointNames[3];
        InitAnimID = part.UnkT38;
        ButterflyAnimUnk = part.UnkT3C;
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.Enemy();
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.UnkT14 = UnkT14;
        part.CharaInitID = CharaInitID;
        part.MovePointNames[0] = (MovePoint1 == "") ? null : MovePoint1;
        part.MovePointNames[1] = (MovePoint2 == "") ? null : MovePoint2;
        part.MovePointNames[2] = (MovePoint3 == "") ? null : MovePoint3;
        part.MovePointNames[3] = (MovePoint4 == "") ? null : MovePoint4;
        part.UnkT38 = InitAnimID;
        part.UnkT3C = ButterflyAnimUnk;
        return part;
    }
}
