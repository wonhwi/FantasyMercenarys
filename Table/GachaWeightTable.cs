using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GachaWeightTable : LazySingleton<GachaWeightTable>, ITableFactory
{
  public Dictionary<ShopLvIndex, int> dictGachaMaxLevelData = new Dictionary<ShopLvIndex, int>();
  public Dictionary<int, List<GachaWeightData>> dictGachaWeightData = new Dictionary<int, List<GachaWeightData>>();

  public void Load()
  {
    dictGachaWeightData.Clear();

    //Gacha 레벨 최대치 Dictionary 데이터 초기화
    foreach (ShopLvIndex key in System.Enum.GetValues(typeof(ShopLvIndex)))
    {
      if (!dictGachaMaxLevelData.ContainsKey(key)) // 중복 방지
      {
        dictGachaMaxLevelData.Add(key, 0);
      }
    }

    string gachaWeightJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.GachaWeight, out gachaWeightJson))
    {
      List<GachaWeightData> gachaWeightDataList = JsonConvert.DeserializeObject<List<GachaWeightData>>(gachaWeightJson);
      foreach (var data in gachaWeightDataList)
      {
        if (!dictGachaWeightData.ContainsKey(data.shopLvIdx))
          dictGachaWeightData[data.shopLvIdx] = new List<GachaWeightData>();


        dictGachaWeightData[data.shopLvIdx].Add(data);


        //Gacha별 MaxLevel 설정
        foreach (ShopLvIndex level in System.Enum.GetValues(typeof(ShopLvIndex)))
        {
          if(data.shopLvIdx - (int)level < 1000)
          {
            dictGachaMaxLevelData[level] = data.shopLvIdx - (int)level;
          }
        }

      }
      Debug.Log("GachaWeight Table Load Success");
    }
  }

  public int GetGachaMaxLevel(ShopCategoryType shopCategoryType)
  {
    ShopLvIndex shopLvIndex = GetShopLvIndex(shopCategoryType);

    return dictGachaMaxLevelData[shopLvIndex];
  }

  /// <summary>
  /// 상점 타입에 따른 StartIndex 반환
  /// </summary>
  /// <param name="shopCategoryType"></param>
  /// <returns></returns>
  public ShopLvIndex GetShopLvIndex(ShopCategoryType shopCategoryType) => shopCategoryType switch
  {
    ShopCategoryType.Skill            => ShopLvIndex.Skill,
    ShopCategoryType.Partner          => ShopLvIndex.Partner,
    ShopCategoryType.Equipment        => ShopLvIndex.Equipment,
    ShopCategoryType.BlessStatue      => ShopLvIndex.BlessStatue,
  };


  public List<GachaWeightData> GetGachaWeightDataList(int shopIndex)
  {
    return dictGachaWeightData[shopIndex];
  }

  public (int mixGrade, int maxGrade) GetGachaGradeRange(int shopIndex)
  {
    int mixGrade = -1;
    int maxGrade = -1;

    List<GachaWeightData> gachaWeightDataList = GetGachaWeightDataList(shopIndex);

    for (int i = 0; i < gachaWeightDataList.Count; i++)
    {
      GachaWeightData gachaWeightData = gachaWeightDataList[i];

      if (mixGrade == -1 || gachaWeightData.grade < mixGrade) mixGrade = gachaWeightData.grade;
      if (maxGrade == -1 || gachaWeightData.grade > maxGrade) maxGrade = gachaWeightData.grade;

    }

    return (mixGrade, maxGrade);
  }


  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
