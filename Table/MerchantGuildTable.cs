using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MerchantGuildTable : LazySingleton<MerchantGuildTable>, ITableFactory
{
  public Dictionary<int, MerchantSkillTree> dictSkillTreeData = new Dictionary<int, MerchantSkillTree>();
  public Dictionary<int, List<MerchantSkillLevelup>> dictSkillLevelUpData = new Dictionary<int, List<MerchantSkillLevelup>>();


  public void Load()
  {
    dictSkillTreeData.Clear();

    string skillTreeJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.MerchantSkillTree, out skillTreeJson))
    {
      List<MerchantSkillTree> skillTreeDataList = JsonConvert.DeserializeObject<List<MerchantSkillTree>>(skillTreeJson);
      foreach (var data in skillTreeDataList)
      {
        if (!dictSkillTreeData.ContainsKey(data.skillIdx))
        {
          dictSkillTreeData.Add(data.skillIdx, data);
        }
        else
          Debug.Log("MerchantSkillTree Table Load Error Index : " + data.skillIdx);
      }
      Debug.Log("MerchantSkillTree Table Load Success");
    }

    string skillLevelUpJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.MerchantSkillLevelup, out skillLevelUpJson))
    {
      List<MerchantSkillLevelup> skillLevelUpDataList = JsonConvert.DeserializeObject<List<MerchantSkillLevelup>>(skillLevelUpJson);
      foreach (var data in skillLevelUpDataList)
      {
        if (!dictSkillLevelUpData.ContainsKey(data.skillIdx))
        {
          dictSkillLevelUpData.Add(data.skillIdx, new List<MerchantSkillLevelup>());
        }

        dictSkillLevelUpData[data.skillIdx].Add(data);
      }
      Debug.Log("MerchantSkillLevelup Table Load Success");
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }

  public MerchantSkillTree GetSkillTree(int skillIdx)
  {
    return dictSkillTreeData[skillIdx];
  }

  public MerchantSkillLevelup GetSkillLevelup(int skillIdx, int skillLv)
  {
    List<MerchantSkillLevelup> skillLvUpDataList = dictSkillLevelUpData[skillIdx];

    MerchantSkillLevelup skillLvUpData = new MerchantSkillLevelup();

    for (int i = 0; i < skillLvUpDataList.Count; i++)
    {
      if (skillLvUpDataList[i].skillLevel == skillLv)
      {
        skillLvUpData = skillLvUpDataList[i];
        break;
      }
    }

    return skillLvUpData;
  }

}
