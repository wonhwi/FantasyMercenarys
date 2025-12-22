using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FantasyMercenarys.Data;
using System;
using UnityEngine.Events;

public enum AdventureGuildQuestState
{
  Ready = 1,    //수락 대기
  OnGoing = 2,  //진행 중
  Complete = 3, //완료
}
public enum AdventureGuildQuestEventType
{
  Accept = 4,   //수락
  Cancel = 5,   //취소
  Refresh = 6  //새로고침
}

public class AdventureGuildQuestSlot : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI questNameText;     //퀘스트 이름
  [SerializeField] private TextMeshProUGUI questTargetText;   //퀘스트 보상 Text
  [SerializeField] private TextMeshProUGUI questGradeText;    //퀘스트 등급 Text

  [SerializeField] private AdventureGuildQuestState questSlotState = AdventureGuildQuestState.Ready;

  [SerializeField] private ItemSlot[] questRewardItemArray;

  [SerializeField] private Button completeButton;    //보상 완료 버튼
  [SerializeField] private Button acceptButton;      //수락 버튼
  [SerializeField] private Button onGoingButton;     //진행 중 버튼
  [SerializeField] private Button cancelButton;      //취소 버튼
  [SerializeField] private Button refreshButton;     //새로고침 버튼 

  [SerializeField] private TextMeshProUGUI coolTimeText;    //쿨타임 출력 Text
  private Coroutine coolTimeCoroutine;

  public Action OnCompleteAction;
  public Action OnAcceptAction;
  public Action OnCancelAction;
  public Action OnRefreshAction;


  private void Awake()
  {
    completeButton.onClick.AddListener(() => OnCompleteAction?.Invoke());
    acceptButton.onClick.AddListener(() => OnAcceptAction?.Invoke());
    cancelButton.onClick.AddListener(() => OnCancelAction?.Invoke());
    refreshButton.onClick.AddListener(() => OnRefreshAction?.Invoke());
  }

  public bool IsAcceptState()
    => questSlotState == AdventureGuildQuestState.OnGoing || questSlotState == AdventureGuildQuestState.Complete;

  public void SetData(GuildQuestInfoData guildQuestInfoData)
  {
    questSlotState = (AdventureGuildQuestState)guildQuestInfoData.questStatus;

    SetInitButtonState();
    SetQuestInfo(guildQuestInfoData);
    SetRewardInfo(guildQuestInfoData);
  }

  private void SetQuestInfo(GuildQuestInfoData guildQuestInfoData)
  {
    AdventureGuildQuestData guildQuestData = AdventureGuildQuestTable.getInstance.GetAdventureGuildQuestData(guildQuestInfoData.guildQuestIdx);

    questNameText.text = LanguageTable.getInstance.GetLanguage(guildQuestData.questNameRecordCd);
    questTargetText.text = string.Format(LanguageTable.getInstance.GetLanguage(guildQuestData.questRecordCd, $"{guildQuestInfoData.currentValue}/{guildQuestInfoData.targetValue}"));
    questGradeText.text = $"의뢰 등급 : {(GradeType)guildQuestData.questGrade}";
  }

  private void SetRewardInfo(GuildQuestInfoData guildQuestInfoData)
  {
    AdventureGuildQuestData guildQuestData = AdventureGuildQuestTable.getInstance.GetAdventureGuildQuestData(guildQuestInfoData.guildQuestIdx);

    SetRewardSlot(0, guildQuestData.rewardIdx1, guildQuestData.rewardValue1);
    SetRewardSlot(1, guildQuestData.rewardIdx2, guildQuestData.rewardValue2);
    SetRewardSlot(2, guildQuestData.rewardIdx3, guildQuestData.rewardValue3);
    SetRewardSlot(3, guildQuestData.rewardIdx4, guildQuestData.rewardValue4);
  }

  private void SetRewardSlot(int index, int itemIdx, int itemValue)
  {
    if (itemIdx <= 0)
    {
      questRewardItemArray[index].gameObject.SetActive(false);
      return;
    }
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);
    questRewardItemArray[index].SetItemCountData(itemData, itemValue);
    questRewardItemArray[index].gameObject.SetActive(true);
  }

  private void SetInitButtonState()
  {
    completeButton.gameObject.SetActive(questSlotState == AdventureGuildQuestState.Complete);
    acceptButton.gameObject  .SetActive(questSlotState == AdventureGuildQuestState.Ready);
    onGoingButton.gameObject .SetActive(questSlotState == AdventureGuildQuestState.OnGoing);
    cancelButton.gameObject  .SetActive(questSlotState == AdventureGuildQuestState.OnGoing);
    refreshButton.gameObject .SetActive(questSlotState == AdventureGuildQuestState.Complete ||
                                        questSlotState == AdventureGuildQuestState.Ready);
  }

  public void SetRefreshCoolTime(int remainSeconds)
  {
    StopCoolTimeCheck();

    if (remainSeconds > 0)
    {
      SetRefreshButtonState(false);
      coolTimeCoroutine = StartCoroutine(CheckCoolTime(remainSeconds));
    }
    else
    {
      SetCoolTimeText(string.Empty);
      SetRefreshButtonState(questSlotState != AdventureGuildQuestState.Complete);
    }
  }

  public void SetCoolTimeText(string timeText)
  {
    coolTimeText.text = timeText;
    coolTimeText.gameObject.SetActive(!string.IsNullOrEmpty(timeText));

  }

  private void SetRefreshButtonState(bool isInteractable)
  {
    refreshButton.interactable = isInteractable;
  }


  private void StopCoolTimeCheck()
  {
    if (coolTimeCoroutine != null)
    {
      StopCoroutine(coolTimeCoroutine);
      coolTimeCoroutine = null;
    }
  }

  private IEnumerator CheckCoolTime(int remainSeconds)
  {
    int time = remainSeconds;

    while (time > 0)
    {
      SetCoolTimeText(FormatUtility.FormatHHMMSS(time));
      yield return YieldInstructionCache.WaitForSeconds(1f);
      time--;
    }

    SetCoolTimeText(string.Empty);
    SetRefreshButtonState(true);
  }

  
}
