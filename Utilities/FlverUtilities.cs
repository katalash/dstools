using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using SoulsFormats;
using Object = UnityEngine.Object;

/// <summary>
/// Flver import/export utilities
/// </summary>
class FlverUtilities
{
    // Hardcoded table of materials that don't have UV index 1 as their lightmap UVs
    static Dictionary<string, int> MaterialLightmapUVIndex = new Dictionary<string, int>()
    {
        { $@"M[ARSN]_l_m", 2 }
    };

    // Helper method to find a map texture for objects that don't have embedded textures
    static Texture2D FindMapTexture(string path)
    {
        var splits = path.Split('\\');
        var mapid = splits[splits.Length - 3];
        var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{mapid}/{Path.GetFileNameWithoutExtension(path)}.texture2d");
        if (asset == null)
        {
            // Attempt to load UDSFM texture
            asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(path)}.texture2d");
        }
        if (asset == null)
        {
            // Attempt to load shared chr textures
            asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/Chr/sharedTextures/{Path.GetFileNameWithoutExtension(path)}.texture2d");
        }
        return asset;
    }

    static public void ImportFlver(FLVER flver, FLVERAssetLink assetLink, string assetName, string texturePath = null, bool mapflver = false)
    {
        Material[] materials = new Material[flver.Materials.Count];

        if (!AssetDatabase.IsValidFolder(assetName))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(assetName + ".blah"), Path.GetFileNameWithoutExtension(assetName + ".blah"));
        }

        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/dstools/Shaders/FLVERShader.shadergraph");
        Shader shaderObj = AssetDatabase.LoadAssetAtPath<Shader>("Assets/dstools/Shaders/ObjFLVERShader.shadergraph");
        Shader shaderDiff = AssetDatabase.LoadAssetAtPath<Shader>("Assets/dstools/Shaders/FLVERShaderDiffuse.shadergraph");
        var t = 0;
        foreach (var m in flver.Materials)
        {
            //string name = m.Name;
            //if (name == null || name == "")
            //{
            string name = Path.GetFileNameWithoutExtension(assetName) + $@"_{t}";
            //}
            bool normalquery = (m.Textures.Where(x => ((x.Type.ToUpper() == "G_BUMPMAPTEXTURE") || (x.Type.ToUpper() == "G_BUMPMAP"))).Count() >= 1);

            Texture2D albedo = null;
            Texture2D specular = null;
            Texture2D normal = null;
            bool IsMapTexture = mapflver;
            if (texturePath != null)
            {
                foreach (var matParam in m.Textures)
                {
                    var paramNameCheck = matParam.Type.ToUpper();
                    if (paramNameCheck == "G_DIFFUSETEXTURE" || paramNameCheck == "G_DIFFUSE")
                    {
                        albedo = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(matParam.Path)}.texture2d");
                        if (albedo == null)
                        {
                            albedo = FindMapTexture(matParam.Path);
                        }
                    }
                    if (paramNameCheck == "G_SPECULARTEXTURE" || paramNameCheck == "G_SPECULAR")
                    {
                        specular = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(matParam.Path)}.texture2d");
                        if (specular == null)
                        {
                            specular = FindMapTexture(matParam.Path);
                        }
                    }
                    if (paramNameCheck == "G_BUMPMAPTEXTURE" || paramNameCheck == "G_BUMPMAP")
                    {
                        normal = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(matParam.Path)}.texture2d");
                        if (normal == null)
                        {
                            normal = FindMapTexture(matParam.Path);
                        }
                    }
                }
            }
            Material mat;
            /*if (IsMapTexture && specular != null)
            {
                mat = new Material(shader);
                mat.SetTexture("_Albedo", albedo);
                mat.SetTexture("_Specular", specular);
                mat.SetTexture("_Normal", normal);
            }
            else */
            if (!normalquery)
            {
                mat = new Material(shaderDiff);
                mat.SetTexture("_Albedo", albedo);
            }
            else
            {
                mat = new Material(shaderObj);
                mat.SetTexture("_Albedo", albedo);
                mat.SetTexture("_Specular", specular);
                mat.SetTexture("_Normal", normal);
            }
            mat.name = name;
            materials[t] = mat;
            t++;
            AssetDatabase.CreateAsset(mat, assetName + "/" + name + ".mat");
        }

        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(assetName));

        int index = 0;
        foreach (var m in flver.Meshes)
        {
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var smcount = 0;
            bool usestangents = false;
            int uvcount = m.Vertices[0].UVs.Count;
            List<Vector2>[] uvs = new List<Vector2>[uvcount];
            List<Material> matList = new List<Material>();

            // Add the mesh to the asset link
            FLVERAssetLink.SubmeshInfo info = new FLVERAssetLink.SubmeshInfo();
            info.Name = flver.Materials[m.MaterialIndex].Name;
            var MTD = AssetDatabase.LoadAssetAtPath<MTDAssetLink>($@"Assets/MTD/{Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)}.asset");
            info.Mtd = MTD;
            assetLink.Submeshes.Add(info);

            int lightmapUVIndex = 1;
            // Use MTD to get lightmap uv index
            if (MTD != null)
            {
                lightmapUVIndex = (MTD.LightmapUVIndex != -1) ? MTD.LightmapUVIndex : 1;
                if (lightmapUVIndex >= uvs.Length)
                    lightmapUVIndex = 1;
            }
            else
            {
                // Do a hardcoded lookup of a material's lightmap UV index from a shitty table :fatcat:
                if (MaterialLightmapUVIndex.ContainsKey(Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)))
                {
                    lightmapUVIndex = MaterialLightmapUVIndex[Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)];
                }
            }
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new List<Vector2>();
            }
            foreach (var v in m.Vertices)
            {
                verts.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                normals.Add(new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z));
                if (v.Tangents.Count > 0)
                {
                    tangents.Add(new Vector4(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z, v.Tangents[0].W));
                    usestangents = true;
                }
                else
                {
                    tangents.Add(new Vector4(0, 0, 0, 1));
                }
                for (int i = 0; i < uvs.Length; i++)
                {
                    // Swap lightmap uvs with uv index 1 because lmao unity
                    if (i == 1)
                    {
                        uvs[i].Add(new Vector2(v.UVs[lightmapUVIndex].X, v.UVs[lightmapUVIndex].Y));
                    }
                    else if (i == lightmapUVIndex)
                    {
                        uvs[i].Add(new Vector2(v.UVs[1].X, v.UVs[1].Y));
                    }
                    else
                    {
                        uvs[i].Add(new Vector2(v.UVs[i].X, v.UVs[i].Y));
                    }
                }
            }
            foreach (var fs in m.FaceSets)
            {
                if (fs.Vertices.Count() > 0 && fs.Flags == FLVER.FaceSet.FSFlags.None)
                {
                    matList.Add(materials[m.MaterialIndex]);
                    smcount++;
                }
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.subMeshCount = smcount;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            if (usestangents)
                mesh.SetTangents(tangents);

            for (int i = 0; i < uvs.Length; i++)
            {
                mesh.SetUVs(i, uvs[i]);
            }

            var submesh = 0;
            foreach (var fs in m.FaceSets)
            {
                if (fs.Vertices.Count() == 0)
                    continue;
                if (fs.Flags != FLVER.FaceSet.FSFlags.None)
                    continue;

                mesh.SetTriangles(fs.GetFacesArray(), submesh, true, 0);
                submesh++;
            }
            mesh.RecalculateBounds();

            // Setup a game object asset
            GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName) + $@"_{index}");
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshRenderer>().materials = matList.ToArray();
            obj.AddComponent<FlverSubmesh>();
            obj.GetComponent<FlverSubmesh>().Link = assetLink;
            obj.GetComponent<FlverSubmesh>().SubmeshIdx = index;
            obj.transform.parent = root.transform;

            AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
            index++;
        }

        root.AddComponent<FlverMesh>();
        root.GetComponent<FlverMesh>().Link = assetLink;

        AssetDatabase.CreateAsset(assetLink, assetName + ".asset");
        AssetDatabase.SaveAssets();
        PrefabUtility.SaveAsPrefabAsset(root, assetName + ".prefab");
        Object.DestroyImmediate(root);
    }

    static public void ImportFlver(string path, string assetName, string texturePath = null)
    {
        FLVER flver;
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        if (path.Contains(".mapbnd"))
        {
            BND4 bnd = BND4.Read(path);
            link.Type = FLVERAssetLink.ContainerType.Mapbnd;
            link.ArchivePath = path;
            link.FlverPath = bnd.Files[0].Name;
            flver = FLVER.Read(bnd.Files[0].Bytes);
        }
        else
        {
            link.Type = FLVERAssetLink.ContainerType.None;
            link.FlverPath = path;
            flver = FLVER.Read(path);
        }

        ImportFlver(flver, link, assetName, texturePath);
    }

    /// <summary>
    /// Effectively "unindexes a mesh" by defining three unique vertices and all of their attributes for each triangle.
    /// Some sort of vertex merging would be nice in the future, since Unity's merging is utterly worthless and corrupts the meshes.
    /// </summary>
    /// <param name="mesh"></param>
    static public void UnindexMesh(Mesh mesh)
    {
        var indices = mesh.GetIndices(0);
        var positions = new List<Vector3>();
        mesh.GetVertices(positions);
        var normals = new List<Vector3>();
        mesh.GetNormals(normals);
        var tangents = new List<Vector4>();
        mesh.GetTangents(tangents);
        var colors = new List<Color>();
        mesh.GetColors(colors);
        List<List<Vector2>> uvs = new List<List<Vector2>>();
        for (int i = 0; i < 8; i++)
        {
            uvs.Add(new List<Vector2>());
            mesh.GetUVs(i, uvs[i]);
        }
        List<int> newIndices = new List<int>();
        List<Vector3> newPositions = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector4> newTangents = new List<Vector4>();
        List<Color> newColors = new List<Color>();
        List<List<Vector2>> newUVs = new List<List<Vector2>>();
        int j = 0;
        while (j < uvs.Count && uvs[j] != null)
        {
            newUVs.Add(new List<Vector2>());
            j++;
        }

        int counter = 0;
        foreach (var index in indices)
        {
            newIndices.Add(counter);
            newPositions.Add(positions[index]);
            if (normals.Count > 0)
                newNormals.Add(normals[index]);
            if (tangents.Count > 0)
                newTangents.Add(tangents[index]);
            if (colors.Count > 0)
                newColors.Add(colors[index]);
            for (int i = 0; i < newUVs.Count; i++)
                if (uvs[i].Count > 0)
                    newUVs[i].Add(uvs[i][index]);
            counter++;
        }

        mesh.SetVertices(newPositions);
        if (normals.Count > 0)
            mesh.SetNormals(newNormals);
        if (tangents.Count > 0)
            mesh.SetTangents(newTangents);
        if (colors.Count > 0)
            mesh.SetColors(newColors);
        for (int i = 0; i < newUVs.Count; i++)
            if (uvs[i].Count > 0)
                mesh.SetUVs(i, newUVs[i]);
        mesh.SetTriangles(newIndices.ToArray(), 0);
    }
}
