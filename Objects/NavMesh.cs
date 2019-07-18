using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

// Extracted navmesh info from an hkx file
class NavMesh : ScriptableObject
{
    [System.Serializable]
    public struct Face
    {
        public int StartEdgeIndex;
        public int EdgeCount;
    }

    [System.Serializable]
    public struct Edge
    {
        public int A;
        public int B;
        public uint OppositeEdge;
        public uint OppositeFace;
        public byte Flags;
    }

    [System.Serializable]
    public struct AABBTreeNode
    {
        public uint id;

        public Vector3 Min;
        public Vector3 Max;

        public int Left;
        public int Right;

        public bool IsTerminal;
        public uint Index;
    }

    [System.Serializable]
    public struct CostGraphNode
    {
        public Vector3 Position;
        public int StartEdgeIndex;
        public int EdgeCount;
    }

    [System.Serializable]
    public struct CostGraphEdge
    {
        public float Cost;
        public ushort Flags;
        public uint TargetNode;
    }

    public Face[] Faces;
    public Edge[] Edges;
    public Vector3[] Vertices;
    public AABBTreeNode[] AABBTree;
    public CostGraphNode[] CostGraphNodes;
    public CostGraphEdge[] CostGraphEdges;
}
