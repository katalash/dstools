using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/NPC")]
public class MSB1NPCPart : MSB1Part
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

    /// <summary>
    /// Controls enemy equipment.
    /// </summary>
    public int CharaInitID;

    public string MovePoint1;
    public string MovePoint2;
    public string MovePoint3;
    public string MovePoint4;

    public byte Unk1, Unk2, Unk3;
    public sbyte Unk4, Unk5, Unk6, Unk7, Unk8, Unk9, Unk10, Unk11;

    public int InitAnimID;
    public int ButterflyAnimUnk;

    public void SetPart(MsbPartsNPC part)
    {
        setBasePart(part);
        CollisionName = part.HitName;
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        TalkID = part.TalkID;
        CharaInitID = part.CharaInitID;
        MovePoint1 = part.MovePoint1;
        MovePoint2 = part.MovePoint2;
        MovePoint3 = part.MovePoint3;
        MovePoint4 = part.MovePoint4;
        Unk1 = part.SubUnk1;
        Unk2 = part.SubUnk2;
        Unk3 = part.SubUnk3;
        Unk4 = part.SubUnk4;
        Unk5 = part.SubUnk5;
        Unk6 = part.SubUnk6;
        Unk7 = part.SubUnk7;
        Unk8 = part.SubUnk8;
        Unk9 = part.SubUnk9;
        Unk10 = part.SubUnk10;
        Unk11 = part.SubUnk11;
        InitAnimID = part.InitAnimID;
        ButterflyAnimUnk = part.m17_Butterfly_Anim_Unk;
    }

    public MsbPartsNPC Serialize(GameObject parent)
    {
        var part = new MsbPartsNPC();
        _Serialize(part, parent);
        part.HitName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
        part.MovePoint1 = (MovePoint1 == "") ? null : MovePoint1;
        part.MovePoint2 = (MovePoint2 == "") ? null : MovePoint2;
        part.MovePoint3 = (MovePoint3 == "") ? null : MovePoint3;
        part.MovePoint4 = (MovePoint4 == "") ? null : MovePoint4;
        part.SubUnk1 = Unk1;
        part.SubUnk2 = Unk2;
        part.SubUnk3 = Unk3;
        part.SubUnk4 = Unk4;
        part.SubUnk5 = Unk5;
        part.SubUnk6 = Unk6;
        part.SubUnk7 = Unk7;
        part.SubUnk8 = Unk8;
        part.SubUnk9 = Unk9;
        part.SubUnk10 = Unk10;
        part.SubUnk11 = Unk11;
        part.InitAnimID = InitAnimID;
        part.m17_Butterfly_Anim_Unk = ButterflyAnimUnk;
        return part;
    }
}
