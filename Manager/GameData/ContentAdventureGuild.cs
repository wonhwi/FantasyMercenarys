using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;

public class ContentAdventureGuild
{
  private Dictionary<int, GuildQuestInfoData> dictGuildQuestInfo = new Dictionary<int, GuildQuestInfoData>();

  private int dailyRequestAcceptCount;
  private int adventureGuildLv;

  /// <summary>
  /// 일일 의뢰 수락 횟수
  /// </summary>
  public int DailyRequestAcceptCount
  {
    get => dailyRequestAcceptCount;
    private set
    { 
      dailyRequestAcceptCount = value;
      DailyAcceptableCount = MaxRequestAcceptCount - value;
    } 
  }


  /// <summary>
  /// 모험가 길드 레벨
  /// </summary>
  public int AdventureGuildLv
  {
    get => adventureGuildLv;
    private set
    {
      adventureGuildLv = value;
      MaxRequestAcceptCount = (int)BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_ADVENTURE_GUILD_LIMIT_QUEST + AdventureGuildLv);
    }
  }
  
  /// <summary>
  /// 모험가 길드 경험치
  /// </summary>
  public int AdventureGuildExp { get; private set; }

  /// <summary>
  /// 모험가 길드 퀘스트 최대 수락 가능 갯수
  /// </summary>
  public int MaxRequestAcceptCount { get; private set; }

  /// <summary>
  /// 일일 의뢰 수락 가능 갯수
  /// </summary>
  public int DailyAcceptableCount { get; private set; }

  public Dictionary<int, GuildQuestInfoData> GetQeustInfoDict()
    => dictGuildQuestInfo;

  public GuildQuestInfoData GetQuestInfoData(int questOrder)
    => dictGuildQuestInfo[questOrder];

  public void SetGuildLvExp(int adventureGuildLv, int adventureGuildExp)
  {
    AdventureGuildLv = adventureGuildLv;
    AdventureGuildExp = adventureGuildExp;
  }

  public void SetDailyRequestAcceptCount(int dailyRequestAcceptCount)
    => DailyRequestAcceptCount = dailyRequestAcceptCount;

  /// <summary>
  /// 로그인 시 실행
  /// </summary>
  public void SetGuildQuestInfoData(List<GuildQuestInfoData> guildQuestInfoDataList)
  {
    for (int i = 0; i < guildQuestInfoDataList.Count; i++)
    {
      GuildQuestInfoData guildQuestInfoData = guildQuestInfoDataList[i];

      if (guildQuestInfoData.currentValue == guildQuestInfoData.targetValue)
        guildQuestInfoData.questStatus = (int)AdventureGuildQuestState.Complete;

      int questOrder = guildQuestInfoData.questOrder;

      dictGuildQuestInfo.Add(questOrder, guildQuestInfoData);
    }
  }

  /// <summary>
  /// 모험가 길드 진입 시 
  /// </summary>
  /// <param name="guildQuestInfoDataList"></param>
  public void UpdateGuildQuestInfoDataList(List<GuildQuestInfoData> guildQuestInfoDataList)
  {
    for (int i = 0; i < guildQuestInfoDataList.Count; i++)
      UpdateGuildQuestInfoData(guildQuestInfoDataList[i]);
  }


  /// <summary>
  /// 모험가 길드 퀘스트 상태 변경, 보상 받기
  /// </summary>
  /// <param name="guildQuestInfoData"></param>
  public void UpdateGuildQuestInfoData(GuildQuestInfoData guildQuestInfoData)
  {
    int index = guildQuestInfoData.questOrder;

    if (guildQuestInfoData.currentValue == guildQuestInfoData.targetValue)
      guildQuestInfoData.questStatus = (int)AdventureGuildQuestState.Complete;

    dictGuildQuestInfo[index] = guildQuestInfoData;
  }


  
}
