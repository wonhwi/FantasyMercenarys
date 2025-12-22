using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PartnerInsightTable : LazySingleton<PartnerInsightTable>, ITableFactory
{
  public Dictionary<int, List<PartnerInsightData>> dictPartnerInsightData = new Dictionary<int, List<PartnerInsightData>>();

  public void Load()
  {
    dictPartnerInsightData.Clear();
    string partnerInsightDataJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.PartnerInsight, out partnerInsightDataJson))
    {
      List<PartnerInsightData> partnetInsightDataList = JsonConvert.DeserializeObject<List<PartnerInsightData>>(partnerInsightDataJson);
      foreach (var data in partnetInsightDataList)
      {
        if (!dictPartnerInsightData.ContainsKey(data.partnerGrade))
        {
          dictPartnerInsightData.Add(data.partnerGrade, new List<PartnerInsightData>());
        }

        dictPartnerInsightData[data.partnerGrade].Add(data);

      }
      Debug.Log("PartnerInsight Table Load Success");
    }
  }

  public PartnerInsightData GetPartnerInsightData(ItemGradeType itemGradeType, int itemLv)
  {
    List<PartnerInsightData> insightDataList = dictPartnerInsightData[(int)itemGradeType];

    if(insightDataList == null)
    {
      Debug.Log($"{itemGradeType} 등급의 InSight 정보가 없습니다.");
      return default;
    }

    for (int i = 0; i < insightDataList.Count; i++)
    {
      PartnerInsightData partnerInsightData = insightDataList[i];

      if(partnerInsightData.partnerMinLv <= itemLv && itemLv <= partnerInsightData.partnerMaxLv)
      {
        return partnerInsightData;
      }
    }

    Debug.Log($"{itemGradeType} 등급과 {itemLv}을 충족하는 데이터가 없습니다.");
    return default;

  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
