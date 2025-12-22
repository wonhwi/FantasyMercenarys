using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharLevelTable : LazySingleton<CharLevelTable>, ITableFactory
{
    public Dictionary<int, CharLevelData> dictCharLevelData = new Dictionary<int, CharLevelData>();

    public void Load()
    {
        dictCharLevelData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_CHAR_LEVEL_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<CharLevelData> charLevelDataList = JsonConvert.DeserializeObject<List<CharLevelData>>(fileData);
        //        foreach (var charLevelData in charLevelDataList)
        //        {
        //            if (!dictCharLevelData.ContainsKey(charLevelData.chLevel))
        //            {
        //                dictCharLevelData.Add(charLevelData.chLevel, charLevelData);
        //            }
        //        }
        //    }
        //}
        string charLevelJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.CharLevel, out charLevelJson))
        {
            List<CharLevelData> charLevelDataList = JsonConvert.DeserializeObject<List<CharLevelData>>(charLevelJson);
            foreach (var data in charLevelDataList)
            {
                if (!dictCharLevelData.ContainsKey(data.chLevel))
                {
                    dictCharLevelData.Add(data.chLevel, data);
                }
                else
                    Debug.Log("charLevel Table Load Error chLevel : " + data.chLevel);
            }
            Debug.Log("charLevel Table Load Success");
        }
    }

    public CharLevelData GetCharLevelData(int playerLv)
    {

    int lv = playerLv;

    if (lv == 0)
      lv = 1;
    

        if(dictCharLevelData.ContainsKey(lv))
        {
            return dictCharLevelData[lv];
        }
        else
        {
            Debug.Log($"No CharLevel Data key : {lv}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }    
}
