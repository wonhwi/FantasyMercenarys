using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundTable : LazySingleton<SoundTable>, ITableFactory
{
  public Dictionary<string, SoundData> dictSoundData = new Dictionary<string, SoundData>();

  public void Load()
  {
    dictSoundData.Clear();

    string soundJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.SoundInfo, out soundJson))
    {
      List<SoundData> soundDataList = JsonConvert.DeserializeObject<List<SoundData>>(soundJson);
      foreach (var data in soundDataList)
      {
        if (!dictSoundData.ContainsKey(data.soundName))
        {
          dictSoundData.Add(data.soundName, data);
        }
        else
          Debug.Log("Sound Table Load Error Index : " + data.soundIdx);
      }
      Debug.Log("Sound Table Load Success");
    }
  }


  public Dictionary<string, SoundData> GetPartnerlDic()
  {
    return dictSoundData;
  }


  public SoundData GetSoundData(string _key)
  {
    if (dictSoundData.ContainsKey(_key))
    {
      return dictSoundData[_key];
    }
    else
    {
      Debug.Log($"No Sound Data key : {_key}");
      return default;
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }

}
