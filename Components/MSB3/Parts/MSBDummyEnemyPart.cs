using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3DummyEnemyPart : MSB3EnemyPart
{
    public new MSB3.Part.DummyEnemy Serialize(GameObject parent)
    {
        var part = new MSB3.Part.DummyEnemy(ID, parent.name);
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
        part.UnkT04 = UnkT04;
        part.UnkT07 = UnkT07;
        part.UnkT08 = UnkT08;
        part.UnkT09 = UnkT09;
        part.UnkT10 = UnkT10;
        part.UnkT11 = UnkT11;
        part.UnkT12 = UnkT12;
        part.UnkT13 = UnkT13;
        part.UnkT14 = UnkT14;
        part.UnkT15 = UnkT15;
        part.UnkT16 = UnkT16;
        part.UnkT17 = UnkT17;
        part.UnkT18 = UnkT18;
        part.UnkT19 = UnkT19;
        return part;
    }
}
