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

    static void EulerToTransform(Vector3 e, Transform t)
    {
        Matrix4x4 mat;
        //mat = Matrix4x4.Rotate(Quaternion.AngleAxis(e.x * Mathf.Rad2Deg, new Vector3(1, 0, 0))) *
        //      Matrix4x4.Rotate(Quaternion.AngleAxis(-e.z * Mathf.Rad2Deg, new Vector3(0, 0, 1))) *
        //      Matrix4x4.Rotate(Quaternion.AngleAxis(e.y * Mathf.Rad2Deg, new Vector3(0, 1, 0)));
        mat = Matrix4x4.Rotate(Quaternion.AngleAxis(e.y * Mathf.Rad2Deg, new Vector3(0, 1, 0))) *
             Matrix4x4.Rotate(Quaternion.AngleAxis(e.z * Mathf.Rad2Deg, new Vector3(0, 0, 1))) *
             Matrix4x4.Rotate(Quaternion.AngleAxis(e.x * Mathf.Rad2Deg, new Vector3(1, 0, 0)));
        t.localRotation = mat.rotation;
        
    }

    static void SetBoneWorldTransform(Transform t, FLVER.Bone[] bones, int boneIndex)
    {
        Matrix4x4 mat = Matrix4x4.identity;
        var bone = bones[boneIndex];
        do
        {
            mat *= Matrix4x4.Scale(new Vector3(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.X * Mathf.Rad2Deg, new Vector3(1, 0, 0)));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.Z * Mathf.Rad2Deg, new Vector3(0, 0, 1)));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.Y * Mathf.Rad2Deg, new Vector3(0, 1, 0)));
            mat *= Matrix4x4.Translate(new Vector3(bone.Translation.X, bone.Translation.Y, bone.Translation.Z));
            bone = (bone.ParentIndex != -1) ? bones[bone.ParentIndex] : null;
        } while (bone != null);
        t.position = new Vector3(mat.m03, mat.m13, mat.m23);
        var scale = new Vector3();
        scale.x = new Vector4(mat.m00, mat.m10, mat.m20, mat.m30).magnitude;
        scale.y = new Vector4(mat.m01, mat.m11, mat.m21, mat.m31).magnitude;
        scale.z = new Vector4(mat.m02, mat.m12, mat.m22, mat.m32).magnitude;
        t.localScale = scale;
        t.rotation = mat.rotation;
    }


    // Helper method to lookup nonlocal textures from another bnd
    static Texture2D FindTexture(string path, DarkSoulsTools.GameType gameType)
    {
        string gamePath = DarkSoulsTools.GameFolder(gameType);

        // Map texture reference
        if (path.Contains(@"\map\"))
        {
            var splits = path.Split('\\');
            var mapid = splits[splits.Length - 3];
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/{mapid}/{Path.GetFileNameWithoutExtension(path)}.dds");
            if (asset == null)
            {
                // Attempt to load UDSFM texture
                asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(path)}.dds");
            }
            return asset;
        }
        // Chr texture reference
        else if (path.Contains(@"\chr\"))
        {
            var splits = path.Split('\\');
            var chrid = splits[splits.Length - 3];
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Chr/{chrid}/{Path.GetFileNameWithoutExtension(path)}.dds");
            if (asset == null)
            {
                // Attempt to load shared chr textures
                asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Chr/sharedTextures/{Path.GetFileNameWithoutExtension(path)}.dds");
            }
            return asset;
        }
        // Parts texture reference
        else if (path.Contains(@"\parts\"))
        {
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Parts/textures/{Path.GetFileNameWithoutExtension(path)}.dds");
            return asset;
        }
        return null;
    }

    static public void ImportFlver(FLVER flver, FLVERAssetLink assetLink, DarkSoulsTools.GameType gameType, string assetName, string texturePath = null, bool mapflver = false)
    {
        Material[] materials = new Material[flver.Materials.Count];
        string gamePath = DarkSoulsTools.GameFolder(gameType);

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
            //bool normalquery = (m.Textures.Where(x => ((x.Type.ToUpper() == "G_BUMPMAPTEXTURE") || (x.Type.ToUpper() == "G_BUMPMAP"))).Count() >= 1);
            bool normalquery = false;

            Texture2D albedo = null;
            Texture2D specular = null;
            Texture2D normal = null;
            bool IsMapTexture = mapflver;
            var MTD = AssetDatabase.LoadAssetAtPath<MTDAssetLink>($@"Assets/{gamePath}/MTD/{Path.GetFileNameWithoutExtension(m.MTD)}.asset");
            if (texturePath != null)
            {
                foreach (var matParam in m.Textures)
                {
                    var paramNameCheck = matParam.Type.ToUpper();
                    if (paramNameCheck == "G_DIFFUSETEXTURE" || paramNameCheck == "G_DIFFUSE" || paramNameCheck.Contains("ALBEDO"))
                    {
                        var texPath = matParam.Path;
                        if (texPath == "")
                        {
                            texPath = MTD.Textures.Find(x => (x.Name == matParam.Type)).TexturePath;
                            if (texPath == "")
                            {
                                continue;
                            }
                        }
                        if (albedo == null)
                        {
                            albedo = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(texPath)}.dds");
                            if (albedo == null)
                            {
                                albedo = FindTexture(texPath, gameType);
                            }
                        }
                    }
                    if (paramNameCheck == "G_SPECULARTEXTURE" || paramNameCheck == "G_SPECULAR")
                    {
                        specular = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(matParam.Path)}.dds");
                        if (specular == null)
                        {
                            specular = FindTexture(matParam.Path, gameType);
                        }
                    }
                    if (paramNameCheck == "G_BUMPMAPTEXTURE" || paramNameCheck == "G_BUMPMAP" || paramNameCheck.Contains("NORMAL"))
                    {
                        var texPath = matParam.Path;
                        if (texPath == "")
                        {
                            texPath = MTD.Textures.Find(x => (x.Name == matParam.Type)).TexturePath;
                            if (texPath == "")
                            {
                                continue;
                            }
                        }
                        if (normal == null)
                        {
                            normal = AssetDatabase.LoadAssetAtPath<Texture2D>($@"{texturePath}/{Path.GetFileNameWithoutExtension(texPath)}.dds");
                            if (normal == null)
                            {
                                normal = FindTexture(texPath, gameType);
                            }
                        }
                        normalquery = true;
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
                mat.SetTexture("_MainTex", albedo);
            }
            else
            {
                mat = new Material(shaderObj);
                mat.SetTexture("_MainTex", albedo);
                mat.SetTexture("_Specular", specular);
                mat.SetTexture("_BumpMap", normal);
            }
            mat.name = name;
            materials[t] = mat;
            t++;
            AssetDatabase.CreateAsset(mat, assetName + "/" + name + ".mat");
        }

        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(assetName));
        GameObject meshesObj = new GameObject("Meshes");
        GameObject bonesObj = new GameObject("Bones");
        meshesObj.transform.parent = root.transform;
        bonesObj.transform.parent = root.transform;

        // import the skeleton
        Transform[] bones = new Transform[flver.Bones.Count];
        Matrix4x4[] bindPoses = new Matrix4x4[flver.Bones.Count];
        for (int i = 0; i < flver.Bones.Count; i++)
        {
            var fbone = flver.Bones[i];
            bones[i] = new GameObject(fbone.Name).transform;
            EulerToTransform(new Vector3(fbone.Rotation.X, fbone.Rotation.Y, fbone.Rotation.Z), bones[i]);
            bones[i].localPosition = new Vector3(fbone.Translation.X, fbone.Translation.Y, fbone.Translation.Z);
            bones[i].localScale = new Vector3(fbone.Scale.X, fbone.Scale.Y, fbone.Scale.Z);
            //SetBoneWorldTransform(bones[i], flver.Bones.ToArray(), i);
            //bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
        }

        // Skeleton parenting
        for (int i = 0; i < flver.Bones.Count; i++)
        {
            var fbone = flver.Bones[i];
            if (fbone.ParentIndex == -1)
            {
                //bones[i].parent = root.transform;
                bones[i].SetParent(bonesObj.transform, false);
                bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
            }
            else
            {
                //bones[i].parent = bones[fbone.ParentIndex];
                bones[i].SetParent(bones[fbone.ParentIndex], false);
                bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
            }
        }

        // Import the meshes
        int index = 0;
        foreach (var m in flver.Meshes)
        {
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var boneweights = new List<BoneWeight>();
            var smcount = 0;
            bool usestangents = false;
            int uvcount = m.Vertices[0].UVs.Count;
            List<Vector2>[] uvs = new List<Vector2>[uvcount];
            List<Material> matList = new List<Material>();

            // Add the mesh to the asset link
            FLVERAssetLink.SubmeshInfo info = new FLVERAssetLink.SubmeshInfo();
            info.Name = flver.Materials[m.MaterialIndex].Name;
            var MTD = AssetDatabase.LoadAssetAtPath<MTDAssetLink>($@"Assets/{gamePath}/MTD/{Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)}.asset");
            info.Mtd = MTD;
            assetLink.Submeshes.Add(info);

            int lightmapUVIndex = 1;
            // Use MTD to get lightmap uv index
            if (gameType != DarkSoulsTools.GameType.Sekiro)
            {
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
            }
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new List<Vector2>();
            }
            bool isSkinned = false;
            foreach (var v in m.Vertices)
            {
                verts.Add(new Vector3(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z));
                normals.Add(new Vector3(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z));
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
                        uvs[i].Add(new Vector2(v.UVs[lightmapUVIndex].X, 1.0f - v.UVs[lightmapUVIndex].Y));
                    }
                    else if (i == lightmapUVIndex)
                    {
                        uvs[i].Add(new Vector2(v.UVs[1].X, 1.0f - v.UVs[1].Y));
                    }
                    else
                    {
                        uvs[i].Add(new Vector2(v.UVs[i].X, 1.0f - v.UVs[i].Y));
                    }
                }
                if (v.BoneWeights != null && v.BoneWeights.Count() > 0)
                {
                    isSkinned = true;
                    var weight = new BoneWeight();
                    weight.boneIndex0 = m.BoneIndices[v.BoneIndices[0]];
                    weight.boneIndex1 = m.BoneIndices[v.BoneIndices[1]];
                    weight.boneIndex2 = m.BoneIndices[v.BoneIndices[2]];
                    weight.boneIndex3 = m.BoneIndices[v.BoneIndices[3]];
                    if (v.BoneWeights[0] < 0.0)
                    {
                        weight.weight0 = 1.0f;
                    }
                    else
                    {
                        weight.weight0 = v.BoneWeights[0];
                    }
                    weight.weight1 = v.BoneWeights[1];
                    weight.weight2 = v.BoneWeights[2];
                    weight.weight3 = v.BoneWeights[3];
                    boneweights.Add(weight);
                }
                else
                {
                    boneweights.Add(new BoneWeight());
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
            if (isSkinned)
            {
                mesh.boneWeights = boneweights.ToArray();
                mesh.bindposes = bindPoses;
            }

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
            if (isSkinned)
            {
                obj.AddComponent<SkinnedMeshRenderer>();
                obj.GetComponent<SkinnedMeshRenderer>().materials = matList.ToArray();
                obj.GetComponent<SkinnedMeshRenderer>().bones = bones;
                obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
            }
            else
            {
                obj.AddComponent<MeshRenderer>();
                obj.GetComponent<MeshRenderer>().materials = matList.ToArray();
                obj.AddComponent<MeshFilter>();
                obj.GetComponent<MeshFilter>().mesh = mesh;
            }
            obj.AddComponent<FlverSubmesh>();
            obj.GetComponent<FlverSubmesh>().Link = assetLink;
            obj.GetComponent<FlverSubmesh>().SubmeshIdx = index;
            obj.transform.parent = meshesObj.transform;

            AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
            index++;
        }

        // If there's no meshes, create an empty one to bind the skeleton to so that Maya works
        // when you export the skeleton (like with c0000).
        if (flver.Meshes.Count == 0)
        {
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var boneweights = new List<BoneWeight>();
            for (var i = 0; i < 3; i++)
            {
                verts.Add(new Vector3(0.0f, 0.0f, 0.0f));
                normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                tangents.Add(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                var weight = new BoneWeight();
                weight.boneIndex0 = 0;
                weight.weight0 = 1.0f;
                boneweights.Add(weight);
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.subMeshCount = 1;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.boneWeights = boneweights.ToArray();
            mesh.bindposes = bindPoses;
            mesh.SetTriangles( new int [] { 0, 1, 2 }, 0);

            GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName) + $@"_{index}");
            obj.AddComponent<SkinnedMeshRenderer>();
            obj.GetComponent<SkinnedMeshRenderer>().bones = bones;
            obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
            obj.transform.parent = meshesObj.transform;

            AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
        }

        root.AddComponent<FlverMesh>();
        root.GetComponent<FlverMesh>().Link = assetLink;

        AssetDatabase.CreateAsset(assetLink, assetName + ".asset");
        AssetDatabase.SaveAssets();
        PrefabUtility.SaveAsPrefabAsset(root, assetName + ".prefab");
        Object.DestroyImmediate(root);
    }

    static public void ImportFlver(DarkSoulsTools.GameType gameType, string path, string assetName, string texturePath = null)
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

        ImportFlver(flver, link, gameType, assetName, texturePath);
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

    [MenuItem("DSTools/FLVER Tools/Create Model Replacement")]
    static void CreateModelReplacement(MenuCommand menuCommand)
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!assetPath.EndsWith(".prefab"))
        {
            EditorUtility.DisplayDialog("Invalid asset", "Please select a prefab imported with DSTools", "Ok");
        }
        //UnityEditor.Formats.Fbx.Exporter.ModelExporter.ExportObject
    }

    // Really hacky initial flver exporter
    [MenuItem("DSTools/FLVER Tools/Export Model")]
    static void ExportModel(MenuCommand menuCommand)
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!assetPath.EndsWith(".fbx"))
        {
            EditorUtility.DisplayDialog("Invalid asset", "Please select an fbx asset", "Ok");
        }

        // Load the FBX as a prefab
        //GameObject obj = PrefabUtility.LoadPrefabContents(assetPath);
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

        // Go through everything and strip the prefixes fbx exporters love to add
        Stack<GameObject> gameObjects = new Stack<GameObject>();
        GameObject bonesRoot = null;
        GameObject meshesRoot = null;
        gameObjects.Push(obj);
        while (gameObjects.Count > 0)
        {
            var o = gameObjects.Pop();
            o.name = o.name.Split(':').Last();
            if (o.name == "Bones")
            {
                bonesRoot = o;
            }
            if (o.name == "Meshes")
            {
                meshesRoot = o;
            }
            for (int i = 0; i < o.transform.childCount; i++)
            {
                gameObjects.Push(o.transform.GetChild(i).gameObject);
            }
        }

        // Load the source c0000 and target flvers
        var sourceBnd = BND4.Read($@"{DarkSoulsTools.Interroot}\chr\c0000.chrbnd.dcx");
        var sourceFlver = FLVER.Read(sourceBnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        var targetBnd = BND4.Read($@"{DarkSoulsTools.Interroot}\parts\lg_m_9000.partsbnd.dcx");
        var targetFlver = FLVER.Read(targetBnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);

        // Build a bone reindexing table
        Dictionary<string, int> SourceBoneTable = new Dictionary<string, int>();
        for (int i = 0; i < sourceFlver.Bones.Count; i++)
        {
            if (!SourceBoneTable.ContainsKey(sourceFlver.Bones[i].Name))
            {
                SourceBoneTable.Add(sourceFlver.Bones[i].Name, i);
            }
        }

        if (meshesRoot == null)
        {
            throw new Exception("Could not find Meshes group for this FBX");
        }

        if (bonesRoot == null)
        {
            throw new Exception("Could not find Bones group for this FBX");
        }

        // Get the mesh object (this is hacky for now)
        var meshObj = meshesRoot.transform.GetChild(0).gameObject;

        // Get the skin and mesh
        var meshSkin = meshObj.GetComponent<SkinnedMeshRenderer>();
        var bones = meshSkin.bones;
        var mesh = meshSkin.sharedMesh;

        // Remap table to recover source bone indices
        var boneRemap = new int[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            var name = bones[i].gameObject.name;
            if (SourceBoneTable.ContainsKey(name))
            {
                boneRemap[i] = SourceBoneTable[name];
            }
            else
            {
                boneRemap[i] = 0;
            }
        }

        // Build the submesh's bone table
        HashSet<int> usedBones = new HashSet<int>();
        foreach (var weight in mesh.boneWeights)
        {
            if (weight.boneIndex0 >= 0)
            {
                usedBones.Add(boneRemap[weight.boneIndex0]);
            }
            if (weight.boneIndex1 >= 0)
            {
                usedBones.Add(boneRemap[weight.boneIndex1]);
            }
            if (weight.boneIndex2 >= 0)
            {
                usedBones.Add(boneRemap[weight.boneIndex2]);
            }
            if (weight.boneIndex3 >= 0)
            {
                usedBones.Add(boneRemap[weight.boneIndex3]);
            }
        }

        // Bad hack
        for (int i = 0; i < usedBones.Max(); i++)
        {
            usedBones.Add(i);
        }

        var submeshBones = usedBones.OrderBy(x => x).ToArray();
        var meshToSubmeshBone = new Dictionary<int, int>();
        for (int i = 0; i < submeshBones.Count(); i++)
        {
            meshToSubmeshBone.Add(submeshBones[i], i);
        }

        // Finally port the mesh to the target
        sourceFlver.Bones.Add(targetFlver.Bones[29]);
        targetFlver.Bones = sourceFlver.Bones;
        targetFlver.Meshes = new List<FLVER.Mesh> { targetFlver.Meshes.First() };
        targetFlver.SekiroUnk = sourceFlver.SekiroUnk;
        var fmesh = targetFlver.Meshes[0];
        fmesh.BoneIndices = submeshBones.ToList();
        var min = mesh.bounds.min;
        var max = mesh.bounds.max;
        //fmesh.BoundingBoxMax = sourceFlver.Header.BoundingBoxMax;
        //fmesh.BoundingBoxMin = new System.Numerics.Vector3(max.x*100, max.y*100, max.z*100);
        //fmesh.BoundingBoxMin = sourceFlver.Header.BoundingBoxMin;
        //fmesh.MaterialIndex = 0;
        //targetFlver.Header.BoundingBoxMin = sourceFlver.Header.BoundingBoxMin;
        //targetFlver.Header.BoundingBoxMax = sourceFlver.Header.BoundingBoxMax;

        /*foreach (var b in usedBones)
        {
            targetFlver.Bones[b].Unk3C = 8;
        }*/
        foreach (var b in targetFlver.Bones)
        {
            if (b.Unk3C == 2)
            {
                b.Unk3C = 8;
            }
        }
        targetFlver.Bones[140].Unk3C = 4;
        targetFlver.Bones[140].Name = "LG_M_9000";

        // Port vertices
        fmesh.Vertices.Clear();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            var vert = new FLVER.Vertex();
            var pos = mesh.vertices[i];
            vert.Positions.Add(new System.Numerics.Vector3(pos.x, pos.y, pos.z));
            var normal = mesh.normals[i];
            vert.Normals.Add(new System.Numerics.Vector4(-normal.x, -normal.y, -normal.z, -1.0f));
            var tangent = mesh.tangents[i];
            vert.Tangents.Add(new System.Numerics.Vector4(-tangent.x, -tangent.y, -tangent.z, -tangent.w));
            var color = new Color32(0xFF, 0xFF, 0xFF, 0xFF); //mesh.colors32[i];
            vert.Colors.Add(new FLVER.Vertex.Color(color.a, color.r, color.g, color.b));
            vert.UVs.Add(new System.Numerics.Vector3(0.0f, 0.0f, 0.0f));
            vert.UVs.Add(new System.Numerics.Vector3(0.0f, 0.0f, 0.0f));
            vert.BoneIndices = new int[4];
            vert.BoneWeights = new float[4];
            var bone = mesh.boneWeights[i];
            vert.BoneWeights[0] = bone.weight0;
            vert.BoneWeights[1] = bone.weight1;
            vert.BoneWeights[2] = bone.weight2;
            vert.BoneWeights[3] = bone.weight3;
            vert.BoneIndices[0] = meshToSubmeshBone[boneRemap[bone.boneIndex0]];
            vert.BoneIndices[1] = meshToSubmeshBone[boneRemap[bone.boneIndex1]];
            vert.BoneIndices[2] = meshToSubmeshBone[boneRemap[bone.boneIndex2]];
            vert.BoneIndices[3] = meshToSubmeshBone[boneRemap[bone.boneIndex3]];
            for (int b = 0; b < 3; b++)
            {
                if (vert.BoneIndices[b] == -1)
                {
                    vert.BoneIndices[b] = 0;
                }
            }

            fmesh.Vertices.Add(vert);
        }

        // Port faceset
        fmesh.FaceSets = new List<FLVER.FaceSet> { fmesh.FaceSets.First() };
        var fset = fmesh.FaceSets[0];
        var tris = new List<uint>();
        for (int i = 0; i < mesh.triangles.Count(); i++)
        {
            tris.Add((uint)mesh.triangles[i]);
        }
        fset.Vertices = tris.ToArray();
        fset.CullBackfaces = false;
        fset.TriangleStrip = false;

        var fset2 = new FLVER.FaceSet(FLVER.FaceSet.FSFlags.LodLevel1, false, false, fset.Unk06, fset.Unk07, fset.IndexSize, fset.Vertices);
        var fset3 = new FLVER.FaceSet(FLVER.FaceSet.FSFlags.LodLevel2, false, false, fset.Unk06, fset.Unk07, fset.IndexSize, fset.Vertices);
        var fset4 = new FLVER.FaceSet(FLVER.FaceSet.FSFlags.Unk80000000, false, false, fset.Unk06, fset.Unk07, fset.IndexSize, fset.Vertices);
        fmesh.FaceSets.Add(fset2);
        fmesh.FaceSets.Add(fset3);
        fmesh.FaceSets.Add(fset4);

        //targetFlver.Materials[0].MTD = $@"M[ARSN].mtd";

        // Finally save
        targetBnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes = targetFlver.Write();
        targetBnd.Write($@"{DarkSoulsTools.ModProjectDirectory}\parts\lg_m_9000.partsbnd.dcx", DCX.Type.SekiroDFLT);
    }
}
