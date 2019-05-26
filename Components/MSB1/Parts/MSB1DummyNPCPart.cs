using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

public class MSB1DummyNPCPart : MSB1NPCPart
{
    [AddComponentMenu("Dark Souls 1/Parts/Dummy NPC")]
    public new MsbPartsNPCDummy Serialize(GameObject parent)
    {
        var part = new MsbPartsNPCDummy();
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
