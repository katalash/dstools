using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Routes/Muffling Box Link")]
public class MSBSMufflingBoxLinkRoute : MSBSRoute
{

    public void SetRoute(MSBS.Route.MufflingBoxLink route)
    {
        setBaseRoute(route);
    }

    public MSBS.Route.MufflingBoxLink Serialize(GameObject parent)
    {
        var model = new MSBS.Route.MufflingBoxLink();
        _Serialize(model, parent);
        return model;
    }
}
