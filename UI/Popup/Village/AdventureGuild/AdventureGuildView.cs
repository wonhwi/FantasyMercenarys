using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RestPacket;

public class AdventureGuildView : MonoBehaviour
{
  [SerializeField] private BundleContentLvInfo bundleGuildLvInfo;
  [SerializeField] private AdventureGuildQuestSlot[] questSlotArray;

  [SerializeField] private TextMeshProUGUI gradeRangeText;
  [SerializeField] private TextMeshProUGUI acceptLimitText;
  [SerializeField] private TextMeshProUGUI acceptCountText;


  public AdventureGuildQuestSlot[] QuestSlotArray => questSlotArray;

  [SerializeField] private Button closeButton;

  public Action OnCloseButtonClicked;

  private void Awake()
  {
    closeButton.onClick.AddListener(() => OnCloseButtonClicked?.Invoke());
  }

  public void SetGuildLvInfo(int guildLv, int guildExp)
  {
    bundleGuildLvInfo.SetData(ShopCategoryType.AdventureGuild, guildLv, guildExp);
  }

  public void SetGradeRangeText(int minGrade, int maxGrade)
  {
    gradeRangeText.text = $"{(GradeType)minGrade} ~ {(GradeType)maxGrade}";
  }


  /// <summary>
  /// 우측 상단 의뢰 상태 Text 설정
  /// </summary>
  /// <param name="acceptableCount">금일 퀘스트 수락 가능 갯수</param>
  /// <param name="maxCount">일일 최대 수락 갯수</param>
  /// <param name="acceptCount">현재 진행 중인 의뢰 갯수</param>
  public void SetQuestCountInfo(int acceptableCount, int maxCount, int acceptCount)
  {
    acceptLimitText.text = $"{acceptableCount}/{maxCount}";
    acceptCountText.text = acceptCount.ToString();
  }

  public void ShowReward(List<InvenData> rewardItemList)
  {
    UIUtility.ShowRewardItemPopup(rewardItemList);
  }
}
