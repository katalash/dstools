using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlverMesh))]
// Note that this doesn't edit multiple objects at a time because it's used for unwrapping UVs, which crashes
// Unity if you try and unwrap more than one submesh at a time :trashcat:
class FlverMeshEditor : Editor
{
    void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        FlverMesh submesh = (FlverMesh)target;
        // Long check to see if generating lightmap UVs is safe
        if (submesh.Link != null)
        {
            if (GUILayout.Button("Generate Lightmap UVs"))
            {
                submesh.Link.GenerateLightmapUVS();
            }

        }
    }
}