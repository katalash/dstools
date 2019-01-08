using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using SoulsFormats;
using MeowDSIO;
using MeowDSIO.DataFiles;
using MeowDSIO.DataTypes.MSB;

public class DarkSoulsTools : EditorWindow
{
    bool LoadMapFlvers = false;
    bool LoadHighResCol = false;

    public enum GameType
    {
        Undefined,
        DarkSoulsPTDE,
        DarkSoulsRemastered,
        DarkSoulsIII,
    }

    GameType type = GameType.Undefined;

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
        return AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{mapid}/{Path.GetFileNameWithoutExtension(path)}.texture2d");
    }

    static void ImportFlver(FLVER flver, FLVERAssetLink assetLink, string assetName, string texturePath = null, bool mapflver=false)
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
            else */if (!normalquery)
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
            int lightmapUVIndex = 1;
            // Do a hardcoded lookup of a material's lightmap UV index because lmao from
            if (MaterialLightmapUVIndex.ContainsKey(Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)))
            {
                lightmapUVIndex = MaterialLightmapUVIndex[Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)];
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
            obj.transform.parent = root.transform;

            AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
            index++;

            // Add the mesh to the asset link
            FLVERAssetLink.SubmeshInfo info = new FLVERAssetLink.SubmeshInfo();
            info.Name = flver.Materials[m.MaterialIndex].Name;
            var MTD = AssetDatabase.LoadAssetAtPath<MTDAssetLink>($@"Assets/MTD/{Path.GetFileNameWithoutExtension(flver.Materials[m.MaterialIndex].MTD)}.asset");
            info.Mtd = MTD;
            assetLink.Submeshes.Add(info);
        }

        PrefabUtility.SaveAsPrefabAsset(root, assetName + ".prefab");
        AssetDatabase.CreateAsset(assetLink, assetName + ".asset");
        DestroyImmediate(root);
    }
    
    static void ImportFlver(string path, string assetName, string texturePath = null)
    {
        FLVER flver;
        FLVERAssetLink link = new FLVERAssetLink();
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

    static void ImportCollisionHKX(HKX hkx, string assetName)
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
        DestroyImmediate(obj);
    }

    static void ImportCollisionHKXBDT(string path, string outputAssetPath)
    {
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(pathBase + ".hkxbhd", pathBase + ".hkxbdt");
        foreach (var file in bxf.Files)
        {
            try
            {
                var hkx = HKX.Read(file.Bytes, HKX.HKXVariation.HKXDS3);
                ImportCollisionHKX(hkx, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load hkx file " + file.Name);
            }
        }
    }

    static void ImportObj(string objpath, string objid)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Obj"))
        {
            AssetDatabase.CreateFolder("Assets", "Obj");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/Obj/{objid}"))
        {
            AssetDatabase.CreateFolder("Assets/Obj", objid);
        }

        if (!File.Exists($@"{objpath}\{objid}.objbnd.dcx"))
        {
            throw new FileNotFoundException("Could not find bnd for object " + objid);
        }
        BND4 objbnd = BND4.Read($@"{objpath}\{objid}.objbnd.dcx");
        var texentries = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".TPF"));
        foreach (var entry in texentries)
        {
            TPF tpf = TPF.Read(entry.Bytes);
            foreach (var tex in tpf.Textures)
            {
                var t2d = CreateTextureFromTPF(tex);
                if (t2d != null)
                {
                    AssetDatabase.CreateAsset(t2d, $@"Assets/Obj/{objid}/{tex.Name}.texture2d");
                }
            }
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = new FLVERAssetLink();
        link.Type = FLVERAssetLink.ContainerType.Objbnd;
        link.ArchivePath = objpath;
        link.FlverPath = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        ImportFlver(flver, link, $@"Assets/Obj/{objid}", $@"Assets/Obj/{objid}");
    }

    static void ImportObjs(string objpath)
    {
        var objFiles = Directory.GetFiles(objpath, @"*.objbnd.dcx")
                    .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                    .Select(Path.GetFileNameWithoutExtension) //Remove .objbnd
                    .ToArray();
        foreach (var obj in objFiles)
        {
            try
            {
                ImportObj(objpath, obj);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error loading obj {obj}: {e.Message}");
            }
        }
    }

    private static int GetNextMultipleOf4(int x)
    {
        if (x % 4 != 0)
            x += 4 - (x % 4);
        else if (x == 0)
            return 4;
        return x;
    }

    private static int GetNextPowerOf2(int x)
    {
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        x++;
        return x;
    }

    private static TextureFormat GetSurfaceFormatFromString(string str)
    {
        switch (str)
        {
            case "DXT1":
                return TextureFormat.DXT1;
            case "DXT3":
                return TextureFormat.DXT5;
            case "DXT5":
                return TextureFormat.DXT5;
            case "ATI1":
                return TextureFormat.DXT1; // Monogame workaround :fatcat:
            case "ATI2":
                return TextureFormat.DXT5;
            default:
                throw new Exception($"Unknown DDS Type: {str}");
        }
    }

    static Texture2D CreateTextureFromTPF(TPF.Texture tpf)
    {
        var textureBytes = tpf.Bytes;
        DDS header = new DDS(textureBytes);
        int height = header.dwHeight;
        int width = header.dwWidth;

        int mipmapCount = header.dwMipMapCount;
        var br = new BinaryReaderEx(false, textureBytes);

        br.Skip((int)header.dataOffset);

        TextureFormat surfaceFormat;
        if (header.ddspf.dwFourCC == "DX10")
        {
            // See if there are DX9 textures
            int fmt = (int)header.header10.dxgiFormat;
            if (fmt == 70 || fmt == 71 || fmt == 72)
                surfaceFormat = TextureFormat.DXT1;
            else if (fmt == 73 || fmt == 74 || fmt == 75)
                surfaceFormat = TextureFormat.DXT5;
            else if (fmt == 76 || fmt == 77 || fmt == 78)
                surfaceFormat = TextureFormat.DXT5;
            else if (fmt == 97 || fmt == 98 || fmt == 99)
                surfaceFormat = TextureFormat.BC7;
            else
            {
                return null;
            }
        }
        else
        {
            surfaceFormat = GetSurfaceFormatFromString(header.ddspf.dwFourCC);
        }
        // Adjust width and height because from has some DXTC textures that have dimensions not a multiple of 4 :shrug:
        Texture2D tex = new Texture2D(GetNextMultipleOf4(width), GetNextMultipleOf4(height), surfaceFormat, mipmapCount > 1);
        List<byte> texels = new List<byte>();
        var totaltexels = 0;
        //for (int i = 0; i < mipmapCount; i++)
        //{
        int w = width;
        int h = height;
        int i = 0;
        while (w > 0 || h > 0)
        {
            int numTexels = GetNextMultipleOf4(width >> i) * GetNextMultipleOf4(height >> i);
            if (surfaceFormat == TextureFormat.DXT1)
                numTexels /= 2;
            //byte[] thisMipMap = br.ReadBytes(numTexels);
            for (int t = 0; t < numTexels; t++)
            {
                if (i < mipmapCount)
                {
                    texels.Add(br.ReadByte());
                }
                else
                {
                    texels.Add(0);
                }
            }
            totaltexels += numTexels;
            i++;
            w >>= 1;
            h >>= 1;
        }
        //}
        
        tex.LoadRawTextureData(texels.ToArray());

        return tex;
    }

    static void ImportTpfbhd(string path)
    {
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(pathBase + ".tpfbhd", pathBase + ".tpfbdt");
        
        if (!AssetDatabase.IsValidFolder("Assets/" + name.Substring(0, 3)))
        {
            AssetDatabase.CreateFolder("Assets", name.Substring(0, 3));
        }

        foreach (var tpf in bxf.Files)
        {
            try
            {
                var tex = CreateTextureFromTPF(TPF.Read(tpf.Bytes).Textures[0]);
                AssetDatabase.CreateAsset(tex, "Assets/" + name.Substring(0, 3) + "/" + Path.GetFileNameWithoutExtension((Path.GetFileNameWithoutExtension(tpf.Name))) + ".texture2d");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading {tpf.Name}: {ex.Message}");
            }
        }
    }

    // Loads all the map textures created by UDSFM
    static void ImportUDSFMMapTpfs(string interroot)
    {
        if (!AssetDatabase.IsValidFolder("Assets/UDSFMMapTextures"))
        {
            AssetDatabase.CreateFolder("Assets", "UDSFMMapTextures");
        }

        string path = interroot + $@"\map\tx\";
        var files = Directory.GetFiles(path, "*.tpf");
        AssetDatabase.StartAssetEditing();
        foreach (var file in files)
        {
            try
            {
                var tpf = TPF.Read(file);
                var tex = CreateTextureFromTPF(tpf.Textures[0]);
                AssetDatabase.CreateAsset(tex, $@"Assets/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(file)}.texture2d");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading {file}: {ex.Message}");
            }
        }
        AssetDatabase.StopAssetEditing();
    }

    private string Interroot = "";
    private List<string> Maps = new List<string>();

    private void UpdateMapList()
    {
        var msbFiles = Directory.GetFileSystemEntries(Interroot + @"\map\MapStudio\", @"*.msb")
            .Select(Path.GetFileNameWithoutExtension);
        Maps = new List<string>();
        var IDSet = new HashSet<string>();
        foreach (var cf in msbFiles)
        {
            var dotIndex = cf.IndexOf('.');
            if (dotIndex >= 0)
            {
                Maps.Add(cf.Substring(0, dotIndex));
                IDSet.Add(cf.Substring(0, dotIndex));
            }
            else
            {
                Maps.Add(cf);
                IDSet.Add(cf);
            }
        }

        var msbFilesDCX = Directory.GetFileSystemEntries(Interroot + @"\map\MapStudio\", @"*.msb.dcx")
            .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension);
        foreach (var cf in msbFilesDCX)
        {
            var dotIndex = cf.IndexOf('.');
            if (dotIndex >= 0)
            {
                if (!IDSet.Contains(cf.Substring(0, dotIndex)))
                    Maps.Add(cf.Substring(0, dotIndex));
            }
            else
            {
                if (!IDSet.Contains(cf))
                    Maps.Add(cf);
            }
        }
    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Dark Souls Tools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DarkSoulsTools window = (DarkSoulsTools)EditorWindow.GetWindow(typeof(DarkSoulsTools));
        window.Show();
    }

    GameObject InstantiateRegion(MSB3.Region region, string type, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.Position.X, region.Position.Y, region.Position.Z);
        obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        if (region.Shape is MSB3.Shape.Box)
        {
            var shape = (MSB3.Shape.Box)region.Shape;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, shape.Height, shape.Depth);
        }
        else if (region.Shape is MSB3.Shape.Sphere)
        {
            var shape = (MSB3.Shape.Sphere)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region.Shape is MSB3.Shape.Point)
        {
            var shape = (MSB3.Shape.Point)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region.Shape is MSB3.Shape.Cylinder)
        {
            var shape = (MSB3.Shape.Cylinder)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        obj.layer = 13;
        obj.transform.parent = parent.transform;
        return obj;
    }

    void onImportDS3Map(object o)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSB3.Read(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx");

            if (!AssetDatabase.IsValidFolder("Assets/" + mapname))
            {
                AssetDatabase.CreateFolder("Assets", mapname);
            }

            // Create an MSB asset link to the DS3 asset
            GameObject AssetLink = new GameObject("MSBAssetLink");
            AssetLink.AddComponent<MSBAssetLink>();
            AssetLink.GetComponent<MSBAssetLink>().Interroot = Interroot;
            AssetLink.GetComponent<MSBAssetLink>().MapID = mapname;
            AssetLink.GetComponent<MSBAssetLink>().MapPath = $@"{Interroot}\map\MapStudio\{mapname}.msb.dcx";

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");

            GameObject MapPieceModels = new GameObject("MapPieces");
            MapPieceModels.transform.parent = ModelsSection.transform;

            // Do a preload of all the flvers
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var mappiece in msb.Models.MapPieces)
                {
                    var assetname = mappiece.Name;
                    if (AssetDatabase.FindAssets($@"Assets/{mapname}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx"))
                            ImportFlver(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/{mapname}/{assetname}", $@"Assets/{mapname.Substring(0, 3)}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            foreach (var mappiece in msb.Models.MapPieces)
            {
                var assetname = mappiece.Name;
                GameObject model = new GameObject(mappiece.Name);
                model.AddComponent<MSB3MapPieceModel>();
                model.GetComponent<MSB3MapPieceModel>().SetModel(mappiece);
                model.transform.parent = MapPieceModels.transform;
            }

            // Load low res hkx assets
            if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}");
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}");
                }
            }

            GameObject ObjectModels = new GameObject("Objects");
            ObjectModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Objects)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB3ObjectModel>();
                model.GetComponent<MSB3ObjectModel>().SetModel(mod);
                model.transform.parent = ObjectModels.transform;
            }

            GameObject PlayerModels = new GameObject("Players");
            PlayerModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Players)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB3PlayerModel>();
                model.GetComponent<MSB3PlayerModel>().SetModel(mod);
                model.transform.parent = PlayerModels.transform;
            }

            GameObject EnemyModels = new GameObject("Enemies");
            EnemyModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Enemies)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB3EnemyModel>();
                model.GetComponent<MSB3EnemyModel>().SetModel(mod);
                model.transform.parent = EnemyModels.transform;
            }

            GameObject CollisionModels = new GameObject("Collisions");
            CollisionModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Collisions)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB3CollisionModel>();
                model.GetComponent<MSB3CollisionModel>().SetModel(mod);
                model.transform.parent = CollisionModels.transform;
            }

            GameObject OtherModels = new GameObject("Others");
            OtherModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Others)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB3OtherModel>();
                model.GetComponent<MSB3OtherModel>().SetModel(mod);
                model.transform.parent = OtherModels.transform;
            }

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapname}/{part.ModelName}.prefab") : null;
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB3MapPiecePart>();
                    obj.GetComponent<MSB3MapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB3MapPiecePart>();
                    obj.GetComponent<MSB3MapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
            }

            GameObject Objects = new GameObject("Objects");
            Objects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Objects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Obj/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB3Part>();
                    obj.GetComponent<MSB3Part>().setBasePart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 10;
                    obj.transform.parent = Objects.transform;
                }
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Collisions)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapname}/{lowHigh}{mapname.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB3CollisionPart>();
                    obj.GetComponent<MSB3CollisionPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 12;
                    obj.transform.parent = Collisions.transform;
                }
            }

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectCollisions)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3ConnectCollisionPart>();
                obj.GetComponent<MSB3ConnectCollisionPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = ConnectCollisions.transform;
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3EnemyPart>();
                obj.GetComponent<MSB3EnemyPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = Enemies.transform;
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3PlayerPart>();
                obj.GetComponent<MSB3PlayerPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = Players.transform;
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3DummyObjectPart>();
                obj.GetComponent<MSB3DummyObjectPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = DummyObjects.transform;
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3DummyEnemyPart>();
                obj.GetComponent<MSB3DummyEnemyPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = DummyEnemies.transform;
            }

            //
            // Regions section
            //
            GameObject Regions = new GameObject("MSBRegions");

            GameObject ActivationAreas = new GameObject("ActivationAreas");
            ActivationAreas.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.ActivationAreas)
            {
                var reg = InstantiateRegion(region, "Activation Area", ActivationAreas);
                reg.AddComponent<MSB3ActivationAreaRegion>();
                reg.GetComponent<MSB3ActivationAreaRegion>().SetRegion(region);
            }

            GameObject EnvEffBoxes = new GameObject("EnvMapEffectBoxes");
            EnvEffBoxes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.EnvironmentMapEffectBoxes)
            {
                var reg = InstantiateRegion(region, "Env Map Effect Box", EnvEffBoxes);
                reg.AddComponent<MSB3EnvironmentEffectBoxRegion>();
                reg.GetComponent<MSB3EnvironmentEffectBoxRegion>().SetRegion(region);
            }

            GameObject EnvMapPoints = new GameObject("EnvMapPoints");
            EnvMapPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.EnvironmentMapPoints)
            {
                var reg = InstantiateRegion(region, "Env Map Point", EnvMapPoints);
                reg.AddComponent<MSB3EnvironmentMapPointRegion>();
                reg.GetComponent<MSB3EnvironmentMapPointRegion>().SetRegion(region);
            }

            GameObject EventRegions = new GameObject("Events");
            EventRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Events)
            {
                var reg = InstantiateRegion(region, "Event", EventRegions);
                reg.AddComponent<MSB3EventRegion>();
                reg.GetComponent<MSB3EventRegion>().SetRegion(region);
            }

            GameObject GeneralRegions = new GameObject("GeneralRegions");
            GeneralRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.General)
            {
                var reg = InstantiateRegion(region, "General", GeneralRegions);
                reg.AddComponent<MSB3GeneralRegion>();
                reg.GetComponent<MSB3GeneralRegion>().SetRegion(region);
            }

            GameObject InvasionPoints = new GameObject("InvasionPoints");
            InvasionPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.InvasionPoints)
            {
                var reg = InstantiateRegion(region, "Invasion Point", InvasionPoints);
                reg.AddComponent<MSB3InvasionPointRegion>();
                reg.GetComponent<MSB3InvasionPointRegion>().SetRegion(region);
            }

            GameObject Messages = new GameObject("Messages");
            Messages.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Messages)
            {
                var reg = InstantiateRegion(region, "Message", Messages);
                reg.AddComponent<MSB3MessageRegion>();
                reg.GetComponent<MSB3MessageRegion>().SetRegion(region);
            }

            GameObject MuffBoxes = new GameObject("MufflingBox");
            MuffBoxes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.MufflingBoxes)
            {
                var reg = InstantiateRegion(region, "Muffling Box", MuffBoxes);
                reg.AddComponent<MSB3MufflingBoxRegion>();
                reg.GetComponent<MSB3MufflingBoxRegion>().SetRegion(region);
            }

            GameObject MuffPortals = new GameObject("MufflingPortals");
            MuffPortals.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.MufflingPortals)
            {
                var reg = InstantiateRegion(region, "Muffling Portal", MuffPortals);
                reg.AddComponent<MSB3MufflingPortal>();
                reg.GetComponent<MSB3MufflingPortal>().SetRegion(region);
            }

            GameObject SFXRegions = new GameObject("SFX");
            SFXRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.SFX)
            {
                var reg = InstantiateRegion(region, "SFX", SFXRegions);
                reg.AddComponent<MSB3SFXRegion>();
                reg.GetComponent<MSB3SFXRegion>().SetRegion(region);
            }

            GameObject SoundRegions = new GameObject("Sounds");
            SoundRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Sounds)
            {
                var reg = InstantiateRegion(region, "Sound", SoundRegions);
                reg.AddComponent<MSB3SoundRegion>();
                reg.GetComponent<MSB3SoundRegion>().SetRegion(region);
            }

            GameObject SpawnPoints = new GameObject("SpawnPoints");
            SpawnPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.SpawnPoints)
            {
                var reg = InstantiateRegion(region, "Spawn Point", SpawnPoints);
                reg.AddComponent<MSB3SpawnPointRegion>();
                reg.GetComponent<MSB3SpawnPointRegion>().SetRegion(region);
            }

            GameObject WalkRoutes = new GameObject("WalkRoutes");
            WalkRoutes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WalkRoutes)
            {
                var reg = InstantiateRegion(region, "Walk Route", WalkRoutes);
                reg.AddComponent<MSB3WalkRouteRegion>();
                reg.GetComponent<MSB3WalkRouteRegion>().SetRegion(region);
            }

            GameObject WarpPoints = new GameObject("WarpPoints");
            WarpPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WarpPoints)
            {
                var reg = InstantiateRegion(region, "Warp Point", WarpPoints);
                reg.AddComponent<MSB3WarpPointRegion>();
                reg.GetComponent<MSB3WarpPointRegion>().SetRegion(region);
            }

            GameObject WindAreas = new GameObject("WindAreas");
            WindAreas.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WindAreas)
            {
                var reg = InstantiateRegion(region, "Wind Area", WindAreas);
                reg.AddComponent<MSB3WindAreaRegion>();
                reg.GetComponent<MSB3WindAreaRegion>().SetRegion(region);
            }

            GameObject WindSFXs = new GameObject("WindSFXRegions");
            WindSFXs.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WindSFX)
            {
                var reg = InstantiateRegion(region, "Wind SFX", WindSFXs);
                reg.AddComponent<MSB3WindSFXRegion>();
                reg.GetComponent<MSB3WindSFXRegion>().SetRegion(region);
            }

            GameObject Unk00s = new GameObject("Unk00Regions");
            Unk00s.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Unk00s)
            {
                var reg = InstantiateRegion(region, "Unk00", Unk00s);
                reg.AddComponent<MSB3Unk00Region>();
                reg.GetComponent<MSB3Unk00Region>().SetRegion(region);
            }

            GameObject Unk12s = new GameObject("Unk12Regions");
            Unk12s.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Unk12s)
            {
                var reg = InstantiateRegion(region, "Unk12", Unk12s);
                reg.AddComponent<MSB3Unk12Region>();
                reg.GetComponent<MSB3Unk12Region>().SetRegion(region);
            }

            //
            // Events Section
            //
            GameObject Events = new GameObject("MSBEvents");

            GameObject Treasures = new GameObject("Treasures");
            Treasures.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Treasures)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3TreasureEvent>();
                evt.GetComponent<MSB3TreasureEvent>().SetEvent(ev);
                evt.transform.parent = Treasures.transform;
            }

            GameObject Generators = new GameObject("Generators");
            Generators.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Generators)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3GeneratorEvent>();
                evt.GetComponent<MSB3GeneratorEvent>().SetEvent(ev);
                evt.transform.parent = Generators.transform;
            }

            GameObject ObjActs = new GameObject("ObjActs");
            ObjActs.transform.parent = Events.transform;
            foreach (var ev in msb.Events.ObjActs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3ObjActEvent>();
                evt.GetComponent<MSB3ObjActEvent>().SetEvent(ev);
                evt.transform.parent = ObjActs.transform;
            }

            GameObject MapOffsets = new GameObject("MapOffsets");
            MapOffsets.transform.parent = Events.transform;
            foreach (var ev in msb.Events.MapOffsets)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3MapOffsetEvent>();
                evt.GetComponent<MSB3MapOffsetEvent>().SetEvent(ev);
                evt.transform.parent = MapOffsets.transform;
            }

            GameObject Invasions = new GameObject("Invasions");
            Invasions.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Invasions)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3InvasionEvent>();
                evt.GetComponent<MSB3InvasionEvent>().SetEvent(ev);
                evt.transform.parent = Invasions.transform;
            }

            GameObject WalkRouteEvents = new GameObject("WalkRoutes");
            WalkRouteEvents.transform.parent = Events.transform;
            foreach (var ev in msb.Events.WalkRoutes)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3WalkRouteEvent>();
                evt.GetComponent<MSB3WalkRouteEvent>().SetEvent(ev);
                evt.transform.parent = WalkRouteEvents.transform;
            }

            GameObject GroupTours = new GameObject("GroupTours");
            GroupTours.transform.parent = Events.transform;
            foreach (var ev in msb.Events.GroupTours)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3GroupTourEvent>();
                evt.GetComponent<MSB3GroupTourEvent>().SetEvent(ev);
                evt.transform.parent = GroupTours.transform;
            }

            GameObject Others = new GameObject("Others");
            Others.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Others)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB3OtherEvent>();
                evt.GetComponent<MSB3OtherEvent>().SetEvent(ev);
                evt.transform.parent = Others.transform;
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import failed", e.Message + "\n" + e.StackTrace, "Ok");
        }
    }

    void onImportDS1Map(object o, bool remastered)
    {
        try
        {
            string mapname = (string)o;
            var msb = DataFile.LoadFromFile<MSB>(Interroot + $@"\map\MapStudio\{mapname}.msb");
            int area = int.Parse(mapname.Substring(1, 2));

            if (!AssetDatabase.IsValidFolder("Assets/" + mapname))
            {
                AssetDatabase.CreateFolder("Assets", mapname);
            }

            // Create an MSB asset link to the DS3 asset
            GameObject AssetLink = new GameObject("MSBAssetLink");
            AssetLink.AddComponent<MSBAssetLink>();
            AssetLink.GetComponent<MSBAssetLink>().Interroot = Interroot;
            AssetLink.GetComponent<MSBAssetLink>().MapID = mapname;
            AssetLink.GetComponent<MSBAssetLink>().MapPath = $@"{Interroot}\map\MapStudio\{mapname}.msb";

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");

            GameObject MapPieceModels = new GameObject("MapPieces");
            MapPieceModels.transform.parent = ModelsSection.transform;

            // Do a preload of all the flvers
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var mappiece in msb.Models.MapPieces)
                {
                    var assetname = mappiece.Name;
                    if (AssetDatabase.FindAssets($@"Assets/{mapname}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapname}\{assetname}A{area:D2}.flver"))
                            ImportFlver(Interroot + $@"\map\{mapname}\{assetname}A{area:D2}.flver", $@"Assets/{mapname}/{assetname}", $@"Assets/UDSFMMapTextures");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            foreach (var mappiece in msb.Models.MapPieces)
            {
                var assetname = mappiece.Name;
                GameObject model = new GameObject(mappiece.Name);
                model.AddComponent<MSB1MapPieceModel>();
                model.GetComponent<MSB1MapPieceModel>().SetModel(mappiece);
                model.transform.parent = MapPieceModels.transform;
            }

            // Load low res hkx assets
            /*if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}");
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}");
                }
            }*/

            GameObject ObjectModels = new GameObject("Objects");
            ObjectModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Objects)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1ObjectModel>();
                model.GetComponent<MSB1ObjectModel>().SetModel(mod);
                model.transform.parent = ObjectModels.transform;
            }

            GameObject PlayerModels = new GameObject("Players");
            PlayerModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Players)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1PlayerModel>();
                model.GetComponent<MSB1PlayerModel>().SetModel(mod);
                model.transform.parent = PlayerModels.transform;
            }

            GameObject EnemyModels = new GameObject("Enemies");
            EnemyModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Characters)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1EnemyModel>();
                model.GetComponent<MSB1EnemyModel>().SetModel(mod);
                model.transform.parent = EnemyModels.transform;
            }

            GameObject CollisionModels = new GameObject("Collisions");
            CollisionModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Collisions)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1CollisionModel>();
                model.GetComponent<MSB1CollisionModel>().SetModel(mod);
                model.transform.parent = CollisionModels.transform;
            }

            GameObject NavModels = new GameObject("Navimeshes");
            NavModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Navimeshes)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1NavimeshModel>();
                model.GetComponent<MSB1NavimeshModel>().SetModel(mod);
                model.transform.parent = NavModels.transform;
            }

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapname}/{part.ModelName}.prefab") : null;
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1MapPiecePart>();
                    obj.GetComponent<MSB1MapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB1MapPiecePart>();
                    obj.GetComponent<MSB1MapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import failed", e.Message + "\n" + e.StackTrace, "Ok");
        }
    }

    void onImportMap(object o)
    {
        if (type == GameType.DarkSoulsPTDE)
        {
            onImportDS1Map(o, false);
        }
        else if (type == GameType.DarkSoulsRemastered)
        {
            onImportDS1Map(o, true);
        }
        else if (type == GameType.DarkSoulsIII)
        {
            onImportDS3Map(o);
        }
    }

    // Helper to get a child from a game object by name
    static GameObject GetChild(GameObject obj, string name)
    {
        var childTransform = obj.transform.Find(name);
        if (childTransform != null)
        {
            return childTransform.gameObject;
        }
        return null;
    }

    // Helper to get children of a node of a certain type
    private static List<GameObject> GetChildrenOfType<T>(GameObject obj) where T : MonoBehaviour
    {
        var output = new List<GameObject>();
        if (obj != null)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var o = obj.transform.GetChild(i).gameObject;
                if (o.GetComponent<T>() != null)
                {
                    output.Add(o);
                }
            }
        }
        return output;
    }

    static void ImportMTDBND(string path, bool isDS3)
    {
        IBinder bnd;
        if (isDS3)
        {
            bnd = BND4.Read(path);
        }
        else
        {
            bnd = BND3.Read(path);
        }

        if (!AssetDatabase.IsValidFolder("Assets/MTD"))
        {
            AssetDatabase.CreateFolder("Assets", "MTD");
        }

        foreach (var file in bnd.Files)
        {
            var mtd = SoulsFormats.MTD.Read(file.Bytes);
            var obj = ScriptableObject.CreateInstance<MTDAssetLink>();
            obj.InitializeFromMTD(mtd, file.Name);
            AssetDatabase.CreateAsset(obj, $@"Assets/MTD/{Path.GetFileNameWithoutExtension(file.Name)}.asset");
        }
    }

    // Serializes the open unity map to an MSB file. Requires an MSBAssetLink object in the scene
    void ExportMap()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a DS3 asset");
        }

        MSB3 export = new MSB3();
        // Export the models
        var Models = GameObject.Find("/MSBModelDeclarations");
        if (Models != null)
        {
            // Export map pieces
            var modelMapPieces = GetChild(Models, "MapPieces");
            if (modelMapPieces != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MapPieceModel>(modelMapPieces))
                {
                    export.Models.MapPieces.Add(obj.GetComponent<MSB3MapPieceModel>().Serialize(obj));
                }
            }

            // Export collision
            var modelCollision = GetChild(Models, "Collisions");
            if (modelCollision != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3CollisionModel>(modelCollision))
                {
                    export.Models.Collisions.Add(obj.GetComponent<MSB3CollisionModel>().Serialize(obj));
                }
            }

            // Export enemies
            var modelEnemy = GetChild(Models, "Enemies");
            if (modelEnemy != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3EnemyModel>(modelEnemy))
                {
                    export.Models.Enemies.Add(obj.GetComponent<MSB3EnemyModel>().Serialize(obj));
                }
            }

            // Export objects
            var modelObject = GetChild(Models, "Objects");
            if (modelObject != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3ObjectModel>(modelObject))
                {
                    export.Models.Objects.Add(obj.GetComponent<MSB3ObjectModel>().Serialize(obj));
                }
            }

            // Export others
            var modelOther = GetChild(Models, "Others");
            if (modelOther != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3OtherModel>(modelOther))
                {
                    export.Models.Others.Add(obj.GetComponent<MSB3OtherModel>().Serialize(obj));
                }
            }

            // Export players
            var modelPlayer = GetChild(Models, "Players");
            if (modelPlayer != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3PlayerModel>(modelPlayer))
                {
                    export.Models.Players.Add(obj.GetComponent<MSB3PlayerModel>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires a model declaration section");
        }

        // Export the models
        var Parts = GameObject.Find("/MSBParts");
        if (Parts != null)
        {
            // Export map pieces
            var partMapPieces = GetChild(Parts, "MapPieces");
            if (partMapPieces != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MapPiecePart>(partMapPieces))
                {
                    export.Parts.MapPieces.Add(obj.GetComponent<MSB3MapPiecePart>().Serialize(obj));
                }
            }

            // Export collisions
            var partCollisions = GetChild(Parts, "Collisions");
            if (partCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3CollisionPart>(partCollisions))
                {
                    export.Parts.Collisions.Add(obj.GetComponent<MSB3CollisionPart>().Serialize(obj));
                }
            }

            // Export connect collisions
            var partConnectCollisions = GetChild(Parts, "ConnectCollisions");
            if (partConnectCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3ConnectCollisionPart>(partConnectCollisions))
                {
                    export.Parts.ConnectCollisions.Add(obj.GetComponent<MSB3ConnectCollisionPart>().Serialize(obj));
                }
            }

            // Export dummy enemies
            var partDummyEnemies = GetChild(Parts, "DummyEnemies");
            if (partDummyEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3DummyEnemyPart>(partDummyEnemies))
                {
                    export.Parts.DummyEnemies.Add(obj.GetComponent<MSB3DummyEnemyPart>().Serialize(obj));
                }
            }

            var partDummyObjects = GetChild(Parts, "DummyObjects");
            if (partDummyObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3DummyObjectPart>(partDummyObjects))
                {
                    export.Parts.DummyObjects.Add(obj.GetComponent<MSB3DummyObjectPart>().Serialize(obj));
                }
            }

            var partEnemies = GetChild(Parts, "Enemies");
            if (partEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3EnemyPart>(partEnemies))
                {
                    export.Parts.Enemies.Add(obj.GetComponent<MSB3EnemyPart>().Serialize(obj));
                }
            }

            var partObjects = GetChild(Parts, "Objects");
            if (partObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3ObjectPart>(partObjects))
                {
                    export.Parts.Objects.Add(obj.GetComponent<MSB3ObjectPart>().Serialize(obj));
                }
            }

            var partPlayers = GetChild(Parts, "Players");
            if (partPlayers != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3PlayerPart>(partPlayers))
                {
                    export.Parts.Players.Add(obj.GetComponent<MSB3PlayerPart>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires a parts section");
        }

        // Export the points/regions
        var Regions = GameObject.Find("/MSBRegions");
        if (Regions != null)
        {

        }
        else
        {
            throw new Exception("MSB exporter requires a regions section");
        }

        export.Write(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".test", SoulsFormats.DCX.Type.DarkSouls3);
    }

    void OnGUI()
    {
        GUILayout.Label("Import Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Set DS Interroot"))
        {
            string file = EditorUtility.OpenFilePanel("Select Dark Souls exe file", "", "exe");
            Interroot = Path.GetDirectoryName(file);
            if (file.ToLower().Contains("darksouls.exe"))
            {
                type = GameType.DarkSoulsPTDE;
            }
            else if (file.ToLower().Contains("darksoulsremastered.exe"))
            {
                type = GameType.DarkSoulsRemastered;
            }
            else if (file.ToLower().Contains("darksoulsiii.exe"))
            {
                type = GameType.DarkSoulsIII;
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid game path", "This does not appear to be a path for a supported game", "Ok");
            }
            UpdateMapList();
        }

        GUILayout.Label("Interroot: " + Interroot);

        if (GUILayout.Button("Import FLVER"))
        {
            string file = EditorUtility.OpenFilePanel("Select a flver", "", "flver,dcx,mapbnd");
            try
            {
                ImportFlver(file, "Assets/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import MTDs"))
        {
            try
            {
                if (type != GameType.DarkSoulsIII)
                {
                    ImportMTDBND(Interroot + $@"\mtd\Mtd.mtdbnd", false);
                }
                else
                {
                    ImportMTDBND(Interroot + $@"\mtd\allmaterialbnd.mtdbnd.dcx", true);
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import TPFBHD"))
        {
            string file = EditorUtility.OpenFilePanel("Select a tpfbhd", "", "tpfbhd");
            try
            {
                ImportTpfbhd(file);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import DS1 UDSFM TPFs"))
        {
            try
            {
                ImportUDSFMMapTpfs(Interroot);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import HKXBHD"))
        {
            string file = EditorUtility.OpenFilePanel("Select a hkxbhd", "", "hkxbhd");
            try
            {
                ImportCollisionHKXBDT(file, "Assets/Test");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import OBJBND"))
        {
            string file = EditorUtility.OpenFilePanel("Select an objbnd", "", "dcx");
            try
            {
                ImportObj(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (GUILayout.Button("Import OBJs"))
        {
            if (Interroot == "")
            {
                EditorUtility.DisplayDialog("Import failed", "Please select the DS3 exe for your interroot directory.", "Ok");
            }
            else
            {
                try
                {
                    ImportObjs(Interroot + $@"\obj");
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
                }
            }
        }

        LoadMapFlvers = GUILayout.Toggle(LoadMapFlvers, "Load map piece models");
        LoadHighResCol = GUILayout.Toggle(LoadHighResCol, "Load high-resolution collision");

        if (GUILayout.Button("Import DS3 Map"))
        {
            GenericMenu menu = new GenericMenu();
            foreach (var map in Maps)
            {
                menu.AddItem(new GUIContent(map), false, onImportMap, map);
            }
            menu.ShowAsContext();
        }

        if (GUILayout.Button("Export DS3 Map"))
        {
            try
            {
                ExportMap();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Map Export failed", e.Message, "Ok");
            }
        }
    }
}