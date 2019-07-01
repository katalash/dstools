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

    public Face[] Faces;
    public Edge[] Edges;
    public Vector3[] Vertices;
}
