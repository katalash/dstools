using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEditor;
using SoulsFormats;
using Object = UnityEngine.Object;

/// <summary>
/// Collision import/export utilities
/// </summary>
class CollisionUtilities
{
    public static void ImportDS3CollisionHKX(HKX hkx, string assetName)
    {
        var verts = new List<Vector3>();
        var normals = new List<Vector3>();
        var indices = new List<int>();

        foreach (var col in hkx.DataSection.Objects)
        {
            if (col is HKX.FSNPCustomParamCompressedMeshShape)
            {
                var meshdata = (HKX.FSNPCustomParamCompressedMeshShape)col;
                var coldata = meshdata.GetMeshShapeData();

                foreach (var chunk in coldata.Chunks.GetArrayData().Elements)
                {
                    for (int i = 0; i < chunk.ByteIndicesLength; i++)
                    {
                        var tri = coldata.MeshIndices.GetArrayData().Elements[i + chunk.ByteIndicesIndex];
                        if (tri.Idx2 == tri.Idx3 && tri.Idx1 != tri.Idx2)
                        {
                            if (tri.Idx0 < chunk.VertexIndicesLength)
                            {
                                ushort index = (ushort)((uint)tri.Idx0 + chunk.SmallVerticesBase);
                                indices.Add(verts.Count);

                                var vert = coldata.SmallVertices.GetArrayData().Elements[index].Decompress(chunk.SmallVertexScale, chunk.SmallVertexOffset);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }
                            else
                            {
                                ushort index = (ushort)(coldata.VertexIndices.GetArrayData().Elements[tri.Idx0 + chunk.VertexIndicesIndex - chunk.VertexIndicesLength].data);
                                indices.Add(verts.Count);

                                var vert = coldata.LargeVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }

                            if (tri.Idx1 < chunk.VertexIndicesLength)
                            {
                                ushort index = (ushort)((uint)tri.Idx1 + chunk.SmallVerticesBase);
                                indices.Add(verts.Count);

                                var vert = coldata.SmallVertices.GetArrayData().Elements[index].Decompress(chunk.SmallVertexScale, chunk.SmallVertexOffset);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }
                            else
                            {
                                ushort index = (ushort)(coldata.VertexIndices.GetArrayData().Elements[tri.Idx1 + chunk.VertexIndicesIndex - chunk.VertexIndicesLength].data);
                                indices.Add(verts.Count);

                                var vert = coldata.LargeVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }

                            if (tri.Idx2 < chunk.VertexIndicesLength)
                            {
                                ushort index = (ushort)((uint)tri.Idx2 + chunk.SmallVerticesBase);
                                indices.Add(verts.Count);

                                var vert = coldata.SmallVertices.GetArrayData().Elements[index].Decompress(chunk.SmallVertexScale, chunk.SmallVertexOffset);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }
                            else
                            {
                                ushort index = (ushort)(coldata.VertexIndices.GetArrayData().Elements[tri.Idx2 + chunk.VertexIndicesIndex - chunk.VertexIndicesLength].data);
                                indices.Add(verts.Count);

                                var vert = coldata.LargeVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                                verts.Add(new Vector3(vert.X, vert.Y, vert.Z));
                            }
                        }
                    }
                }
            }
        }

        if (indices.Count == 0)
            return;

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.subMeshCount = 1;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices.ToArray(), 0, true);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        AssetDatabase.CreateAsset(mesh, assetName + ".mesh");

        // Setup a game object asset
        GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName));
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>();
        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshRenderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/dstools/Materials/CollisionMeshMaterial.mat");

        PrefabUtility.SaveAsPrefabAsset(obj, assetName + ".prefab");
        Object.DestroyImmediate(obj);
    }

    public static void ImportDS1CollisionHKX(HKX hkx, string assetName)
    {
        // Setup a game object asset
        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(assetName));

        if (!AssetDatabase.IsValidFolder(assetName))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(assetName + ".blah"), Path.GetFileNameWithoutExtension(assetName + ".blah"));
        }

        int index = 0;
        foreach (var col in hkx.DataSection.Objects)
        {
            if (col is HKX.HKPStorageExtendedMeshShapeMeshSubpartStorage)
            {
                var verts = new List<Vector3>();
                var normals = new List<Vector3>();
                var indices = new List<int>();

                var coldata = (HKX.HKPStorageExtendedMeshShapeMeshSubpartStorage)col;

                for (int i = 0; i < coldata.Indices16.Size/4; i++)
                {
                    var vert0 = coldata.Vertices.GetArrayData().Elements[coldata.Indices16.GetArrayData().Elements[i*4].data];
                    var vert1 = coldata.Vertices.GetArrayData().Elements[coldata.Indices16.GetArrayData().Elements[i*4+1].data];
                    var vert2 = coldata.Vertices.GetArrayData().Elements[coldata.Indices16.GetArrayData().Elements[i*4+2].data];
                    verts.Add(new Vector3(vert0.Vector.X, vert0.Vector.Y, vert0.Vector.Z));
                    verts.Add(new Vector3(vert1.Vector.X, vert1.Vector.Y, vert1.Vector.Z));
                    verts.Add(new Vector3(vert2.Vector.X, vert2.Vector.Y, vert2.Vector.Z));
                    indices.Add(i*3);
                    indices.Add(i*3+1);
                    indices.Add(i*3+2);
                }

                var mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.subMeshCount = 1;
                mesh.SetVertices(verts);
                mesh.SetTriangles(indices.ToArray(), 0, true);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                AssetDatabase.CreateAsset(mesh, assetName + "/" + Path.GetFileNameWithoutExtension(assetName) + "_" + index + ".mesh");

                // Setup a game object asset
                GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName) + $@"_{index}");
                obj.AddComponent<MeshFilter>();
                obj.AddComponent<MeshRenderer>();
                obj.GetComponent<MeshFilter>().mesh = mesh;
                obj.GetComponent<MeshRenderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/dstools/Materials/CollisionMeshMaterial.mat");
                obj.transform.parent = root.transform;

                index++;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(root, assetName + ".prefab");
        Object.DestroyImmediate(root);
    }
}
