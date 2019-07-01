using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

class NavMeshInstance : MonoBehaviour
{
    private Mesh NavmeshVisual;

    private void UpdateNavmeshVisual(NavMesh mesh)
    {
        NavmeshVisual = new Mesh();
        NavmeshVisual.vertices = mesh.Vertices;
        Vector3[] normals = new Vector3[mesh.Vertices.Length];
        for (var n = 0; n < normals.Length; n++)
        {
            normals[n] = new Vector3(0.0f, 1.0f, 0.0f);
        }
        NavmeshVisual.normals = normals;
        List<int> triangles = new List<int>();
        foreach (var face in mesh.Faces)
        {
            if (face.EdgeCount < 3)
                continue;
            for (int e = 0; e < 3; e++)
            {
                triangles.Add(mesh.Edges[face.StartEdgeIndex+e].A);
            }
        }
        NavmeshVisual.triangles = triangles.ToArray();
    }

    private NavMesh lastNavMesh;

    public NavMesh NavMesh;

    public void OnDrawGizmosSelected()
    {
        if (NavMesh != lastNavMesh)
        {
            lastNavMesh = NavMesh;
            UpdateNavmeshVisual(NavMesh);
        }
        if (NavmeshVisual != null)
        {
            //Gizmos.DrawWireMesh(NavmeshVisual, transform.position, transform.rotation);
            // Probably super slow but also super easy
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach (var e in NavMesh.Edges)
            {
                Gizmos.DrawLine(NavMesh.Vertices[e.A], NavMesh.Vertices[e.B]);
            }
        }
    }
}

