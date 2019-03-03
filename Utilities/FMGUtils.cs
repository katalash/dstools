using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

class FMGUtils
{
    private static DarkSoulsTools.GameType GameType = DarkSoulsTools.GameType.Undefined;
    private static string FMGBndPath = "";
    private static FMG ItemFMG = null;

    public static void ReloadFmgs()
    {
        BND4 fmgBnd = BND4.Read(FMGBndPath);
        ItemFMG = FMG.Read(fmgBnd.Files.Find(x => Path.GetFileName(x.Name) == "アイテム名.fmg").Bytes);
    }

    public static void LoadFmgs(DarkSoulsTools.GameType gameType, string fmgPath)
    {
        if (gameType != DarkSoulsTools.GameType.DarkSoulsIII)
        {
            return;
        }

        GameType = gameType;
        FMGBndPath = fmgPath;
        ReloadFmgs();
    }

    public static string LookupItemName(int itemID)
    {
        if (itemID == 0)
        {
            return "No Item";
        }

        if (ItemFMG == null)
        {
            return "?ItemName?";
        }

        var entry = ItemFMG.Entries.Find(x => x.ID == itemID);
        if (entry != null)
        {
            return entry.Text;
        }
        return "?ItemName?";
    }
}
