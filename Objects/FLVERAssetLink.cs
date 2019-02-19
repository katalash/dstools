using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// An "asset" that is associated by name to a unity asset that defines a mesh, and is used to export it to a flver along with some export settings
/// </summary>
class FLVERAssetLink : ScriptableObject
{
    /// <summary>
    /// Defines the container/archive that the exported flver will be exported into
    /// </summary>
    public enum ContainerType
    {
        None,
        Mapbnd,
        Objbnd,
        Chrbnd,
    }

    public ContainerType Type = ContainerType.None;

    /// <summary>
    /// Interroot relative path to the archive that this asset is stored into
    /// </summary>
    public string ArchivePath = null;

    /// <summary>
    /// Path to the Flver. Either a virtual path for archived flvers, or an interroot relative path for naked flvers
    /// </summary>
    public string FlverPath = null;

    /// <summary>
    /// Triggers a full from scratch export of the flver. Note that this may break existing game meshes and should only be used for full new models.
    /// Partial submesh exports should be used for existing game models.
    /// </summary>
    public bool FullExport = false;

    /// <summary>
    /// Stores metadata about all the flver submeshes
    /// </summary>
    [System.Serializable]
    public class SubmeshInfo
    {
        /// <summary>
        /// Internal name for the submesh
        /// </summary>
        public string Name;

        /// <summary>
        /// Link the MTD asset link
        /// </summary>
        public MTDAssetLink Mtd;

        /// <summary>
        /// Describes the level of exporting this submesh. Lightmap UV only preserves everything else and tries to export the lightmap uvs only, and also tags for lightmap uv generation
        /// </summary>
        public enum MeshExportMode
        {
            NoExport,
            LightmapUVOnly,
            FullExport,
        }

        public MeshExportMode ExportMode = MeshExportMode.NoExport;
    }

    public List<SubmeshInfo> Submeshes = new List<SubmeshInfo>();

    /// <summary>
    /// Generates new lightmap UVs for a submesh and set export mode to LightmapUVOnly
    /// </summary>
    public void GenerateLightmapUVSForSubmesh(int submesh)
    {
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
        if (assetPath == null)
        {
            throw new Exception("This asset link appears to not have been serialized.");
        }

        if (submesh >= Submeshes.Count)
        {
            throw new Exception("Invalid submesh ID to unwrap uvs for.");
        }

        if (Submeshes[submesh].Mtd == null)
        {
            throw new Exception("Cannot generate lightmap UVs for submesh with no assigned MTD.");
        }

        if (Submeshes[submesh].Mtd.LightmapUVIndex == -1)
        {
            throw new Exception("This submesh is using a material that isn't lightmapped.");
        }

        //for (int i = 0; i < Submeshes.Count; i++)
        //{

        // Mark lightmaps for export
        Submeshes[submesh].ExportMode = SubmeshInfo.MeshExportMode.LightmapUVOnly;

        var submeshPath = $@"{assetPath}/{name}/{name}_{submesh}.mesh";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(submeshPath);
        if (mesh == null)
        {
            throw new Exception($@"Could not load underlying mesh for {submeshPath}.");
        }
        //var newmesh = Instantiate(mesh);
        FlverUtilities.UnindexMesh(mesh);
        //Unwrapping.GenerateSecondaryUVSet(newmesh);
        var lmuvs = Unwrapping.GeneratePerTriangleUV(mesh);
        mesh.SetUVs(1, lmuvs.ToList());
        mesh.UploadMeshData(false);
        AssetDatabase.SaveAssets();
        //AssetDatabase.CreateAsset(newmesh, $@"{assetPath}/{name}/{name}_{submesh}_2.mesh");
    }

    /// <summary>
    /// Generates lightmap uvs for an entire mesh (with the eligible lightmapped submeshes). Designed to fit everything into
    /// a single atlas.
    /// </summary>
    public void GenerateLightmapUVS()
    {
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

        List<Mesh> meshesToProcess = new List<Mesh>();
        List<int> meshIndexBases = new List<int>();

        // Find all the eligible meshes, mark them reexportable, and add them to the list
        for (int i = 0; i < Submeshes.Count; i++)
        {
            if (Submeshes[i].Mtd == null)
            {
                continue;
            }

            if (Submeshes[i].Mtd.LightmapUVIndex == -1)
            {
                continue;
            }

            var submeshPath = $@"{assetPath}/{name}/{name}_{i}.mesh";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(submeshPath);
            if (mesh == null)
            {
                throw new Exception($@"Could not load underlying mesh for {submeshPath}.");
            }

            Submeshes[i].ExportMode = SubmeshInfo.MeshExportMode.LightmapUVOnly;
            meshesToProcess.Add(mesh);
        }

        // Preprocess all the meshes
        foreach (var m in meshesToProcess)
        {
            FlverUtilities.UnindexMesh(m);
        }
        AssetDatabase.SaveAssets();

        // Create a giant mesh to do the unindexing with
        List<int> indices = new List<int>();
        List<Vector3> positions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();
        List<Vector2> uvs0 = new List<Vector2>();
        List<Vector2> uvs1 = new List<Vector2>();
        int currentIndex = 0;
        Mesh combined = new Mesh();
        foreach (var mesh in meshesToProcess)
        {
            meshIndexBases.Add(currentIndex);
            foreach (var pos in mesh.vertices)
            {
                positions.Add(pos);
            }

            foreach (var normal in mesh.normals)
            {
                normals.Add(normal);
            }

            foreach (var tangent in mesh.tangents)
            {
                tangents.Add(tangent);
            }

            foreach (var index in mesh.GetIndices(0))
            {
                indices.Add(index + currentIndex);
            }

            foreach (var uv in mesh.uv)
            {
                uvs0.Add(uv);
            }

            foreach (var uv in mesh.uv2)
            {
                uvs1.Add(uv);
            }

            currentIndex += mesh.vertices.Length;
        }

        // Do the unwrap
        combined.SetVertices(positions);
        combined.SetNormals(normals);
        combined.SetTangents(tangents);
        combined.SetUVs(0, uvs0);
        combined.SetUVs(1, uvs1);
        combined.SetTriangles(indices.ToArray(), 0);
        var lmuvs = Unwrapping.GeneratePerTriangleUV(combined);

        // Extract the uvs and apply back to the submeshes
        for (int i = 0; i < meshesToProcess.Count; i++)
        {
            meshesToProcess[i].SetUVs(1, lmuvs.Skip(meshIndexBases[i]).Take(meshesToProcess[i].vertices.Length).ToList());
            meshesToProcess[i].UploadMeshData(false);
        }
        AssetDatabase.SaveAssets();
    }
}

