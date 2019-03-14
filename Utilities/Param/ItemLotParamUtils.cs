using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

/// <summary>
/// Generic item lot param class that can be used for all the games
/// </summary>
class ItemLotParam
{
    public int[] ItemID = new int[8];
    public uint[] ItemCategory = new uint[8];
    public short[] ItemBasePoint = new short[8];
    public short[] ItemCumulatePoint = new short[8];
    public int[] GetItemFlagID = new int[8];
    public int GetItemFlagIDG = -1;
    public int CumulateNumFlagID = -1;
    public byte CumulateMaxNum = 0;
    public byte ItemRarity = 0;
    public byte[] ItemNum = new byte[8];
    public bool[] EnableLuck = new bool[8];
    public bool[] CumulateReset = new bool[8];
    public sbyte ClearCount = -1;

    public ItemLotParam()
    {

    }

    /// <summary>
    /// Initialize from DS3 item lot param
    /// </summary>
    /// <param name="lot"></param>
    public ItemLotParam(PARAM64.Row lot)
    {
        for (int i = 0; i < 8; i++)
        {
            ItemID[i] = (int)lot[$@"ItemLotId{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            ItemCategory[i] = (uint)lot[$@"LotItemCategory0{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            ItemBasePoint[i] = (short)lot[$@"LotItemBasePoint0{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            ItemCumulatePoint[i] = (short)lot[$@"cumulateLotPoint0{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            GetItemFlagID[i] = (int)lot[$@"GetItemFlagId0{i + 1}"].Value;
        }

        GetItemFlagIDG = (int)lot["getItemFlagId"].Value;
        CumulateNumFlagID = (int)lot["cumulateNumFlagId"].Value;
        CumulateMaxNum = (byte)lot["cumulateNumMax"].Value;
        ItemRarity = (byte)lot["LotItemRarity"].Value;

        for (int i = 0; i < 8; i++)
        {
            ItemNum[i] = (byte)lot[$@"LotItemNum{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            EnableLuck[i] = (bool)lot[$@"EnableLuck0{i + 1}"].Value;
        }

        for (int i = 0; i < 8; i++)
        {
            CumulateReset[i] = (bool)lot[$@"cumulateReset0{i + 1}"].Value;
        }

        ClearCount = (sbyte)lot["ClearCount"].Value;
    }
}

/// <summary>
/// Class that can load, lookup, modify, and save item lot params. Used as a base for item
/// editing
/// </summary>
class ItemLotParamUtils
{
    private static PARAM64 DS3Param = null;
    private static DarkSoulsTools.GameType GameType = DarkSoulsTools.GameType.Undefined;
    private static string ParamPath = "";

    public static List<Tuple<int, string>> ItemNameList = null;

    public static void ReloadParams()
    {
        BND4 paramBnd = SFUtil.DecryptDS3Regulation(DarkSoulsTools.GetOverridenPath(ParamPath));
        DS3Param = PARAM64.Read(paramBnd.Files.Find(x => Path.GetFileName(x.Name) == "ItemLotParam.param").Bytes);
        PARAM64.Layout layout = PARAM64.Layout.ReadXMLFile($@"{Application.dataPath.Replace('/', '\\')}\dstools\ParamLayouts\DS3\{DS3Param.ID}.xml");
        DS3Param.SetLayout(layout);

        // Build and cache the item name list
        HashSet<int> usedItemIds = new HashSet<int>();
        ItemNameList = new List<Tuple<int, string>>();
        foreach (var row in DS3Param.Rows)
        {
            ItemLotParam param = new ItemLotParam(row);
            foreach (int id in param.ItemID)
            {
                if (!usedItemIds.Contains(id))
                {
                    usedItemIds.Add(id);
                    ItemNameList.Add(new Tuple<int, string>(id, FMGUtils.LookupItemName(id)));
                }
            }
        }
        ItemNameList.Sort((a, b) => StringComparer.InvariantCulture.Compare(a.Item2, b.Item2));
    }

    public static void LoadParams(DarkSoulsTools.GameType gameType, string paramPath)
    {
        if (gameType != DarkSoulsTools.GameType.DarkSoulsIII)
        {
            return;
        }

        GameType = gameType;
        ParamPath = paramPath;
        ReloadParams();
    }

    public static void SaveParams()
    {
        if (GameType != DarkSoulsTools.GameType.DarkSoulsIII || DS3Param == null)
        {
            return;
        }

        BND4 paramBnd = SFUtil.DecryptDS3Regulation(DarkSoulsTools.GetOverridenPath(ParamPath));
        var param = paramBnd.Files.Find(x => Path.GetFileName(x.Name) == "ItemLotParam.param");
        param.Bytes = DS3Param.Write();

        // Save a backup if one doesn't exist
        if (!File.Exists(ParamPath + ".backup"))
        {
            File.Copy(ParamPath, ParamPath + ".backup");
        }

        string paramPath = ParamPath;
        if (DarkSoulsTools.GetModProjectPathForFile(ParamPath) != null)
        {
            paramPath = DarkSoulsTools.GetModProjectPathForFile(ParamPath);
        }

        // Write as a temporary file to make sure there are no errors before overwriting current file 
        if (File.Exists(paramPath + ".temp"))
        {
            File.Delete(paramPath + ".temp");
        }
        SFUtil.EncryptDS3Regulation(paramPath + ".temp", paramBnd);

        // Make a copy of the previous map
        File.Copy(paramPath, paramPath + ".prev", true);

        // Move temp file as new map file
        File.Delete(paramPath);
        File.Move(paramPath + ".temp", paramPath);
    }

    public static ItemLotParam LookupItemLot(int ID)
    {
        if (DS3Param == null)
            return null;

        var row = DS3Param.Rows.Find(r => r.ID == ID);
        if (row != null)
        {
            return new ItemLotParam(row);
        }
        return null;
    }

    public static void UpdateItemLot(int ID, ItemLotParam lot)
    {
        var row = DS3Param.Rows.Find(r => r.ID == ID);
        if (row != null)
        {
            for (int i = 0; i < 8; i++)
            {
                row[$@"ItemLotId{i + 1}"].Value = lot.ItemID[i];
            }
        }
    }
}
