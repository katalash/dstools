using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

/// <summary>
/// A component attached to the game object that has a mesh filter with a flver submesh attached.
/// Used for some bookkeeping and exposing controls to the user for Specific things
/// </summary>
class FlverSubmesh : MonoBehaviour
{
    /// <summary>
    /// Direct link to the asset link for updating stuff
    /// </summary>
    public FLVERAssetLink Link;

    /// <summary>
    /// Index of the submesh this represents
    /// </summary>
    public int SubmeshIdx;

    /*public void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        var normals = mesh.normals;
        var tangents = mesh.tangents;
        var pos = mesh.vertices;
        for (int i = 0; i < pos.Count(); i++)
        {
            Gizmos.DrawRay(pos[i], tangents[i] * tangents[i].w);
        }
    }*/
}
