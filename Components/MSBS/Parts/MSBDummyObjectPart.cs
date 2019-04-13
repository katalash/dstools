using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Dummy Object")]
public class MSBSDummyObjectPart : MSBSPart
{
    public MSBSUnkStruct5Part Unk5;

    public string CollisionPartName1;
    public string CollisionPartName2;

    public bool EnableObjAnimNetSyncStructure;
    public bool SetMainObjStructureBooleans;

    public int UnkT20;

    public short AnimID;
    public short UnkT18;
    public short UnkT1A;

    public byte UnkT0C;
    public byte UnkT0E;

    public void SetPart(MSBS.Part.DummyObject part)
    {
        setBasePart(part);
        Unk5 = gameObject.AddComponent<MSBSUnkStruct5Part>();
        Unk5.setStruct(part.Unk5);
        CollisionPartName1 = part.CollisionPartName1;
        UnkT0C = part.UnkT0C;
        EnableObjAnimNetSyncStructure = part.EnableObjAnimNetSyncStructure;
        UnkT0E = part.UnkT0E;
        SetMainObjStructureBooleans = part.SetMainObjStructureBooleans;
        AnimID = part.AnimID;
        UnkT18 = part.UnkT18;
        UnkT1A = part.UnkT1A;
        UnkT20 = part.UnkT20;
        CollisionPartName2 = part.CollisionPartName2;
    }


    public MSBS.Part.DummyObject Serialize(GameObject parent)
    {
        var part = new MSBS.Part.DummyObject();
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
        return part;
    }
}
