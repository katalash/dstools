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
        Bloodborne,
    }

    GameType type = GameType.Undefined;

    static void ImportCollisionHKXBDT(string path, string outputAssetPath, GameType game)
    {
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(pathBase + ".hkxbhd", pathBase + ".hkxbdt");
        foreach (var file in bxf.Files)
        {
            try
            {
                var hkx = HKX.Read(file.Bytes, (game == GameType.Bloodborne) ? HKX.HKXVariation.HKXBloodBorne : HKX.HKXVariation.HKXDS3);
                CollisionUtilities.ImportDS3CollisionHKX(hkx, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load hkx file " + file.Name);
            }
        }
    }

    static void ImportObj(string objpath, string objid, GameType type)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Obj"))
        {
            AssetDatabase.CreateFolder("Assets", "Obj");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/Obj/{objid}"))
        {
            AssetDatabase.CreateFolder("Assets/Obj", objid);
        }

        IBinder objbnd;
        string path = "";
        if (File.Exists($@"{objpath}\{objid}.objbnd.dcx"))
        {
            path = $@"{objpath}\{objid}.objbnd.dcx";
        }
        else if (File.Exists($@"{objpath}\{objid}.objbnd"))
        {
            path = $@"{objpath}\{objid}.objbnd";
        }
        else
        {
            throw new FileNotFoundException("Could not find bnd for object " + objid);
        }

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
        {
            objbnd = BND4.Read(path);
        }
        else
        {
            objbnd = BND3.Read(path);
        }
        var texentries = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".TPF"));
        foreach (var entry in texentries)
        {
            TPF tpf = TPF.Read(entry.Bytes);
            if (type == GameType.Bloodborne)
            {
                tpf.ConvertPS4ToPC();
            }
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
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Objbnd;
        link.ArchivePath = objpath;
        link.FlverPath = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, $@"Assets/Obj/{objid}", $@"Assets/Obj/{objid}");
    }

    static void ImportObjs(string objpath, GameType type)
    {
        var objFiles = Directory.GetFiles(objpath, @"*.objbnd.dcx")
                    .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                    .Select(Path.GetFileNameWithoutExtension) //Remove .objbnd
                    .ToArray();
        if (objFiles.Count() == 0)
        {
            objFiles = Directory.GetFiles(objpath, @"*.objbnd")
                    .Select(Path.GetFileNameWithoutExtension) //Remove .objbnd
                    .ToArray();
        }
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var obj in objFiles)
            {
                try
                {
                    ImportObj(objpath, obj, type);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading obj {obj}: {e.Message}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    static void ImportChr(string chrpath, string chrid, GameType type)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Chr"))
        {
            AssetDatabase.CreateFolder("Assets", "Chr");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/Chr/{chrid}"))
        {
            AssetDatabase.CreateFolder("Assets/Chr", chrid);
        }

        IBinder chrbnd;
        string path = "";
        if (File.Exists($@"{chrpath}\{chrid}.chrbnd.dcx"))
        {
            path = $@"{chrpath}\{chrid}.chrbnd.dcx";
        }
        else if (File.Exists($@"{chrpath}\{chrid}.chrbnd"))
        {
            path = $@"{chrpath}\{chrid}.chrbnd";
        }
        else
        {
            throw new FileNotFoundException("Could not find bnd for character " + chrid);
        }

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
        {
            chrbnd = BND4.Read(path);
        }
        else
        {
            chrbnd = BND3.Read(path);
        }
        var texentries = chrbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".TPF"));
        foreach (var entry in texentries)
        {
            TPF tpf = TPF.Read(entry.Bytes);
            if (type == GameType.Bloodborne)
            {
                tpf.ConvertPS4ToPC();
            }
            foreach (var tex in tpf.Textures)
            {
                var t2d = CreateTextureFromTPF(tex);
                if (t2d != null)
                {
                    AssetDatabase.CreateAsset(t2d, $@"Assets/Chr/{chrid}/{tex.Name}.texture2d");
                }
            }
        }

        // Load external DS1 UDSFM textures
        if (type == GameType.DarkSoulsPTDE)
        {
            if (Directory.Exists($@"{chrpath}\{chrid}"))
            {
                if (!AssetDatabase.IsValidFolder($@"Assets/Chr/sharedTextures"))
                {
                    AssetDatabase.CreateFolder("Assets/Chr", "sharedTextures");
                }
                var tpfFiles = Directory.GetFiles($@"{chrpath}\{chrid}", @"*.tpf")
                    .ToArray();
                foreach (var file in tpfFiles)
                {
                    TPF tpf = TPF.Read(file);
                    foreach (var tex in tpf.Textures)
                    {
                        var t2d = CreateTextureFromTPF(tex);
                        if (t2d != null)
                        {
                            AssetDatabase.CreateAsset(t2d, $@"Assets/Chr/sharedTextures/{tex.Name}.texture2d");
                        }
                    }
                }
            }
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(chrbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Chrbnd;
        link.ArchivePath = chrpath;
        link.FlverPath = chrbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, $@"Assets/Chr/{chrid}", $@"Assets/Chr/{chrid}");
    }

    static void ImportChrs(string chrpath, GameType type)
    {
        var chrFiles = Directory.GetFiles(chrpath, @"*.chrbnd.dcx")
                    .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                    .Select(Path.GetFileNameWithoutExtension) 
                    .ToArray();
        if (chrFiles.Count() == 0)
        {
            chrFiles = Directory.GetFiles(chrpath, @"*.chrbnd")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
        }
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var chr in chrFiles)
            {
                try
                {
                    ImportChr(chrpath, chr, type);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading chr {chr}: {e.Message}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
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

    static void ImportTpfbhd(string path, bool isPS4 = false)
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
                var t = TPF.Read(tpf.Bytes);
                if (isPS4)
                {
                    t.ConvertPS4ToPC();
                }
                var tex = CreateTextureFromTPF(t.Textures[0]);
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
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z), obj.transform);
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

    static void EulerToTransform(Vector3 e, Transform t)
    {
        /*float yaw = Mathf.Deg2Rad * e.z;
        float pitch = Mathf.Deg2Rad * e.y;
        float roll = Mathf.Deg2Rad * e.x;
        float cy = Mathf.Cos(yaw * 0.5f);
        float sy = Mathf.Sin(yaw * 0.5f);
        float cp = Mathf.Cos(pitch * 0.5f);
        float sp = Mathf.Sin(pitch * 0.5f);
        float cr = Mathf.Cos(roll * 0.5f);
        float sr = Mathf.Sin(roll * 0.5f);

        return new Quaternion(cy * cp * sr - sy * sp * cr,
                              sy * cp * sr + cy * sp * cr,
                              sy * cp * cr - cy * sp * sr,
                              cy * cp * cr + sy * sp * sr);*/

        // Apply in XZY order
        t.Rotate(new Vector3(1, 0, 0), e.x, Space.World); 
        t.Rotate(new Vector3(0, 1, 0), e.y, Space.World);
        t.Rotate(new Vector3(0, 0, 1), e.z, Space.World);
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
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/{mapname}/{assetname}", $@"Assets/{mapname.Substring(0, 3)}");
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
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd", $@"Assets/{mapname}", type);
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
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                    obj.AddComponent<MSB3ObjectPart>();
                    obj.GetComponent<MSB3ObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 10;
                    obj.transform.parent = Objects.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB3ObjectPart>();
                    obj.GetComponent<MSB3ObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = ConnectCollisions.transform;
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Chr/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB3EnemyPart>();
                    obj.GetComponent<MSB3EnemyPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 11;
                    obj.transform.parent = Enemies.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB3EnemyPart>();
                    obj.GetComponent<MSB3EnemyPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 11;
                    obj.transform.parent = Enemies.transform;
                }
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB3PlayerPart>();
                obj.GetComponent<MSB3PlayerPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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

    void onImportBBMap(object o)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSBBB.Read(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx");

            // Make adjusted mapname because bloodborne is a :fatcat:
            var mapnameAdj = mapname.Substring(0, 6) + "_00_00";

            if (!AssetDatabase.IsValidFolder("Assets/" + mapnameAdj))
            {
                AssetDatabase.CreateFolder("Assets", mapnameAdj);
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
                    if (AssetDatabase.FindAssets($@"Assets/{mapnameAdj}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx"))
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx", $@"Assets/{mapnameAdj}/{assetname}", $@"Assets/{mapnameAdj.Substring(0, 3)}");
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
                model.AddComponent<MSBBBMapPieceModel>();
                model.GetComponent<MSBBBMapPieceModel>().SetModel(mappiece);
                model.transform.parent = MapPieceModels.transform;
            }

            // Load low res hkx assets
            if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\map\{mapnameAdj}\h{mapnameAdj.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\h{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/{mapnameAdj}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/{mapnameAdj}", type);
                }
            }

            GameObject ObjectModels = new GameObject("Objects");
            ObjectModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Objects)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBBBObjectModel>();
                model.GetComponent<MSBBBObjectModel>().SetModel(mod);
                model.transform.parent = ObjectModels.transform;
            }

            GameObject PlayerModels = new GameObject("Players");
            PlayerModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Players)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBBBPlayerModel>();
                model.GetComponent<MSBBBPlayerModel>().SetModel(mod);
                model.transform.parent = PlayerModels.transform;
            }

            GameObject EnemyModels = new GameObject("Enemies");
            EnemyModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Enemies)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBBBEnemyModel>();
                model.GetComponent<MSBBBEnemyModel>().SetModel(mod);
                model.transform.parent = EnemyModels.transform;
            }

            GameObject CollisionModels = new GameObject("Collisions");
            CollisionModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Collisions)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBBBCollisionModel>();
                model.GetComponent<MSBBBCollisionModel>().SetModel(mod);
                model.transform.parent = CollisionModels.transform;
            }

            GameObject OtherModels = new GameObject("Others");
            OtherModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Others)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBBBOtherModel>();
                model.GetComponent<MSBBBOtherModel>().SetModel(mod);
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapnameAdj}/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBBBMapPiecePart>(); 
                    obj.GetComponent<MSBBBMapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSBBBMapPiecePart>();
                    obj.GetComponent<MSBBBMapPiecePart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                    obj.AddComponent<MSBBBObjectPart>();
                    obj.GetComponent<MSBBBObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 10;
                    obj.transform.parent = Objects.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSBBBObjectPart>();
                    obj.GetComponent<MSBBBObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapnameAdj}/{lowHigh}{mapnameAdj.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBBBCollisionPart>();
                    obj.GetComponent<MSBBBCollisionPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
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
                obj.AddComponent<MSBBBConnectCollisionPart>();
                obj.GetComponent<MSBBBConnectCollisionPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = ConnectCollisions.transform;
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Chr/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBBBEnemyPart>();
                    obj.GetComponent<MSBBBEnemyPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 11;
                    obj.transform.parent = Enemies.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSBBBEnemyPart>();
                    obj.GetComponent<MSBBBEnemyPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 11;
                    obj.transform.parent = Enemies.transform;
                }
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSBBBPlayerPart>();
                obj.GetComponent<MSBBBPlayerPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = Players.transform;
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSBBBDummyObjectPart>();
                obj.GetComponent<MSBBBDummyObjectPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = DummyObjects.transform;
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSBBBDummyEnemyPart>();
                obj.GetComponent<MSBBBDummyEnemyPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.transform.parent = DummyEnemies.transform;
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
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapname}\{assetname}A{area:D2}.flver", $@"Assets/{mapname}/{assetname}", $@"Assets/UDSFMMapTextures");
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
            AssetDatabase.StartAssetEditing();
            foreach (var mod in msb.Models.Collisions)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSB1CollisionModel>();
                model.GetComponent<MSB1CollisionModel>().SetModel(mod);
                model.transform.parent = CollisionModels.transform;

                if (type == GameType.DarkSoulsPTDE)
                {
                    var prefix = LoadHighResCol ? "h" : "l";
                    var path = $@"{Interroot}\map\{mapname}\{prefix}{mod.Name.Substring(1)}A{area:D2}.hkx";
                    if (File.Exists(path))
                    {
                        CollisionUtilities.ImportDS1CollisionHKX(HKX.Read(path, HKX.HKXVariation.HKXDS1), $@"Assets/{mapname}/{prefix}{mod.Name.Substring(1)}");
                    }
                }
            }
            AssetDatabase.StopAssetEditing();
    
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

            GameObject Objects = new GameObject("Objects");
            Objects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Objects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Obj/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1ObjectPart>();
                    obj.GetComponent<MSB1ObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 10;
                    obj.transform.parent = Objects.transform;
                }
            }

            GameObject NPCs = new GameObject("NPCs");
            NPCs.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.NPCs)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Chr/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1NPCPart>();
                    obj.GetComponent<MSB1NPCPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 11;
                    obj.transform.parent = NPCs.transform;
                }
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Hits)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/{mapname}/{lowHigh}{part.ModelName.Substring(1)}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1CollisionPart>();
                    obj.GetComponent<MSB1CollisionPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 12;
                    obj.transform.parent = Collisions.transform;
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
        else if (type == GameType.Bloodborne)
        {
            onImportBBMap(o);
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
            var regionActAreas = GetChild(Regions, "ActivationAreas");
            if (regionActAreas != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3ActivationAreaRegion>(regionActAreas))
                {
                    export.Regions.ActivationAreas.Add(obj.GetComponent<MSB3ActivationAreaRegion>().Serialize(obj));
                }
            }

            var regionEnvMapEffectBoxes = GetChild(Regions, "EnvMapEffectBoxes");
            if (regionEnvMapEffectBoxes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3EnvironmentEffectBoxRegion>(regionEnvMapEffectBoxes))
                {
                    export.Regions.EnvironmentMapEffectBoxes.Add(obj.GetComponent<MSB3EnvironmentEffectBoxRegion>().Serialize(obj));
                }
            }

            var regionEnvMapPoints = GetChild(Regions, "EnvMapPoints");
            if (regionEnvMapPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3EnvironmentMapPointRegion>(regionEnvMapPoints))
                {
                    export.Regions.EnvironmentMapPoints.Add(obj.GetComponent<MSB3EnvironmentMapPointRegion>().Serialize(obj));
                }
            }

            var regionEvents = GetChild(Regions, "Events");
            if (regionEvents != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3EventRegion>(regionEvents))
                {
                    export.Regions.Events.Add(obj.GetComponent<MSB3EventRegion>().Serialize(obj));
                }
            }

            var regionGeneral = GetChild(Regions, "GeneralRegions");
            if (regionGeneral != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3GeneralRegion>(regionGeneral))
                {
                    export.Regions.General.Add(obj.GetComponent<MSB3GeneralRegion>().Serialize(obj));
                }
            }

            var regionInvasionPoints = GetChild(Regions, "InvasionPoints");
            if (regionInvasionPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3InvasionPointRegion>(regionInvasionPoints))
                {
                    export.Regions.InvasionPoints.Add(obj.GetComponent<MSB3InvasionPointRegion>().Serialize(obj));
                }
            }

            var regionMessages = GetChild(Regions, "Messages");
            if (regionMessages != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MessageRegion>(regionMessages))
                {
                    export.Regions.Messages.Add(obj.GetComponent<MSB3MessageRegion>().Serialize(obj));
                }
            }

            var regionMufflingBox = GetChild(Regions, "MufflingBox");
            if (regionMufflingBox != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MufflingBoxRegion>(regionMufflingBox))
                {
                    export.Regions.MufflingBoxes.Add(obj.GetComponent<MSB3MufflingBoxRegion>().Serialize(obj));
                }
            }

            var regionMufflingPortals = GetChild(Regions, "MufflingPortals");
            if (regionMufflingPortals != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MufflingPortal>(regionMufflingPortals))
                {
                    export.Regions.MufflingPortals.Add(obj.GetComponent<MSB3MufflingPortal>().Serialize(obj));
                }
            }

            var regionSFX = GetChild(Regions, "SFX");
            if (regionSFX != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3SFXRegion>(regionSFX))
                {
                    export.Regions.SFX.Add(obj.GetComponent<MSB3SFXRegion>().Serialize(obj));
                }
            }

            var regionSounds = GetChild(Regions, "Sounds");
            if (regionSounds != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3SoundRegion>(regionSounds))
                {
                    export.Regions.Sounds.Add(obj.GetComponent<MSB3SoundRegion>().Serialize(obj));
                }
            }

            var regionSpawnPoints = GetChild(Regions, "SpawnPoints");
            if (regionSpawnPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3SpawnPointRegion>(regionSpawnPoints))
                {
                    export.Regions.SpawnPoints.Add(obj.GetComponent<MSB3SpawnPointRegion>().Serialize(obj));
                }
            }

            var regionWalkRoutes = GetChild(Regions, "WalkRoutes");
            if (regionWalkRoutes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3WalkRouteRegion>(regionWalkRoutes))
                {
                    export.Regions.WalkRoutes.Add(obj.GetComponent<MSB3WalkRouteRegion>().Serialize(obj));
                }
            }

            var regionWarpPoints = GetChild(Regions, "WarpPoints");
            if (regionWarpPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3WarpPointRegion>(regionWarpPoints))
                {
                    export.Regions.WarpPoints.Add(obj.GetComponent<MSB3WarpPointRegion>().Serialize(obj));
                }
            }

            var regionWindAreas = GetChild(Regions, "WindAreas");
            if (regionWindAreas != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3WindAreaRegion>(regionWindAreas))
                {
                    export.Regions.WindAreas.Add(obj.GetComponent<MSB3WindAreaRegion>().Serialize(obj));
                }
            }

            var regionWindSFX = GetChild(Regions, "WindSFXRegions");
            if (regionWindSFX != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3WindSFXRegion>(regionWindSFX))
                {
                    export.Regions.WindSFX.Add(obj.GetComponent<MSB3WindSFXRegion>().Serialize(obj));
                }
            }

            var regionUnk00 = GetChild(Regions, "Unk00Regions");
            if (regionUnk00 != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3Unk00Region>(regionUnk00))
                {
                    export.Regions.Unk00s.Add(obj.GetComponent<MSB3Unk00Region>().Serialize(obj));
                }
            }

            var regionUnk12 = GetChild(Regions, "Unk12Regions");
            if (regionUnk12 != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3Unk12Region>(regionUnk12))
                {
                    export.Regions.Unk12s.Add(obj.GetComponent<MSB3Unk12Region>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires a regions section");
        }

        // Export the points/regions
        var Events = GameObject.Find("/MSBEvents");
        if (Events != null)
        {
            var eventTreasures = GetChild(Events, "Treasures");
            if (eventTreasures != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3TreasureEvent>(eventTreasures))
                {
                    export.Events.Treasures.Add(obj.GetComponent<MSB3TreasureEvent>().Serialize(obj));
                }
            }

            var eventGenerators = GetChild(Events, "Generators");
            if (eventGenerators != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3GeneratorEvent>(eventGenerators))
                {
                    export.Events.Generators.Add(obj.GetComponent<MSB3GeneratorEvent>().Serialize(obj));
                }
            }

            var eventObjActs = GetChild(Events, "ObjActs");
            if (eventObjActs != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3ObjActEvent>(eventObjActs))
                {
                    export.Events.ObjActs.Add(obj.GetComponent<MSB3ObjActEvent>().Serialize(obj));
                }
            }

            var eventMapOffsets = GetChild(Events, "MapOffsets");
            if (eventMapOffsets != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3MapOffsetEvent>(eventMapOffsets))
                {
                    export.Events.MapOffsets.Add(obj.GetComponent<MSB3MapOffsetEvent>().Serialize(obj));
                }
            }

            var eventInvasions = GetChild(Events, "Invasions");
            if (eventInvasions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3InvasionEvent>(eventInvasions))
                {
                    export.Events.Invasions.Add(obj.GetComponent<MSB3InvasionEvent>().Serialize(obj));
                }
            }

            var eventWalkRoutes = GetChild(Events, "WalkRoutes");
            if (eventWalkRoutes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3WalkRouteEvent>(eventWalkRoutes))
                {
                    export.Events.WalkRoutes.Add(obj.GetComponent<MSB3WalkRouteEvent>().Serialize(obj));
                }
            }

            var eventGroupTours = GetChild(Events, "GroupTours");
            if (eventGroupTours != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3GroupTourEvent>(eventGroupTours))
                {
                    export.Events.GroupTours.Add(obj.GetComponent<MSB3GroupTourEvent>().Serialize(obj));
                }
            }

            var eventOthers = GetChild(Events, "Others");
            if (eventOthers != null)
            {
                foreach (var obj in GetChildrenOfType<MSB3OtherEvent>(eventOthers))
                {
                    export.Events.Others.Add(obj.GetComponent<MSB3OtherEvent>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires an events section");
        }

        // Save a backup if one doesn't exist
        if (!File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup"))
        {
            File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup");
        }

        export.Write(AssetLink.GetComponent<MSBAssetLink>().MapPath, SoulsFormats.DCX.Type.DarkSouls3);
        //export.Write(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".msb", SoulsFormats.DCX.Type.None);
    }

    void OnGUI()
    { 
        GUILayout.Label("Import Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Set DS Interroot"))
        {
            string file = EditorUtility.OpenFilePanel("Select Dark Souls exe file", "", "exe,bin");
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
            else if (file.ToLower().Contains("eboot.bin"))
            {
                type = GameType.Bloodborne;
                Interroot = Interroot + $@"\dvdroot_ps4";
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
                FlverUtilities.ImportFlver(file, "Assets/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)));
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
                ImportTpfbhd(file, (type == GameType.Bloodborne));
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
                ImportCollisionHKXBDT(file, "Assets/Test", type);
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
                ImportObj(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)), type);
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
                    ImportObjs(Interroot + $@"\obj", type);
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
                }
            }
        }

        if (GUILayout.Button("Import CHRs"))
        {
            if (Interroot == "")
            {
                EditorUtility.DisplayDialog("Import failed", "Please select the DS3 exe for your interroot directory.", "Ok");
            }
            else
            {
                try
                {
                    ImportChrs(Interroot + $@"\chr", type);
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
                }
            }
        }

        if (GUILayout.Button("Generate new lightmap UVs for flver"))
        {
            try
            {
                if (Selection.activeObject != null && Selection.activeObject is FLVERAssetLink)
                {
                    //((FLVERAssetLink)Selection.activeObject).GenerateLightmapUVS();
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Failed", e.Message, "Ok");
            }
        }

        LoadMapFlvers = GUILayout.Toggle(LoadMapFlvers, "Load map piece models");
        LoadHighResCol = GUILayout.Toggle(LoadHighResCol, "Load high-resolution collision");

        if (GUILayout.Button("Import Map"))
        {
            GenericMenu menu = new GenericMenu();
            foreach (var map in Maps)
            {
                menu.AddItem(new GUIContent(map), false, onImportMap, map);
            }
            menu.ShowAsContext();
        }

        if (GUILayout.Button("Export Map"))
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