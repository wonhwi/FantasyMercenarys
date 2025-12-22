using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentPrayerCenter
{
  //기도원 동료 회복 상태
  public Dictionary<long, PartnerPrayData> dictPartnerPray = new Dictionary<long, PartnerPrayData>();
  //기도원 동료 장착 데이터
  public Dictionary<int, PrayerData> mountPrayerMap = new Dictionary<int, PrayerData>();

  /// <summary>
  /// 로그인 시, 동료 기도원 배치, 기도원 진입시 동료 정보 최신화시 실행시키는 함수
  /// </summary>
  /// <param name="partnerPrayList"></param>
  public void UpdatePartnerPrayData(List<PartnerPrayData> partnerPrayList)
  {
    if (partnerPrayList == null) return;

    foreach (var item in partnerPrayList)
    {
      if (dictPartnerPray.ContainsKey(item.invenIdx))
      {
        dictPartnerPray.Remove(item.invenIdx);
      }
      dictPartnerPray.Add(item.invenIdx, item); // invenIdx를 키로 사용
    }
  }

  /// <summary>
  /// 기도원에서 신앙심 획득 시 장착된 동료들의 데이터로 현재 동료 회복 상태 업데이트 (인벤토리 업데이트 용도)
  /// </summary>
  /// <param name="prayerDataList"></param>
  public void UpdatePartnerPrayData(List<PrayerData> prayerDataList)
  {
    if (prayerDataList == null) return;

    foreach (PrayerData prayerData in prayerDataList)
    {
      long invenIdx = dictPartnerPray.FirstOrDefault(n => n.Value.itemIdx == prayerData.partnerIdx).Key;

      if(invenIdx != -1)
      {
        dictPartnerPray[invenIdx].lostHP = prayerData.maxHp - prayerData.hp;
      }
    }
  }

  /// <summary>
  /// 현재 슬롯에 장착한 동료들 정보 업데이트
  /// </summary>
  /// <param name="prayerDataList"></param>
  public void UpdateMountPrayerSlotData(List<PrayerData> prayerDataList)
  {
    if (prayerDataList == null) return;
    foreach (PrayerData prayerData in prayerDataList)
    {
      if (prayerData.partnerIdx == 0)
      {
        if (this.mountPrayerMap.ContainsKey(prayerData.slot))
        {
          this.mountPrayerMap.Remove(prayerData.slot);
        }
      }
      else
      {
        if (this.mountPrayerMap.ContainsKey(prayerData.slot))
        {
          this.mountPrayerMap.Remove(prayerData.slot);
        }
        this.mountPrayerMap.Add(prayerData.slot, prayerData);
      }
    }

  }
}
