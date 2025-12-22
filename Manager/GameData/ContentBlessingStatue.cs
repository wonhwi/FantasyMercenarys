using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBlessingStatue
{
  private int blessingLv;          //  가호 레벨
  private long useBlessingPoints;  // 사용한 가호 경험치

  public Dictionary<int, BlessingPresetData> blessingPresetMap = new Dictionary<int, BlessingPresetData>();

  public int GetBlessingLv() => blessingLv;
  public long GetUseBlessingPoints() => useBlessingPoints;  // 사용한 가호 경험치

  public void SetBlessingLv(int blessingLv) 
    => this.blessingLv = blessingLv;
  public void SetUseBlessingPoints(long useBlessingPoints)
    => this.useBlessingPoints = useBlessingPoints;

  /// <summary>
  /// 로그인 시 프리셋 정보 캐싱
  /// </summary>
  /// <param name="blessingPresetList"></param>
  public void UpdateBlessingPresetData(List<BlessingPresetData> blessingPresetList)
  {
    if (blessingPresetList == null) return;

    int presetNumber = 0;

    foreach (var blessingPreset in blessingPresetList)
    {
      if (blessingPresetMap.ContainsKey(blessingPreset.presetNumber))
      {
        blessingPresetMap.Remove(blessingPreset.presetNumber);
      }
      blessingPresetMap.Add(blessingPreset.presetNumber, blessingPreset);

      if(blessingPreset.isSelect)
        presetNumber = blessingPreset.presetNumber;
    }

    PlayerStatUpdate(presetNumber);
  }

  /// <summary>
  /// 해당 프리셋의 슬롯 상태 변경 시 실행하는 함수
  /// </summary>
  /// <param name="type"></param>
  /// <param name="presetNum"></param>
  /// <param name="slotNum"></param>
  public void UpdateBlessingSlotData(int type, int presetNum, int slotNum)
  {
    BlessStatueSlotType slotType = BlessStatueSlotType.Unlock;

    if (type == (int)PresetUpdateType.LockSlot)
      slotType = BlessStatueSlotType.Lock;
    else if (type == (int)PresetUpdateType.UnlockSlot)
      slotType = BlessStatueSlotType.Unlock;

    BlessingPresetData blessingPresetData = blessingPresetMap[presetNum];

    List<BlessingData> blessingDataList = blessingPresetMap[presetNum].blessingDataList;

    BlessingData blessingData = blessingDataList[slotNum - 1];

    blessingData.activeStatus = (int)slotType;

    for (int i = 0; i < blessingDataList.Count; i++)
    {
      if (blessingDataList[i].sortOrder == slotNum)
        blessingDataList[i] = blessingData;
    }

    blessingPresetMap[presetNum] = blessingPresetData;
    Debug.LogError(JsonConvert.SerializeObject(blessingPresetData, Formatting.Indented));
  }

  /// <summary>
  /// 선택한 프리셋 적용, 저장
  /// </summary>
  /// <param name="presetNum"></param>
  public void UpdateBlessingPresetApply(int presetNum)
  {
    for (int i = 0; i < blessingPresetMap.Count; i++)
    {
      int presetIndex = i + 1;

      BlessingPresetData blessingPresetData = blessingPresetMap[presetIndex];

      if (presetIndex == presetNum)
      {
        blessingPresetData.isSelect = true;
      }
      else
      {
        blessingPresetData.isSelect = false;
      }

      blessingPresetMap[presetIndex] = blessingPresetData;
    }

    PlayerStatUpdate(presetNum);
  }

  /// <summary>
  /// 축복 실행시 해당 프리셋 축복 캐싱 리스트 업데이트 해주는 부분
  /// </summary>
  /// <param name="presetNum"></param>
  /// <param name="blessingDataList"></param>
  public void UpdateBlessingPreset(int presetNum, List<BlessingData> blessingDataList)
  {
    if (blessingDataList == null) return;

    BlessingPresetData blessingPresetData = blessingPresetMap[presetNum];

    blessingPresetData.blessingDataList = blessingDataList;

    blessingPresetMap[presetNum] = blessingPresetData;
  }

  /// <summary>
  /// 슬롯 해금 하였을때 실행하는 함수로 캐싱 데이터 업데이트 해주는 함수
  /// </summary>
  /// <param name="presetNum"></param>
  /// <param name="slotNum"></param>
  /// <param name="blessingData"></param>
  public void UpdateBlessingData(int presetNum, int slotNum, BlessingData blessingData)
  {
    if (blessingData.IsNull()) return;

    //Debug.LogError(JsonConvert.SerializeObject(blessingPresetMap[presetNum], Formatting.Indented));

    List<BlessingData> blessingDataList = blessingPresetMap[presetNum].blessingDataList;

    for (int i = 0; i < blessingDataList.Count; i++)
    {
      if (blessingDataList[i].sortOrder == slotNum)
        blessingDataList[i] = blessingData;
    }

    //Debug.LogError(JsonConvert.SerializeObject(blessingPresetMap[presetNum], Formatting.Indented));
  }


  #region PlayerStat
  public void PlayerStatUpdate(int presetNum)
  {
    PlayerStatManager.getInstance.UpdateBlessingStats(blessingPresetMap[presetNum]);
  }
  #endregion
}
