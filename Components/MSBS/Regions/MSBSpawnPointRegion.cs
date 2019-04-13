using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Spawn Point")]
public class MSBSSpawnPointRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.SpawnPoint region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.SpawnPoint Serialize(GameObject parent)
    {
        var region = new MSBS.Region.SpawnPoint();
        _Serialize(region, parent);
        return region;
    }
}
