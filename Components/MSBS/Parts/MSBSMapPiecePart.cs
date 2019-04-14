using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Map Piece")]
public class MSBSMapPiecePart : MSBSPart
{

    public MSBSUnkStruct1Part Unk1;
    public MSBSUnkStruct5Part Unk5;
    public int Unk00;
    public int Unk04;
    public int Unk08;
    public int Unk0C;
    public int Unk10;
    public int Unk14;

    public void SetPart(MSBS.Part.MapPiece part)
    {
        setBasePart(part);
        Unk1 = gameObject.AddComponent<MSBSUnkStruct1Part>();
        Unk1.setStruct(part.Unk1);
        Unk5 = gameObject.AddComponent<MSBSUnkStruct5Part>();
        Unk5.setStruct(part.Unk5);
        Unk00 = part.Unk7.Unk00;
        Unk04 = part.Unk7.Unk04;
        Unk08 = part.Unk7.Unk08;
        Unk0C = part.Unk7.Unk0C;
        Unk10 = part.Unk7.Unk10;
        Unk14 = part.Unk7.Unk14;

    }


    public MSBS.Part.MapPiece Serialize(GameObject parent)
    {
        var part = new MSBS.Part.MapPiece();
        _Serialize(part, parent);
        part.Unk1 = Unk1.Serialize();
        part.Unk5 = Unk5.Serialize();
        part.Unk7 = new MSBS.Part.UnkStruct7();
        part.Unk7.Unk00 = Unk00;
        part.Unk7.Unk04 = Unk04;
        part.Unk7.Unk08 = Unk08;
        part.Unk7.Unk0C = Unk0C;
        part.Unk7.Unk10 = Unk10;
        part.Unk7.Unk14 = Unk14;
        return part;
    }
}
