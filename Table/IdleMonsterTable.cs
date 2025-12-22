using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IdleMonsterTable : LazySingleton<IdleMonsterTable>, ITableFactory
{
    public Dictionary<int, MonsterData> dictMonsterData = new Dictionary<int, MonsterData>();


    public void Load()
    {
        dictMonsterData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_MONSTER_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<MonsterData> monsterDataList = JsonConvert.DeserializeObject<List<MonsterData>>(fileData);
        //        foreach (var monster in monsterDataList)
        //        {
        //            if (!dictMonsterData.ContainsKey(monster.monsterIdx))
        //            {
        //                dictMonsterData.Add(monster.monsterIdx, monster);
        //            }
        //        }
        //    }
        //}
        string monsterJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Monster, out monsterJson))
        {
            List<MonsterData> monsterDataList = JsonConvert.DeserializeObject<List<MonsterData>>(monsterJson);
            foreach (var data in monsterDataList)
            {
                if (!dictMonsterData.ContainsKey(data.monsterIdx))
                {
                    dictMonsterData.Add(data.monsterIdx, data);
                }
                else
                    Debug.Log("Monster Table Load Error Index : " + data.monsterIdx);
            }
            Debug.Log("Monster Table Load Success");
        }
    }

    public MonsterData GetMonsterData(int _key)
    {
        if (dictMonsterData.ContainsKey(_key))
        {
            return dictMonsterData[_key];
        }
        else
        {
            Debug.Log($"No Monster Data Key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
