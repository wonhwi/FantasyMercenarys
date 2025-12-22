using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentHopeCenter
{
  public Dictionary<int, HopeCenterLotteryData> dictLotteryData = new Dictionary<int, HopeCenterLotteryData>();

  /// <summary>
  /// 로그인 시 실행
  /// </summary>
  public void LoadLotteryData(List<HopeCenterLotteryData> lotteryDataList)
  {
    foreach (var lotterdyData in lotteryDataList)
    {
      UpdateLotteryData(lotterdyData);
    }

  }

  /// <summary>
  /// API 호출 시 업데이트 함수
  /// </summary>
  /// <param name="recruitData"></param>
  public void UpdateLotteryData(HopeCenterLotteryData lotteryData)
  {
    dictLotteryData[lotteryData.lotteryIdx] = lotteryData;
  }


}
