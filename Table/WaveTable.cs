using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WaveTable : LazySingleton<WaveTable>, ITableFactory
{
    public Dictionary<int, WaveData> dictWaveData = new Dictionary<int, WaveData>();

    public void Load()
    {
        dictWaveData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_WAVE_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<WaveData> waveDataList = JsonConvert.DeserializeObject<List<WaveData>>(fileData);
        //        foreach (var waveData in waveDataList)
        //        {
        //            if (!dictWaveData.ContainsKey(waveData.waveType))
        //            {
        //                dictWaveData.Add(waveData.waveType, waveData);
        //            }
        //        }
        //    }
        //}
        string waveJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.WaveInfo, out waveJson))
        {
            List<WaveData> waveDataList = JsonConvert.DeserializeObject<List<WaveData>>(waveJson);
            foreach (var data in waveDataList)
            {
                if (!dictWaveData.ContainsKey(data.waveType))
                {
                    dictWaveData.Add(data.waveType, data);
                }
                else
                    Debug.Log("Wave Table Load Error Type : " + data.waveType);
            }
            Debug.Log("Wave Table Load Success");
        }
    }

    public WaveData GetWaveData(int _key)
    {
        if (dictWaveData.ContainsKey(_key))
        {
            return dictWaveData[_key];
        }
        else
        {
            Debug.Log($"No Wave Data key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
