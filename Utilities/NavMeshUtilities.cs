using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;
using UnityEngine;
using UnityEditor;

class NavMeshUtilities
{
    public static void ImportNavimeshHKX(HKX hkx, string assetName)
    {
        NavMesh nm = new NavMesh();
        List<NavMesh.Edge> edges = new List<NavMesh.Edge>();
        List<NavMesh.Face> faces = new List<NavMesh.Face>();
        List<Vector3> vertices = new List<Vector3>();

        foreach (var cl in hkx.DataSection.Objects)
        {
            if (cl is HKX.HKAINavMesh)
            {
                var mesh = (HKX.HKAINavMesh)cl;
                foreach (var f in mesh.Faces.GetArrayData().Elements)
                {
                    var face = new NavMesh.Face();
                    face.StartEdgeIndex = f.StartEdgeIndex;
                    face.EdgeCount = f.NumEdges;
                    faces.Add(face);
                }
                foreach (var e in mesh.Edges.GetArrayData().Elements)
                {
                    var edge = new NavMesh.Edge();
                    edge.A = e.A;
                    edge.B = e.B;
                    edges.Add(edge);
                }
                foreach (var v in mesh.Vertices.GetArrayData().Elements)
                {
                    vertices.Add(new Vector3(v.Vector.X, v.Vector.Y, v.Vector.Z));
                }
                nm.Faces = faces.ToArray();
                nm.Edges = edges.ToArray();
                nm.Vertices = vertices.ToArray();
                break;
            }
        }
        AssetDatabase.CreateAsset(nm, assetName + ".asset");
    }
}
