using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapTable : LazySingleton<MapTable>, ITableFactory
{
    public Dictionary<int, MapData> dictMapData = new Dictionary<int, MapData>();

    public void Load()
    {
        dictMapData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_MAP_FILE_NAME;

        //if(File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if(fileData != null)
        //    {
        //        List<MapData> mapDataList = JsonConvert.DeserializeObject<List<MapData>>(fileData);
        //        foreach (var mapData in mapDataList)
        //        {
        //            if(!dictMapData.ContainsKey(mapData.mapIdx))
        //            {
        //                dictMapData.Add(mapData.mapIdx, mapData);
        //            }
        //        }
        //    }
        //}
        string mapJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.MapInfo, out mapJson))
        {
            List<MapData> mapDataList = JsonConvert.DeserializeObject<List<MapData>>(mapJson);
            foreach (var data in mapDataList)
            {
                if (!dictMapData.ContainsKey(data.mapIdx))
                {
                    dictMapData.Add(data.mapIdx, data);
                }
                else
                    Debug.Log("Map Table Load Error Index : " + data.mapIdx);
            }
            Debug.Log("Map Table Load Success");
        }
    }

    public MapData GetMapData(int _key)
    {
        if (dictMapData.ContainsKey(_key))
        {
            return dictMapData[_key];
        }
        else
        {
            Debug.Log($"No Map Data key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
