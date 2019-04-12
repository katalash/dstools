using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Object")]
public class MSBSObjectPart : MSBSDummyObjectPart
{
    public MSBSUnkStruct1Part Unk1;


    public void SetPart(MSBS.Part.Object part)
    {
        base.SetPart(part);
        Unk1 = gameObject.AddComponent<MSBSUnkStruct1Part>();
        Unk1.setStruct(part.Unk1);
    }

    public new MSBS.Part.Object Serialize(GameObject parent)
    {
        var part = new MSBS.Part.Object();
        _Serialize(part, parent);
        part.Unk5 = Unk5.Serialize();
        part.CollisionPartName1 = CollisionPartName1;
        part.UnkT0C = UnkT0C;
        part.EnableObjAnimNetSyncStructure = EnableObjAnimNetSyncStructure;
        part.UnkT0E = UnkT0E;
        part.SetMainObjStructureBooleans = SetMainObjStructureBooleans;
        part.AnimID = AnimID;
        part.UnkT18 = UnkT18;
        part.UnkT1A = UnkT1A;
        part.UnkT20 = UnkT20;
        part.CollisionPartName2 = CollisionPartName2;
        part.Unk1 = Unk1.Serialize();
        return part;
    }
}
