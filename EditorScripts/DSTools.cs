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
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

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

    static string GameFolder(GameType game)
    {
        if (game == GameType.DarkSoulsPTDE)
        {
            if (!AssetDatabase.IsValidFolder("Assets/DS1"))
            {
                AssetDatabase.CreateFolder("Assets", "DS1");
            }
            return "DS1";
        }
        else if (game == GameType.DarkSoulsRemastered)
        {
            if (!AssetDatabase.IsValidFolder("Assets/DSR"))
            {
                AssetDatabase.CreateFolder("Assets", "DSR");
            }
            return "DSR";
        }
        else if (game == GameType.DarkSoulsIII)
        {
            if (!AssetDatabase.IsValidFolder("Assets/DS3"))
            {
                AssetDatabase.CreateFolder("Assets", "DS3");
            }
            return "DS3";
        }
        else if (game == GameType.Bloodborne)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne"))
            {
                AssetDatabase.CreateFolder("Assets", "Bloodborne");
            }
            return "Bloodborne";
        }
        throw new Exception("No directory for game type");
    }

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
        string gameFolder = GameFolder(type);
        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Obj"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Obj");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Obj/{objid}"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Obj", objid);
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
                    AssetDatabase.CreateAsset(t2d, $@"Assets/{gameFolder}/Obj/{objid}/{tex.Name}.texture2d");
                }
            }
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Objbnd;
        link.ArchivePath = objpath;
        link.FlverPath = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, $@"Assets/{gameFolder}/Obj/{objid}", $@"Assets/{gameFolder}/Obj/{objid}");
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
        string gameFolder = GameFolder(type);
        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Chr"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Chr");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Chr/{chrid}"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Chr", chrid);
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
                    AssetDatabase.CreateAsset(t2d, $@"Assets/{gameFolder}/Chr/{chrid}/{tex.Name}.texture2d");
                }
            }
        }

        // Load external DS1 UDSFM textures
        if (type == GameType.DarkSoulsPTDE)
        {
            if (Directory.Exists($@"{chrpath}\{chrid}"))
            {
                if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Chr/sharedTextures"))
                {
                    AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Chr", "sharedTextures");
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
                            AssetDatabase.CreateAsset(t2d, $@"Assets/{gameFolder}/Chr/sharedTextures/{tex.Name}.texture2d");
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
        FlverUtilities.ImportFlver(flver, link, $@"Assets/{gameFolder}/Chr/{chrid}", $@"Assets/{gameFolder}/Chr/{chrid}");
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

    static void ImportTpfbhd(string path, GameType type, bool isPS4 = false)
    {
        string gameFolder = GameFolder(type);
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(pathBase + ".tpfbhd", pathBase + ".tpfbdt");
        
        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/" + name.Substring(0, 3)))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", name.Substring(0, 3));
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
                AssetDatabase.CreateAsset(tex, $@"Assets/{gameFolder}/" + name.Substring(0, 3) + "/" + Path.GetFileNameWithoutExtension((Path.GetFileNameWithoutExtension(tpf.Name))) + ".texture2d");
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
        string gameFolder = GameFolder(GameType.DarkSoulsPTDE);
        if (!AssetDatabase.IsValidFolder("Assets/DS1/UDSFMMapTextures"))
        {
            AssetDatabase.CreateFolder("Assets/DS1", "UDSFMMapTextures");
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
                AssetDatabase.CreateAsset(tex, $@"Assets/DS1/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(file)}.texture2d");
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

    GameObject InstantiateRegion(MSBBB.Region region, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.Position.X, region.Position.Y, region.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z), obj.transform);
        if (region is MSBBB.Region.Box)
        {
            var shape = (MSBBB.Region.Box)region;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, shape.Height, shape.Length);
        }
        else if (region is MSBBB.Region.Sphere)
        {
            var shape = (MSBBB.Region.Sphere)region;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region is MSBBB.Region.Point)
        {
            var shape = (MSBBB.Region.Point)region;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region is MSBBB.Region.Cylinder)
        {
            var shape = (MSBBB.Region.Cylinder)region;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        else
        {
            Debug.Log("Unsupported region type encountered");
        }
        obj.layer = 13;
        obj.transform.parent = parent.transform;
        return obj;
    }

    GameObject InstantiateRegion(MsbRegionBase region, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.PosX, region.PosY, region.PosZ);
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.RotX, region.RotY, region.RotZ), obj.transform);
        if (region is MsbRegionBox)
        {
            var shape = (MsbRegionBox)region;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.WidthX, shape.HeightY, shape.DepthZ);
        }
        else if (region is MsbRegionSphere)
        {
            var shape = (MsbRegionSphere)region;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region is MsbRegionPoint)
        {
            var shape = (MsbRegionPoint)region;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region is MsbRegionCylinder)
        {
            var shape = (MsbRegionCylinder)region;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        else
        {
            Debug.Log("Unsupported region type encountered");
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
            string gameFolder = GameFolder(GameType.DarkSoulsIII);

            if (!AssetDatabase.IsValidFolder("Assets/DS3/" + mapname))
            {
                AssetDatabase.CreateFolder("Assets/DS3", mapname);
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
                    if (AssetDatabase.FindAssets($@"Assets/DS3/{mapname}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx"))
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/DS3/{mapname}/{assetname}", $@"Assets/DS3/{mapname.Substring(0, 3)}");
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
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\h{mapname.Substring(1)}.hkxbhd", $@"Assets/DS3/{mapname}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapname}\l{mapname.Substring(1)}.hkxbhd", $@"Assets/DS3/{mapname}", type);
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
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/{mapname}/{part.ModelName}.prefab") : null;
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/Obj/{part.ModelName}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/{mapname}/{lowHigh}{mapname.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/Chr/{part.ModelName}.prefab");
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
            foreach (var ev in msb.Events.PseudoMultiplayers)
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
            GameFolder(GameType.Bloodborne);

            // Make adjusted mapname because bloodborne is a :fatcat:
            var mapnameAdj = mapname.Substring(0, 6) + "_00_00";

            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne/" + mapnameAdj))
            {
                AssetDatabase.CreateFolder("Assets/Bloodborne", mapnameAdj);
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
                    if (AssetDatabase.FindAssets($@"Assets/Bloodborne/{mapnameAdj}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx"))
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx", $@"Assets/Bloodborne/{mapnameAdj}/{assetname}", $@"Assets/Bloodborne/{mapnameAdj.Substring(0, 3)}");
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
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\h{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/Bloodborne/{mapnameAdj}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/Bloodborne/{mapnameAdj}", type);
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/{mapnameAdj}/{part.ModelName}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Obj/{part.ModelName}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/{mapnameAdj}/{lowHigh}{mapnameAdj.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Chr/{part.ModelName}.prefab");
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Obj/{part.ModelName}.prefab");
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
                    obj.layer = 12;
                    obj.transform.parent = DummyObjects.transform;
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
                    obj.layer = 12;
                    obj.transform.parent = DummyObjects.transform;
                }
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Chr/{part.ModelName}.prefab");
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
                    obj.layer = 13;
                    obj.transform.parent = DummyEnemies.transform;
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
                    obj.layer = 13;
                    obj.transform.parent = DummyEnemies.transform;
                }
            }

            //
            // Regions section
            //
            GameObject Regions = new GameObject("MSBRegions");
            foreach (var region in msb.Regions.Regions)
            {
                var reg = InstantiateRegion(region, Regions);
                reg.AddComponent<MSBBBRegion>();
                reg.GetComponent<MSBBBRegion>().setBaseRegion(region);
            }

            //
            // Events Section
            //
            GameObject Events = new GameObject("MSBEvents");

            GameObject Sounds = new GameObject("Sounds");
            Sounds.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Sounds)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBSoundEvent>();
                evt.GetComponent<MSBBBSoundEvent>().SetEvent(ev);
                evt.transform.parent = Sounds.transform;
            }

            GameObject SFXs = new GameObject("SFX");
            SFXs.transform.parent = Events.transform;
            foreach (var ev in msb.Events.SFXs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBSFXEvent>();
                evt.GetComponent<MSBBBSFXEvent>().SetEvent(ev);
                evt.transform.parent = SFXs.transform;
            }

            GameObject Treasures = new GameObject("Treasures");
            Treasures.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Treasures)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBTreasureEvent>();
                evt.GetComponent<MSBBBTreasureEvent>().SetEvent(ev);
                evt.transform.parent = Treasures.transform;
            }

            GameObject Generators = new GameObject("Generators");
            Generators.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Generators)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBGeneratorEvent>();
                evt.GetComponent<MSBBBGeneratorEvent>().SetEvent(ev);
                evt.transform.parent = Generators.transform;
            }

            GameObject Messages = new GameObject("Messages");
            Messages.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Messages)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBMessageEvent>();
                evt.GetComponent<MSBBBMessageEvent>().SetEvent(ev);
                evt.transform.parent = Messages.transform;
            }

            GameObject ObjActs = new GameObject("ObjActs");
            ObjActs.transform.parent = Events.transform;
            foreach (var ev in msb.Events.ObjActs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBObjActEvent>();
                evt.GetComponent<MSBBBObjActEvent>().SetEvent(ev);
                evt.transform.parent = ObjActs.transform;
            }

            GameObject SpawnPoints = new GameObject("SpawnPoints");
            SpawnPoints.transform.parent = Events.transform;
            foreach (var ev in msb.Events.SpawnPoints)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBSpawnPointEvent>();
                evt.GetComponent<MSBBBSpawnPointEvent>().SetEvent(ev);
                evt.transform.parent = SpawnPoints.transform;
            }

            GameObject MapOffsets = new GameObject("MapOffsets");
            MapOffsets.transform.parent = Events.transform;
            foreach (var ev in msb.Events.MapOffsets)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBMapOffsetEvent>();
                evt.GetComponent<MSBBBMapOffsetEvent>().SetEvent(ev);
                evt.transform.parent = MapOffsets.transform;
            }

            GameObject Navimeshes = new GameObject("Navimeshes");
            Navimeshes.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Navimeshes)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBNavimeshEvent>();
                evt.GetComponent<MSBBBNavimeshEvent>().SetEvent(ev);
                evt.transform.parent = Navimeshes.transform;
            }

            GameObject Environments = new GameObject("Environments");
            Environments.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Environments)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBEnvironmentEvent>();
                evt.GetComponent<MSBBBEnvironmentEvent>().SetEvent(ev);
                evt.transform.parent = Environments.transform;
            }

            GameObject Winds = new GameObject("Winds");
            Winds.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Winds)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBWindEvent>();
                evt.GetComponent<MSBBBWindEvent>().SetEvent(ev);
                evt.transform.parent = Winds.transform;
            }

            GameObject Invasions = new GameObject("Invasions");
            Invasions.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Invasions)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBInvasionEvent>();
                evt.GetComponent<MSBBBInvasionEvent>().SetEvent(ev);
                evt.transform.parent = Invasions.transform;
            }

            GameObject WalkRoutes = new GameObject("WalkRoutes");
            WalkRoutes.transform.parent = Events.transform;
            foreach (var ev in msb.Events.WalkRoutes)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBWalkRouteEvent>();
                evt.GetComponent<MSBBBWalkRouteEvent>().SetEvent(ev);
                evt.transform.parent = WalkRoutes.transform;
            }

            GameObject Unknowns = new GameObject("Unknowns");
            Unknowns.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Unknowns)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBUnknownEvent>();
                evt.GetComponent<MSBBBUnknownEvent>().SetEvent(ev);
                evt.transform.parent = Unknowns.transform;
            }

            GameObject GroupTours = new GameObject("GroupTours");
            GroupTours.transform.parent = Events.transform;
            foreach (var ev in msb.Events.GroupTours)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBGroupTourEvent>();
                evt.GetComponent<MSBBBGroupTourEvent>().SetEvent(ev);
                evt.transform.parent = GroupTours.transform;
            }

            GameObject Others = new GameObject("Others");
            Others.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Others)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBBBOtherEvent>();
                evt.GetComponent<MSBBBOtherEvent>().SetEvent(ev);
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
            GameFolder(GameType.DarkSoulsPTDE);
            // Make adjusted mapname so darkroot garden meme works
            var mapnameAdj = mapname.Substring(0, 6) + "_00_00";

            if (!AssetDatabase.IsValidFolder("Assets/DS1/" + mapnameAdj))
            {
                AssetDatabase.CreateFolder("Assets/DS1", mapnameAdj);
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
                    if (AssetDatabase.FindAssets($@"Assets/DS1/{mapnameAdj}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapnameAdj}\{assetname}A{area:D2}.flver"))
                            FlverUtilities.ImportFlver(Interroot + $@"\map\{mapnameAdj}\{assetname}A{area:D2}.flver", $@"Assets/DS1/{mapnameAdj}/{assetname}", $@"Assets/DS1/UDSFMMapTextures");
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
                    var path = $@"{Interroot}\map\{mapnameAdj}\{prefix}{mod.Name.Substring(1)}A{area:D2}.hkx";
                    if (File.Exists(path))
                    {
                        CollisionUtilities.ImportDS1CollisionHKX(HKX.Read(path, HKX.HKXVariation.HKXDS1), $@"Assets/DS1/{mapnameAdj}/{prefix}{mod.Name.Substring(1)}");
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
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/{mapnameAdj}/{part.ModelName}.prefab") : null;
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
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
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB1ObjectPart>();
                    obj.GetComponent<MSB1ObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 10;
                    obj.transform.parent = Objects.transform;
                }
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1DummyObjectPart>();
                    obj.GetComponent<MSB1DummyObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 10;
                    obj.transform.parent = DummyObjects.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB1DummyObjectPart>();
                    obj.GetComponent<MSB1DummyObjectPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 10;
                    obj.transform.parent = DummyObjects.transform;
                }
            }

            GameObject NPCs = new GameObject("NPCs");
            NPCs.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.NPCs)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Chr/{part.ModelName}.prefab");
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
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB1NPCPart>();
                    obj.GetComponent<MSB1NPCPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 11;
                    obj.transform.parent = NPCs.transform;
                }
            }

            GameObject DummyNPCs = new GameObject("DummyNPCs");
            DummyNPCs.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyNPCs)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSB1DummyNPCPart>();
                    obj.GetComponent<MSB1DummyNPCPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 11;
                    obj.transform.parent = DummyNPCs.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.AddComponent<MSB1DummyNPCPart>();
                    obj.GetComponent<MSB1DummyNPCPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                    obj.transform.rotation = Quaternion.Euler(part.RotX, part.RotY, part.RotZ);
                    obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                    obj.layer = 11;
                    obj.transform.parent = DummyNPCs.transform;
                }
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Hits)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/{mapnameAdj}/{lowHigh}{part.ModelName.Substring(1)}.prefab");
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

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectHits)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB1ConnectCollisionPart>();
                obj.GetComponent<MSB1ConnectCollisionPart>().SetPart(part);
                obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                EulerToTransform(new Vector3(part.RotX, part.RotY, part.RotZ), obj.transform);
                obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                obj.transform.parent = ConnectCollisions.transform;
            }

            GameObject Navimeshes = new GameObject("Navimeshes");
            Navimeshes.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Navimeshes)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB1NavimeshPart>();
                obj.GetComponent<MSB1NavimeshPart>().SetPart(part);
                obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                EulerToTransform(new Vector3(part.RotX, part.RotY, part.RotZ), obj.transform);
                obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                obj.transform.parent = Navimeshes.transform;
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSB1PlayerPart>();
                obj.GetComponent<MSB1PlayerPart>().SetPart(part);
                obj.transform.position = new Vector3(part.PosX, part.PosY, part.PosZ);
                EulerToTransform(new Vector3(part.RotX, part.RotY, part.RotZ), obj.transform);
                obj.transform.localScale = new Vector3(part.ScaleX, part.ScaleY, part.ScaleZ);
                obj.transform.parent = Players.transform;
            }

            //
            // Regions section
            //
            GameObject Regions = new GameObject("MSBRegions");
            foreach (var region in msb.Regions)
            {
                var reg = InstantiateRegion(region, Regions);
                reg.AddComponent<MSB1Region>();
                reg.GetComponent<MSB1Region>().setBaseRegion(region);
            }

            //
            // Events Section
            //
            GameObject Events = new GameObject("MSBEvents");

            GameObject Environments = new GameObject("Environments");
            Environments.transform.parent = Events.transform;
            foreach (var ev in msb.Events.EnvLightMapSpot)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1EnvironmentEvent>();
                evt.GetComponent<MSB1EnvironmentEvent>().SetEvent(ev);
                evt.transform.parent = Environments.transform;
            }

            GameObject Generators = new GameObject("Generators");
            Generators.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Generators)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1GeneratorEvent>();
                evt.GetComponent<MSB1GeneratorEvent>().SetEvent(ev);
                evt.transform.parent = Generators.transform;
            }

            GameObject Invasions = new GameObject("Invasions");
            Invasions.transform.parent = Events.transform;
            foreach (var ev in msb.Events.NpcWorldInvitations)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1InvasionEvent>();
                evt.GetComponent<MSB1InvasionEvent>().SetEvent(ev);
                evt.transform.parent = Invasions.transform;
            }

            GameObject Lights = new GameObject("Lights");
            Lights.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Lights)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1LightEvent>();
                evt.GetComponent<MSB1LightEvent>().SetEvent(ev);
                evt.transform.parent = Lights.transform;
            }

            GameObject MapOffsets = new GameObject("MapOffsets");
            MapOffsets.transform.parent = Events.transform;
            foreach (var ev in msb.Events.MapOffsets)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1MapOffsetEvent>();
                evt.GetComponent<MSB1MapOffsetEvent>().SetEvent(ev);
                evt.transform.parent = MapOffsets.transform;
            }

            GameObject Messages = new GameObject("Messages");
            Messages.transform.parent = Events.transform;
            foreach (var ev in msb.Events.BloodMessages)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1MessageEvent>();
                evt.GetComponent<MSB1MessageEvent>().SetEvent(ev);
                evt.transform.parent = Messages.transform;
            }

            GameObject NavimeshEvents = new GameObject("Navimeshes");
            NavimeshEvents.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Navimeshes)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1NavimeshEvent>();
                evt.GetComponent<MSB1NavimeshEvent>().SetEvent(ev);
                evt.transform.parent = NavimeshEvents.transform;
            }

            GameObject ObjActs = new GameObject("ObjActs");
            ObjActs.transform.parent = Events.transform;
            foreach (var ev in msb.Events.ObjActs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1ObjActEvent>();
                evt.GetComponent<MSB1ObjActEvent>().SetEvent(ev);
                evt.transform.parent = ObjActs.transform;
            }

            GameObject SFX = new GameObject("SFX");
            SFX.transform.parent = Events.transform;
            foreach (var ev in msb.Events.SFXs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1SFXEvent>();
                evt.GetComponent<MSB1SFXEvent>().SetEvent(ev);
                evt.transform.parent = SFX.transform;
            }

            GameObject Sounds = new GameObject("Sounds");
            Sounds.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Sounds)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1SoundEvent>();
                evt.GetComponent<MSB1SoundEvent>().SetEvent(ev);
                evt.transform.parent = Sounds.transform;
            }

            GameObject SpawnPoints = new GameObject("SpawnPoints");
            SpawnPoints.transform.parent = Events.transform;
            foreach (var ev in msb.Events.SpawnPoints)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1SpawnPointEvent>();
                evt.GetComponent<MSB1SpawnPointEvent>().SetEvent(ev);
                evt.transform.parent = SpawnPoints.transform;
            }

            GameObject Treasures = new GameObject("Treasures");
            Treasures.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Treasures)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1TreasureEvent>();
                evt.GetComponent<MSB1TreasureEvent>().SetEvent(ev);
                evt.transform.parent = Treasures.transform;
            }

            GameObject Winds = new GameObject("Winds");
            Winds.transform.parent = Events.transform;
            foreach (var ev in msb.Events.WindSFXs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSB1WindEvent>();
                evt.GetComponent<MSB1WindEvent>().SetEvent(ev);
                evt.transform.parent = Winds.transform;
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
    public static List<GameObject> GetChildrenOfType<T>(GameObject obj) where T : MonoBehaviour
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

    public static List<GameObject> GetChildren(GameObject obj)
    {
        var output = new List<GameObject>();
        if (obj != null)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var o = obj.transform.GetChild(i).gameObject;
                output.Add(o);
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

    void ExportMapDS1()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a DS1 asset");
        }

        MSB export = new MSB();
        // Export the models
        var Models = GameObject.Find("/MSBModelDeclarations");
        if (Models != null)
        {
            // Export map pieces
            var modelMapPieces = GetChild(Models, "MapPieces");
            if (modelMapPieces != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1MapPieceModel>(modelMapPieces))
                {
                    export.Models.MapPieces.Add(obj.GetComponent<MSB1MapPieceModel>().Serialize(obj));
                }
            }

            // Export collision
            var modelCollision = GetChild(Models, "Collisions");
            if (modelCollision != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1CollisionModel>(modelCollision))
                {
                    export.Models.Collisions.Add(obj.GetComponent<MSB1CollisionModel>().Serialize(obj));
                }
            }

            // Export enemies
            var modelEnemy = GetChild(Models, "Enemies");
            if (modelEnemy != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1EnemyModel>(modelEnemy))
                {
                    export.Models.Characters.Add(obj.GetComponent<MSB1EnemyModel>().Serialize(obj));
                }
            }

            // Export objects
            var modelObject = GetChild(Models, "Objects");
            if (modelObject != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1ObjectModel>(modelObject))
                {
                    export.Models.Objects.Add(obj.GetComponent<MSB1ObjectModel>().Serialize(obj));
                }
            }

            // Export others
            var modelNavimeshes = GetChild(Models, "Navimeshes");
            if (modelNavimeshes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1NavimeshModel>(modelNavimeshes))
                {
                    export.Models.Navimeshes.Add(obj.GetComponent<MSB1NavimeshModel>().Serialize(obj));
                }
            }

            // Export players
            var modelPlayer = GetChild(Models, "Players");
            if (modelPlayer != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1PlayerModel>(modelPlayer))
                {
                    export.Models.Players.Add(obj.GetComponent<MSB1PlayerModel>().Serialize(obj));
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
                foreach (var obj in GetChildrenOfType<MSB1MapPiecePart>(partMapPieces))
                {
                    export.Parts.MapPieces.Add(obj.GetComponent<MSB1MapPiecePart>().Serialize(obj));
                }
            }

            // Export collisions
            var partCollisions = GetChild(Parts, "Collisions");
            if (partCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1CollisionPart>(partCollisions))
                {
                    export.Parts.Hits.Add(obj.GetComponent<MSB1CollisionPart>().Serialize(obj));
                }
            }

            // Export connect collisions
            var partConnectCollisions = GetChild(Parts, "ConnectCollisions");
            if (partConnectCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1ConnectCollisionPart>(partConnectCollisions))
                {
                    export.Parts.ConnectHits.Add(obj.GetComponent<MSB1ConnectCollisionPart>().Serialize(obj));
                }
            }

            // Export dummy enemies
            var partDummyEnemies = GetChild(Parts, "DummyNPCs");
            if (partDummyEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1DummyNPCPart>(partDummyEnemies))
                {
                    export.Parts.DummyNPCs.Add(obj.GetComponent<MSB1DummyNPCPart>().Serialize(obj));
                }
            }

            var partDummyObjects = GetChild(Parts, "DummyObjects");
            if (partDummyObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1DummyObjectPart>(partDummyObjects))
                {
                    export.Parts.DummyObjects.Add(obj.GetComponent<MSB1DummyObjectPart>().Serialize(obj));
                }
            }

            var partEnemies = GetChild(Parts, "NPCs");
            if (partEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1NPCPart>(partEnemies))
                {
                    export.Parts.NPCs.Add(obj.GetComponent<MSB1NPCPart>().Serialize(obj));
                }
            }

            var partObjects = GetChild(Parts, "Objects");
            if (partObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1ObjectPart>(partObjects))
                {
                    export.Parts.Objects.Add(obj.GetComponent<MSB1ObjectPart>().Serialize(obj));
                }
            }

            var partPlayers = GetChild(Parts, "Players");
            if (partPlayers != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1PlayerPart>(partPlayers))
                {
                    export.Parts.Players.Add(obj.GetComponent<MSB1PlayerPart>().Serialize(obj));
                }
            }

            var partNavimeshes = GetChild(Parts, "Navimeshes");
            if (partNavimeshes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1NavimeshPart>(partNavimeshes))
                {
                    export.Parts.Navimeshes.Add(obj.GetComponent<MSB1NavimeshPart>().Serialize(obj));
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
            foreach (var obj in GetChildrenOfType<MSB1Region>(Regions))
            {
                if (obj.GetComponent<SphereCollider>() != null && obj.GetComponent<MSB1Region>().IsPoint)
                {
                    export.Regions.Points.Add((MsbRegionPoint)obj.GetComponent<MSB1Region>().Serialize(new MsbRegionPoint(export.Regions), obj));
                }
                else if (obj.GetComponent<BoxCollider>() != null)
                {
                    export.Regions.Boxes.Add((MsbRegionBox)obj.GetComponent<MSB1Region>().Serialize(new MsbRegionBox(export.Regions), obj));
                }
                else if (obj.GetComponent<SphereCollider>() != null)
                {
                    export.Regions.Spheres.Add((MsbRegionSphere)obj.GetComponent<MSB1Region>().Serialize(new MsbRegionSphere(export.Regions), obj));
                }
                else if (obj.GetComponent<CapsuleCollider>() != null)
                {
                    export.Regions.Cylinders.Add((MsbRegionCylinder)obj.GetComponent<MSB1Region>().Serialize(new MsbRegionCylinder(export.Regions), obj));
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
                foreach (var obj in GetChildrenOfType<MSB1TreasureEvent>(eventTreasures))
                {
                    export.Events.Treasures.Add(obj.GetComponent<MSB1TreasureEvent>().Serialize(obj));
                }
            }

            var eventEnvironments = GetChild(Events, "Environments");
            if (eventEnvironments != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1EnvironmentEvent>(eventEnvironments))
                {
                    export.Events.EnvLightMapSpot.Add(obj.GetComponent<MSB1EnvironmentEvent>().Serialize(obj));
                }
            }

            var eventGenerators = GetChild(Events, "Generators");
            if (eventGenerators != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1GeneratorEvent>(eventGenerators))
                {
                    export.Events.Generators.Add(obj.GetComponent<MSB1GeneratorEvent>().Serialize(obj));
                }
            }

            var eventObjActs = GetChild(Events, "ObjActs");
            if (eventObjActs != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1ObjActEvent>(eventObjActs))
                {
                    export.Events.ObjActs.Add(obj.GetComponent<MSB1ObjActEvent>().Serialize(obj));
                }
            }

            var eventMapOffsets = GetChild(Events, "MapOffsets");
            if (eventMapOffsets != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1MapOffsetEvent>(eventMapOffsets))
                {
                    export.Events.MapOffsets.Add(obj.GetComponent<MSB1MapOffsetEvent>().Serialize(obj));
                }
            }

            var eventInvasions = GetChild(Events, "Invasions");
            if (eventInvasions != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1InvasionEvent>(eventInvasions))
                {
                    export.Events.NpcWorldInvitations.Add(obj.GetComponent<MSB1InvasionEvent>().Serialize(obj));
                }
            }

            var eventLights = GetChild(Events, "Lights");
            if (eventLights != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1LightEvent>(eventLights))
                {
                    export.Events.Lights.Add(obj.GetComponent<MSB1LightEvent>().Serialize(obj));
                }
            }

            var eventMessages = GetChild(Events, "Messages");
            if (eventMessages != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1MessageEvent>(eventMessages))
                {
                    export.Events.BloodMessages.Add(obj.GetComponent<MSB1MessageEvent>().Serialize(obj));
                }
            }

            var eventNavimeshes = GetChild(Events, "Navimeshes");
            if (eventNavimeshes != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1NavimeshEvent>(eventNavimeshes))
                {
                    export.Events.Navimeshes.Add(obj.GetComponent<MSB1NavimeshEvent>().Serialize(obj));
                }
            }

            var eventSFX = GetChild(Events, "SFX");
            if (eventSFX != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1SFXEvent>(eventSFX))
                {
                    export.Events.SFXs.Add(obj.GetComponent<MSB1SFXEvent>().Serialize(obj));
                }
            }

            var eventSounds = GetChild(Events, "Sounds");
            if (eventSounds != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1SoundEvent>(eventSounds))
                {
                    export.Events.Sounds.Add(obj.GetComponent<MSB1SoundEvent>().Serialize(obj));
                }
            }

            var eventSpawnPoints = GetChild(Events, "SpawnPoints");
            if (eventSpawnPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1SpawnPointEvent>(eventSpawnPoints))
                {
                    export.Events.SpawnPoints.Add(obj.GetComponent<MSB1SpawnPointEvent>().Serialize(obj));
                }
            }

            var eventWinds = GetChild(Events, "Winds");
            if (eventWinds != null)
            {
                foreach (var obj in GetChildrenOfType<MSB1WindEvent>(eventWinds))
                {
                    export.Events.WindSFXs.Add(obj.GetComponent<MSB1WindEvent>().Serialize(obj));
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

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        if (File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp"))
        {
            File.Delete(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp");
        }
        //export.Write(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp", SoulsFormats.DCX.Type.DarkSouls3);
        export.IsDcxCompressed = false;
        DataFile.SaveToFile<MSB>(export, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp");

        // Make a copy of the previous map
        File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".prev", true);

        // Move temp file as new map file
        File.Delete(AssetLink.GetComponent<MSBAssetLink>().MapPath);
        File.Move(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp", AssetLink.GetComponent<MSBAssetLink>().MapPath);
    }

    void ExportMapDS3()
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
                    export.Events.PseudoMultiplayers.Add(obj.GetComponent<MSB3InvasionEvent>().Serialize(obj));
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

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        if (File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp"))
        {
            File.Delete(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp");
        }
        export.Write(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp", SoulsFormats.DCX.Type.DarkSouls3);

        // Make a copy of the previous map
        File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".prev", true);

        // Move temp file as new map file
        File.Delete(AssetLink.GetComponent<MSBAssetLink>().MapPath);
        File.Move(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".temp", AssetLink.GetComponent<MSBAssetLink>().MapPath);
    }

    // Serializes the open unity map to an MSB file. Requires an MSBAssetLink object in the scene
    void ExportMap()
    {
        if (type == GameType.DarkSoulsPTDE)
        {
            ExportMapDS1();
        }
        else if (type == GameType.Bloodborne)
        {

        }
        else if (type == GameType.DarkSoulsIII)
        {
            ExportMapDS3();
        }
    }

    void OnGUI()
    { 
        GUILayout.Label("Import Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Set Game Root Directory"))
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
                FMGUtils.LoadFmgs(GameType.DarkSoulsIII, $@"{Interroot}\msg\engus\item_dlc2.msgbnd.dcx");
                ItemLotParamUtils.LoadParams(GameType.DarkSoulsIII, $@"{Interroot}\data0.bdt");
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

        GUILayout.Label("Development Test Tools (don't use)", EditorStyles.boldLabel);
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


        GUILayout.Label("Game Asset Importers", EditorStyles.boldLabel);
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
                ImportTpfbhd(file, type, (type == GameType.Bloodborne));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed", e.Message, "Ok");
            }
        }

        if (type == GameType.DarkSoulsPTDE)
        {
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

        GUILayout.Label("Map tools", EditorStyles.boldLabel);

        LoadMapFlvers = GUILayout.Toggle(LoadMapFlvers, "Load map piece models (slow)");
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
                EditorUtility.DisplayDialog("Map Export failed: " + e.Message, e.StackTrace, "Ok");
            }
        }
    }
}