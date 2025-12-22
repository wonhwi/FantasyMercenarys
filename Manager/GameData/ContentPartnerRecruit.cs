using System.Collections;
using System.Collections.Generic;
using FantasyMercenarys.Data;
using UnityEngine;

public class ContentPartnerRecruit
{
  private Dictionary<int, UserRecruitInfoDTO> dictRecruitData = new Dictionary<int, UserRecruitInfoDTO>();

  //API 호출 전 슬롯에 대기 상태로 있는 리스트
  public List<int> mountSlotList = new List<int>();

  public UserRecruitInfoDTO GetRecruitInfoData(int slotNum)
  {
    return dictRecruitData[slotNum];
  }

  public Dictionary<int, UserRecruitInfoDTO> GetRecruitInfoDict()
    => dictRecruitData;


  /// <summary>
  /// 로그인 시 실행
  /// </summary>
  public void SetRecruitInfoData(List<UserRecruitInfoDTO> recruitDataList)
  {
    foreach (var recruitData in recruitDataList)
    {
      UpdateRecruitInfoData(recruitData);
    }

  }

  /// <summary>
  /// API 호출 시 업데이트 함수
  /// </summary>
  /// <param name="recruitData"></param>
  public void UpdateRecruitInfoData(UserRecruitInfoDTO recruitData)
  {
    dictRecruitData[recruitData.slotNum] = recruitData;
  }

  
}