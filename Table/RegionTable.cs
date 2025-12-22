using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RegionTable : LazySingleton<RegionTable>, ITableFactory
{
    public Dictionary<int, RegionData> dictRegionData = new Dictionary<int, RegionData>();

    public void Load()
    {
        dictRegionData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_REGION_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<RegionData> regionDataList = JsonConvert.DeserializeObject<List<RegionData>>(fileData);
        //        foreach (var regionData in regionDataList)
        //        {
        //            if (!dictRegionData.ContainsKey(regionData.regionIdx))
        //            {
        //                dictRegionData.Add(regionData.regionIdx, regionData);
        //            }
        //        }
        //    }
        //}
        string regionJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.RegionInfo, out regionJson))
        {
            List<RegionData> regionDataList = JsonConvert.DeserializeObject<List<RegionData>>(regionJson);
            foreach (var data in regionDataList)
            {
                if (!dictRegionData.ContainsKey(data.regionIdx))
                {
                    dictRegionData.Add(data.regionIdx, data);
                }
                else
                    Debug.Log("Region Table Load Error Index : " + data.regionIdx);
            }
            Debug.Log("Region Table Load Success");
        }
    }
    
    public RegionData GetRegionData(int _key)
    {
        if (dictRegionData.ContainsKey(_key))
        {
            return dictRegionData[_key];
        }
        else
        {
            Debug.Log($"No Region Data Key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
