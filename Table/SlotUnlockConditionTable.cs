using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;



public class SlotUnlockConditionTable : LazySingleton<SlotUnlockConditionTable>, ITableFactory
{
  
  public Dictionary<int, SlotUnlockConditionData> dictSlotUnlockData = new Dictionary<int, SlotUnlockConditionData>();

  public void Load()
  {
    dictSlotUnlockData.Clear();
    string slotUnlockJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.SlotUnlockCondition, out slotUnlockJson))
    {
      List<SlotUnlockConditionData> slotDataList = JsonConvert.DeserializeObject<List<SlotUnlockConditionData>>(slotUnlockJson);
      foreach (var data in slotDataList)
      {
        if (!dictSlotUnlockData.ContainsKey(data.slotIdx))
        {
          dictSlotUnlockData.Add(data.slotIdx, data);
        }
        else
          Debug.Log("SlotUnlockCondition Table Load Error Index : " + data.slotIdx);
      }
      Debug.Log("SlotUnlockCondition Table Load Success");
    }
  }

  /// <summary>
  /// 해금 조건에 따른 모든 잠금 해제 정보를 받아오는 함수
  /// 캐릭터 레벨업 시 API 호출 시 조건 만족하는 데이터 확인 및 호출 목적
  /// </summary>
  /// <param name="conditionType">잠금 해제 조건 타입</param>
  /// <returns></returns>
  public IEnumerable<SlotUnlockConditionData> GetSlotUnlockConditionDataList(SlotUnlockConditionType conditionType)
  {
    return dictSlotUnlockData.Values.Where(n => n.unlockType == (int)conditionType);
  }

  /// <summary>
  /// 콘텐츠 타입 별 잠금 해제 정보 받아오는 함수
  /// </summary>
  /// <param name="slotUnlockType">콘텐츠 종류 타입</param>
  /// <returns></returns>
  public IEnumerable<SlotUnlockConditionData> GetSlotUnlockConditionDataList(SlotUnlockContentType slotUnlockType)
  {
    return dictSlotUnlockData.Values.Where(n => n.slotType == (int)slotUnlockType);
  }


  /// <summary>
  /// 클라이언트 전용함수 (해당 콘텐츠 슬롯의 잠금 해제 정보 조건을 반환하는 함수)
  /// </summary>
  /// <param name="slotUnlockType">콘텐츠 타입</param>
  /// <param name="slotNumber">슬롯 번호</param>
  /// <returns>(해금 조건 타입, 해금 조건 값)</returns>
  public (SlotUnlockConditionType unlockConditionType, int unLockValue) GetSlotUnlockConditionData(SlotUnlockContentType slotUnlockType, int slotNumber)
  {
    SlotUnlockConditionData slotData = new SlotUnlockConditionData();

    foreach (var slotUnlockData in dictSlotUnlockData.Values)
    {
      if(slotUnlockData.slotType == (int)slotUnlockType && slotUnlockData.slotNumber == slotNumber)
      {
        slotData = slotUnlockData;
        break;
      }
    }

    return ((SlotUnlockConditionType)slotData.unlockType, slotData.unlockValue);
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }

}


