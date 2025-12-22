using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EquipmentItemTable : LazySingleton<EquipmentItemTable>, ITableFactory
{
  public Dictionary<int, EquipmentItemData> dictEquipmentItemData = new Dictionary<int, EquipmentItemData>();

  public List<EquipmentItemData> costumeWeaponList = new List<EquipmentItemData>();
  public List<EquipmentItemData> costumeHatList = new List<EquipmentItemData>();
  public List<EquipmentItemData> costumeGlassesList = new List<EquipmentItemData>();
  public List<EquipmentItemData> costumeClothesList = new List<EquipmentItemData>();

  #region 장비 랜덤 스탯
  private Dictionary<int, List<MetaEquipRandomStatGroup>> dictEquipRandomStat = new Dictionary<int, List<MetaEquipRandomStatGroup>>();
  private Dictionary<int, int> dictTotalRandomGroupWeight = new Dictionary<int, int>(); //랜덤 그룹별 추가 스탯 전체 확률값
  #endregion
  public void Load()
  {
    dictEquipmentItemData.Clear();

    costumeWeaponList.Clear();
    costumeHatList.Clear();
    costumeGlassesList.Clear();
    costumeClothesList.Clear();

    int index = 0;
    int weight = 0;

    string equipmentItemJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Equipment, out equipmentItemJson))
    {
      List<EquipmentItemData> equipmentItemDataList = JsonConvert.DeserializeObject<List<EquipmentItemData>>(equipmentItemJson);
      foreach (var data in equipmentItemDataList)
      {
        if (!dictEquipmentItemData.ContainsKey(data.equipItemIdx))
        {
          dictEquipmentItemData.Add(data.equipItemIdx, data);
          InitCostumeListData(data);
        }
        else
          Debug.Log("EquipmentItemTable Table Load Error Index : " + data.equipItemIdx);
      }
      Debug.Log("EquipmentItemTable Table Load Success");
    }

    string equipmentRandomStatJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.MetaEquipRandomStatGroup, out equipmentRandomStatJson))
    {
      List<MetaEquipRandomStatGroup> equipRandomStatGroupList = JsonConvert.DeserializeObject<List<MetaEquipRandomStatGroup>>(equipmentRandomStatJson);
      foreach (var data in equipRandomStatGroupList)
      {
        if (!dictEquipRandomStat.ContainsKey(data.randomStatGroupIdx))
        {
          List<MetaEquipRandomStatGroup> datas = new List<MetaEquipRandomStatGroup>();
          for (int i = index; i < equipRandomStatGroupList.Count; i++)
          {
            if (datas.Count > 0)
            {
              if (datas[0].randomStatGroupIdx != equipRandomStatGroupList[i].randomStatGroupIdx)
              {
                index = i;
                break;
              }
            }
            datas.Add(equipRandomStatGroupList[i]);
          }
          dictEquipRandomStat.Add(data.randomStatGroupIdx, datas);
          for (int i = 0; i < datas.Count; i++)
          {
            weight += datas[i].weight;
          }
          dictTotalRandomGroupWeight.Add(data.randomStatGroupIdx, weight);
          weight = 0;
        }
      }
      Debug.Log("EquipRandomStatGroup Table Load Success");
    }
  }

  public Dictionary<int, EquipmentItemData> GetEquipmentItemDic()
  {
    return dictEquipmentItemData;
  }


  public List<EquipmentItemData> GetCostumeTargetList(EquipmentMountType mountType) => mountType switch
  {
    EquipmentMountType.Weapon  => costumeWeaponList,
    EquipmentMountType.Helmet  => costumeHatList,
    EquipmentMountType.Glasses => costumeGlassesList,
    EquipmentMountType.Armor   => costumeClothesList,
    _ => null
  };

  public EquipmentItemData GetEquipmentItemData(int _idx)
  {
    if (dictEquipmentItemData.ContainsKey(_idx))
      return dictEquipmentItemData[_idx];
    else
    {
      Debug.Log($"Not Found EquipmentItem : {_idx}");
      return default;
    }
  }

  private void InitCostumeListData(EquipmentItemData _data)
  {
    if (_data.isCostume)
    {
      GetCostumeTargetList((EquipmentMountType)_data.mountType).Add(_data);
    }
  }
  public void Reload()
  {
    throw new System.NotImplementedException();
  }

  public InvenData RewardEquipInfo(int itemIdx, int itemLv)
  {
    MNMRandom random = GameDataManager.getInstance.GetLampRandom();
    int itemGroupKey = ItemTable.getInstance.GetItemData(itemIdx).itemGroup;

    if (dictEquipmentItemData.TryGetValue(itemIdx, out EquipmentItemData item))
    {
      InvenData invenData = new InvenData();
      invenData.itemIdx = itemIdx;
      invenData.itemLv = itemLv;
      invenData.itemGroup = itemGroupKey;
      invenData.itemCount = 1;
      invenData.statDataList = new List<StatData>();

      Debug.LogError($"아이템 레벨 : {itemLv}");

      //기본 스탯 획득
      SetDefaultStat(invenData);

      //추가 스탯 획득
      int addStatCount = random.Next(item.randomStatMin, item.randomStatMax);

      SetAddStat(invenData, item.randomStatGroupIdx, addStatCount);

      return invenData;
    }
    return null;
  }


  /// <summary>
  /// EquipmentTable에 있는 StatType (기본 스텟 기반 데이터 설정)
  /// </summary>
  /// <param name="invenData"></param>
  public void SetDefaultStat(InvenData invenData)
  {
    EquipmentItemData equipmentItemData = GetEquipmentItemData(invenData.itemIdx);

    StatType statType1 = (StatType)equipmentItemData.statType1;
    StatType statType2 = (StatType)equipmentItemData.statType2;
    StatType statType3 = (StatType)equipmentItemData.statType3;
    
    float statValu1 = equipmentItemData.statValue1;
    float statValu2 = equipmentItemData.statValue2;
    float statValu3 = equipmentItemData.statValue3;

    SetStat(invenData, statType1, statValu1);
    SetStat(invenData, statType2, statValu2);
    SetStat(invenData, statType3, statValu3);
  }


  public void SetAddStat(InvenData invenData, int randomStatGroupIdx, int addStatCount)
  {
    MNMRandom random = GameDataManager.getInstance.GetLampRandom();

    var statGroupList = dictEquipRandomStat[randomStatGroupIdx];
    int totalWeight   = dictTotalRandomGroupWeight[randomStatGroupIdx];

    //중복 방지용 List 변수
    List<int> statTypeList = new List<int>();

    //추가 Stat 갯수 만큼 반복
    for (int i = 0; i < addStatCount; i++)
    {
      float cumulativeWeight = 0;

      //체력, 공격력, 방어도 추가 스텟에 들어가야하기때문에 60 -> 0으로 변경
      int randomValue = random.Next(0, totalWeight);

      foreach (var stat in statGroupList)
      {
        //이미 스텟을 보유중이면 Continue
        if (statTypeList.Contains(stat.statType))
          continue;

        cumulativeWeight += stat.weight;

        if (randomValue <= cumulativeWeight)
        {
          float statValue = stat.statBaseValue;

          int marginValue = (int)stat.marginValue;               //오차 범위
          int randomMargin = random.Next(-marginValue, marginValue);        //랜덤 오차 범위 설정
          float offsetPer = (1f + randomMargin / 100f);

          statValue *= offsetPer;

          SetStat(invenData, (StatType)stat.statType, statValue);

          statTypeList.Add(stat.statType);    //중복 처리 방지 획득한 스텟 변수에 추가
          totalWeight -= stat.weight;         //weight 보정
          break;
        }
      }
    }
  }

  public void SetStat(InvenData invenData, StatType statType, float statValue)
  {
    MNMRandom random = GameDataManager.getInstance.GetLampRandom();

    int itemLv = invenData.itemLv;                                         //장비 레벨
    float statPer = BaseTable.getInstance.GetBaseValue((int)statType);     //장비 스텟 가중치

    float statValueResult = statValue + (statValue * itemLv * statPer);   //최종 스텟 결과 값

    StatData statData = new StatData();

    statData.statType = (int)statType;
    statData.statValue = StatUtility.GetStatValue(statType, statValueResult);

    Debug.LogError($"최종 스텟 값 {statValueResult} : {statData.statValue}");

    invenData.statDataList.Add(statData);
  }

}
