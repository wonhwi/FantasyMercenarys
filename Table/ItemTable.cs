using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemTable : LazySingleton<ItemTable>, ITableFactory
{
    public Dictionary<int, ItemData> dictItemData = new Dictionary<int, ItemData>();

    public void Load()
    {
        dictItemData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_ITEM_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(fileData);
        //        foreach (var itemData in itemDataList)
        //        {
        //            if (!dictItemData.ContainsKey(itemData.itemIdx))
        //            {
        //                dictItemData.Add(itemData.itemIdx, itemData);
        //            }
        //        }
        //    }
        //}
        string itemJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Item, out itemJson))
        {
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(itemJson);
            foreach (var data in itemDataList)
            {
                if (!dictItemData.ContainsKey(data.itemIdx))
                {
                    dictItemData.Add(data.itemIdx, data);
                }
                else
                    Debug.Log("Item Table Load Error Index : " + data.itemIdx);
            }
            Debug.Log("Item Table Load Success");
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }

  public ItemData GetItemData(int itemIdx)
  {
    if(dictItemData.ContainsKey(itemIdx))
    {
      return dictItemData[itemIdx];
    }

    return default;
  }

  public int GetItemGrade(int itemIdx)
  {
    if (dictItemData.ContainsKey(itemIdx))
    {
      return dictItemData[itemIdx].itemGrade;
    }

    Debug.Log($"해당 아이템의 정보가 없습니다 : {itemIdx}");

    return -1;
  }
}
