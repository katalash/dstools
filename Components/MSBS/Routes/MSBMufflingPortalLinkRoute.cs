using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Routes/Muffling Portal Link")]
public class MSBSMufflingPortalLinkRoute : MSBSRoute
{

    public void SetRoute(MSBS.Route.MufflingPortalLink route)
    {
        setBaseRoute(route);
    }

    public MSBS.Route.MufflingPortalLink Serialize(GameObject parent)
    {
        var model = new MSBS.Route.MufflingPortalLink();
        _Serialize(model, parent);
        return model;
    }
}
