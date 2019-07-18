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
        NavMesh nm = ScriptableObject.CreateInstance<NavMesh>();
        List<NavMesh.Edge> edges = new List<NavMesh.Edge>();
        List<NavMesh.Face> faces = new List<NavMesh.Face>();
        List<Vector3> vertices = new List<Vector3>();
        List<NavMesh.CostGraphNode> cgnodes = new List<NavMesh.CostGraphNode>();
        List<NavMesh.CostGraphEdge> cgedges = new List<NavMesh.CostGraphEdge>();

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
                continue;
            }
            else if (cl is HKX.HKCDStaticAABBTreeStorage)
            {
                var tree = (HKX.HKCDStaticAABBTreeStorage)cl;
                var nodes = tree.CompressedTree.GetArrayData().Elements;
                var unpacked = new NavMesh.AABBTreeNode[nodes.Count];
                for (uint i = 0; i < unpacked.Length; i++)
                {
                    unpacked[i] = new NavMesh.AABBTreeNode();
                    unpacked[i].id = i;
                }
                unpacked[0].Min = new Vector3(tree.AABBMin.X, tree.AABBMin.Y, tree.AABBMin.Z);
                unpacked[0].Max = new Vector3(tree.AABBMax.X, tree.AABBMax.Y, tree.AABBMax.Z);
                // Propogate the decompression down the tree
                for (uint i = 0; i < unpacked.Length; i++)
                {
                    var cnode = nodes[(int)i];
                    if ((cnode.IDX0 & 0x80) > 0)
                    {
                        uint left = i + 1;
                        uint right = i + ((((uint)cnode.IDX0 & 0x7F) << 8) | (uint)cnode.IDX1) * 2;
                        unpacked[(int)left].Min = nodes[(int)left].DecompressMin(unpacked[i].Min, unpacked[i].Max);
                        unpacked[(int)left].Max = nodes[(int)left].DecompressMax(unpacked[i].Min, unpacked[i].Max);
                        unpacked[(int)right].Min = nodes[(int)right].DecompressMin(unpacked[i].Min, unpacked[i].Max);
                        unpacked[(int)right].Max = nodes[(int)right].DecompressMax(unpacked[i].Min, unpacked[i].Max);
                        unpacked[i].Left = (int)left;
                        unpacked[i].Right = (int)right;
                        unpacked[i].IsTerminal = false;
                    }
                    else
                    {
                        unpacked[i].IsTerminal = true;
                        unpacked[i].Left = -1;
                        unpacked[i].Right = -1;
                        unpacked[i].Index = (((uint)cnode.IDX0 & 0x7F) << 8) | (uint)cnode.IDX1;
                    }
                }
                nm.AABBTree = unpacked;
            }
            else if (cl is HKX.HKAIDirectedGraphExplicitCost)
            {
                var graph = (HKX.HKAIDirectedGraphExplicitCost)cl;
                int i = 0;
                foreach (var n in graph.Nodes.GetArrayData().Elements)
                {
                    var node = new NavMesh.CostGraphNode();
                    node.EdgeCount = n.NumEdges;
                    node.StartEdgeIndex = n.StartEdgeIndex;
                    var pos = graph.Positions.GetArrayData().Elements[i].Vector;
                    node.Position = new Vector3(pos.X, pos.Y, pos.Z);
                    cgnodes.Add(node);
                    i++;
                }
                foreach (var e in graph.Edges.GetArrayData().Elements)
                {
                    var edge = new NavMesh.CostGraphEdge();
                    edge.Flags = e.Flags;
                    edge.TargetNode = e.TargetNode;
                    var floatbytes = BitConverter.GetBytes(((uint)e.Cost) << 16);
                    edge.Cost = BitConverter.ToSingle(floatbytes, 0);
                    cgedges.Add(edge);
                }
                nm.CostGraphEdges = cgedges.ToArray();
                nm.CostGraphNodes = cgnodes.ToArray();
            }
        }
        AssetDatabase.CreateAsset(nm, assetName + ".asset");
    }
}
