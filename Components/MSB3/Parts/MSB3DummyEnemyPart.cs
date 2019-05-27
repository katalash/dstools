using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Dummy Enemy")]
public class MSB3DummyEnemyPart : MSB3EnemyPart
{
    public new MSB3.Part.DummyEnemy Serialize(GameObject parent)
    {
        var part = new MSB3.Part.DummyEnemy(parent.name);
        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
        part.UnkT04 = UnkT04;
        part.ChrManipulatorAllocationParameter = ChrManipulatorAllocationParameter;
        part.WalkRouteName = (WalkRouteName == "") ? null : WalkRouteName;
        part.BackupEventAnimID = BackupEventAnimID;
        part.UnkT78 = UnkT78;
        part.UnkT84 = UnkT84;
        return part;
    }
}
