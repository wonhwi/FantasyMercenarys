using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;
using SkillState = MerchantGuildSkillUISlot.SkillState;

public class MerchantGuildPresenter
{
  private MerchantGuildModel model;
  private MerchantGuildView view;

  public MerchantGuildPresenter(MerchantGuildModel model, MerchantGuildView view, MerchantGuildUIPopup popUp)
  {
    this.model = model;
    this.view = view;


    view.OnClosePopup = () =>
    {
      view.bundleCountDownText.InitCountDown();

      view.DisableCurrency();
      popUp.Hide();
    };

    view.OnClickSpeedBoost = async () => {
      var popup = await NewUIManager.getInstance.Show<EquipmentFastTimePopupController>("FantasyMercenary/Popup/Equipment/EquipmentFastTimePopup");
      popup.SetLeftTime(10000);
    }; 
    view.OnClickMaxLv = () => UIUtility.ShowToastMessagePopup("현재 최고 레벨 단계");

  }


  public void SetData()
  {
    view.EnableCurrency();

    SetSkillData();
    
    SelectFirstSlot();
  }

  /// <summary>
  /// 스킬 데이터 기반 슬롯 정보 출력
  /// </summary>
  private void SetSkillData()
  {
    var enumerator = model.GetMetaSkillTreeKeyData().GetEnumerator();

    while (enumerator.MoveNext())
      SetSlotData(enumerator.Current);
  }

  /// <summary>
  /// 스킬 슬롯 설정
  /// </summary>
  /// <param name="skillIdx"></param>
  public void SetSlotData(int skillIdx)
  {
    MerchantSkillTree skillTreeData = model.GetSkillTreeData(skillIdx);

    int slotIdx = model.GetDataIndex(skillIdx);
    int skillLv = model.GetHasItemLv(skillIdx);
    var slot = GetSkillSlot(slotIdx);

    //내가 가지고 있는 아이템 여부
    bool hasItem = model.HasItem(skillIdx);
    //현재 연구중인 아이템 여부
    bool isResearchItem = model.IsResearchItem(skillIdx);
    //해당 아이템의 선행 조건을 만족했는지의 여부 확인
    bool isConditionValid = model.IsConditionValid(skillIdx);
    //최대 레벨일 경우
    bool isMaxLvItem = model.IsMaxLvItem(skillIdx, skillLv);


    slot.SetItemImage(skillTreeData.skillIconImage);
    slot.SetLvText(skillTreeData.skillMaxLevel, skillLv);

    if (isResearchItem)
    {
      slot.SetSlotState(SkillState.Researching);
    }
    else
    {
      if (hasItem)
      {
        if (isMaxLvItem)
          slot.SetSlotState(SkillState.MaxLevel);
        else
          slot.SetSlotState(SkillState.Activate);
      }
      else
      {
        if (isConditionValid)
          slot.SetSlotState(SkillState.Activate);
        else
          slot.SetSlotState(SkillState.DeActive);
      }
    }


    slot.OnClickSlot = () =>
    {
      bool isResearchItem = model.IsResearchItem(skillIdx);

      SetSelectSlot(slot);
      SetDetailSkillSlot(skillIdx, skillLv);
      SetDetailData(skillIdx, skillLv);
      SetSkillTreeLine(skillIdx);
      SetSlotHighlight(slotIdx);

      //연구 중 상태
      if (isResearchItem)
      {
        SetCountDown(skillIdx);

        view.SetButtonState(SkillState.Researching);
      }
      //최대레벨 상태
      else if (isMaxLvItem)
      {
        view.SetButtonState(SkillState.MaxLevel);
      }
      //버튼 활성화, 비활성화
      else
      {
        SetConsumeData(skillIdx, skillLv);

        view.OnClickResearch = () => OnResearchItem(skillIdx, skillLv + 1);
      }
    };

  }

  /// <summary>
  /// 좌측 하단 슬롯 업데이트
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  public void SetDetailSkillSlot(int skillIdx, int skillLv)
  {
    MerchantSkillTree skillTreeData = model.GetSkillTreeData(skillIdx);

    var slot = view.PreviewSlot;

    slot.SetItemImage(skillTreeData.skillIconImage);
    slot.SetLvText(skillTreeData.skillMaxLevel, skillLv);
  }

  /// <summary>
  /// 하단 아이템 세부 정보 출력
  /// </summary>
  public void SetDetailData(int skillIdx, int skillLv)
  {
    MerchantSkillTree skillTreeData = model.GetSkillTreeData(skillIdx);

    //최대 레벨일 경우
    bool isMaxLvItem = model.IsMaxLvItem(skillIdx, skillLv);

    float curSkillValue = 0f;
    float nextSkillValue = 0f;

    if (skillLv != 0)
    {
      MerchantSkillLevelup currentLevelUpData = model.GetSkillLevelUpData(skillIdx, skillLv);

      curSkillValue = currentLevelUpData.skillValue;

      //최대 레벨이면 현재의 값과 동일하게 다음 레벨값 설정
      if (isMaxLvItem)
      {
        nextSkillValue = curSkillValue;
      }
      else
      {
        MerchantSkillLevelup nextLevelUpData = model.GetSkillLevelUpData(skillIdx, skillLv + 1);

        nextSkillValue = nextLevelUpData.skillValue;
      }
    }
    else
    {
      MerchantSkillLevelup nextLevelUpData = model.GetSkillLevelUpData(skillIdx, skillLv + 1);

      nextSkillValue = nextLevelUpData.skillValue;
    }

    view.SetSkillNameText(skillTreeData.skillName);
    view.SetCurDetailText(skillTreeData.skillRecordCd, curSkillValue);
    view.SetNextDetailText(skillTreeData.skillRecordCd, nextSkillValue);
  }

  #region 스킬 트리 라인 활성화
  /// <summary>
  /// 스킬 트리 라인 활성화
  /// </summary>
  /// <param name="skillIdx"></param>
  public void SetSkillTreeLine(int skillIdx)
  {
    view.InitRoadLine();

    ActiveSkillTreeLine(skillIdx);

    //여기에 내가 획득한 아이템들에 대해서도 함수를 실행시켜줘야할것같다.
    var enumerator = model.GetUserHasSkillKeyData().GetEnumerator();

    while (enumerator.MoveNext())
      ActiveSkillTreeLine(enumerator.Current);
  }

  /// <summary>
  /// 스킬 인덱스 기반 선행 스킬 확인 후 스킬트리 라인 활성화 실행
  /// </summary>
  /// <param name="skillIdx"></param>
  private void ActiveConditionSkillLine(int skillIdx)
  {
    MerchantSkillTree skillTreeData = model.GetSkillTreeData(skillIdx);

    int preSkill1 = skillTreeData.preSkillIdx1;
    int preSkill2 = skillTreeData.preSkillIdx2;

    ActiveSkillTreeLine(preSkill1);
    ActiveSkillTreeLine(preSkill2);
  }

  /// <summary>
  /// SkillIndex 기반으로 스킬트리 라인 활성화
  /// </summary>
  /// <param name="skillIdx"></param>
  private void ActiveSkillTreeLine(int skillIdx)
  {
    if (skillIdx != 0)
    {
      int slotIdx = model.GetDataIndex(skillIdx);

      GameObject[] roadLines = GetSkillSlotRoadLines(slotIdx);

      for (int i = 0; i < roadLines.Length; i++)
      {
        roadLines[i].SetActive(true);
      }

      ActiveConditionSkillLine(skillIdx);
    }
  }

  #endregion


  /// <summary>
  /// 슬롯 선택 시 이펙트 활성화
  /// </summary>
  /// <param name="slotIdx"></param>
  public void SetSlotHighlight(int slotIdx)
  {
    var slot = GetSkillSlot(slotIdx);

    view.selectHighlight.gameObject.SetActive(true);
    view.selectHighlight.SetParent(slot.itemButton.transform);
    view.selectHighlight.anchoredPosition = Vector2.zero;
  }

  /// <summary>
  /// 연구 중 상태일때 카운트 다운
  /// </summary>
  /// <param name="skillIdx"></param>
  private void SetCountDown(int skillIdx)
  {
    int totalSeconds = CodeUtility.GetTotalSeconds(model.GetUpgradeCompleteAt());

    view.bundleCountDownText.OnComplete = null;
    view.bundleCountDownText.OnComplete = () => OnSkillUpdate(skillIdx);

    view.bundleCountDownText.InitCountDown();

    view.bundleCountDownText.SetCountDown(totalSeconds);
  }

  /// <summary>
  /// 선택한 스킬의 소모 재화 출력
  /// 버튼 활성화, 비활성화, 최대레벨 상태
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  public void SetConsumeData(int skillIdx, int skillLv)
  {
    MerchantSkillLevelup nextLevelUpData = model.GetSkillLevelUpData(skillIdx, skillLv + 1);

    int meterialPoint = nextLevelUpData.materialPoint;
    int researchTime = nextLevelUpData.researchTime;


    ConsumeRequirement requirement = new ConsumeRequirement()
    {
      itemIdx = ConstantManager.ITEM_CURRENCY_RESEARCH_FUNDS,
      itemCount = meterialPoint
    };

    view.SetConsumeData(requirement, researchTime);

    bool isEnought = view.GetConsumeIsEnought();
    bool isConditionValid = model.IsConditionValid(skillIdx);
    bool isUpgrade = model.GetIsUpgrade();

    if(isEnought && isConditionValid && !isUpgrade)
    {
      view.SetButtonState(SkillState.Activate);
    }
    else
    {
      view.SetButtonState(SkillState.DeActive);
    }
  }

  private void SetSelectSlot(MerchantGuildSkillUISlot slot)
  {
    view.selectSlot = slot;
  }
  /// <summary>
  /// 아이템 슬롯 반환 함수
  /// </summary>
  /// <param name="slotIdx"></param>
  /// <returns></returns>
  private MerchantGuildSkillUISlot GetSkillSlot(int slotIdx)
  {
    return view.SkillUISlots[slotIdx].slot;
  }

  private GameObject[] GetSkillSlotRoadLines(int slotIdx)
  {
    return view.SkillUISlots[slotIdx].roadLines;
  }

  /// <summary>
  /// 만약 레벨업이 되어서 다음 스킬트리가 열려있다면
  /// </summary>
  /// <param name="skillIdx"></param>
  private void UpdateLevelUpSkillTree()
  {
    var enumerator = model.GetMetaSkillTreeKeyData().GetEnumerator();

    while (enumerator.MoveNext())
    {
      int skillIdx = enumerator.Current;

      bool isConditionValid = model.IsConditionValid(skillIdx);

      if(isConditionValid)
      {
        SetSlotData(skillIdx);
      }
    }
  }

  /// <summary>
  /// 팝업 진입시 가장 우측 상단에 있는 슬롯 자동 선택
  /// </summary>
  private void SelectFirstSlot()
  {
    int slotIdx = 0;

    bool isUpgrading = model.GetIsUpgrade();

    if(isUpgrading)
    {
      int itemIdx = model.GetUpgradeSkillIdx();

      slotIdx = model.GetDataIndex(itemIdx);
    }
    else
    {
      slotIdx = model.GetLastSkillSlotIndex();
    }

    var slot = GetSkillSlot(slotIdx);

    slot.OnClickSlot?.Invoke();

    view.InitContentPosition(slotIdx);
  }

  #region API

  public async void OnSkillUpdate(int skillIdx)
  {
    await APIManager.getInstance.REQ_MerchantGuildSkillTreeLoad<RES_MerchantGuildSkillTreeLoad>((responseResult) =>
    {
      //해당 슬롯 업데이트
      SetSlotData(skillIdx);

      //최근에 선택한 슬롯의 아랫 정보 업데이트
      if(skillIdx != responseResult.upgradeSkillIdx)
        view.selectSlot.OnClickSlot?.Invoke();

      UpdateLevelUpSkillTree();
    });
  }

  public async void OnResearchItem(int skillIdx, int skillLv)
  {
    bool isConditionValid = model.IsConditionValid(skillIdx);
    bool isUpgrade = model.GetIsUpgrade();
    bool isEnought = view.GetConsumeIsEnought();

    if (!isEnought)
    {
      UIUtility.ShowToastMessagePopup("연구 비용이 부족");
      return;
    }

    if (!isConditionValid)
    {
      UIUtility.ShowToastMessagePopup("선행 조건 미충족");
      return;
    }

    if (isUpgrade)
    {
      UIUtility.ShowToastMessagePopup("다른 연구 진행 중");
      return;
    }

    await APIManager.getInstance.REQ_MerchantGuildSkillTreeUpgrade<RES_MerchantGuildSkillTreeUpgrade>(skillIdx, skillLv, (responseResult) => 
    {
      int slotIdx = model.GetDataIndex(skillIdx);
      var slot = GetSkillSlot(slotIdx);

      SetSlotData(responseResult.skillIdx);
      slot.OnClickSlot?.Invoke();
    });

  }

  #endregion

}
