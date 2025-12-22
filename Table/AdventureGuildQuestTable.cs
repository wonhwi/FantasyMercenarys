using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class AdventureGuildQuestTable : LazySingleton<AdventureGuildQuestTable>, ITableFactory
{
  public Dictionary<int, AdventureGuildQuestData> dictAdventureGuildQuestData = new Dictionary<int, AdventureGuildQuestData>();

  public void Load()
  {
    dictAdventureGuildQuestData.Clear();
    string adventureGuildQuestJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.AdventureGuildQuest, out adventureGuildQuestJson))
    {
      List<AdventureGuildQuestData> adventureGuildQuestDataList = JsonConvert.DeserializeObject<List<AdventureGuildQuestData>>(adventureGuildQuestJson);
      foreach (var data in adventureGuildQuestDataList)
      {
        if (!dictAdventureGuildQuestData.ContainsKey(data.guildQuestIdx))
        {
          dictAdventureGuildQuestData.Add(data.guildQuestIdx, data);
        }
        else
          Debug.Log("AdventureGuildQuest Table Load Error Index : " + data.guildQuestIdx);
      }
      Debug.Log("AdventureGuildQuest Table Load Success");
    }
  }

  public AdventureGuildQuestData GetAdventureGuildQuestData(int guildQuestIndex)
  {
    if (dictAdventureGuildQuestData.ContainsKey(guildQuestIndex))
      return dictAdventureGuildQuestData[guildQuestIndex];
    else
      return default;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
