using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;

public class MSBSCompositeShape : MonoBehaviour
{
    public string ChildRegionName1;
    public int Unk041;
    public string ChildRegionName2;
    public int Unk042;
    public string ChildRegionName3;
    public int Unk043;
    public string ChildRegionName4;
    public int Unk044;
    public string ChildRegionName5;
    public int Unk045;
    public string ChildRegionName6;
    public int Unk046;
    public string ChildRegionName7;
    public int Unk047;
    public string ChildRegionName8;
    public int Unk048;



    public void setShape(MSBS.Shape.Composite shape)
    {
        ChildRegionName1 = shape.Children[0].RegionName;
        Unk041 = shape.Children[0].Unk04;
        ChildRegionName2 = shape.Children[1].RegionName;
        Unk042 = shape.Children[1].Unk04;
        ChildRegionName3 = shape.Children[2].RegionName;
        Unk043 = shape.Children[2].Unk04;
        ChildRegionName4 = shape.Children[3].RegionName;
        Unk044 = shape.Children[3].Unk04;
        ChildRegionName5 = shape.Children[4].RegionName;
        Unk045 = shape.Children[4].Unk04;
        ChildRegionName6 = shape.Children[5].RegionName;
        Unk046 = shape.Children[5].Unk04;
        ChildRegionName7 = shape.Children[6].RegionName;
        Unk047 = shape.Children[6].Unk04;
        ChildRegionName8 = shape.Children[7].RegionName;
        Unk048 = shape.Children[7].Unk04;
    }

    
    public MSBS.Shape.Composite Serialize()
    {
        MSBS.Shape.Composite shape = new MSBS.Shape.Composite();
        shape.Children[0].RegionName = ChildRegionName1;
        shape.Children[0].Unk04 = Unk041;
        shape.Children[1].RegionName = ChildRegionName2;
        shape.Children[1].Unk04 = Unk042;
        shape.Children[2].RegionName = ChildRegionName3;
        shape.Children[2].Unk04 = Unk043;
        shape.Children[3].RegionName = ChildRegionName4;
        shape.Children[3].Unk04 = Unk044;
        shape.Children[4].RegionName = ChildRegionName5;
        shape.Children[4].Unk04 = Unk045;
        shape.Children[5].RegionName = ChildRegionName6;
        shape.Children[5].Unk04 = Unk046;
        shape.Children[6].RegionName = ChildRegionName7;
        shape.Children[6].Unk04 = Unk047;
        shape.Children[7].RegionName = ChildRegionName8;
        shape.Children[7].Unk04 = Unk048;
        return shape;
    }
}
