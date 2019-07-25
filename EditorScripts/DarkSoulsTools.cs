using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering.HDPipeline;
using SoulsFormats;

public class DarkSoulsTools : EditorWindow
{
    bool LoadMapFlvers = false;
    bool LoadHighResCol = false;

    bool PreservePartsPose = true;

    string ChaliceID = "Chalice ID...";

    public enum GameType
    {
        Undefined,
        DarkSoulsPTDE,
        DarkSoulsRemastered,
        DarkSoulsIISOTFS,
        DarkSoulsIII,
        Bloodborne,
        Sekiro,
    }

    static GameType type = GameType.Undefined;
    public static GameType GetGameType()
    {
        return type;
    }

    static public string GameFolder(GameType game)
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
        else if (game == GameType.DarkSoulsIISOTFS)
        {
            if (!AssetDatabase.IsValidFolder("Assets/DS2SOTFS"))
            {
                AssetDatabase.CreateFolder("Assets", "DS2SOTFS");
            }
            return "DS2SOTFS";
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
        else if (game == GameType.Sekiro)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Sekiro"))
            {
                AssetDatabase.CreateFolder("Assets", "Sekiro");
            }
            return "Sekiro";
        }
        throw new Exception("No directory for game type");
    }

    static void ImportCollisionHKXBDT(string path, string outputAssetPath, GameType game)
    {
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(GetOverridenPath(pathBase + ".hkxbhd"), GetOverridenPath(pathBase + ".hkxbdt"));
        foreach (var file in bxf.Files)
        {
            try
            {
                var hkx = HKX.Read(file.Bytes, (game == GameType.Bloodborne) ? HKX.HKXVariation.HKXBloodBorne : HKX.HKXVariation.HKXDS3);
                if (game == GameType.DarkSoulsIISOTFS)
                {
                    CollisionUtilities.ImportDS1CollisionHKX(hkx, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)));
                }
                else
                {
                    CollisionUtilities.ImportDS3CollisionHKX(hkx, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load hkx file " + file.Name);
            }
        }
    }

    static void ImportNavmeshHKXBND(string path, string outputAssetPath, GameType game)
    {
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
        var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
        IBinder bnd = BND4.Read(GetOverridenPath(pathBase + ".nvmhktbnd.dcx"));
        foreach (var file in bnd.Files)
        {
            try
            {
                var hkx = HKX.Read(file.Bytes, (game == GameType.Bloodborne) ? HKX.HKXVariation.HKXBloodBorne : HKX.HKXVariation.HKXDS3);
                NavMeshUtilities.ImportNavimeshHKX(hkx, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)));
            }
            catch (Exception e)
            {
                //Debug.LogError("Failed to load hkx file " + file.Name);
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            } 
        }
    }

    static void ImportMapBDT(string path, string outputAssetPath, string mapname)
    {
        AssetDatabase.StartAssetEditing();
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(GetOverridenPath(pathBase + ".mapbhd"), GetOverridenPath(pathBase + ".mapbdt"));
        foreach (var file in bxf.Files)
        {
            try
            {
                var flver = FLVER.Read(file.Bytes);
                FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
                link.Type = FLVERAssetLink.ContainerType.Mapbdt;
                link.ArchivePath = pathBase;
                link.FlverPath = file.Name;
                FlverUtilities.ImportFlver(flver, link, type, outputAssetPath + "/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.Name)), $@"Assets/DS2SOTFS/{mapname}", true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load hkx file " + file.Name);
            }
        }
        AssetDatabase.StopAssetEditing();
    }

    static void ImportObjTextures(string objpath, string objid, GameType type)
    {
        string gameFolder = GameFolder(type);

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

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            objbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            objbnd = BND3.Read(GetOverridenPath(path));
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
                CreateTextureFromTPF($@"{gameFolder}/Obj/{objid}/{tex.Name}.dds", tex);
            }
        }
    }

    static void ImportObj(string objpath, string objid, GameType type)
    {
        string gameFolder = GameFolder(type);

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

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            objbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            objbnd = BND3.Read(GetOverridenPath(path));
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Objbnd;
        link.ArchivePath = objpath;
        link.FlverPath = objbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, type, $@"Assets/{gameFolder}/Obj/{objid}", $@"Assets/{gameFolder}/Obj/{objid}");
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
        try
        {
            string gameFolder = GameFolder(type);
            if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Obj"))
            {
                AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Obj");
            }
            AssetDatabase.StartAssetEditing();
            foreach (var obj in objFiles)
            {
                try
                {
                    ImportObjTextures(objpath, obj, type);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading obj {obj} textures: {e.Message}");
                }
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.StartAssetEditing();
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

    static void ImportChrTexbnd(string chrpath, string chrid, GameType type)
    {
        string gameFolder = GameFolder(type);

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Chr/{chrid}"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Chr", chrid);
        }

        // Load a separate texbnd if needed
        IBinder texbnd;
        string tpath = null;
        if (File.Exists($@"{chrpath}\{chrid}.texbnd.dcx"))
        {
            tpath = $@"{chrpath}\{chrid}.texbnd.dcx";
        }
        else if (File.Exists($@"{chrpath}\{chrid}.texbnd"))
        {
            tpath = $@"{chrpath}\{chrid}.texbnd";
        }

        if (tpath != null)
        {
            texbnd = BND4.Read(GetOverridenPath(tpath));

            var tentries = texbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".TPF"));
            foreach (var entry in tentries)
            {
                TPF tpf = TPF.Read(entry.Bytes);
                if (type == GameType.Bloodborne)
                {
                    tpf.ConvertPS4ToPC();
                }
                foreach (var tex in tpf.Textures)
                {
                    CreateTextureFromTPF($@"{gameFolder}/Chr/{chrid}/{tex.Name}.dds", tex);
                }
            }
        }
    }

    static void ImportChrTextures(string chrpath, string chrid, GameType type)
    {
        string gameFolder = GameFolder(type);

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

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            chrbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            chrbnd = BND3.Read(GetOverridenPath(path));
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
                CreateTextureFromTPF($@"{gameFolder}/Chr/{chrid}/{tex.Name}.dds", tex);
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
                        CreateTextureFromTPF($@"{gameFolder}/Chr/sharedTextures/{tex.Name}.dds", tex);
                    }
                }
            }
        }
    }

    static void ImportChr(string chrpath, string chrid, GameType type)
    {
        string gameFolder = GameFolder(type);

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

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            chrbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            chrbnd = BND3.Read(GetOverridenPath(path));
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(chrbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Chrbnd;
        link.ArchivePath = chrpath;
        link.FlverPath = chrbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, type, $@"Assets/{gameFolder}/Chr/{chrid}", $@"Assets/{gameFolder}/Chr/{chrid}");
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
        var texFiles = Directory.GetFiles(chrpath, @"*.texbnd.dcx")
            .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
        if (texFiles.Count() == 0)
        {
            texFiles = Directory.GetFiles(chrpath, @"*.texbnd")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
        }
        try
        {
            // Import all textures before the models because they can reference each other
            string gameFolder = GameFolder(type);
            if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Chr"))
            {
                AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Chr");
            }
            AssetDatabase.StartAssetEditing();
            foreach (var chr in texFiles)
            {
                try
                {
                    ImportChrTexbnd(chrpath, chr, type);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading chr {chr} textures: {e.Message}, {e.StackTrace}");
                }
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.StartAssetEditing();
            foreach (var chr in chrFiles)
            {
                try
                {
                    ImportChrTextures(chrpath, chr, type);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading chr {chr} textures: {e.Message}, {e.StackTrace}");
                }
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.StartAssetEditing();
            foreach (var chr in chrFiles)
            {
                try
                {
                    ImportChr(chrpath, chr, type);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading chr {chr}: {e.Message}, {e.StackTrace}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    static void ImportPartTextures(string partpath, string partname, GameType type)
    {
        string gameFolder = GameFolder(type);
        /*if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Parts");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts/textures"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Parts", "textures");
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts/{partname}"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Parts", partname);
        }*/

        IBinder partsbnd;
        string path = "";
        if (File.Exists($@"{partpath}\{partname}.partsbnd.dcx"))
        {
            path = $@"{partpath}\{partname}.partsbnd.dcx";
        }
        else if (File.Exists($@"{partpath}\{partname}.partsbnd"))
        {
            path = $@"{partpath}\{partname}.partsbnd";
        }
        else
        {
            throw new FileNotFoundException("Could not find bnd for part " + partname);
        }

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            partsbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            partsbnd = BND3.Read(GetOverridenPath(path));
        }
        var texentries = partsbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".TPF"));
        foreach (var entry in texentries)
        {
            TPF tpf = TPF.Read(entry.Bytes);
            if (type == GameType.Bloodborne)
            {
                tpf.ConvertPS4ToPC();
            }
            foreach (var tex in tpf.Textures)
            {
                CreateTextureFromTPF($@"{gameFolder}/Parts/textures/{tex.Name}.dds", tex);
            }
        }
    }

    static void ImportPart(string partpath, string partname, GameType type)
    {
        string gameFolder = GameFolder(type);

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts/{partname}"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Parts", partname);
        }

        IBinder partsbnd;
        string path = "";
        if (File.Exists($@"{partpath}\{partname}.partsbnd.dcx"))
        {
            path = $@"{partpath}\{partname}.partsbnd.dcx";
        }
        else if (File.Exists($@"{partpath}\{partname}.partsbnd"))
        {
            path = $@"{partpath}\{partname}.partsbnd";
        }
        else
        {
            throw new FileNotFoundException("Could not find bnd for part " + partname);
        }

        if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.Sekiro)
        {
            partsbnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            partsbnd = BND3.Read(GetOverridenPath(path));
        }

        // Should only be one flver in a bnd
        var flver = FLVER.Read(partsbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Bytes);
        FLVERAssetLink link = ScriptableObject.CreateInstance<FLVERAssetLink>();
        link.Type = FLVERAssetLink.ContainerType.Partsbnd;
        link.ArchivePath = partpath;
        link.FlverPath = partsbnd.Files.Where(x => x.Name.ToUpper().EndsWith(".FLVER")).First().Name;
        FlverUtilities.ImportFlver(flver, link, type, $@"Assets/{gameFolder}/Parts/{partname}", $@"Assets/{gameFolder}/Parts/{partname}");
    }

    static void ImportParts(string partspath, GameType type)
    {
        string gameFolder = GameFolder(type);

        var partsFiles = Directory.GetFiles(partspath, @"*.partsbnd.dcx")
                    .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
        if (partsFiles.Count() == 0)
        {
            partsFiles = Directory.GetFiles(partspath, @"*.partsbnd")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
        }
        try
        {
            // Try and import common parts textures
            if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts"))
            {
                AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "Parts");
            }
            if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/Parts/textures"))
            {
                AssetDatabase.CreateFolder($@"Assets/{gameFolder}/Parts", "textures");
            }

            // Import all textures before the models because they can reference each other
            AssetDatabase.StartAssetEditing();

            if (File.Exists($@"{partspath}\common_body.tpf.dcx"))
            {
                TPF tpf = TPF.Read($@"{partspath}\common_body.tpf.dcx");
                if (type == GameType.Bloodborne)
                {
                    tpf.ConvertPS4ToPC();
                }
                foreach (var tex in tpf.Textures)
                {
                    CreateTextureFromTPF($@"{gameFolder}/Parts/textures/{tex.Name}.dds", tex);
                }
            }
            foreach (var part in partsFiles)
            {
                try
                {
                    ImportPartTextures(partspath, part, type);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading part {part}: {e.Message}");
                }
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.StartAssetEditing();
            foreach (var part in partsFiles)
            {
                try
                {
                    ImportPart(partspath, part, type);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading part {part}: {e.Message}");
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

    // Serialize a TPF file as a DDS and import it into Unity
    static void CreateTextureFromTPF(string texPath, TPF.Texture tpf)
    {
        var textureBytes = tpf.Bytes;
        var assetsPath = Application.dataPath.Replace('/', '\\');
        var adjTexPath = texPath.Replace('/', '\\');
        if (!File.Exists($@"{assetsPath}\{adjTexPath}"))
        {
            using (FileStream stream = File.Create($@"{assetsPath}\{adjTexPath}"))
            {
                BinaryWriterEx bw = new BinaryWriterEx(false, stream);
                bw.WriteBytes(textureBytes);
                bw.Finish();
            }
            AssetDatabase.ImportAsset($@"Assets/{texPath}", ImportAssetOptions.ForceUncompressedImport);
        }
    }

    static void ImportTpfbhd(string path, GameType type, bool isPS4 = false)
    {
        string gameFolder = GameFolder(type);
        var pathBase = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);
        BXF4 bxf = BXF4.Read(pathBase + ".tpfbhd", pathBase + ".tpfbdt");

        var mapname = name.Substring(0, 3);
        if (type == GameType.DarkSoulsIISOTFS)
        {
            // DS2 has textures per map
            mapname = $@"m{name.Substring(1)}";
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/" + mapname))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", mapname);
        }

        AssetDatabase.StartAssetEditing();
        foreach (var tpf in bxf.Files)
        {
            try
            {
                var t = TPF.Read(tpf.Bytes);
                if (isPS4)
                {
                    t.ConvertPS4ToPC();
                }
                CreateTextureFromTPF($@"{gameFolder}/" + mapname + "/" + Path.GetFileNameWithoutExtension((Path.GetFileNameWithoutExtension(tpf.Name))) + ".dds", t.Textures[0]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading {tpf.Name}: {ex.Message}");
            }
        }
        AssetDatabase.StopAssetEditing();
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
                CreateTextureFromTPF($@"DS1/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(file)}.dds", tpf.Textures[0]);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading {file}: {ex.Message}");
            }
        }
        AssetDatabase.StopAssetEditing();
    }

    public static string Interroot = "";
    public static string ModProjectDirectory = null;
    private List<string> Maps = new List<string>();

    /// <summary>
    /// Translates an interroot file path into a mod project path
    /// </summary>
    /// <param name="FilePath">File path</param>
    /// <returns></returns>
    public static string GetModProjectPathForFile(string FilePath)
    {
        if (ModProjectDirectory == null || Interroot == null)
        {
            return null;
        }

        return ModProjectDirectory + FilePath.Substring(Interroot.Length);
    }

    /// <summary>
    /// Searches for an overriden mod project file and returns the path to that
    /// if it exists. Otherwise returns the current path.
    /// </summary>
    /// <param name="path">The path to search for an override</param>
    /// <returns></returns>
    public static string GetOverridenPath(string path)
    {
        string newPath = GetModProjectPathForFile(path);
        if (newPath != null)
        {
            if (File.Exists(newPath))
            {
                return GetModProjectPathForFile(path);
            }
        }
        return path;
    }

    private void UpdateMapList()
    {
        Maps = new List<string>();

        // DS2 has its own structure for msbs, where they are all inside individual folders
        if (type == GameType.DarkSoulsIISOTFS)
        {
            var maps = Directory.GetFileSystemEntries(Interroot + @"\map", @"m*");
            foreach (var map in maps)
            {
                Maps.Add(Path.GetFileNameWithoutExtension($@"{map}.blah"));
            }
            return;
        }

        var msbFiles = Directory.GetFileSystemEntries(Interroot + @"\map\MapStudio\", @"*.msb")
            .Select(Path.GetFileNameWithoutExtension);
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

    static GameObject InstantiateRegion(MSB3.Region region, GameObject parent)
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

    static GameObject InstantiateRegion(MSBS.Region region, string type, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.Position.X, region.Position.Y, region.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z), obj.transform);
        if (region.Shape is MSBS.Shape.Box)
        {
            var shape = (MSBS.Shape.Box)region.Shape;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, shape.Height, shape.Depth);
        }
        else if (region.Shape is MSBS.Shape.Sphere)
        {
            var shape = (MSBS.Shape.Sphere)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region.Shape is MSBS.Shape.Point)
        {
            var shape = (MSBS.Shape.Point)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region.Shape is MSBS.Shape.Cylinder)
        {
            var shape = (MSBS.Shape.Cylinder)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        if (region.Shape is MSBS.Shape.Rect)
        {
            var shape = (MSBS.Shape.Rect)region.Shape;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, 1, shape.Depth);
        }
        else if (region.Shape is MSBS.Shape.Composite)
        {
            var shape = (MSBS.Shape.Composite)region.Shape;
            obj.AddComponent<MSBSCompositeShape>();
            var col = obj.GetComponent<MSBSCompositeShape>();
            col.setShape(shape);
        }
        obj.layer = 13;
        obj.transform.parent = parent.transform;
        return obj;
    }

    static GameObject InstantiateRegion(MSBBB.Region region, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.Position.X, region.Position.Y, region.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z), obj.transform);
        if (region.Shape.Type == MSBBB.ShapeType.Box)
        {
            var shape = (MSBBB.Shape.Box)region.Shape;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, shape.Height, shape.Depth);
        }
        else if (region.Shape.Type == MSBBB.ShapeType.Sphere)
        {
            var shape = (MSBBB.Shape.Sphere)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region.Shape.Type == MSBBB.ShapeType.Point)
        {
            var shape = (MSBBB.Shape.Point)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region.Shape.Type == MSBBB.ShapeType.Cylinder)
        {
            var shape = (MSBBB.Shape.Cylinder)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        else if (region.Shape.Type == MSBBB.ShapeType.Circle)
        {
            var shape = (MSBBB.Shape.Circle)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Radius;
        }
        else
        {
            Debug.Log("Unsupported region type encountered");
        }
        obj.layer = 13;
        obj.transform.parent = parent.transform;
        return obj;
    }

    static GameObject InstantiateRegion(MSB1.Region region, GameObject parent)
    {
        GameObject obj = new GameObject(region.Name);
        obj.transform.position = new Vector3(region.Position.X, region.Position.Y, region.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(region.Rotation.X, region.Rotation.Y, region.Rotation.Z);
        EulerToTransform(new Vector3(region.Rotation.X, region.Rotation.Y, region.Rotation.Z), obj.transform);
        if (region.Shape.Type == MSB1.ShapeType.Box)
        {
            var shape = (MSB1.Shape.Box)region.Shape;
            obj.AddComponent<BoxCollider>();
            var col = obj.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(shape.Width, shape.Height, shape.Depth);
        }
        else if (region.Shape.Type == MSB1.ShapeType.Sphere)
        {
            var shape = (MSB1.Shape.Sphere)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
        }
        else if (region.Shape.Type == MSB1.ShapeType.Point)
        {
            var shape = (MSB1.Shape.Point)region.Shape;
            obj.AddComponent<SphereCollider>();
            var col = obj.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.0f;
        }
        else if (region.Shape.Type == MSB1.ShapeType.Cylinder)
        {
            var shape = (MSB1.Shape.Cylinder)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Height;
        }
        else if (region.Shape.Type == MSB1.ShapeType.Circle)
        {
            var shape = (MSB1.Shape.Circle)region.Shape;
            obj.AddComponent<CapsuleCollider>();
            var col = obj.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = shape.Radius;
            col.height = shape.Radius;
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

        // Apply in YZX order
        t.Rotate(new Vector3(0, 1, 0), e.y, Space.World);
        t.Rotate(new Vector3(0, 0, 1), e.z, Space.World);
        t.Rotate(new Vector3(1, 0, 0), e.x, Space.World);
    }

    static void EulerToTransformBTL(Vector3 e, Transform t)
    {
        // Apply in YZX order
        t.Rotate(new Vector3(1, 0, 0), e.x, Space.World);
        t.Rotate(new Vector3(0, 1, 0), e.y, Space.World);
        t.Rotate(new Vector3(0, 0, 1), e.z, Space.World);

    }

    public static void InstantiateDS3Model<T, U>(string mdlName, GameObject modelsRoot, List<U> models) where T : MSB3Model where U : MSB3.Model
    {
        GameObject obj = new GameObject(mdlName);
        obj.transform.parent = modelsRoot.transform;
        foreach (var md in models)
        {
            GameObject mdl = new GameObject(md.Name);
            mdl.AddComponent<T>();
            mdl.GetComponent<T>().SetModel(md);
            mdl.transform.parent = obj.transform;
        }
    }

    public static void InitializeDS3Part<T>(GameObject obj, MSB3.Part part, int layer, GameObject parent) where T : MSB3Part
    {
        obj.AddComponent<T>();
        obj.GetComponent<T>().SetPart(part);
        obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
        EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
        obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
        obj.layer = layer;
        obj.transform.parent = parent.transform;
    }

    public static void InstantiateDS3Region<T, U>(string regionName, GameObject regionsRoot, List<U> regions) where T : MSB3Region where U : MSB3.Region
    {
        GameObject obj = new GameObject(regionName);
        obj.transform.parent = regionsRoot.transform;
        foreach (var region in regions)
        {
            var reg = InstantiateRegion(region, obj);
            reg.AddComponent<T>();
            reg.GetComponent<T>().SetRegion(region);
        }
    }

    public static void InstantiateDS3Event<T, U>(string evtName, GameObject eventsRoot, List<U> events) where T : MSB3Event where U : MSB3.Event
    {
        GameObject obj = new GameObject(evtName);
        obj.transform.parent = eventsRoot.transform;
        foreach (var ev in events)
        {
            GameObject evt = new GameObject(ev.Name);
            evt.AddComponent<T>();
            evt.GetComponent<T>().SetEvent(ev);
            evt.transform.parent = obj.transform;
        }
    }

    void onImportDS3Map(object o)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSB3.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx"));
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
                            FlverUtilities.ImportFlver(type, Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/DS3/{mapname}/{assetname}", $@"Assets/DS3/{mapname.Substring(0, 3)}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
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

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");
            InstantiateDS3Model<MSB3MapPieceModel, MSB3.Model.MapPiece>("MapPieces", ModelsSection, msb.Models.MapPieces);
            InstantiateDS3Model<MSB3ObjectModel, MSB3.Model.Object>("Objects", ModelsSection, msb.Models.Objects);
            InstantiateDS3Model<MSB3PlayerModel, MSB3.Model.Player>("Players", ModelsSection, msb.Models.Players);
            InstantiateDS3Model<MSB3EnemyModel, MSB3.Model.Enemy>("Enemies", ModelsSection, msb.Models.Enemies);
            InstantiateDS3Model<MSB3CollisionModel, MSB3.Model.Collision>("Collisions", ModelsSection, msb.Models.Collisions);
            InstantiateDS3Model<MSB3OtherModel, MSB3.Model.Other>("Others", ModelsSection, msb.Models.Others);

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/{mapname}/{part.ModelName}.prefab") : null;
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS3Part<MSB3MapPiecePart>(obj, part, 9, MapPieces);
            }

            GameObject Objects = new GameObject("Objects");
            Objects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Objects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS3Part<MSB3ObjectPart>(obj, part, 10, Objects);
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Collisions)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/{mapname}/{lowHigh}{mapname.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS3Part<MSB3CollisionPart>(obj, part, 12, Collisions);
            }

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectCollisions)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS3Part<MSB3ConnectCollisionPart>(obj, part, 12, ConnectCollisions);
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS3/Chr/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS3Part<MSB3EnemyPart>(obj, part, 11, Enemies);
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS3Part<MSB3PlayerPart>(obj, part, 11, Players);
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS3Part<MSB3DummyObjectPart>(obj, part, 10, DummyObjects);
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS3Part<MSB3DummyEnemyPart>(obj, part, 11, DummyEnemies);
            }

            //
            // Regions section
            //
            GameObject RegionsSection = new GameObject("MSBRegions");
            InstantiateDS3Region<MSB3ActivationAreaRegion, MSB3.Region.ActivationArea>("ActivationAreas", RegionsSection, msb.Regions.ActivationAreas);
            InstantiateDS3Region<MSB3EnvironmentEffectBoxRegion, MSB3.Region.EnvironmentMapEffectBox>("EnvMapEffectBoxes", RegionsSection, msb.Regions.EnvironmentMapEffectBoxes);
            InstantiateDS3Region<MSB3EnvironmentMapPointRegion, MSB3.Region.EnvironmentMapPoint>("EnvMapPoints", RegionsSection, msb.Regions.EnvironmentMapPoints);
            InstantiateDS3Region<MSB3EventRegion, MSB3.Region.Event>("Events", RegionsSection, msb.Regions.Events);
            InstantiateDS3Region<MSB3GeneralRegion, MSB3.Region.General>("GeneralRegions", RegionsSection, msb.Regions.General);
            InstantiateDS3Region<MSB3InvasionPointRegion, MSB3.Region.InvasionPoint>("InvasionPoints", RegionsSection, msb.Regions.InvasionPoints);
            InstantiateDS3Region<MSB3MessageRegion, MSB3.Region.Message>("Messages", RegionsSection, msb.Regions.Messages);
            InstantiateDS3Region<MSB3MufflingBoxRegion, MSB3.Region.MufflingBox>("MufflingBox", RegionsSection, msb.Regions.MufflingBoxes);
            InstantiateDS3Region<MSB3MufflingPortal, MSB3.Region.MufflingPortal>("MufflingPortals", RegionsSection, msb.Regions.MufflingPortals);
            InstantiateDS3Region<MSB3SFXRegion, MSB3.Region.SFX>("SFX", RegionsSection, msb.Regions.SFX);
            InstantiateDS3Region<MSB3SoundRegion, MSB3.Region.Sound>("Sounds", RegionsSection, msb.Regions.Sounds);
            InstantiateDS3Region<MSB3SpawnPointRegion, MSB3.Region.SpawnPoint>("SpawnPoints", RegionsSection, msb.Regions.SpawnPoints);
            InstantiateDS3Region<MSB3WalkRouteRegion, MSB3.Region.WalkRoute>("WalkRoutes", RegionsSection, msb.Regions.WalkRoutes);
            InstantiateDS3Region<MSB3WarpPointRegion, MSB3.Region.WarpPoint>("WarpPoints", RegionsSection, msb.Regions.WarpPoints);
            InstantiateDS3Region<MSB3WindAreaRegion, MSB3.Region.WindArea>("WindAreas", RegionsSection, msb.Regions.WindAreas);
            InstantiateDS3Region<MSB3WindSFXRegion, MSB3.Region.WindSFX>("WindSFXRegions", RegionsSection, msb.Regions.WindSFX);
            InstantiateDS3Region<MSB3Unk00Region, MSB3.Region.Unk00>("Unk00Regions", RegionsSection, msb.Regions.Unk00s);
            InstantiateDS3Region<MSB3Unk12Region, MSB3.Region.Unk12>("Unk12Regions", RegionsSection, msb.Regions.Unk12s);

            //
            // Events Section
            //
            GameObject Events = new GameObject("MSBEvents");

            InstantiateDS3Event<MSB3TreasureEvent, MSB3.Event.Treasure>("Treasures", Events, msb.Events.Treasures);
            InstantiateDS3Event<MSB3GeneratorEvent, MSB3.Event.Generator>("Generators", Events, msb.Events.Generators);
            InstantiateDS3Event<MSB3ObjActEvent, MSB3.Event.ObjAct>("ObjActs", Events, msb.Events.ObjActs);
            InstantiateDS3Event<MSB3MapOffsetEvent, MSB3.Event.MapOffset>("MapOffsets", Events, msb.Events.MapOffsets);
            InstantiateDS3Event<MSB3InvasionEvent, MSB3.Event.PseudoMultiplayer>("Invasions", Events, msb.Events.PseudoMultiplayers);
            InstantiateDS3Event<MSB3WalkRouteEvent, MSB3.Event.WalkRoute>("WalkRoutes", Events, msb.Events.WalkRoutes);
            InstantiateDS3Event<MSB3GroupTourEvent, MSB3.Event.GroupTour>("GroupTours", Events, msb.Events.GroupTours);
            InstantiateDS3Event<MSB3OtherEvent, MSB3.Event.Other>("Others", Events, msb.Events.Others);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import failed", e.Message + "\n" + e.StackTrace, "Ok");
        }
    }

    void onImportDS2Map(object o)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSB2.Read(GetOverridenPath(Interroot + $@"\map\{mapname}\{mapname}.msb"));
            string gameFolder = GameFolder(GameType.DarkSoulsIISOTFS);

            if (!AssetDatabase.IsValidFolder("Assets/DS2SOTFS/" + mapname))
            {
                AssetDatabase.CreateFolder("Assets/DS2SOTFS", mapname);
            }

            // Create an MSB asset link to the DS2 asset
            GameObject AssetLink = new GameObject("MSBAssetLink");
            AssetLink.AddComponent<MSBAssetLink>();
            AssetLink.GetComponent<MSBAssetLink>().Interroot = Interroot;
            AssetLink.GetComponent<MSBAssetLink>().MapID = mapname;
            AssetLink.GetComponent<MSBAssetLink>().MapPath = $@"{Interroot}\map\{mapname}\{mapname}.msb";

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");

            GameObject MapPieceModels = new GameObject("MapPieces");
            MapPieceModels.transform.parent = ModelsSection.transform;

            // Do a preload of all the flvers
            /*try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var mappiece in msb.Models.MapPieces)
                {
                    var assetname = mappiece.Name;
                    if (AssetDatabase.FindAssets($@"Assets/DS3/{mapname}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx"))
                            FlverUtilities.ImportFlver(type, Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/DS3/{mapname}/{assetname}", $@"Assets/DS3/{mapname.Substring(0, 3)}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }*/

            // Do a preload of all the flvers
            if (File.Exists($@"{Interroot}\model\map\{mapname}.mapbhd") && LoadMapFlvers)
            {
                ImportMapBDT($@"{Interroot}\model\map\{mapname}", $@"Assets/DS2SOTFS/{mapname}", mapname);
            }

            foreach (var mappiece in msb.Models.MapPieces)
            {
                var assetname = mappiece.Name;
                GameObject model = new GameObject(mappiece.Name);
                model.transform.parent = MapPieceModels.transform;
            }

            // Load low res hkx assets
            if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\model\map\h{mapname.Substring(1)}.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\model\map\h{mapname.Substring(1)}.hkxbhd", $@"Assets/DS2SOTFS/{mapname}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\model\map\l{mapname.Substring(1)}.hkxbhd"))
                {
                    AssetDatabase.StartAssetEditing();
                    ImportCollisionHKXBDT(Interroot + $@"\model\map\l{mapname.Substring(1)}.hkxbhd", $@"Assets/DS2SOTFS/{mapname}", type);
                    AssetDatabase.StopAssetEditing();
                }
            }

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = true ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS2SOTFS/{mapname}/{part.ModelName}.prefab") : null;
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
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
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 9;
                    obj.transform.parent = MapPieces.transform;
                }
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            // "Items" section because ds2 msb isn't known well
            foreach (var part in msb.Parts.Items)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS2SOTFS/{mapname}/{lowHigh}{part.ModelName.Substring(1)}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 12;
                    obj.transform.parent = Collisions.transform;
                }
                else
                {
                    GameObject obj = new GameObject(part.Name);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
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

    public static void InstantiateBBModel<T, U>(string mdlName, GameObject modelsRoot, List<U> models) where T : MSBBBModel where U : MSBBB.Model
    {
        GameObject obj = new GameObject(mdlName);
        obj.transform.parent = modelsRoot.transform;
        foreach (var md in models)
        {
            GameObject mdl = new GameObject(md.Name);
            mdl.AddComponent<T>();
            mdl.GetComponent<T>().SetModel(md);
            mdl.transform.parent = obj.transform;
        }
    }

    public static void InitializeBBPart<T>(GameObject obj, MSBBB.Part part, int layer, GameObject parent) where T : MSBBBPart
    {
        obj.AddComponent<T>();
        obj.GetComponent<T>().SetPart(part);
        obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
        EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
        obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
        obj.layer = layer;
        obj.transform.parent = parent.transform;
    }

    public static void InstantiateBBEvent<T, U>(string evtName, GameObject eventsRoot, List<U> events) where T : MSBBBEvent where U : MSBBB.Event
    {
        GameObject obj = new GameObject(evtName);
        obj.transform.parent = eventsRoot.transform;
        foreach (var ev in events)
        {
            GameObject evt = new GameObject(ev.Name);
            evt.AddComponent<T>();
            evt.GetComponent<T>().SetEvent(ev);
            evt.transform.parent = obj.transform;
        }
    }

    void onImportBBMap(object o, bool chalice=false)
    {
        try
        {
            string mapname = (string)o;
            MSBBB msb = null;
            var mapbase = mapname.Substring(0, 9) + "_00";
            if (chalice)
            {
                if (mapbase.StartsWith("m29"))
                {
                    msb = MSBBB.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapbase}\{mapname}.msb.dcx"));
                }
                else
                {
                    msb = MSBBB.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx"));
                }
            }
            else
            {
                msb = MSBBB.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx"));
            }
            GameFolder(GameType.Bloodborne);

            // Make adjusted mapname because bloodborne is a :fatcat:
            var mapnameAdj = mapname.Substring(0, 6) + "_00_00";
            if (chalice)
            {
                mapnameAdj = "m29_00_00_00";
            }

            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne/" + mapnameAdj))
            {
                AssetDatabase.CreateFolder("Assets/Bloodborne", mapnameAdj);
            }

            // Create an MSB asset link to the BB asset
            GameObject AssetLink = new GameObject("MSBAssetLink");
            AssetLink.AddComponent<MSBAssetLink>();
            AssetLink.GetComponent<MSBAssetLink>().Interroot = Interroot;
            AssetLink.GetComponent<MSBAssetLink>().MapID = mapname;
            if (chalice)
            {
                AssetLink.GetComponent<MSBAssetLink>().MapPath = $@"{Interroot}\map\MapStudio\{mapbase}\{mapname}.msb.dcx";
            }
            else
            {
                AssetLink.GetComponent<MSBAssetLink>().MapPath = $@"{Interroot}\map\MapStudio\{mapname}.msb.dcx";
            }

            // Do a preload of all the flvers
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var mappiece in msb.Models.MapPieces)
                {
                    var assetname = mappiece.Name;
                    if (AssetDatabase.FindAssets($@"Assets/Bloodborne/{mapnameAdj}/{assetname}.prefab").Length == 0 && LoadMapFlvers && !chalice)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx"))
                            FlverUtilities.ImportFlver(type, Interroot + $@"\map\{mapnameAdj}\{mapnameAdj}_{assetname.Substring(1)}.flver.dcx", $@"Assets/Bloodborne/{mapnameAdj}/{assetname}", $@"Assets/Bloodborne/{mapnameAdj.Substring(0, 3)}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            // Load low res hkx assets
            if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\map\{mapnameAdj}\h{mapnameAdj.Substring(1)}.hkxbhd") && !chalice)
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\h{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/Bloodborne/{mapnameAdj}", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd") && !chalice)
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\{mapnameAdj}\l{mapnameAdj.Substring(1)}.hkxbhd", $@"Assets/Bloodborne/{mapnameAdj}", type);
                }
            }

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");
            InstantiateBBModel<MSBBBMapPieceModel, MSBBB.Model.MapPiece>("MapPieces", ModelsSection, msb.Models.MapPieces);
            InstantiateBBModel<MSBBBObjectModel, MSBBB.Model.Object>("Objects", ModelsSection, msb.Models.Objects);
            InstantiateBBModel<MSBBBPlayerModel, MSBBB.Model.Player>("Players", ModelsSection, msb.Models.Players);
            InstantiateBBModel<MSBBBEnemyModel, MSBBB.Model.Enemy>("Enemies", ModelsSection, msb.Models.Enemies);
            InstantiateBBModel<MSBBBCollisionModel, MSBBB.Model.Collision>("Collisions", ModelsSection, msb.Models.Collisions);
            InstantiateBBModel<MSBBBOtherModel, MSBBB.Model.Other>("Others", ModelsSection, msb.Models.Others);

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = null;
                if (chalice)
                {
                    src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/m29_00_00_00/m29_00_00_00_{part.ModelName.Substring(1)}.prefab");
                }
                else
                {
                    src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/{mapnameAdj}/{part.ModelName}.prefab");
                }

                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBMapPiecePart>(obj, part, 9, MapPieces);
            }

            GameObject Objects = new GameObject("Objects");
            Objects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Objects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBObjectPart>(obj, part, 10, Objects);
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Collisions)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/{mapnameAdj}/{lowHigh}{mapnameAdj.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBCollisionPart>(obj, part, 12, Collisions);
            }

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectCollisions)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeBBPart<MSBBBConnectCollisionPart>(obj, part, 12, ConnectCollisions);
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Chr/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBEnemyPart>(obj, part, 11, Enemies);
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeBBPart<MSBBBPlayerPart>(obj, part, 11, Players);
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBDummyObjectPart>(obj, part, 10, DummyObjects);
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Bloodborne/Chr/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeBBPart<MSBBBDummyEnemyPart>(obj, part, 11, DummyEnemies);
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
            InstantiateBBEvent<MSBBBSoundEvent, MSBBB.Event.Sound>("Sounds", Events, msb.Events.Sounds);
            InstantiateBBEvent<MSBBBSFXEvent, MSBBB.Event.SFX>("SFX", Events, msb.Events.SFXs);
            InstantiateBBEvent<MSBBBTreasureEvent, MSBBB.Event.Treasure>("Treasures", Events, msb.Events.Treasures);
            InstantiateBBEvent<MSBBBGeneratorEvent, MSBBB.Event.Generator>("Generators", Events, msb.Events.Generators);
            InstantiateBBEvent<MSBBBMessageEvent, MSBBB.Event.Message>("Messages", Events, msb.Events.Messages);
            InstantiateBBEvent<MSBBBObjActEvent, MSBBB.Event.ObjAct>("ObjActs", Events, msb.Events.ObjActs);
            InstantiateBBEvent<MSBBBSpawnPointEvent, MSBBB.Event.SpawnPoint>("SpawnPoints", Events, msb.Events.SpawnPoints);
            InstantiateBBEvent<MSBBBMapOffsetEvent, MSBBB.Event.MapOffset>("MapOffsets", Events, msb.Events.MapOffsets);
            InstantiateBBEvent<MSBBBNavimeshEvent, MSBBB.Event.Navimesh>("Navimeshes", Events, msb.Events.Navimeshes);
            InstantiateBBEvent<MSBBBEnvironmentEvent, MSBBB.Event.Environment>("Environments", Events, msb.Events.Environments);
            InstantiateBBEvent<MSBBBWindEvent, MSBBB.Event.Wind>("Winds", Events, msb.Events.Mysteries);
            InstantiateBBEvent<MSBBBInvasionEvent, MSBBB.Event.Invasion>("Invasions", Events, msb.Events.Invasions);
            InstantiateBBEvent<MSBBBWalkRouteEvent, MSBBB.Event.WalkRoute>("WalkRoutes", Events, msb.Events.WalkRoutes);
            InstantiateBBEvent<MSBBBUnknownEvent, MSBBB.Event.Unknown>("Unknowns", Events, msb.Events.Unknowns);
            InstantiateBBEvent<MSBBBGroupTourEvent, MSBBB.Event.GroupTour>("GroupTours", Events, msb.Events.GroupTours);
            InstantiateBBEvent<MSBBBMultiSummoningPointEvent, MSBBB.Event.MultiSummoningPoint>("MultiSummoningPoints", Events, msb.Events.MultiSummoningPoints);
            InstantiateBBEvent<MSBBBOtherEvent, MSBBB.Event.Other>("Others", Events, msb.Events.Others);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import failed", e.Message + "\n" + e.StackTrace, "Ok");
        }
    }

    public static void InstantiateDS1Model<T, U>(string mdlName, GameObject modelsRoot, List<U> models) where T : MSB1Model where U : MSB1.Model
    {
        GameObject obj = new GameObject(mdlName);
        obj.transform.parent = modelsRoot.transform;
        foreach (var md in models)
        {
            GameObject mdl = new GameObject(md.Name);
            mdl.AddComponent<T>();
            mdl.GetComponent<T>().SetModel(md);
            mdl.transform.parent = obj.transform;
        }
    }

    public static void InitializeDS1Part<T>(GameObject obj, MSB1.Part part, int layer, GameObject parent) where T : MSB1Part
    {
        obj.AddComponent<T>();
        obj.GetComponent<T>().SetPart(part);
        obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
        //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
        EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
        obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
        obj.layer = layer;
        obj.transform.parent = parent.transform;
    }

    public static void InstantiateDS1Event<T, U>(string evtName, GameObject eventsRoot, List<U> events) where T : MSB1Event where U : MSB1.Event
    {
        GameObject obj = new GameObject(evtName);
        obj.transform.parent = eventsRoot.transform;
        foreach (var ev in events)
        {
            GameObject evt = new GameObject(ev.Name);
            evt.AddComponent<T>();
            evt.GetComponent<T>().SetEvent(ev);
            evt.transform.parent = obj.transform;
        }
    }

    void onImportDS1Map(object o, bool remastered)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSB1.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapname}.msb"));
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
                            FlverUtilities.ImportFlver(type, Interroot + $@"\map\{mapnameAdj}\{assetname}A{area:D2}.flver", $@"Assets/DS1/{mapnameAdj}/{assetname}", $@"Assets/DS1/UDSFMMapTextures");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            //
            // Models section
            //
            GameObject ModelsSection = new GameObject("MSBModelDeclarations");

            GameObject MapPieceModels = new GameObject("MapPieces");
            MapPieceModels.transform.parent = ModelsSection.transform;

            InstantiateDS1Model<MSB1MapPieceModel, MSB1.Model>("MapPieces", ModelsSection, msb.Models.MapPieces);
            InstantiateDS1Model<MSB1ObjectModel, MSB1.Model>("Objects", ModelsSection, msb.Models.Objects);
            InstantiateDS1Model<MSB1PlayerModel, MSB1.Model>("Players", ModelsSection, msb.Models.Players);
            InstantiateDS1Model<MSB1EnemyModel, MSB1.Model>("Enemies", ModelsSection, msb.Models.Enemies);
            InstantiateDS1Model<MSB1NavimeshModel, MSB1.Model>("Navimeshes", ModelsSection, msb.Models.Navmeshes);

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

            //
            // Parts Section
            //
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/{mapnameAdj}/{part.ModelName}.prefab") : null;
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1MapPiecePart>(obj, part, 9, MapPieces);
            }

            GameObject Objects = new GameObject("Objects");
            Objects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Objects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1ObjectPart>(obj, part, 10, Objects);
            }

            GameObject DummyObjects = new GameObject("DummyObjects");
            DummyObjects.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyObjects)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1DummyObjectPart>(obj, part, 10, DummyObjects);
            }

            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Enemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Chr/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1EnemyPart>(obj, part, 11, Enemies);
            }

            GameObject DummyEnemies = new GameObject("DummyEnemies");
            DummyEnemies.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.DummyEnemies)
            {
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/Obj/{part.ModelName}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1DummyEnemyPart>(obj, part, 11, DummyEnemies);
            }

            GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Collisions)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/DS1/{mapnameAdj}/{lowHigh}{part.ModelName.Substring(1)}.prefab");
                GameObject obj = null;
                if (src != null)
                {
                    obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                }
                else
                {
                    obj = new GameObject(part.Name);
                }
                InitializeDS1Part<MSB1CollisionPart>(obj, part, 12, Collisions);
            }

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectCollisions)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS1Part<MSB1ConnectCollisionPart>(obj, part, 12, ConnectCollisions);
            }

            GameObject Navimeshes = new GameObject("Navimeshes");
            Navimeshes.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Navmeshes)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS1Part<MSB1NavimeshPart>(obj, part, 12, Navimeshes);
            }

            GameObject Players = new GameObject("Players");
            Players.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Players)
            {
                GameObject obj = new GameObject(part.Name);
                InitializeDS1Part<MSB1PlayerPart>(obj, part, 12, Players);
            }

            //
            // Regions section
            //
            GameObject Regions = new GameObject("MSBRegions");
            foreach (var region in msb.Regions.Regions)
            {
                var reg = InstantiateRegion(region, Regions);
                reg.AddComponent<MSB1Region>();
                reg.GetComponent<MSB1Region>().setBaseRegion(region);
            }

            //
            // Events Section
            //
            GameObject Events = new GameObject("MSBEvents");

            InstantiateDS1Event<MSB1EnvironmentEvent, MSB1.Event.Environment>("Environments", Events, msb.Events.Environments);
            InstantiateDS1Event<MSB1GeneratorEvent, MSB1.Event.Generator>("Generators", Events, msb.Events.Generators);
            InstantiateDS1Event<MSB1InvasionEvent, MSB1.Event.PseudoMultiplayer>("Invasions", Events, msb.Events.PseudoMultiplayers);
            InstantiateDS1Event<MSB1LightEvent, MSB1.Event.Light>("Lights", Events, msb.Events.Lights);
            InstantiateDS1Event<MSB1MapOffsetEvent, MSB1.Event.MapOffset>("Mapoffsets", Events, msb.Events.MapOffsets);
            InstantiateDS1Event<MSB1MessageEvent, MSB1.Event.Message>("Messages", Events, msb.Events.Messages);
            InstantiateDS1Event<MSB1NavimeshEvent, MSB1.Event.Navmesh>("Navimeshes", Events, msb.Events.Navmeshes);
            InstantiateDS1Event<MSB1ObjActEvent, MSB1.Event.ObjAct>("ObjActs", Events, msb.Events.ObjActs);
            InstantiateDS1Event<MSB1SFXEvent, MSB1.Event.SFX>("SFX", Events, msb.Events.SFXs);
            InstantiateDS1Event<MSB1SoundEvent, MSB1.Event.Sound>("Sounds", Events, msb.Events.Sounds);
            InstantiateDS1Event<MSB1SpawnPointEvent, MSB1.Event.SpawnPoint>("SpawnPoints", Events, msb.Events.SpawnPoints);
            InstantiateDS1Event<MSB1TreasureEvent, MSB1.Event.Treasure>("Treasures", Events, msb.Events.Treasures);
            InstantiateDS1Event<MSB1WindEvent, MSB1.Event.WindSFX>("Winds", Events, msb.Events.WindSFXs);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import failed", e.Message + "\n" + e.StackTrace, "Ok");
        }
    }
    
    void onImportSekiroMap(object o)
    {
        try
        {
            string mapname = (string)o;
            var msb = MSBS.Read(GetOverridenPath(Interroot + $@"\map\MapStudio\{mapname}.msb.dcx"));
            string gameFolder = GameFolder(GameType.Sekiro);

            if (!AssetDatabase.IsValidFolder("Assets/Sekiro/" + mapname))
            {
                AssetDatabase.CreateFolder("Assets/Sekiro", mapname);
            }
            
            // Create an MSB asset link to the Sekiro asset
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
                    if (AssetDatabase.FindAssets($@"Assets/Sekiro/{mapname}/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx")) {
                            try
                            {
                                FlverUtilities.ImportFlver(GameType.Sekiro, Interroot + $@"\map\{mapname}\{mapname}_{assetname.Substring(1)}.mapbnd.dcx", $@"Assets/Sekiro/{mapname}/{assetname}", $@"Assets/Sekiro/{mapname.Substring(0, 3)}");
                            }
                            catch
                            {
                                Debug.LogError(assetname + " failed model import");
                            }
                        }     
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
                model.AddComponent<MSBSMapPieceModel>();
                model.GetComponent<MSBSMapPieceModel>().SetModel(mappiece);
                model.transform.parent = MapPieceModels.transform;
            }
            /*
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
            */

            GameObject ObjectModels = new GameObject("Objects");
            ObjectModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Objects)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBSObjectModel>();
                model.GetComponent<MSBSObjectModel>().SetModel(mod);
                model.transform.parent = ObjectModels.transform;
            }
            
            GameObject PlayerModels = new GameObject("Players");
            PlayerModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Players)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBSPlayerModel>();
                model.GetComponent<MSBSPlayerModel>().SetModel(mod);
                model.transform.parent = PlayerModels.transform;
            }

            GameObject EnemyModels = new GameObject("Enemies");
            EnemyModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Enemies)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBSEnemyModel>();
                model.GetComponent<MSBSEnemyModel>().SetModel(mod);
                model.transform.parent = EnemyModels.transform;
            }

            GameObject CollisionModels = new GameObject("Collisions");
            CollisionModels.transform.parent = ModelsSection.transform;
            foreach (var mod in msb.Models.Collisions)
            {
                GameObject model = new GameObject(mod.Name);
                model.AddComponent<MSBSCollisionModel>();
                model.GetComponent<MSBSCollisionModel>().SetModel(mod);
                model.transform.parent = CollisionModels.transform;
            }

            //
            // Parts Section
            //
            
            GameObject PartsSection = new GameObject("MSBParts");

            GameObject MapPieces = new GameObject("MapPieces");
            MapPieces.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.MapPieces)
            {
                //GameObject src = LoadMapFlvers ? AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Sekiro/{mapname}/{part.ModelName}.prefab") : null;
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Sekiro/{mapname}/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBSMapPiecePart>();
                    obj.GetComponent<MSBSMapPiecePart>().SetPart(part);
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
                    obj.AddComponent<MSBSMapPiecePart>();
                    obj.GetComponent<MSBSMapPiecePart>().SetPart(part);
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Sekiro/Obj/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBSObjectPart>();
                    obj.GetComponent<MSBSObjectPart>().SetPart(part);
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
                    obj.AddComponent<MSBSObjectPart>();
                    obj.GetComponent<MSBSObjectPart>().SetPart(part);
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
                GameObject obj = new GameObject(part.Name);
                obj.name = part.Name;
                obj.AddComponent<MSBSCollisionPart>();
                obj.GetComponent<MSBSCollisionPart>().SetPart(part);
                obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                obj.layer = 12;
                obj.transform.parent = Collisions.transform;
            }

            /* GameObject Collisions = new GameObject("Collisions");
            Collisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.Collisions)
            {
                string lowHigh = LoadHighResCol ? "h" : "l";
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Sekiro/{mapname}/{lowHigh}{mapname.Substring(1)}_{part.ModelName.Substring(1)}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBSCollisionPart>();
                    obj.GetComponent<MSBSCollisionPart>().SetPart(part);
                    obj.transform.position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
                    //obj.transform.rotation = Quaternion.Euler(part.Rotation.X, part.Rotation.Y, part.Rotation.Z);
                    EulerToTransform(new Vector3(part.Rotation.X, part.Rotation.Y, part.Rotation.Z), obj.transform);
                    obj.transform.localScale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);
                    obj.layer = 12;
                    obj.transform.parent = Collisions.transform;
                }
            }
            */

            GameObject ConnectCollisions = new GameObject("ConnectCollisions");
            ConnectCollisions.transform.parent = PartsSection.transform;
            foreach (var part in msb.Parts.ConnectCollisions)
            {
                GameObject obj = new GameObject(part.Name);
                obj.AddComponent<MSBSConnectCollisionPart>();
                obj.GetComponent<MSBSConnectCollisionPart>().SetPart(part);
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
                GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>($@"Assets/Sekiro/Chr/{part.ModelName}.prefab");
                if (src != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)src);
                    obj.name = part.Name;
                    obj.AddComponent<MSBSEnemyPart>();
                    obj.GetComponent<MSBSEnemyPart>().SetPart(part);
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
                    obj.AddComponent<MSBSEnemyPart>();
                    obj.GetComponent<MSBSEnemyPart>().SetPart(part);
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
                obj.AddComponent<MSBSPlayerPart>();
                obj.GetComponent<MSBSPlayerPart>().SetPart(part);
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
                obj.AddComponent<MSBSDummyObjectPart>();
                obj.GetComponent<MSBSDummyObjectPart>().SetPart(part);
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
                obj.AddComponent<MSBSDummyEnemyPart>();
                obj.GetComponent<MSBSDummyEnemyPart>().SetPart(part);
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
                reg.AddComponent<MSBSActivationAreaRegion>();
                reg.GetComponent<MSBSActivationAreaRegion>().SetRegion(region);
            }

            GameObject EnvEffBoxes = new GameObject("EnvMapEffectBoxes");
            EnvEffBoxes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.EnvironmentMapEffectBoxes)
            {
                var reg = InstantiateRegion(region, "Env Map Effect Box", EnvEffBoxes);
                reg.AddComponent<MSBSEnvironmentEffectBoxRegion>();
                reg.GetComponent<MSBSEnvironmentEffectBoxRegion>().SetRegion(region);
            }

            GameObject EnvMapPoints = new GameObject("EnvMapPoints");
            EnvMapPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.EnvironmentMapPoints)
            {
                var reg = InstantiateRegion(region, "Env Map Point", EnvMapPoints);
                reg.AddComponent<MSBSEnvironmentMapPointRegion>();
                reg.GetComponent<MSBSEnvironmentMapPointRegion>().SetRegion(region);
            }

            GameObject EventRegions = new GameObject("Events");
            EventRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Events)
            {
                var reg = InstantiateRegion(region, "Event", EventRegions);
                reg.AddComponent<MSBSEventRegion>();
                reg.GetComponent<MSBSEventRegion>().SetRegion(region);
            }

            GameObject InvasionPoints = new GameObject("InvasionPoints");
            InvasionPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.InvasionPoints)
            {
                var reg = InstantiateRegion(region, "Invasion Point", InvasionPoints);
                reg.AddComponent<MSBSInvasionPointRegion>();
                reg.GetComponent<MSBSInvasionPointRegion>().SetRegion(region);
            }

            GameObject MuffBoxes = new GameObject("MufflingBox");
            MuffBoxes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.MufflingBoxes)
            {
                var reg = InstantiateRegion(region, "Muffling Box", MuffBoxes);
                reg.AddComponent<MSBSMufflingBoxRegion>();
                reg.GetComponent<MSBSMufflingBoxRegion>().SetRegion(region);
            }

            GameObject MuffPortals = new GameObject("MufflingPortals");
            MuffPortals.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.MufflingPortals)
            {
                var reg = InstantiateRegion(region, "Muffling Portal", MuffPortals);
                reg.AddComponent<MSBSMufflingPortal>();
                reg.GetComponent<MSBSMufflingPortal>().SetRegion(region);
            }

            GameObject SFXRegions = new GameObject("SFX");
            SFXRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.SFXs)
            {
                var reg = InstantiateRegion(region, "SFX", SFXRegions);
                reg.AddComponent<MSBSSFXRegion>();
                reg.GetComponent<MSBSSFXRegion>().SetRegion(region);
            }

            GameObject SoundRegions = new GameObject("Sounds");
            SoundRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Sounds)
            {
                var reg = InstantiateRegion(region, "Sound", SoundRegions);
                reg.AddComponent<MSBSSoundRegion>();
                reg.GetComponent<MSBSSoundRegion>().SetRegion(region);
            }

            GameObject SpawnPoints = new GameObject("SpawnPoints");
            SpawnPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.SpawnPoints)
            {
                var reg = InstantiateRegion(region, "Spawn Point", SpawnPoints);
                reg.AddComponent<MSBSSpawnPointRegion>();
                reg.GetComponent<MSBSSpawnPointRegion>().SetRegion(region);
            }

            GameObject WalkRoutes = new GameObject("WalkRoutes");
            WalkRoutes.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WalkRoutes)
            {
                var reg = InstantiateRegion(region, "Walk Route", WalkRoutes);
                reg.AddComponent<MSBSWalkRouteRegion>();
                reg.GetComponent<MSBSWalkRouteRegion>().SetRegion(region);
            }

            GameObject WarpPoints = new GameObject("WarpPoints");
            WarpPoints.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WarpPoints)
            {
                var reg = InstantiateRegion(region, "Warp Point", WarpPoints);
                reg.AddComponent<MSBSWarpPointRegion>();
                reg.GetComponent<MSBSWarpPointRegion>().SetRegion(region);
            }

            GameObject WindAreas = new GameObject("WindAreas");
            WindAreas.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WindAreas)
            {
                var reg = InstantiateRegion(region, "Wind Area", WindAreas);
                reg.AddComponent<MSBSWindAreaRegion>();
                reg.GetComponent<MSBSWindAreaRegion>().SetRegion(region);
            }

            GameObject WindSFXs = new GameObject("WindSFXRegions");
            WindSFXs.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.WindSFXs)
            {
                var reg = InstantiateRegion(region, "Wind SFX", WindSFXs);
                reg.AddComponent<MSBSWindSFXRegion>();
                reg.GetComponent<MSBSWindSFXRegion>().SetRegion(region);
            }

            GameObject Region0s = new GameObject("Region0Regions");
            Region0s.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Region0s)
            {
                var reg = InstantiateRegion(region, "Region0", Region0s);
                reg.AddComponent<MSBSRegion0Region>();
                reg.GetComponent<MSBSRegion0Region>().SetRegion(region);
            }

            GameObject Region23s = new GameObject("Region23Regions");
            Region23s.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Region23s)
            {
                var reg = InstantiateRegion(region, "Region23", Region23s);
                reg.AddComponent<MSBSRegion23Region>();
                reg.GetComponent<MSBSRegion23Region>().SetRegion(region);
            }


            GameObject Region24s = new GameObject("Region24Regions");
            Region24s.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Region24s)
            {
                var reg = InstantiateRegion(region, "Region24", Region24s);
                reg.AddComponent<MSBSRegion24Region>();
                reg.GetComponent<MSBSRegion24Region>().SetRegion(region);
            }

            GameObject PartsGroups = new GameObject("PartsGroups");
            PartsGroups.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.PartsGroups)
            {
                var reg = InstantiateRegion(region, "Parts Group", PartsGroups);
                reg.AddComponent<MSBSPartsGroupRegion>();
                reg.GetComponent<MSBSPartsGroupRegion>().SetRegion(region);
            }

            GameObject AutoDrawGroups = new GameObject("AutoDrawGroups");
            AutoDrawGroups.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.AutoDrawGroups)
            {
                var reg = InstantiateRegion(region, "Auto Draw Group", PartsGroups);
                reg.AddComponent<MSBSAutoDrawGroupRegion>();
                reg.GetComponent<MSBSAutoDrawGroupRegion>().SetRegion(region);
            }

            GameObject OtherRegions = new GameObject("OtherRegions");
            OtherRegions.transform.parent = Regions.transform;
            foreach (var region in msb.Regions.Others)
            {
                var reg = InstantiateRegion(region, "Other Region", OtherRegions);
                reg.AddComponent<MSBSOtherRegion>();
                reg.GetComponent<MSBSOtherRegion>().SetRegion(region);
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
                evt.AddComponent<MSBSTreasureEvent>();
                evt.GetComponent<MSBSTreasureEvent>().SetEvent(ev);
                evt.transform.parent = Treasures.transform;
            }

            GameObject Generators = new GameObject("Generators");
            Generators.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Generators)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSGeneratorEvent>();
                evt.GetComponent<MSBSGeneratorEvent>().SetEvent(ev);
                evt.transform.parent = Generators.transform;
            }

            GameObject ObjActs = new GameObject("ObjActs");
            ObjActs.transform.parent = Events.transform;
            foreach (var ev in msb.Events.ObjActs)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSObjActEvent>();
                evt.GetComponent<MSBSObjActEvent>().SetEvent(ev);
                evt.transform.parent = ObjActs.transform;
            }

            GameObject MapOffsets = new GameObject("MapOffsets");
            MapOffsets.transform.parent = Events.transform;
            foreach (var ev in msb.Events.MapOffsets)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSMapOffsetEvent>();
                evt.GetComponent<MSBSMapOffsetEvent>().SetEvent(ev);
                evt.transform.parent = MapOffsets.transform;
            }

            GameObject WalkRouteEvents = new GameObject("WalkRoutes");
            WalkRouteEvents.transform.parent = Events.transform;
            foreach (var ev in msb.Events.WalkRoutes)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSWalkRouteEvent>();
                evt.GetComponent<MSBSWalkRouteEvent>().SetEvent(ev);
                evt.transform.parent = WalkRouteEvents.transform;
            }

            GameObject GroupTours = new GameObject("GroupTours");
            GroupTours.transform.parent = Events.transform;
            foreach (var ev in msb.Events.GroupTours)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSGroupTourEvent>();
                evt.GetComponent<MSBSGroupTourEvent>().SetEvent(ev);
                evt.transform.parent = GroupTours.transform;
            }

            GameObject Event17s = new GameObject("Event17Events");
            Event17s.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Event17s)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSEvent17Event>();
                evt.GetComponent<MSBSEvent17Event>().SetEvent(ev);
                evt.transform.parent = Event17s.transform;
            }

            GameObject Event18s = new GameObject("Event18Events");
            Event18s.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Event18s)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSEvent18Event>();
                evt.GetComponent<MSBSEvent18Event>().SetEvent(ev);
                evt.transform.parent = Event18s.transform;
            }

            GameObject Event20s = new GameObject("Event20Events");
            Event20s.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Event20s)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSEvent20Event>();
                evt.GetComponent<MSBSEvent20Event>().SetEvent(ev);
                evt.transform.parent = Event20s.transform;
            }

            GameObject Event21s = new GameObject("Event21Events");
            Event21s.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Event21s)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSEvent21Event>();
                evt.GetComponent<MSBSEvent21Event>().SetEvent(ev);
                evt.transform.parent = Event21s.transform;
            }

            GameObject Event23s = new GameObject("Event23Events");
            Event23s.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Talks)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSTalkEvent>();
                evt.GetComponent<MSBSTalkEvent>().SetEvent(ev);
                evt.transform.parent = Event23s.transform;
            }

            GameObject PartsGroupEvents = new GameObject("PartsGroups");
            PartsGroupEvents.transform.parent = Events.transform;
            foreach (var ev in msb.Events.PartsGroups)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSPartsGroupEvent>();
                evt.GetComponent<MSBSPartsGroupEvent>().SetEvent(ev);
                evt.transform.parent = PartsGroupEvents.transform;
            }

            GameObject AutoDrawGroupEvents = new GameObject("AutoDrawGroups");
            AutoDrawGroupEvents.transform.parent = Events.transform;
            foreach (var ev in msb.Events.AutoDrawGroups)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSAutoDrawGroupEvent>();
                evt.GetComponent<MSBSAutoDrawGroupEvent>().SetEvent(ev);
                evt.transform.parent = AutoDrawGroupEvents.transform;
            }

            GameObject Others = new GameObject("Others");
            Others.transform.parent = Events.transform;
            foreach (var ev in msb.Events.Others)
            {
                GameObject evt = new GameObject(ev.Name);
                evt.AddComponent<MSBSOtherEvent>();
                evt.GetComponent<MSBSOtherEvent>().SetEvent(ev);
                evt.transform.parent = Others.transform;
            }

            //
            // Routes section
            //
            GameObject Routes = new GameObject("MSBRoutes");

            GameObject MufflingBoxLinks = new GameObject("MufflingBoxLinks");
            MufflingBoxLinks.transform.parent = Routes.transform;
            foreach (var route in msb.Routes.MufflingBoxLinks)
            {
                GameObject rou = new GameObject(route.Name);
                rou.AddComponent<MSBSMufflingBoxLinkRoute>();
                rou.GetComponent<MSBSMufflingBoxLinkRoute>().SetRoute(route);
                rou.transform.parent = MufflingBoxLinks.transform;
            }

            GameObject MufflingPortalLinks = new GameObject("MufflingPortalLinks");
            MufflingPortalLinks.transform.parent = Routes.transform;
            foreach (var route in msb.Routes.MufflingPortalLinks)
            {
                GameObject rou = new GameObject(route.Name);
                rou.AddComponent<MSBSMufflingPortalLinkRoute>();
                rou.GetComponent<MSBSMufflingPortalLinkRoute>().SetRoute(route);
                rou.transform.parent = MufflingPortalLinks.transform;
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
        else if (type == GameType.DarkSoulsIISOTFS)
        {
            onImportDS2Map(o);
        }
        else if (type == GameType.DarkSoulsIII)
        {
            onImportDS3Map(o);
        }
        else if (type == GameType.Bloodborne)
        {
            onImportBBMap(o);
        }
        else if (type == GameType.Sekiro)
        {
            onImportSekiroMap(o);
        }
    }

    // Temporary hardcoded table of DS2 map offsets while MSB2 is under construction
    static Dictionary<string, Vector3> DS2MapOffsets = new Dictionary<string, Vector3>
    {
        { "m10_02_00_00", new Vector3(-498.00f,   30.00f, -260.00f) },
        { "m10_04_00_00", new Vector3(    0.0f,     0.0f,     0.0f) },
        { "m10_10_00_00", new Vector3( 208.00f,   10.00f, -133.00f) },
        { "m10_14_00_00", new Vector3(-631.00f,   94.00f,  -88.00f) },
        { "m10_15_00_00", new Vector3(-598.00f,   57.00f, -154.00f) },
        { "m10_16_00_00", new Vector3( -78.00f,    4.00f,  562.00f) },
        { "m10_17_00_00", new Vector3(-644.00f,   18.00f,  314.00f) },
        { "m10_18_00_00", new Vector3(  52.00f,  -70.00f,  487.00f) },
        { "m10_19_00_00", new Vector3(-589.00f,  166.00f,  605.00f) },
        { "m10_23_00_00", new Vector3(-166.00f,  -11.00f,  -10.00f) },
        { "m10_25_00_00", new Vector3( 188.00f, -151.00f,  -40.00f) },
        { "m10_27_00_00", new Vector3(-712.00f,   12.00f, -434.00f) },
        { "m10_29_00_00", new Vector3(-124.00f,   19.00f, -103.00f) },
        { "m10_30_00_00", new Vector3( -20.00f,  -11.00f,  358.00f) },
        { "m10_31_00_00", new Vector3( -14.00f,  -15.00f,  163.00f) },
        { "m10_32_00_00", new Vector3(-488.00f,   55.00f, -587.00f) },
        { "m10_33_00_00", new Vector3(-479.00f,   -9.00f,  211.00f) },
        { "m10_34_00_00", new Vector3(-315.00f,  -73.00f,   60.00f) },
        { "m20_10_00_00", new Vector3( 206.00f,   -4.00f, -175.61f) },
        { "m20_11_00_00", new Vector3(-903.00f,  -29.00f, -242.00f) },
        { "m20_21_00_00", new Vector3(-356.00f,   28.00f, -363.00f) },
        { "m20_24_00_00", new Vector3(-1115.00f, -157.00f, -196.00f) },
        { "m20_26_00_00", new Vector3(    0.0f,     0.0f,     0.0f) },
        { "m40_03_00_00", new Vector3(-1048.00f,   53.00f,  376.00f) },
        { "m50_35_00_00", new Vector3(    0.0f,     0.0f,     0.0f) },
        { "m50_36_00_00", new Vector3(    0.0f,     0.0f,     0.0f) },
        { "m50_37_00_00", new Vector3(    0.0f,     0.0f,     0.0f) },
        { "m50_38_00_00", new Vector3(    0.0f,     0.0f,     0.0f) }
    };

    void onImportBtl(object o)
    {
        var assetLink = GameObject.Find("MSBAssetLink");
        var alc = assetLink.GetComponent<MSBAssetLink>();
        var mapid = alc.MapID;

        // Create a folder for BTLs
        var btlroot = GameObject.Find("/BTLLights");
        if (btlroot == null)
        {
            btlroot = new GameObject("BTLLights");
        }

        string btlid = (string)o;
        var btlobject = GameObject.Find($@"/BTLLights/{btlid}");
        if (btlobject == null)
        {
            btlobject = new GameObject(btlid);
            btlobject.transform.parent = btlroot.transform;
            // Attempt to find a MapOffset object to transform the lights
            var mapoffset = GameObject.Find($@"/MSBEvents/MapOffsets");
            if (mapoffset != null)
            {
                if (type == GameType.DarkSoulsIII)
                {
                    var mo = mapoffset.GetComponentsInChildren<MSB3MapOffsetEvent>().FirstOrDefault();
                    if (mo != null)
                    {
                        btlobject.transform.position = new Vector3(mo.Position.x, mo.Position.y, mo.Position.z);
                        btlobject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, mo.Degree, 0.0f));
                    }
                }
                else if (type == GameType.Sekiro)
                {
                    var mo = mapoffset.GetComponentsInChildren<MSBSMapOffsetEvent>().FirstOrDefault();
                    if (mo != null)
                    {
                        btlobject.transform.position = new Vector3(mo.Position.x, mo.Position.y, mo.Position.z);
                        btlobject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, mo.Degree, 0.0f));
                    }
                }
            }
            // DS2 has a hardcoded fallback table since MapOffsets aren't implemented yet
            if (type == GameType.DarkSoulsIISOTFS)
            {
                if (DS2MapOffsets.ContainsKey(mapid))
                {
                    btlobject.transform.position = DS2MapOffsets[mapid];
                }
            }
            btlobject.AddComponent<BTLAssetLink>();
            var comp = btlobject.GetComponent<BTLAssetLink>();
            comp.BTLID = btlid;
            comp.BTLPath = Interroot + $@"\map\{mapid}\{btlid}.btl.dcx";
        }

        BTL btlfile = null;
        if (type == GameType.DarkSoulsIISOTFS)
        {
            // In DS2, the BTL is located inside the map's gibdt file
            var pathbase = Interroot + $@"\model\map\g{mapid.Substring(1)}";
            btlobject.GetComponent<BTLAssetLink>().BTLPath = pathbase;
            var bhdpath = GetOverridenPath(pathbase + ".gibhd");
            var bdtpath = GetOverridenPath(pathbase + ".gibdt");
            var bdt = BXF4.Read(bhdpath, bdtpath);
            var btl = bdt.Files.Find(x => x.Name.Contains("light.btl.dcx"));
            btlfile = BTL.Read(btl.Bytes);
        }
        else
        {
            btlfile = BTL.Read(GetOverridenPath(Interroot + $@"\map\{mapid}\{btlid}.btl.dcx"));
        }
        foreach (var light in btlfile.Lights)
        {
            GameObject obj = new GameObject(light.Name);
            BTLDS3Light l = null;
            if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne || type == GameType.DarkSoulsIISOTFS)
            {
                obj.AddComponent<BTLDS3Light>();
                obj.GetComponent<BTLDS3Light>().SetFromLight(light);
                l = obj.GetComponent<BTLDS3Light>();
            }
            else if (type == GameType.Sekiro)
            {
                obj.AddComponent<BTLSekiroLight>();
                obj.GetComponent<BTLSekiroLight>().SetFromLight(light);
                l = obj.GetComponent<BTLSekiroLight>();
            }
            obj.AddComponent<Light>();
            var lcomp = obj.GetComponent<Light>();
            var hdlcomp = obj.AddComponent<HDAdditionalLightData>();
            if (l.LightType == BTL.LightType.Spot)
            {
                lcomp.type = LightType.Spot;
                lcomp.spotAngle = l.ConeAngle;
            }
            else if (l.LightType == BTL.LightType.Directional)
            {
                lcomp.type = LightType.Directional;
            }
            lcomp.color = l.DiffuseColor;
            lcomp.range = l.Radius;
            hdlcomp.lightUnit = LightUnit.Lux;
            hdlcomp.luxAtDistance = l.Radius;
            hdlcomp.intensity = l.DiffusePower;
            obj.transform.parent = btlobject.transform;
            obj.transform.localPosition = new Vector3(light.Position.X, light.Position.Y, light.Position.Z);
            //EulerToTransformBTL(new Vector3(light.Rotation.X * Mathf.Rad2Deg, light.Rotation.Y * Mathf.Rad2Deg, light.Rotation.Z * Mathf.Rad2Deg), obj.transform);
            obj.transform.localEulerAngles = new Vector3(light.Rotation.X * Mathf.Rad2Deg, light.Rotation.Y * Mathf.Rad2Deg, light.Rotation.Z * Mathf.Rad2Deg);
        }
    }

    void ExportBTLs()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link. Make sure a valid map is imported.");
        }
        var al = AssetLink.GetComponent<MSBAssetLink>();

        var btlRoot = GameObject.Find("/BTLLights");
        if (btlRoot == null)
        {
            throw new Exception("No lights have been imported. Please import a BTL set for the current map");
        }

        for (int i = 0; i < btlRoot.transform.childCount; i++)
        {
            var btlset = btlRoot.transform.GetChild(i).gameObject;
            var btlal = btlset.GetComponent<BTLAssetLink>();
            if (btlal == null)
            {
                continue;
            }

            BTL export = new BTL();
            if (type == GameType.DarkSoulsIISOTFS)
            {
                export.Version = 2;
                export.LongOffsets = false;
                foreach (var l in GetChildrenOfType<BTLDS3Light>(btlset))
                {
                    export.Lights.Add(l.GetComponent<BTLDS3Light>().Serialize(l));
                }
            }
            else if (type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
            {
                export.Version = 6;
                foreach (var l in GetChildrenOfType<BTLDS3Light>(btlset))
                {
                    export.Lights.Add(l.GetComponent<BTLDS3Light>().Serialize(l));
                }
            }
            else if (type == GameType.Sekiro)
            {
                foreach (var l in GetChildrenOfType<BTLSekiroLight>(btlset))
                {
                    export.Lights.Add(l.GetComponent<BTLSekiroLight>().Serialize(l));
                }
            }

            // Directory setup for overrides
            if (ModProjectDirectory != null)
            {
                if (type == GameType.DarkSoulsIISOTFS)
                {
                    if (!Directory.Exists($@"{ModProjectDirectory}\model\map"))
                    {
                        Directory.CreateDirectory($@"{ModProjectDirectory}\model\map");
                    }
                }
                else
                {
                    if (!Directory.Exists($@"{ModProjectDirectory}\map\{al.MapID}"))
                    {
                        Directory.CreateDirectory($@"{ModProjectDirectory}\map\{al.MapID}");
                    }
                }
            }

            // Save a backup if one doesn't exist
            if (ModProjectDirectory == null && !File.Exists(btlal.BTLPath + ".gibhd.backup"))
            {
                if (type == GameType.DarkSoulsIISOTFS)
                {
                    File.Copy(btlal.BTLPath + ".gibhd", btlal.BTLPath + ".gibhd" + ".backup");
                    File.Copy(btlal.BTLPath + ".gibdt", btlal.BTLPath + ".gibdt" + ".backup");
                }
                else
                {
                    File.Copy(btlal.BTLPath, btlal.BTLPath + ".backup");
                }
            }

            // Write as a temporary file to make sure there are no errors before overwriting current file 
            string btlPath = btlal.BTLPath;
            if (GetModProjectPathForFile(btlPath) != null)
            {
                btlPath = GetModProjectPathForFile(btlPath);
            }

            if (File.Exists(btlPath + ".temp"))
            {
                if (type == GameType.DarkSoulsIISOTFS)
                {
                    File.Delete(btlal.BTLPath + ".gibhd.temp");
                    File.Delete(btlal.BTLPath + ".gibdt.temp");
                }
                else
                {
                    File.Delete(btlPath + ".temp");
                }
            }

            if (type == GameType.DarkSoulsIISOTFS)
            {
                var bytes = export.Write(SoulsFormats.DCX.Type.DarkSouls1);

                // Open the BXF file
                var path = btlPath;
                if (!File.Exists(path + ".gibhd"))
                {
                    path = btlal.BTLPath;
                }
                var archive = BXF4.Read(path + ".gibhd", path + ".gibdt");
                archive.Files.Find(x => x.Name.Contains("light.btl.dcx")).Bytes = bytes;
                archive.Write(btlPath + ".gibhd.temp", btlPath + ".gibdt.temp");

                // Make a copy of the previous map
                if (File.Exists(btlPath))
                {
                    File.Copy(btlPath + ".gibhd", btlPath + ".gibhd.prev", true);
                    File.Copy(btlPath + ".gibdt", btlPath + ".gibdt.prev", true);
                }

                // Move temp file as new map file
                File.Delete(btlPath + ".gibhd");
                File.Delete(btlPath + ".gibdt");
                File.Move(btlPath + ".gibhd.temp", btlPath + ".gibhd");
                File.Move(btlPath + ".gibdt.temp", btlPath + ".gibdt");
            }
            else
            {
                export.Write(btlPath + ".temp", (type == GameType.Sekiro) ? SoulsFormats.DCX.Type.SekiroDFLT : SoulsFormats.DCX.Type.DarkSouls3);

                // Make a copy of the previous map
                if (File.Exists(btlPath))
                {
                    File.Copy(btlPath, btlPath + ".prev", true);
                }

                // Move temp file as new map file
                File.Delete(btlPath);
                File.Move(btlPath + ".temp", btlPath);
            }
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

    static void ImportMTDBND(string path, GameType type)
    {
        IBinder bnd;
        var gameFolder = GameFolder(type);
        if (type == GameType.Bloodborne || type == GameType.DarkSoulsIII || type == GameType.Sekiro || type == GameType.DarkSoulsIISOTFS)
        {
            bnd = BND4.Read(GetOverridenPath(path));
        }
        else
        {
            bnd = BND3.Read(GetOverridenPath(path));
        }

        if (!AssetDatabase.IsValidFolder($@"Assets/{gameFolder}/MTD"))
        {
            AssetDatabase.CreateFolder($@"Assets/{gameFolder}", "MTD");
        }

        AssetDatabase.StartAssetEditing();
        foreach (var file in bnd.Files)
        {
            var mtd = SoulsFormats.MTD.Read(file.Bytes);
            var obj = ScriptableObject.CreateInstance<MTDAssetLink>();
            obj.InitializeFromMTD(mtd, file.Name);
            AssetDatabase.CreateAsset(obj, $@"Assets/{gameFolder}/MTD/{Path.GetFileNameWithoutExtension(file.Name)}.asset");
        }
        AssetDatabase.StopAssetEditing();
    }

    static void SerializeDS1Models<T, U>(List<U> exportList) where T : MSB1Model where U : MSB1.Model
    {
        var models = FindObjectsOfType<T>();
        if (models != null)
        {
            foreach (var obj in models)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeDS1Parts<T, U>(List<U> exportList) where T : MSB1Part where U : MSB1.Part
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeDS1Events<T, U>(List<U> exportList) where T : MSB1Event where U : MSB1.Event
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    void ExportMapDS1()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a DS1 asset");
        }

        MSB1 export = new MSB1();

        // Models
        SerializeDS1Models<MSB1MapPieceModel, MSB1.Model>(export.Models.MapPieces);
        SerializeDS1Models<MSB1CollisionModel, MSB1.Model>(export.Models.Collisions);
        SerializeDS1Models<MSB1EnemyModel, MSB1.Model>(export.Models.Enemies);
        SerializeDS1Models<MSB1ObjectModel, MSB1.Model>(export.Models.Objects);
        SerializeDS1Models<MSB1NavimeshModel, MSB1.Model>(export.Models.Navmeshes);
        SerializeDS1Models<MSB1PlayerModel, MSB1.Model>(export.Models.Players);

        // Parts
        SerializeDS1Parts<MSB1MapPiecePart, MSB1.Part.MapPiece>(export.Parts.MapPieces);
        SerializeDS1Parts<MSB1CollisionPart, MSB1.Part.Collision>(export.Parts.Collisions);
        SerializeDS1Parts<MSB1ConnectCollisionPart, MSB1.Part.ConnectCollision>(export.Parts.ConnectCollisions);
        SerializeDS1Parts<MSB1DummyEnemyPart, MSB1.Part.DummyEnemy>(export.Parts.DummyEnemies);
        SerializeDS1Parts<MSB1DummyObjectPart, MSB1.Part.DummyObject>(export.Parts.DummyObjects);
        SerializeDS1Parts<MSB1EnemyPart, MSB1.Part.Enemy>(export.Parts.Enemies);
        SerializeDS1Parts<MSB1ObjectPart, MSB1.Part.Object>(export.Parts.Objects);
        SerializeDS1Parts<MSB1PlayerPart, MSB1.Part.Player>(export.Parts.Players);
        SerializeDS1Parts<MSB1NavimeshPart, MSB1.Part.Navmesh>(export.Parts.Navmeshes);

        // Regions
        var regions = FindObjectsOfType<MSB1Region>();
        if (regions != null)
        {
            foreach (var obj in regions)
            {
                export.Regions.Regions.Add(obj.Serialize(new MSB1.Region(obj.name), obj.gameObject));
            }
        }

        // Events
        SerializeDS1Events<MSB1TreasureEvent, MSB1.Event.Treasure>(export.Events.Treasures);
        SerializeDS1Events<MSB1EnvironmentEvent, MSB1.Event.Environment>(export.Events.Environments);
        SerializeDS1Events<MSB1GeneratorEvent, MSB1.Event.Generator>(export.Events.Generators);
        SerializeDS1Events<MSB1ObjActEvent, MSB1.Event.ObjAct>(export.Events.ObjActs);
        SerializeDS1Events<MSB1MapOffsetEvent, MSB1.Event.MapOffset>(export.Events.MapOffsets);
        SerializeDS1Events<MSB1InvasionEvent, MSB1.Event.PseudoMultiplayer>(export.Events.PseudoMultiplayers);
        SerializeDS1Events<MSB1LightEvent, MSB1.Event.Light>(export.Events.Lights);
        SerializeDS1Events<MSB1MessageEvent, MSB1.Event.Message>(export.Events.Messages);
        SerializeDS1Events<MSB1NavimeshEvent, MSB1.Event.Navmesh>(export.Events.Navmeshes);
        SerializeDS1Events<MSB1SFXEvent, MSB1.Event.SFX>(export.Events.SFXs);
        SerializeDS1Events<MSB1SoundEvent, MSB1.Event.Sound>(export.Events.Sounds);
        SerializeDS1Events<MSB1SpawnPointEvent, MSB1.Event.SpawnPoint>(export.Events.SpawnPoints);
        SerializeDS1Events<MSB1WindEvent, MSB1.Event.WindSFX>(export.Events.WindSFXs);

        // Directory setup for overrides
        if (ModProjectDirectory != null)
        {
            if (!Directory.Exists($@"{ModProjectDirectory}\map\mapstudio"))
            {
                Directory.CreateDirectory($@"{ModProjectDirectory}\map\mapstudio");
            }
        }

        // Save a backup if one doesn't exist and not using a mod project directory
        if (ModProjectDirectory == null && !File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup"))
        {
            File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup");
        }

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        string mapPath = AssetLink.GetComponent<MSBAssetLink>().MapPath;
        if (GetModProjectPathForFile(mapPath) != null)
        {
            mapPath = GetModProjectPathForFile(mapPath);
        }

        if (File.Exists(mapPath + ".temp"))
        {
            File.Delete(mapPath + ".temp");
        }
        export.Write(mapPath + ".temp", SoulsFormats.DCX.Type.DarkSouls1);

        // Make a copy of the previous map
        File.Copy(mapPath, mapPath + ".prev", true);

        // Move temp file as new map file
        File.Delete(mapPath);
        File.Move(mapPath + ".temp", mapPath);
    }

    static void SerializeBBModels<T, U>(List<U> exportList) where T : MSBBBModel where U : MSBBB.Model
    {
        var models = FindObjectsOfType<T>();
        if (models != null)
        {
            foreach (var obj in models)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeBBParts<T, U>(List<U> exportList) where T : MSBBBPart where U : MSBBB.Part
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeBBEvents<T, U>(List<U> exportList) where T : MSBBBEvent where U : MSBBB.Event
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    void ExportMapBB()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a BB asset");
        }

        MSBBB export = new MSBBB();

        // Models
        SerializeBBModels<MSBBBMapPieceModel, MSBBB.Model.MapPiece>(export.Models.MapPieces);
        SerializeBBModels<MSBBBCollisionModel, MSBBB.Model.Collision>(export.Models.Collisions);
        SerializeBBModels<MSBBBEnemyModel, MSBBB.Model.Enemy>(export.Models.Enemies);
        SerializeBBModels<MSBBBObjectModel, MSBBB.Model.Object>(export.Models.Objects);
        SerializeBBModels<MSBBBOtherModel, MSBBB.Model.Other>(export.Models.Others);
        SerializeBBModels<MSBBBPlayerModel, MSBBB.Model.Player>(export.Models.Players);

        // Parts
        SerializeBBParts<MSBBBMapPiecePart, MSBBB.Part.MapPiece>(export.Parts.MapPieces);
        SerializeBBParts<MSBBBCollisionPart, MSBBB.Part.Collision>(export.Parts.Collisions);
        SerializeBBParts<MSBBBConnectCollisionPart, MSBBB.Part.ConnectCollision>(export.Parts.ConnectCollisions);
        SerializeBBParts<MSBBBDummyEnemyPart, MSBBB.Part.DummyEnemy>(export.Parts.DummyEnemies);
        SerializeBBParts<MSBBBDummyObjectPart, MSBBB.Part.DummyObject>(export.Parts.DummyObjects);
        SerializeBBParts<MSBBBEnemyPart, MSBBB.Part.Enemy>(export.Parts.Enemies);
        SerializeBBParts<MSBBBObjectPart, MSBBB.Part.Object>(export.Parts.Objects);
        SerializeBBParts<MSBBBPlayerPart, MSBBB.Part.Player>(export.Parts.Players);

        // Regions
        var regions = FindObjectsOfType<MSBBBRegion>();
        if (regions != null)
        {
            foreach (var obj in regions)
            {
                export.Regions.Regions.Add(obj.Serialize(new MSBBB.Region(obj.name), obj.gameObject));
            }
        }

        // Events
        SerializeBBEvents<MSBBBSoundEvent, MSBBB.Event.Sound>(export.Events.Sounds);
        SerializeBBEvents<MSBBBSFXEvent, MSBBB.Event.SFX>(export.Events.SFXs);
        SerializeBBEvents<MSBBBTreasureEvent, MSBBB.Event.Treasure>(export.Events.Treasures);
        SerializeBBEvents<MSBBBGeneratorEvent, MSBBB.Event.Generator>(export.Events.Generators);
        SerializeBBEvents<MSBBBMessageEvent, MSBBB.Event.Message>(export.Events.Messages);
        SerializeBBEvents<MSBBBObjActEvent, MSBBB.Event.ObjAct>(export.Events.ObjActs);
        SerializeBBEvents<MSBBBSpawnPointEvent, MSBBB.Event.SpawnPoint>(export.Events.SpawnPoints);
        SerializeBBEvents<MSBBBMapOffsetEvent, MSBBB.Event.MapOffset>(export.Events.MapOffsets);
        SerializeBBEvents<MSBBBNavimeshEvent, MSBBB.Event.Navimesh>(export.Events.Navimeshes);
        SerializeBBEvents<MSBBBEnvironmentEvent, MSBBB.Event.Environment>(export.Events.Environments);
        SerializeBBEvents<MSBBBInvasionEvent, MSBBB.Event.Invasion>(export.Events.Invasions);
        SerializeBBEvents<MSBBBWindEvent, MSBBB.Event.Wind>(export.Events.Mysteries);
        SerializeBBEvents<MSBBBWalkRouteEvent, MSBBB.Event.WalkRoute>(export.Events.WalkRoutes);
        SerializeBBEvents<MSBBBUnknownEvent, MSBBB.Event.Unknown>(export.Events.Unknowns);
        SerializeBBEvents<MSBBBGroupTourEvent, MSBBB.Event.GroupTour>(export.Events.GroupTours);
        SerializeBBEvents<MSBBBMultiSummoningPointEvent, MSBBB.Event.MultiSummoningPoint>(export.Events.MultiSummoningPoints);
        SerializeBBEvents<MSBBBOtherEvent, MSBBB.Event.Other>(export.Events.Others);

        // Directory setup for overrides
        if (ModProjectDirectory != null)
        {
            if (!Directory.Exists($@"{ModProjectDirectory}\map\mapstudio"))
            {
                Directory.CreateDirectory($@"{ModProjectDirectory}\map\mapstudio");
            }
        }

        // Save a backup if one doesn't exist
        if (ModProjectDirectory == null && !File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup"))
        {
            File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup");
        }

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        string mapPath = AssetLink.GetComponent<MSBAssetLink>().MapPath;
        if (GetModProjectPathForFile(mapPath) != null)
        {
            mapPath = GetModProjectPathForFile(mapPath);
            if (!Directory.Exists(Path.GetDirectoryName(mapPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(mapPath));
            }
        }

        if (File.Exists(mapPath + ".temp"))
        {
            File.Delete(mapPath + ".temp");
        }
        export.Write(mapPath + ".temp", SoulsFormats.DCX.Type.DarkSouls3);

        // Make a copy of the previous map
        if (File.Exists(mapPath))
        {
            File.Copy(mapPath, mapPath + ".prev", true);
        }

        // Move temp file as new map file
        if (File.Exists(mapPath))
        {
            File.Delete(mapPath);
        }
        File.Move(mapPath + ".temp", mapPath);
    }

    static void SerializeDS3Models<T, U>(List<U> exportList) where T : MSB3Model where U : MSB3.Model
    {
        var models = FindObjectsOfType<T>();
        if (models != null)
        {
            foreach (var obj in models)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeDS3Parts<T, U>(List<U> exportList) where T : MSB3Part where U : MSB3.Part
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeDS3Regions<T, U>(List<U> exportList) where T : MSB3Region where U : MSB3.Region
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    static void SerializeDS3Events<T, U>(List<U> exportList) where T : MSB3Event where U : MSB3.Event
    {
        var parts = FindObjectsOfType<T>();
        if (parts != null)
        {
            foreach (var obj in parts)
            {
                exportList.Add((U)obj.Serialize(obj.gameObject));
            }
        }
    }

    void ExportMapDS3()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a DS3 asset");
        }

        MSB3 export = new MSB3();

        // Models
        SerializeDS3Models<MSB3MapPieceModel, MSB3.Model.MapPiece>(export.Models.MapPieces);
        SerializeDS3Models<MSB3CollisionModel, MSB3.Model.Collision>(export.Models.Collisions);
        SerializeDS3Models<MSB3EnemyModel, MSB3.Model.Enemy>(export.Models.Enemies);
        SerializeDS3Models<MSB3ObjectModel, MSB3.Model.Object>(export.Models.Objects);
        SerializeDS3Models<MSB3OtherModel, MSB3.Model.Other>(export.Models.Others);
        SerializeDS3Models<MSB3PlayerModel, MSB3.Model.Player>(export.Models.Players);

        // Parts
        SerializeDS3Parts<MSB3MapPiecePart, MSB3.Part.MapPiece>(export.Parts.MapPieces);
        SerializeDS3Parts<MSB3CollisionPart, MSB3.Part.Collision>(export.Parts.Collisions);
        SerializeDS3Parts<MSB3ConnectCollisionPart, MSB3.Part.ConnectCollision>(export.Parts.ConnectCollisions);
        SerializeDS3Parts<MSB3DummyEnemyPart, MSB3.Part.DummyEnemy>(export.Parts.DummyEnemies);
        SerializeDS3Parts<MSB3DummyObjectPart, MSB3.Part.DummyObject>(export.Parts.DummyObjects);
        SerializeDS3Parts<MSB3EnemyPart, MSB3.Part.Enemy>(export.Parts.Enemies);
        SerializeDS3Parts<MSB3ObjectPart, MSB3.Part.Object>(export.Parts.Objects);
        SerializeDS3Parts<MSB3PlayerPart, MSB3.Part.Player>(export.Parts.Players);

        // Regions
        SerializeDS3Regions<MSB3ActivationAreaRegion, MSB3.Region.ActivationArea>(export.Regions.ActivationAreas);
        SerializeDS3Regions<MSB3EnvironmentEffectBoxRegion, MSB3.Region.EnvironmentMapEffectBox>(export.Regions.EnvironmentMapEffectBoxes);
        SerializeDS3Regions<MSB3EnvironmentMapPointRegion, MSB3.Region.EnvironmentMapPoint>(export.Regions.EnvironmentMapPoints);
        SerializeDS3Regions<MSB3EventRegion, MSB3.Region.Event>(export.Regions.Events);
        SerializeDS3Regions<MSB3GeneralRegion, MSB3.Region.General>(export.Regions.General);
        SerializeDS3Regions<MSB3InvasionPointRegion, MSB3.Region.InvasionPoint>(export.Regions.InvasionPoints);
        SerializeDS3Regions<MSB3MessageRegion, MSB3.Region.Message>(export.Regions.Messages);
        SerializeDS3Regions<MSB3MufflingBoxRegion, MSB3.Region.MufflingBox>(export.Regions.MufflingBoxes);
        SerializeDS3Regions<MSB3MufflingPortal, MSB3.Region.MufflingPortal>(export.Regions.MufflingPortals);
        SerializeDS3Regions<MSB3SFXRegion, MSB3.Region.SFX>(export.Regions.SFX);
        SerializeDS3Regions<MSB3SoundRegion, MSB3.Region.Sound>(export.Regions.Sounds);
        SerializeDS3Regions<MSB3SpawnPointRegion, MSB3.Region.SpawnPoint>(export.Regions.SpawnPoints);
        SerializeDS3Regions<MSB3WalkRouteRegion, MSB3.Region.WalkRoute>(export.Regions.WalkRoutes);
        SerializeDS3Regions<MSB3WarpPointRegion, MSB3.Region.WarpPoint>(export.Regions.WarpPoints);
        SerializeDS3Regions<MSB3WindAreaRegion, MSB3.Region.WindArea>(export.Regions.WindAreas);
        SerializeDS3Regions<MSB3WindSFXRegion, MSB3.Region.WindSFX>(export.Regions.WindSFX);
        SerializeDS3Regions<MSB3Unk00Region, MSB3.Region.Unk00>(export.Regions.Unk00s);
        SerializeDS3Regions<MSB3Unk12Region, MSB3.Region.Unk12>(export.Regions.Unk12s);

        // Events
        SerializeDS3Events<MSB3TreasureEvent, MSB3.Event.Treasure>(export.Events.Treasures);
        SerializeDS3Events<MSB3GeneratorEvent, MSB3.Event.Generator>(export.Events.Generators);
        SerializeDS3Events<MSB3ObjActEvent, MSB3.Event.ObjAct>(export.Events.ObjActs);
        SerializeDS3Events<MSB3MapOffsetEvent, MSB3.Event.MapOffset>(export.Events.MapOffsets);
        SerializeDS3Events<MSB3InvasionEvent, MSB3.Event.PseudoMultiplayer>(export.Events.PseudoMultiplayers);
        SerializeDS3Events<MSB3WalkRouteEvent, MSB3.Event.WalkRoute>(export.Events.WalkRoutes);
        SerializeDS3Events<MSB3GroupTourEvent, MSB3.Event.GroupTour>(export.Events.GroupTours);
        SerializeDS3Events<MSB3OtherEvent, MSB3.Event.Other>(export.Events.Others);

        // Attempt to restore parts pose section from backup map if needed
        if (PreservePartsPose)
        {
            string backupMap = AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup";
            if (!File.Exists(backupMap))
            {
                backupMap = AssetLink.GetComponent<MSBAssetLink>().MapPath;
                if (!File.Exists(backupMap))
                {
                    backupMap = null;
                }
            }
            if (backupMap != null)
            {
                var back = MSB3.Read(backupMap);
                export.BoneNames = back.BoneNames;
                export.Layers = back.Layers;
                export.PartsPoses = back.PartsPoses;
                export.Routes = back.Routes; 
            }
        }

        // Directory setup for overrides
        if (ModProjectDirectory != null)
        {
            if (!Directory.Exists($@"{ModProjectDirectory}\map\mapstudio"))
            {
                Directory.CreateDirectory($@"{ModProjectDirectory}\map\mapstudio");
            }
        }

        // Save a backup if one doesn't exist
        if (ModProjectDirectory == null && !File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup"))
        {
            File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup");
        }

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        string mapPath = AssetLink.GetComponent<MSBAssetLink>().MapPath;
        if (GetModProjectPathForFile(mapPath) != null)
        {
            mapPath = GetModProjectPathForFile(mapPath);
        }

        if (File.Exists(mapPath + ".temp"))
        {
            File.Delete(mapPath + ".temp");
        }
        export.Write(mapPath + ".temp", SoulsFormats.DCX.Type.DarkSouls3);

        // Make a copy of the previous map
        if (File.Exists(mapPath))
        {
            File.Copy(mapPath, mapPath + ".prev", true);
        }

        // Move temp file as new map file
        File.Delete(mapPath);
        File.Move(mapPath + ".temp", mapPath);
    }
    
    void ExportMapSekiro()
    {
        var AssetLink = GameObject.Find("MSBAssetLink");
        if (AssetLink == null || AssetLink.GetComponent<MSBAssetLink>() == null)
        {
            throw new Exception("Could not find a valid MSB asset link to a Sekiro asset.");
        }

        MSBS export = new MSBS();
        // Export the models
        var Models = GameObject.Find("/MSBModelDeclarations");
        if (Models != null)
        {
            // Export map pieces
            var modelMapPieces = GetChild(Models, "MapPieces");
            if (modelMapPieces != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMapPieceModel>(modelMapPieces))
                {
                    export.Models.MapPieces.Add(obj.GetComponent<MSBSMapPieceModel>().Serialize(obj));
                }
            }

            // Export collision
            var modelCollision = GetChild(Models, "Collisions");
            if (modelCollision != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSCollisionModel>(modelCollision))
                {
                    export.Models.Collisions.Add(obj.GetComponent<MSBSCollisionModel>().Serialize(obj));
                }
            }

            // Export enemies
            var modelEnemy = GetChild(Models, "Enemies");
            if (modelEnemy != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEnemyModel>(modelEnemy))
                {
                    export.Models.Enemies.Add(obj.GetComponent<MSBSEnemyModel>().Serialize(obj));
                }
            }

            // Export objects
            var modelObject = GetChild(Models, "Objects");
            if (modelObject != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSObjectModel>(modelObject))
                {
                    export.Models.Objects.Add(obj.GetComponent<MSBSObjectModel>().Serialize(obj));
                }
            }

            // Export players
            var modelPlayer = GetChild(Models, "Players");
            if (modelPlayer != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSPlayerModel>(modelPlayer))
                {
                    export.Models.Players.Add(obj.GetComponent<MSBSPlayerModel>().Serialize(obj));
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
                foreach (var obj in GetChildrenOfType<MSBSMapPiecePart>(partMapPieces))
                {
                    export.Parts.MapPieces.Add(obj.GetComponent<MSBSMapPiecePart>().Serialize(obj));
                }
            }

            // Export collisions
            var partCollisions = GetChild(Parts, "Collisions");
            if (partCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSCollisionPart>(partCollisions))
                {
                    export.Parts.Collisions.Add(obj.GetComponent<MSBSCollisionPart>().Serialize(obj));
                }
            }
            
            // Export connect collisions
            var partConnectCollisions = GetChild(Parts, "ConnectCollisions");
            if (partConnectCollisions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSConnectCollisionPart>(partConnectCollisions))
                {
                    export.Parts.ConnectCollisions.Add(obj.GetComponent<MSBSConnectCollisionPart>().Serialize(obj));
                }
            }
            
            // Export dummy enemies
            var partDummyEnemies = GetChild(Parts, "DummyEnemies");
            if (partDummyEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSDummyEnemyPart>(partDummyEnemies))
                {
                    export.Parts.DummyEnemies.Add(obj.GetComponent<MSBSDummyEnemyPart>().Serialize(obj));
                }
            }

            var partDummyObjects = GetChild(Parts, "DummyObjects");
            if (partDummyObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSDummyObjectPart>(partDummyObjects))
                {
                    export.Parts.DummyObjects.Add(obj.GetComponent<MSBSDummyObjectPart>().Serialize(obj));
                }
            }
            
            var partEnemies = GetChild(Parts, "Enemies");
            if (partEnemies != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEnemyPart>(partEnemies))
                {
                    export.Parts.Enemies.Add(obj.GetComponent<MSBSEnemyPart>().Serialize(obj));
                }
            }
           
            var partObjects = GetChild(Parts, "Objects");
            if (partObjects != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSObjectPart>(partObjects))
                {
                    export.Parts.Objects.Add(obj.GetComponent<MSBSObjectPart>().Serialize(obj));
                }
            }
            
            var partPlayers = GetChild(Parts, "Players");
            if (partPlayers != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSPlayerPart>(partPlayers))
                {
                    export.Parts.Players.Add(obj.GetComponent<MSBSPlayerPart>().Serialize(obj));
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
                foreach (var obj in GetChildrenOfType<MSBSActivationAreaRegion>(regionActAreas))
                {
                    export.Regions.ActivationAreas.Add(obj.GetComponent<MSBSActivationAreaRegion>().Serialize(obj));
                }
            }

            var regionEnvMapEffectBoxes = GetChild(Regions, "EnvMapEffectBoxes");
            if (regionEnvMapEffectBoxes != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEnvironmentEffectBoxRegion>(regionEnvMapEffectBoxes))
                {
                    export.Regions.EnvironmentMapEffectBoxes.Add(obj.GetComponent<MSBSEnvironmentEffectBoxRegion>().Serialize(obj));
                }
            }

            var regionEnvMapPoints = GetChild(Regions, "EnvMapPoints");
            if (regionEnvMapPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEnvironmentMapPointRegion>(regionEnvMapPoints))
                {
                    export.Regions.EnvironmentMapPoints.Add(obj.GetComponent<MSBSEnvironmentMapPointRegion>().Serialize(obj));
                }
            }

            var regionEvents = GetChild(Regions, "Events");
            if (regionEvents != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEventRegion>(regionEvents))
                {
                    export.Regions.Events.Add(obj.GetComponent<MSBSEventRegion>().Serialize(obj));
                }
            }

            var regionInvasionPoints = GetChild(Regions, "InvasionPoints");
            if (regionInvasionPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSInvasionPointRegion>(regionInvasionPoints))
                {
                    export.Regions.InvasionPoints.Add(obj.GetComponent<MSBSInvasionPointRegion>().Serialize(obj));
                }
            }

            var regionMufflingBox = GetChild(Regions, "MufflingBox");
            if (regionMufflingBox != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMufflingBoxRegion>(regionMufflingBox))
                {
                    export.Regions.MufflingBoxes.Add(obj.GetComponent<MSBSMufflingBoxRegion>().Serialize(obj));
                }
            }

            var regionMufflingPortals = GetChild(Regions, "MufflingPortals");
            if (regionMufflingPortals != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMufflingPortal>(regionMufflingPortals))
                {
                    export.Regions.MufflingPortals.Add(obj.GetComponent<MSBSMufflingPortal>().Serialize(obj));
                }
            }

            var regionSFX = GetChild(Regions, "SFX");
            if (regionSFX != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSSFXRegion>(regionSFX))
                {
                    export.Regions.SFXs.Add(obj.GetComponent<MSBSSFXRegion>().Serialize(obj));
                }
            }

            var regionSounds = GetChild(Regions, "Sounds");
            if (regionSounds != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSSoundRegion>(regionSounds))
                {
                    export.Regions.Sounds.Add(obj.GetComponent<MSBSSoundRegion>().Serialize(obj));
                }
            }

            var regionSpawnPoints = GetChild(Regions, "SpawnPoints");
            if (regionSpawnPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSSpawnPointRegion>(regionSpawnPoints))
                {
                    export.Regions.SpawnPoints.Add(obj.GetComponent<MSBSSpawnPointRegion>().Serialize(obj));
                }
            }

            var regionWalkRoutes = GetChild(Regions, "WalkRoutes");
            if (regionWalkRoutes != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSWalkRouteRegion>(regionWalkRoutes))
                {
                    export.Regions.WalkRoutes.Add(obj.GetComponent<MSBSWalkRouteRegion>().Serialize(obj));
                }
            }

            var regionWarpPoints = GetChild(Regions, "WarpPoints");
            if (regionWarpPoints != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSWarpPointRegion>(regionWarpPoints))
                {
                    export.Regions.WarpPoints.Add(obj.GetComponent<MSBSWarpPointRegion>().Serialize(obj));
                }
            }

            var regionWindAreas = GetChild(Regions, "WindAreas");
            if (regionWindAreas != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSWindAreaRegion>(regionWindAreas))
                {
                    export.Regions.WindAreas.Add(obj.GetComponent<MSBSWindAreaRegion>().Serialize(obj));
                }
            }

            var regionWindSFX = GetChild(Regions, "WindSFXRegions");
            if (regionWindSFX != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSWindSFXRegion>(regionWindSFX))
                {
                    export.Regions.WindSFXs.Add(obj.GetComponent<MSBSWindSFXRegion>().Serialize(obj));
                }
            }

            var region0Regions = GetChild(Regions, "Region0Regions");
            if (region0Regions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSRegion0Region>(region0Regions))
                {
                    export.Regions.Region0s.Add(obj.GetComponent<MSBSRegion0Region>().Serialize(obj));
                }
            }

            var region23Regions = GetChild(Regions, "Region23Regions");
            if (region23Regions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSRegion23Region>(region23Regions))
                {
                    export.Regions.Region23s.Add(obj.GetComponent<MSBSRegion23Region>().Serialize(obj));
                }
            }

            var region24Regions = GetChild(Regions, "Region24Regions");
            if (region24Regions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSRegion24Region>(region24Regions))
                {
                    export.Regions.Region24s.Add(obj.GetComponent<MSBSRegion24Region>().Serialize(obj));
                }
            }

            var partsGroups = GetChild(Regions, "PartsGroups");
            if (partsGroups != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSPartsGroupRegion>(partsGroups))
                {
                    export.Regions.PartsGroups.Add(obj.GetComponent<MSBSPartsGroupRegion>().Serialize(obj));
                }
            }

            var autoDrawGroups = GetChild(Regions, "AutoDrawGroups");
            if (autoDrawGroups != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSAutoDrawGroupRegion>(autoDrawGroups))
                {
                    export.Regions.AutoDrawGroups.Add(obj.GetComponent<MSBSAutoDrawGroupRegion>().Serialize(obj));
                }
            }

            var otherRegions = GetChild(Regions, "OtherRegions");
            if (otherRegions != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSOtherRegion>(otherRegions))
                {
                    export.Regions.Others.Add(obj.GetComponent<MSBSOtherRegion>().Serialize(obj));
                }
            }

        }
        else
        {
            throw new Exception("MSB exporter requires a regions section");
        }
        
        // Export the events
        var Events = GameObject.Find("/MSBEvents");
        if (Events != null)
        {
            var eventTreasures = GetChild(Events, "Treasures");
            if (eventTreasures != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSTreasureEvent>(eventTreasures))
                {
                    export.Events.Treasures.Add(obj.GetComponent<MSBSTreasureEvent>().Serialize(obj));
                }
            }

            var eventGenerators = GetChild(Events, "Generators");
            if (eventGenerators != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSGeneratorEvent>(eventGenerators))
                {
                    export.Events.Generators.Add(obj.GetComponent<MSBSGeneratorEvent>().Serialize(obj));
                }
            }

            var eventObjActs = GetChild(Events, "ObjActs");
            if (eventObjActs != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSObjActEvent>(eventObjActs))
                {
                    export.Events.ObjActs.Add(obj.GetComponent<MSBSObjActEvent>().Serialize(obj));
                }
            }

            var eventMapOffsets = GetChild(Events, "MapOffsets");
            if (eventMapOffsets != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMapOffsetEvent>(eventMapOffsets))
                {
                    export.Events.MapOffsets.Add(obj.GetComponent<MSBSMapOffsetEvent>().Serialize(obj));
                }
            }

            var eventWalkRoutes = GetChild(Events, "WalkRoutes");
            if (eventWalkRoutes != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSWalkRouteEvent>(eventWalkRoutes))
                {
                    export.Events.WalkRoutes.Add(obj.GetComponent<MSBSWalkRouteEvent>().Serialize(obj));
                }
            }

            var eventGroupTours = GetChild(Events, "GroupTours");
            if (eventGroupTours != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSGroupTourEvent>(eventGroupTours))
                {
                    export.Events.GroupTours.Add(obj.GetComponent<MSBSGroupTourEvent>().Serialize(obj));
                }
            }

            var event17s = GetChild(Events, "Event17Events");
            if (event17s != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEvent17Event>(event17s))
                {
                    export.Events.Event17s.Add(obj.GetComponent<MSBSEvent17Event>().Serialize(obj));
                }
            }

            var event18s = GetChild(Events, "Event18Events");
            if (event18s != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEvent18Event>(event18s))
                {
                    export.Events.Event18s.Add(obj.GetComponent<MSBSEvent18Event>().Serialize(obj));
                }
            }

            var event20s = GetChild(Events, "Event20Events");
            if (event20s != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEvent20Event>(event20s))
                {
                    export.Events.Event20s.Add(obj.GetComponent<MSBSEvent20Event>().Serialize(obj));
                }
            }

            var event21s = GetChild(Events, "Event21Events");
            if (event21s != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSEvent21Event>(event21s))
                {
                    export.Events.Event21s.Add(obj.GetComponent<MSBSEvent21Event>().Serialize(obj));
                }
            }

            var partsGroupEvents = GetChild(Events, "PartsGroups");
            if (partsGroupEvents != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSPartsGroupEvent>(partsGroupEvents))
                {
                    export.Events.PartsGroups.Add(obj.GetComponent<MSBSPartsGroupEvent>().Serialize(obj));
                }
            }

            var event23s = GetChild(Events, "Event23Events");
            if (event23s != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSTalkEvent>(event23s))
                {
                    export.Events.Talks.Add(obj.GetComponent<MSBSTalkEvent>().Serialize(obj));
                }
            }

            var autoDrawGroupEvents = GetChild(Events, "AutoDrawGroups");
            if (autoDrawGroupEvents != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSAutoDrawGroupEvent>(autoDrawGroupEvents))
                {
                    export.Events.AutoDrawGroups.Add(obj.GetComponent<MSBSAutoDrawGroupEvent>().Serialize(obj));
                }
            }

            var eventOthers = GetChild(Events, "Others");
            if (eventOthers != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSOtherEvent>(eventOthers))
                {
                    export.Events.Others.Add(obj.GetComponent<MSBSOtherEvent>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires an events section");
        }

        // Export the routes
        var Routes = GameObject.Find("/MSBRoutes");
        if (Routes != null)
        {
            var mufflingBoxLinks = GetChild(Routes, "MufflingBoxLinks");
            if (mufflingBoxLinks != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMufflingBoxLinkRoute>(mufflingBoxLinks))
                {
                    export.Routes.MufflingBoxLinks.Add(obj.GetComponent<MSBSMufflingBoxLinkRoute>().Serialize(obj));
                }
            }

            var mufflingPortalLinks = GetChild(Routes, "MufflingPortalLinks");
            if (mufflingPortalLinks != null)
            {
                foreach (var obj in GetChildrenOfType<MSBSMufflingPortalLinkRoute>(mufflingPortalLinks))
                {
                    export.Routes.MufflingPortalLinks.Add(obj.GetComponent<MSBSMufflingPortalLinkRoute>().Serialize(obj));
                }
            }
        }
        else
        {
            throw new Exception("MSB exporter requires a routes section");
        }

        // Directory setup for overrides
        if (ModProjectDirectory != null)
        {
            if (!Directory.Exists($@"{ModProjectDirectory}\map\mapstudio"))
            {
                Directory.CreateDirectory($@"{ModProjectDirectory}\map\mapstudio");
            }
        }
        // Save a backup if one doesn't exist
        if (ModProjectDirectory == null && !File.Exists(AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup"))
        {
            File.Copy(AssetLink.GetComponent<MSBAssetLink>().MapPath, AssetLink.GetComponent<MSBAssetLink>().MapPath + ".backup");
        }

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        string mapPath = AssetLink.GetComponent<MSBAssetLink>().MapPath;
        if (GetModProjectPathForFile(mapPath) != null)
        {
            mapPath = GetModProjectPathForFile(mapPath);
        }

        if (File.Exists(mapPath + ".temp"))
        {
            File.Delete(mapPath + ".temp");
        }
        export.Write(mapPath + ".temp", SoulsFormats.DCX.Type.SekiroDFLT);

        // Make a copy of the previous map
        if (File.Exists(mapPath))
        {
            File.Copy(mapPath, mapPath + ".prev", true);
        }

        // Move temp file as new map file
        File.Delete(mapPath);
        File.Move(mapPath + ".temp", mapPath);
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
            ExportMapBB();
        }
        else if (type == GameType.DarkSoulsIII)
        {
            ExportMapDS3();
        }
        else if (type == GameType.Sekiro)
        {
            ExportMapSekiro();
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
            else if (file.ToLower().Contains("darksoulsii.exe"))
            {
                type = GameType.DarkSoulsIISOTFS;
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
            else if (file.ToLower().Contains("sekiro.exe"))
            {
                type = GameType.Sekiro;
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid game path", "This does not appear to be a path for a supported game", "Ok");
            }
            UpdateMapList();
        }

        if (GUILayout.Button("Set Mod Project Directory (optional)"))
        {
            string directory = EditorUtility.OpenFolderPanel("Select directory for mod project", Interroot, "");
            if (directory != "")
            {
                ModProjectDirectory = Path.GetFullPath(directory);
            }
        }

        GUILayout.Label("Interroot: " + Interroot);
        GUILayout.Label("Mod Project: " + ModProjectDirectory);

        GUILayout.Label("Development Test Tools (don't use)", EditorStyles.boldLabel);
        if (GUILayout.Button("Import FLVER"))
        {
            string file = EditorUtility.OpenFilePanel("Select a flver", "", "flver,dcx,mapbnd");
            try
            {
                FlverUtilities.ImportFlver(type, file, "Assets/" + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed: " + e.Message, e.StackTrace, "Ok");
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

        if (GUILayout.Button("Import CHRBND"))
        {
            string file = EditorUtility.OpenFilePanel("Select an chrbnd", "", "dcx,chrbnd");
            try
            {
                AssetDatabase.StartAssetEditing();
                ImportChrTexbnd(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)), type);
                ImportChrTextures(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)), type);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.StartAssetEditing();
                ImportChr(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)), type);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Import failed: " + e.Message, e.StackTrace, "Ok");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
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
                if (type == GameType.DarkSoulsIISOTFS)
                {
                    ImportMTDBND(Interroot + $@"\material\allmaterialbnd.bnd", type);
                }
                else if (type != GameType.DarkSoulsIII && type != GameType.Sekiro)
                {
                    ImportMTDBND(Interroot + $@"\mtd\Mtd.mtdbnd", type);
                }
                else
                {
                    ImportMTDBND(Interroot + $@"\mtd\allmaterialbnd.mtdbnd.dcx", type);
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
                    EditorUtility.DisplayDialog("Import failed: " + e.Message, e.StackTrace, "Ok");
                }
            }
        }

        if (GUILayout.Button("Import Parts"))
        {
            if (Interroot == "")
            {
                EditorUtility.DisplayDialog("Import failed", "Please select the DS3 exe for your interroot directory.", "Ok");
            }
            else
            {
                try
                {
                    ImportParts(Interroot + $@"\parts", type);
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Import failed: " + e.Message, e.StackTrace, "Ok");
                }
            }
        }

        if (type == GameType.Bloodborne && GUILayout.Button("Import Chalice Assets"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne/m29_00_00_00"))
            {
                AssetDatabase.CreateFolder("Assets/Bloodborne", "m29_00_00_00");
            }
            // Load low res hkx assets
            AssetDatabase.StartAssetEditing();
            if (LoadHighResCol)
            {
                if (File.Exists(Interroot + $@"\map\m29_00_00_00\h29_00_00_00.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\m29_00_00_00\h29_00_00_00.hkxbhd", $@"Assets/Bloodborne/m29_00_00_00", type);
                }
            }
            else
            {
                if (File.Exists(Interroot + $@"\map\m29_00_00_00\l29_00_00_00.hkxbhd"))
                {
                    ImportCollisionHKXBDT(Interroot + $@"\map\m29_00_00_00\l29_00_00_00.hkxbhd", $@"Assets/Bloodborne/m29_00_00_00", type);
                }
            }
            AssetDatabase.StopAssetEditing();

            // Import all the map piece meshes
            try
            {
                string path = Interroot + $@"\map\m29_00_00_00";
                var files = Directory.GetFiles(path, "*.flver.dcx");
                AssetDatabase.StartAssetEditing();
                foreach (var mappiece in files)
                {
                    var assetname = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(mappiece));
                    if (AssetDatabase.FindAssets($@"Assets/Bloodborne/m29_00_00_00/{assetname}.prefab").Length == 0 && LoadMapFlvers)
                    {
                        if (File.Exists(mappiece))
                        {
                            try
                            {
                                FlverUtilities.ImportFlver(GameType.Bloodborne, mappiece, $@"Assets/Bloodborne/m29_00_00_00/{assetname}", $@"Assets/Bloodborne/m29");
                            }
                            catch
                            {
                                Debug.LogError(assetname + " failed model import");
                            }
                        }
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        if (type == GameType.Bloodborne && GUILayout.Button("Import Chalice Navimeshes"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne/m29_00_00_00"))
            {
                AssetDatabase.CreateFolder("Assets/Bloodborne", "m29_00_00_00");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Bloodborne/m29_00_00_00/navmesh"))
            {
                AssetDatabase.CreateFolder("Assets/Bloodborne/m29_00_00_00", "navmesh");
            }
            // Load low res hkx assets
            AssetDatabase.StartAssetEditing();
            if (File.Exists(Interroot + $@"\map\m29_00_00_00\m29_00_00_00.nvmhktbnd.dcx"))
            {
                ImportNavmeshHKXBND(Interroot + $@"\map\m29_00_00_00\m29_00_00_00.nvmhktbnd.dcx", $@"Assets/Bloodborne/m29_00_00_00/navmesh", type);
            }
            AssetDatabase.StopAssetEditing();
        }

        if (type == GameType.DarkSoulsIII && GUILayout.Button("Import Map Navimeshes") && GameObject.Find("MSBAssetLink") != null)
        {
            var assetLink = GameObject.Find("MSBAssetLink");
            var alc = assetLink.GetComponent<MSBAssetLink>();
            var mapid = alc.MapID;

            if (!AssetDatabase.IsValidFolder($@"Assets/DS3/{mapid}"))
            {
                AssetDatabase.CreateFolder("Assets/DS3", mapid);
            }
            if (!AssetDatabase.IsValidFolder($"Assets/DS3/{mapid}/navmesh"))
            {
                AssetDatabase.CreateFolder($"Assets/DS3/{mapid}", "navmesh");
            }
            // Load low res hkx assets
            AssetDatabase.StartAssetEditing();
            if (File.Exists(Interroot + $@"\map\{mapid}\{mapid}.nvmhktbnd.dcx"))
            {
                ImportNavmeshHKXBND(Interroot + $@"\map\{mapid}\{mapid}.nvmhktbnd.dcx", $@"Assets/DS3/{mapid}/navmesh", type);
            }
            AssetDatabase.StopAssetEditing();
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

        if (type == GameType.Bloodborne)
        {
            ChaliceID = GUILayout.TextField(ChaliceID);
            if (GUILayout.Button("Import Chalice Map"))
            {
                onImportBBMap(ChaliceID, true);
            }
        }

        if ((type == GameType.DarkSoulsIII || type == GameType.Sekiro || type == GameType.DarkSoulsIISOTFS) && GameObject.Find("MSBAssetLink") != null)
        {
            if (GUILayout.Button("Import BTL (Lights)"))
            {
                GenericMenu menu = new GenericMenu();
                var assetLink = GameObject.Find("MSBAssetLink");
                var alc = assetLink.GetComponent<MSBAssetLink>();
                var mapid = alc.MapID;
                if (type == GameType.DarkSoulsIISOTFS)
                {
                    menu.AddItem(new GUIContent("light"), false, onImportBtl, "light");
                }
                else
                {
                    var btlFiles = Directory.GetFileSystemEntries(Interroot + $@"\map\{mapid}\", @"*.btl.dcx")
                        .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension);
                    var btls = new List<string>();
                    foreach (var btl in btlFiles)
                    {
                        menu.AddItem(new GUIContent(btl), false, onImportBtl, btl);
                    }
                }
                menu.ShowAsContext();
            }
        }

        if (type == GameType.DarkSoulsIII)
        {
            PreservePartsPose = GUILayout.Toggle(PreservePartsPose, "Preserve parts pose (will restore from backup if needed)");
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

        if ((type == GameType.DarkSoulsIII || type == GameType.Sekiro || type == GameType.DarkSoulsIISOTFS) && GUILayout.Button("Export BTLs (lights)"))
        {
            try
            {
                ExportBTLs();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("BTL Export failed: " + e.Message, e.StackTrace, "Ok");
            }
        }
    }
}