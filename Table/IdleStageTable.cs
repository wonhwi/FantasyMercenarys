using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IdleStageTable : LazySingleton<IdleStageTable>, ITableFactory
{
    public Dictionary<int, IdleStageData> dictStageData = new Dictionary<int, IdleStageData>();

    private const int CHAPTER_STAGE_COUNT = 10;

    public void Load()
    {
        dictStageData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_STAGE_FILE_NAME;

        ////int index = 0;
        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<IdleStageData> stageDataList = JsonConvert.DeserializeObject<List<IdleStageData>>(fileData);                
        //        foreach (var stageData in stageDataList)
        //        {
        //            if(!dictStageData.ContainsKey(stageData.stageIdx))
        //            {
        //                dictStageData.Add(stageData.stageIdx, stageData);
        //            }
        //        }                
        //    }
        //}
        string stageJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.StageInfo, out stageJson))
        {
            List<IdleStageData> stageDataList = JsonConvert.DeserializeObject<List<IdleStageData>>(stageJson);
            foreach (var data in stageDataList)
            {
                if (!dictStageData.ContainsKey(data.stageIdx))
                {
                    dictStageData.Add(data.stageIdx, data);
                }
                else
                    Debug.Log("Stage Table Load Error Index : " + data.stageIdx);
            }
            Debug.Log("Stage Table Load Success");
        }
    }

    public IdleStageData GetStageData(int _key)
    {
        if (dictStageData.ContainsKey(_key))
        {
            return dictStageData[_key];
        }
        else
        {
            Debug.Log($"No Stage Data key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
