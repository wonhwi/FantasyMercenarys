using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PartnerSkillCraftConditionTable : LazySingleton<PartnerSkillCraftConditionTable>, ITableFactory
{
  public Dictionary<int, List<PartnerSkillCraftConditionData>> dictCraftConditionData = new Dictionary<int, List<PartnerSkillCraftConditionData>>();

  public void Load()
  {
    dictCraftConditionData.Clear();
    string craftConditionDataJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.PartnerSkillCraftCondition, out craftConditionDataJson))
    {
      List<PartnerSkillCraftConditionData> craftConditionDataList = JsonConvert.DeserializeObject<List<PartnerSkillCraftConditionData>>(craftConditionDataJson);
      foreach (var data in craftConditionDataList)
      {
        if (!dictCraftConditionData.ContainsKey(data.craftType))
        {
          dictCraftConditionData.Add(data.craftType, new List<PartnerSkillCraftConditionData>());
        }

        dictCraftConditionData[data.craftType].Add(data);

      }
      Debug.Log("CraftCondition Table Load Success");
    }
  }

  /// <summary>
  /// 해당 제작 타입의 가장 첫 번째 데이터 출력
  /// 해당 슬롯에 해당하는 아이템이 존재하지 않을시 데이터 출력하는 근거로 사용
  /// </summary>
  /// <param name="itemGroup"></param>
  /// <param name="itemgGradeType"></param>
  /// <returns></returns>
  public PartnerSkillCraftConditionData GetCraftConditionDefaultData(ItemGroup itemGroup)
  {
    if (dictCraftConditionData.ContainsKey((int)itemGroup))
      return dictCraftConditionData[(int)itemGroup].FirstOrDefault();
    else
      return default;

  }

  public PartnerSkillCraftConditionData GetCraftConditionData(ItemGroup itemGroup, ItemGradeType itemgGradeType)
  {
    if (dictCraftConditionData.ContainsKey((int)itemGroup))
      return dictCraftConditionData[(int)itemGroup].Find(n => n.craftGrade == (int)itemgGradeType);
    else
      return default;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
