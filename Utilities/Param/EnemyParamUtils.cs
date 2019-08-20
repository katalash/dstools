using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

/// <summary>
/// Class that can load, lookup, modify, and save item lot params. Used as a base for item
/// editing
/// </summary>
class EnemyParamUtils
{
    private static PARAM EnemyParam = null;
    private static string ParamPath = "";
    private static Dictionary<long, PARAM.Row> ParamDictionary;

    public static void ReloadParams()
    {
        if (!File.Exists(ParamPath))
        {
            Debug.Log("DS2 enc_regulation.bnd.dcx not found");
            return;
        }
        if (!BND4.Is(DarkSoulsTools.GetOverridenPath(ParamPath)))
        {
            Debug.Log("Decrypt your regulation by saving in Yapped");
            return;
        }
        BND4 paramBnd = BND4.Read(DarkSoulsTools.GetOverridenPath(ParamPath));
        EnemyParam = PARAM.Read(paramBnd.Files.Find(x => Path.GetFileName(x.Name) == "EnemyParam.param").Bytes);
        PARAM.Layout layout = PARAM.Layout.ReadXMLFile($@"{Application.dataPath.Replace('/', '\\')}\dstools\ParamLayouts\DS2SOTFS\{EnemyParam.ID}.xml");
        EnemyParam.SetLayout(layout);

        // Build and cache the enemy param dictionary
        ParamDictionary = new Dictionary<long, PARAM.Row>();
        foreach (var row in EnemyParam.Rows)
        {
            ParamDictionary.Add(row.ID, row);
        }
    }

    public static void LoadParams(string paramPath)
    {
        ParamPath = paramPath;
        ReloadParams();
    }

    public static string GetChrIDForEnemy(long enemyID)
    {
        if (EnemyParam != null)
        {
            if (ParamDictionary.ContainsKey(enemyID))
            {
                return $@"{ParamDictionary[enemyID]["ChrID"].Value:D4}";
            }
        }
        return null;
    }
}
