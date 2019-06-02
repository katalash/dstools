using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Parts/Dummy Enemy")]
public class MSBBBDummyEnemyPart : MSBBBEnemyPart
{
    public new MSBBB.Part.DummyEnemy Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.DummyEnemy(parent.name);
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
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
        return part;
    }
}
