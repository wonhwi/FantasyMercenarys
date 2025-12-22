using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스텟 출력 모듈화 스크립트
/// </summary>
public class BundleStatText : MonoBehaviour
{
  private readonly int DEFAULT_STAT_COUNT = 3;

  [SerializeField] private UIStatText[] uiStatTextArray;
  
  public void SetData(List<StatData> statDataList)
  {
    for (int i = 0; i < uiStatTextArray.Length; i++)
      uiStatTextArray[i].gameObject.SetActive(false);

    for (int j = 0; j < statDataList.Count; j++)
    {
      uiStatTextArray[j].SetData((StatType)statDataList[j].statType, statDataList[j].statValue);
      uiStatTextArray[j].gameObject.SetActive(true);
    }

  }

  public void SetCompareData(List<StatData> statDataList, List<StatData> targetDataList)
  {
    for (int i = 0; i < uiStatTextArray.Length; i++)
      uiStatTextArray[i].gameObject.SetActive(false);

    //기본 스텟 비교
    for (int j = 0; j < DEFAULT_STAT_COUNT; j++)
    {
      StatData statData = statDataList[j];

      int findIndex = targetDataList.FindIndex(0, DEFAULT_STAT_COUNT, n => n.statType == statData.statType);

      float targetValue = 0f;

      if (findIndex != -1)
        targetValue = targetDataList[findIndex].statValue;

      uiStatTextArray[j].SetCompareData((StatType)statData.statType, statData.statValue, targetValue);
      uiStatTextArray[j].gameObject.SetActive(true);
    }

    //추가 스텟 비교

    for (int k = DEFAULT_STAT_COUNT; k < statDataList.Count; k++)
    {
      StatData statData = statDataList[k];

      int findIndex = targetDataList.FindIndex(DEFAULT_STAT_COUNT, targetDataList.Count - DEFAULT_STAT_COUNT, n => n.statType == statData.statType);

      float targetValue = 0f;

      if(findIndex != -1)
        targetValue = targetDataList[findIndex].statValue;

      uiStatTextArray[k].SetCompareData((StatType)statData.statType, statData.statValue, targetValue);
      uiStatTextArray[k].gameObject.SetActive(true);
    }
  }

  public void SetCompareEmptyData(List<StatData> statDataList)
  {
    for (int i = 0; i < uiStatTextArray.Length; i++)
      uiStatTextArray[i].gameObject.SetActive(false);

    for (int j = 0; j < statDataList.Count; j++)
    {
      StatData statData = statDataList[j];

      float targetValue = 0f;

      uiStatTextArray[j].SetCompareData((StatType)statData.statType, statData.statValue, targetValue);
      uiStatTextArray[j].gameObject.SetActive(true);
    }
  }
}