using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FantasyMercenarys.Data;
using System.Text;

public class RecruitmentSlot : UIUnlockSlot
{
  private enum ExploreType
  {
    Ready,
    Exploring,
    Complete
  }

  public RecruitmentView view;
  public RecruitmentModel model;

  public PartnerExploreSlotData exploreSlotData;

  [Header("탐험 정보")]
  [SerializeField] private TextMeshProUGUI exploreNameText;
  [SerializeField] private TextMeshProUGUI exploreGradeText;
  [SerializeField] private TextMeshProUGUI exploreTimeText;

  [Header("등록 동료 정보")]
  [SerializeField] private RecruitmentUISlot[] recruitmentUISlots;

  [Header("보상 정보")]
  [SerializeField] private Image rewardItem;
  [SerializeField] private TextMeshProUGUI rewardCountText;

  [Header("카운트 다운")]
  [SerializeField] private BundleCountDownText bundleCountDownText;

  [Header("상태 버튼 정보")]
  [SerializeField] private GameObject ReadyGroup;
  [SerializeField] private GameObject ExploringGroup;
  [SerializeField] private GameObject CompleteGroup;

  [SerializeField] private Button exploreStartButton;    //탐험 시작 버튼
  [SerializeField] private Button exploringButton;       //탐험 중
  [SerializeField] private Button exploreCompleteButton; //탐험 완료 버튼

  public Action OnExploreStart;     //탐험 시작
  public Action OnExploreCancel;    //탐험 취소
  public Action OnExploreComplete;  //탐험 완료

  public Action<Predicate<PartnerData>> OnLoadInven;  //조건에 맞는 인벤토리 출력

  /// <summary>
  /// API 호출용
  /// </summary>
  /// <returns></returns>
  public IEnumerable<long> GetInvenIndexList()
  {
    for (int i = 0; i < recruitmentUISlots.Length; i++)
    {
      InvenDataSlot invenDataSlot = recruitmentUISlots[i].invenDataSlot;

      InvenData invenData = invenDataSlot.GetInvenData();

      if (invenData != null)
      {
        yield return invenData.invenIdx;
      }
    }
  }


  /// <summary>
  /// 인벤토리 열었을떄 중복안되게 처리용도
  /// </summary>
  /// <returns></returns>
  public IEnumerable<int> GetInvenItemList()
  {
    for (int i = 0; i < recruitmentUISlots.Length; i++)
    {
      InvenDataSlot invenDataSlot = recruitmentUISlots[i].invenDataSlot;

      InvenData invenData = invenDataSlot.GetInvenData();

      if (invenData != null)
      {
        yield return invenData.itemIdx;
      }
    }
  }



  public void InitExploreSlotData(PartnerExploreSlotData exploreSlotData)
  {
    this.exploreSlotData = exploreSlotData;
  }

  public void InitSlotEvent()
  {
    this.exploreStartButton   .onClick.AddListener(() => { exploreStartButton.gameObject.SetActive(false); OnExploreStart?.Invoke(); });
    this.exploringButton      .onClick.AddListener(() => OnExploreCancel?.Invoke());
    this.exploreCompleteButton.onClick.AddListener(() => { exploreCompleteButton.gameObject.SetActive(false); OnExploreComplete?.Invoke(); });

    int gradeMinLimit = exploreSlotData.gradeMinLimit;
    int gradeMaxLimit = exploreSlotData.gradeMaxLimit;

    for (int i = 0; i < recruitmentUISlots.Length; i++)
    {
      int index = i;

      RecruitmentUISlot slot = recruitmentUISlots[index];

      slot.OnClickInven = () => OnSlotInvenClicked(slot, gradeMinLimit, gradeMaxLimit);
    }
  }

  private void OnSlotInvenClicked(RecruitmentUISlot slot, int gradeMinLimit, int gradeMaxLimit)
  {
    OnLoadInven?.Invoke(
          n => gradeMinLimit <= n.partnerGrade 
           && n.partnerGrade <= gradeMaxLimit
           && !model.IsMountItem(n.partnerIdx)
    );

    //인벤토리의 ApplyButton Event에 함수 새로 연결
    view.OnClickMount = (data) => OnApplyButtonClicked(slot, data);
  }

  private void OnApplyButtonClicked(RecruitmentUISlot slot, InvenData data)
  {
    //슬롯에 데이터 삽입
    slot.SetData(data);

    model.AddMountList(data.itemIdx);
    // 클릭시 초기화 및 보상 정보 업데이트
    slot.invenDataSlot.OnClickEvent(() =>
    {
      slot.ClearData();
      UpdateRewardCount();
      model.RemoveMountList(data.itemIdx);
    });

    UpdateRewardCount();
  }


  public void InitSlot()
  {
    for (int i = 0; i < recruitmentUISlots.Length; i++)
    {
      recruitmentUISlots[i].ClearData();
    }
  }

  public void InitCountDown()
  {
    bundleCountDownText.InitCountDown();
  }

  public void SetViewData()
  {
    int durationTime = exploreSlotData.exploreDurationMinute;

    exploreGradeText.text = $"제한 등급 : {LanguageTable.getInstance.GetLanguage(exploreSlotData.gradeNoti)}";
    exploreNameText.text  = LanguageTable.getInstance.GetLanguage(exploreSlotData.exploreAreaName);
    exploreTimeText.text  = $"탐험시간 : {durationTime / 60}시간";
    
    ItemData rewardItemData = ItemTable.getInstance.GetItemData(exploreSlotData.randomBoxIdx);

    rewardItem.sprite = NewResourceManager.getInstance.LoadItemSprite((ItemGroup)rewardItemData.itemGroup, rewardItemData.iconImage);

    SetRewardCount(0);
  }

  /// <summary>
  /// 슬롯 설정
  /// </summary>
  /// <param name="recruitInfo">데이터</param>
  /// <param name="isRemainTime">남은 시간 존재 여부</param>
  public void SetData(UserRecruitInfoDTO recruitInfo, bool isRemainTime)
  {
    ExploreType exploreType = (ExploreType)recruitInfo.advState;

    if(exploreType == ExploreType.Exploring)
    {
      //종료 시간이 0보다 작으면 완료 상태로 변경
      if (!isRemainTime)
        exploreType = ExploreType.Complete;
    }

    for (int i = 0; i < recruitInfo.mountPartnerList.Count; i++)
    {
      long partnerIdx = recruitInfo.mountPartnerList[i];

      InvenData invenData = GameDataManager.getInstance.dictPartner[partnerIdx];

      if(invenData != null)
      {
        //아이템 슬롯 설정
        recruitmentUISlots[i].SetData(invenData);
      }
      
    }

    SetRewardCount(recruitInfo.partnerInsight);
    SetButtonState(exploreType);
    SetButtonInteractable(false);

    if (isRemainTime)
      bundleCountDownText.OnComplete = () => SetButtonState(ExploreType.Complete);

  }

  private void SetButtonState(ExploreType exploreType)
  {
    ReadyGroup    .gameObject.SetActive(exploreType == ExploreType.Ready);
    ExploringGroup.gameObject.SetActive(exploreType == ExploreType.Exploring);
    CompleteGroup .gameObject.SetActive(exploreType == ExploreType.Complete);
  }

  public void SetCountDown(int remainSeconds)
  {
    bundleCountDownText.InitCountDown();

    bundleCountDownText.SetCountDown(remainSeconds);
  }


  /// <summary>
  /// 관찰력 계산 및 보상 갯수 출력 Update
  /// </summary>
  public void UpdateRewardCount()
  {
    int insightValue = 0;
    bool isValidSlot = false;

    for (int i = 0; i < recruitmentUISlots.Length; i++)
    {
      InvenDataSlot invenDataSlot = recruitmentUISlots[i].invenDataSlot;

      InvenData invenData = recruitmentUISlots[i].invenDataSlot.GetInvenData();

      if (invenData != null)
      {
        ItemGradeType itemGradeType = (ItemGradeType)invenDataSlot.GetItemData().itemGrade;
        int itemLv = invenData.itemLv;

        insightValue += PartnerInsightTable.getInstance.GetPartnerInsightData(itemGradeType, itemLv).partnerInsight;

        isValidSlot = true;
      }
    }

    SetRewardCount(insightValue);
    SetButtonInteractable(isValidSlot);
  }

  public void SetButtonInteractable(bool interactable)
  {
    exploreStartButton.interactable = interactable;
  }

  public void SetRewardCount(int insightValue)
  {
    int rewardCount = GetRewardCount(insightValue);

    if (rewardCount != 0)
      rewardCountText.text = $"x{rewardCount}";
    else
      rewardCountText.text = null;
  }


  /// <summary>
  /// 관찰력에 따른 보상 정보 반환
  /// </summary>
  /// <param name="insightValue"></param>
  /// <returns></returns>
  private int GetRewardCount(int insightValue)
  {
    if (exploreSlotData.reward5Insight <= insightValue)
      return 5;
    else if (exploreSlotData.reward4Insight <= insightValue)
      return 4;
    else if (exploreSlotData.reward3Insight <= insightValue)
      return 3;
    else if (exploreSlotData.reward2Insight <= insightValue)
      return 2;
    else if (exploreSlotData.reward1Insight <= insightValue)
      return 1;

    return 0;
  }

}
