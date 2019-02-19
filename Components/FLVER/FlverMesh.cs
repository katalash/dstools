using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

/// <summary>
/// A component attached to a gameobject that "instances" a flver. Used to help link with ds/unity asset management and edit lightmapping stuff
/// </summary>
class FlverMesh : MonoBehaviour
{
    /// <summary>
    /// Direct link to the asset link for updating stuff
    /// </summary>
    public FLVERAssetLink Link;
}
