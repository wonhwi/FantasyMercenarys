using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCenterModel
{
  private LotteryTicketTable lotteryTicketTable => LotteryTicketTable.getInstance;
  private ContentHopeCenter contentModel => GameDataManager.getInstance.userContentsData.hopeCenter;

  #region Table
  public IEnumerable<int> GetMetaLotteryTicketKeyData()
  {
    return lotteryTicketTable.dictLotteryTicketData.Keys;

  }

  public LotteryTicketData GetLotteryTicketData(int lotteryIdx)
  {
    return lotteryTicketTable.dictLotteryTicketData[lotteryIdx];
  }

  /// <summary>
  /// 메타데이터 기반 Index 순서반환해서 재 사용
  /// </summary>
  /// <param name="lotteryIdx"></param>
  /// <returns></returns>
  public int GetDataIndex(int lotteryIdx)
  {
    if (lotteryIdx == 0)
      return 0;

    int index = 0;

    //스킬 정보가 없으면 결과적으로 0 Return
    foreach (var lotteryTicketData in lotteryTicketTable.dictLotteryTicketData)
    {
      if (lotteryTicketData.Key == lotteryIdx)
      {
        return index;
      }
      index++;
    }

    return index;
  }

  #endregion
  #region Content

  public HopeCenterLotteryData GetUserLotteryData(int lotteryIdx)
  {
    return contentModel.dictLotteryData[lotteryIdx];
  }
  #endregion
}
