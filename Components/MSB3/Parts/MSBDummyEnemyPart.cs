using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Dummy Enemy")]
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
        part.ChrManipulatorAllocationParameter = ChrManipulatorAllocationParameter;
        part.UnkT20 = UnkT20;
        part.BackupEventAnimID = BackupEventAnimID;
        part.UnkT78 = UnkT78;
        part.UnkT84 = UnkT84;
        part.UnkT8C = UnkT8C;
        part.UnkT94 = UnkT94;
        part.UnkT9C = UnkT9C;
        part.UnkTA4 = UnkTA4;
        part.UnkTAC = UnkTAC;
        part.UnkTC0 = UnkTC0;
        part.UnkTC4 = UnkTC4;
        part.UnkTC8 = UnkTC8;
        part.UnkTCC = UnkTCC;
        return part;
    }
}
