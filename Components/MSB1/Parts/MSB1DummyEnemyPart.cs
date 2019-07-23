using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB1DummyEnemyPart : MSB1EnemyPart
{
    [AddComponentMenu("Dark Souls 1/Parts/Dummy NPC")]
    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.DummyEnemy();
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
