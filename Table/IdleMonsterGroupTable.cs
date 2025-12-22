using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IdleMonsterGroupTable : LazySingleton<IdleMonsterGroupTable>, ITableFactory
{
    public Dictionary<int, List<MonsterGroupData>> dictMonsterGroupData = new Dictionary<int, List<MonsterGroupData>>();

    public void Load()
    {
        dictMonsterGroupData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_MONSTER_GROUP_FILE_NAME;

        int index = 0;
        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<MonsterGroupData> monsterGroupDataList = JsonConvert.DeserializeObject<List<MonsterGroupData>>(fileData);
        //        foreach (var monsterGroupData in monsterGroupDataList)
        //        {
        //            if(!dictMonsterGroupData.ContainsKey(monsterGroupData.monsterGroupIdx))
        //            {
        //                List<MonsterGroupData> datas = new List<MonsterGroupData>();
        //                for (int i = index; i < monsterGroupDataList.Count; i++)
        //                {
        //                    if(datas.Count > 0)
        //                    {
        //                        if(datas[0].monsterGroupIdx != monsterGroupDataList[i].monsterGroupIdx)
        //                        {
        //                            index = i;
        //                            break;
        //                        }                                         
        //                    }
        //                    datas.Add(monsterGroupDataList[i]);
        //                }
        //                dictMonsterGroupData.Add(monsterGroupData.monsterGroupIdx, datas);
        //            }
        //        }
        //    }
        //}
        string monsterGroupJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.MonsterGroup, out monsterGroupJson))
        {
            List<MonsterGroupData> monsterGroupDataList = JsonConvert.DeserializeObject<List<MonsterGroupData>>(monsterGroupJson);
            foreach (var data in monsterGroupDataList)
            {
                if (!dictMonsterGroupData.ContainsKey(data.monsterGroupIdx))
                {
                    List<MonsterGroupData> datas = new List<MonsterGroupData>();
                    for (int i = index; i < monsterGroupDataList.Count; i++)
                    {
                        if (datas.Count > 0)
                        {
                            if (datas[0].monsterGroupIdx != monsterGroupDataList[i].monsterGroupIdx)
                            {
                                index = i;
                                break;
                            }
                        }
                        datas.Add(monsterGroupDataList[i]);
                    }
                    dictMonsterGroupData.Add(data.monsterGroupIdx, datas);
                }                
            }
            Debug.Log("MonsterGroup Table Load Success");
        }
    }

    public List<MonsterGroupData> GetMonsterGroupData(int _key)
    {
        if (dictMonsterGroupData.ContainsKey(_key))
        {
            return dictMonsterGroupData[_key];
        }
        else
        {
            Debug.Log($"No MonsterGroup Data key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }    
}
