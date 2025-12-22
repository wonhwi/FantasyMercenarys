using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBreedingGround
{
  public int daliyObejctPoint { get; private set; }
  public BreedingGroundsData breedingGroundsData { get; private set; }
  public Dictionary<int, CreatureData> dictCreatureData = new Dictionary<int, CreatureData>();

  /// <summary>
  /// 로그인 시, 팝업 진입 시 실행
  /// </summary>
  public void LoadCreatureData(List<CreatureData> creatureDataList)
  {
    if (creatureDataList != null)
    {
      foreach (var creatureData in creatureDataList)
      {
        UpdateCreatureData(creatureData);
      }
    }
  }

  /// <summary>
  /// 로그인 시, 팝업 진입 시 실행
  /// </summary>
  public void LoadBreedingGroundData(BreedingGroundsData breedingGroundsData)
  {
    UpdateBreedingGroundData(breedingGroundsData);
  }

  public void UpdateDaliyObejctPoint(int dailyObjectPoint)
  {
    this.daliyObejctPoint = dailyObjectPoint;
  }

  public void UpdateBreedingGroundData(BreedingGroundsData breedingGroundsData)
  {
    this.breedingGroundsData = breedingGroundsData;
  }

  public void UpdateBreedingSlotData(int[] slotList)
  {
    this.breedingGroundsData.slotList = slotList;
  }

  /// <summary>
  /// API 호출 시 업데이트 함수
  /// </summary>
  /// <param name="recruitData"></param>
  public void UpdateCreatureData(CreatureData creatureData)
  {
    dictCreatureData[creatureData.creatureIdx] = creatureData;
  }

  /// <summary>
  /// 크리처 소유 유무
  /// </summary>
  /// <param name="creatureIdx"></param>
  /// <returns></returns>
  public bool HasCreature(int creatureIdx)
  {
    return dictCreatureData.ContainsKey(creatureIdx);
  }

  /// <summary>
  /// 사육장 관련 유저가 소지하고 있는 크리쳐 데이터 반환
  /// </summary>
  /// <returns></returns>
  public CreatureData GetCreatureData(int creatureIdx)
  {
    if (HasCreature(creatureIdx))
      return dictCreatureData[creatureIdx];
    else
      return null;
  }


}
