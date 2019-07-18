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

    public bool DebugDrawNavMesh = false;
    public bool DebugDrawCostGraph = false;
    public int DebugHighlightFace = -1;
    public int DebugHighlightEdge = -1;
    public int DebugHighlightAABBNode = -1;
    public int DebugHighlightCostGraphNode = -1;

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
            var highlightEdgeSet = new HashSet<NavMesh.Edge>();

            if (DebugHighlightFace > -1 && DebugHighlightFace < NavMesh.Faces.Length)
            {
                var face = NavMesh.Faces[DebugHighlightFace];
                Gizmos.color = Color.red;
                for (int i = 0; i < face.EdgeCount; i++)
                {
                    var e = NavMesh.Edges[face.StartEdgeIndex + i];
                    highlightEdgeSet.Add(e);
                    Gizmos.DrawLine(NavMesh.Vertices[e.A], NavMesh.Vertices[e.B]);
                }
                Gizmos.color = Color.white;
            }

            if (DebugHighlightEdge > -1 && DebugHighlightEdge < NavMesh.Edges.Length)
            {
                Gizmos.color = Color.red;
                var e = NavMesh.Edges[DebugHighlightEdge];
                highlightEdgeSet.Add(e);
                Gizmos.DrawLine(NavMesh.Vertices[e.A], NavMesh.Vertices[e.B]);
                Gizmos.color = Color.white;
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            if (DebugDrawNavMesh)
            {
                foreach (var e in NavMesh.Edges)
                {
                    if (!highlightEdgeSet.Contains(e))
                        Gizmos.DrawLine(NavMesh.Vertices[e.A], NavMesh.Vertices[e.B]);
                }
            }

            if (DebugDrawCostGraph)
            {
                foreach (var n in NavMesh.CostGraphNodes)
                {
                    for (int i = 0; i < n.EdgeCount; i++)
                    {
                        var edge = NavMesh.CostGraphEdges[n.StartEdgeIndex + i];
                        var to = NavMesh.CostGraphNodes[edge.TargetNode];
                        Gizmos.DrawLine(n.Position, to.Position);
                    }
                }
            }

            Gizmos.color = Color.red;
            foreach (var e in highlightEdgeSet)
            {
                Gizmos.DrawLine(NavMesh.Vertices[e.A], NavMesh.Vertices[e.B]);
            }
            Gizmos.color = Color.white;

            if (DebugHighlightAABBNode > -1 && DebugHighlightAABBNode < NavMesh.AABBTree.Length)
            {
                Gizmos.color = Color.blue;
                var n = NavMesh.AABBTree[DebugHighlightAABBNode];
                Gizmos.DrawWireCube(Vector3.Lerp(n.Min, n.Max, 0.5f), n.Max - n.Min);
                Gizmos.color = Color.white;
            }

            if (DebugHighlightCostGraphNode > -1 && DebugHighlightCostGraphNode < NavMesh.CostGraphNodes.Length)
            {
                Gizmos.color = Color.green;
                var n = NavMesh.CostGraphNodes[DebugHighlightCostGraphNode];
                Gizmos.DrawSphere(n.Position, 0.5f);
                for (int i = 0; i < n.EdgeCount; i++)
                {
                    var edge = NavMesh.CostGraphEdges[n.StartEdgeIndex + i];
                    var to = NavMesh.CostGraphNodes[edge.TargetNode];
                    Gizmos.DrawLine(n.Position, to.Position);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}

