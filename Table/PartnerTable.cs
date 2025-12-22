using Assets.ZNetwork.Data;
using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PartnerTable : LazySingleton<PartnerTable>, ITableFactory
{
  public Dictionary<int, PartnerData> dictPartnerData = new Dictionary<int, PartnerData>();

  public Dictionary<ItemGradeType, int> dictGradeCombineCount = new Dictionary<ItemGradeType, int>();

  public void Load()
  {
    dictPartnerData.Clear();
    string partnerJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Partner, out partnerJson))
    {
      List<PartnerData> partnerDataList = JsonConvert.DeserializeObject<List<PartnerData>>(partnerJson);
      foreach (var data in partnerDataList)
      {
        if (!dictPartnerData.ContainsKey(data.partnerIdx))
        {
          dictPartnerData.Add(data.partnerIdx, data);

          AddGradeCombineCount(data);
        }
        else
          Debug.Log("partner Table Load Error Index : " + data.partnerIdx);
      }
      Debug.Log("partner Table Load Success");
    }

  }

  public int GetPartnerCount()
  {
    return dictPartnerData.Count;
  }

  private void AddGradeCombineCount(PartnerData partnerData)
  {
    ItemGradeType gradeType = (ItemGradeType)partnerData.partnerGrade;

    if (partnerData.isGetCombine)
    {
      if (dictGradeCombineCount.ContainsKey(gradeType))
      {
        dictGradeCombineCount[gradeType]++;
      }
      else
      {
        dictGradeCombineCount.Add(gradeType, 1);
      }
    }
  }


  public Dictionary<int, PartnerData> GetPartnerlDic()
  {
    return dictPartnerData;
  }


  public PartnerData GetPartnerData(int _key)
  {
    if (dictPartnerData.ContainsKey(_key))
    {
      return dictPartnerData[_key];
    }
    else
    {
      Debug.Log($"No Partner Data key : {_key}");
      throw new System.NotImplementedException();
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }

  


  /// <summary>
  /// 기도원 장착 가능 상태
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public bool IsCanMountPary(InvenData invenData)
  {
    int curHP = GetPrayerCurrentHP(invenData);
    
    int minHP = (int)BaseTable.getInstance.GetBaseValue((int)MetaBaseType.REQUIRED_MINIMUM_PRAYER_SLOT_FOR_PRAYER_HOUSE_HP);

    return curHP >= minHP;
  }

  public int GetPrayerCurrentHP(InvenData invenData)
  {
    return GetPrayerMaxHP(invenData) - GameDataManager.getInstance.userContentsData.prayerCenter.dictPartnerPray[invenData.invenIdx].lostHP;
  }

  /// <summary>
  /// 파트너 기도 MaxHp
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public int GetPrayerMaxHP(InvenData invenData)
  {
     PartnerData partnerData = GetPartnerData(invenData.itemIdx);
     float hpA = BaseTable.getInstance.GetBaseValue((int)MetaBaseType.PRAYER_HOUSE_ALLY_HEALTH_VARIABLE_A);
     float hpB = BaseTable.getInstance.GetBaseValue((int)MetaBaseType.PRAYER_HOUSE_ALLY_HEALTH_VARIABLE_B);
     return (int)(hpA + (partnerData.partnerGrade * (1 + invenData.itemLv / hpB)));
  }

  /// <summary>
  /// 분당 파트너 신앙심 획득 수치
  /// </summary>
  /// <param name="itemIdx"></param>
  /// <param name="itemLevel"></param>
  /// <returns></returns>
  public int GetPartnerFaithMin(int itemIdx, int itemLevel)
  {
    float faithA = BaseTable.getInstance.GetBaseValue((int)MetaBaseType.PRAYER_HOUSE_ACQUIRED_FAITH_VARIABLE_A);
    float faithB = BaseTable.getInstance.GetBaseValue((int)MetaBaseType.PRAYER_HOUSE_ACQUIRED_FAITH_VARIABLE_B);

    int gradeType = ItemTable.getInstance.GetItemGrade(itemIdx);

    return (int)(faithA + (gradeType * (1 + itemLevel / faithB)));
  }

  /// <summary>
  /// 회복 HP계산 (분)
  /// </summary>
  /// <param name="partnerPrayData">파트너 기도 데이터</param>
  public void CalcRecoveryHp(PartnerPrayData partnerPrayData)
  {
    DateTime now = DateTime.Now;
    if (partnerPrayData.lastRecoveryDt == null)
    {
      partnerPrayData.lostHP = 0;
      partnerPrayData.lastRecoveryDt = now;
      return; // 초기화 후 즉시 반환
    }

    var timeDifference = DateTime.Now - partnerPrayData.lastRecoveryDt;

    int elapsedMinutes = (int)timeDifference.Value.TotalMinutes;
    int remainingSeconds = timeDifference.Value.Seconds;

    if(elapsedMinutes == 0)
    {
      return;
    }

    partnerPrayData.lostHP -= elapsedMinutes;
    partnerPrayData.lastRecoveryDt = now.AddSeconds(-remainingSeconds);
    if (partnerPrayData.lostHP < 0) partnerPrayData.lostHP = 0;

  }

  /// <summary>
  /// 플레이어의 모든 동료들에 대한 보유 효과 값 반환
  /// </summary>
  /// <returns></returns>
  public float GetHasPartnersStat()
  {
    float totalValue = 0f;

    var itmeMap = GameDataManager.getInstance.dictPartner;

    foreach (var item in itmeMap)
    {
      InvenData invenData = item.Value;

      if (invenData != null)
      {
        int itemLevel = invenData.itemLv;

        totalValue += GetPartnerHasValue(invenData.itemIdx, itemLevel);
      }
    }

    return totalValue;
  }


  public float GetPartnerHasValue(int itemIdx, int itemLevel)
  {
    PartnerData partnerData = this.GetPartnerData(itemIdx);

    return partnerData.ownershipValue + partnerData.ownershipIncrease * itemLevel;
  }




  /// <summary>
  /// 회복 HP계산
  /// </summary>
  /// <param name="startDt">회복 시작 시간</param>
  /// <param name="partnerPrayData">파트너 기도 데이터</param>
  //public void CalcRecoveryHp(PartnerPrayData partnerPrayData)
  //  {
  //      DateTime now = DateTime.Now;
  //      if (partnerPrayData.lastRecoveryDt == null)
  //      {
  //          partnerPrayData.lostHP = 0;
  //          partnerPrayData.lastRecoveryDt = now;
  //          return; // 초기화 후 즉시 반환
  //      }
  //      int elapsedSeconds = (int)(now - partnerPrayData.lastRecoveryDt).Value.TotalSeconds; // 경과된 초 계산
  //      int recoveredHp = elapsedSeconds;
  //      // HP 회복 적용
  //      partnerPrayData.lostHP -= recoveredHp;
  //      partnerPrayData.lastRecoveryDt = now;
  //      if (partnerPrayData.lostHP < 0) partnerPrayData.lostHP = 0;
  //  }
}
