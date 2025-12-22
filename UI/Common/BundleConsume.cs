using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ConsumeSlotUI : BaseSlotUI
{
  public GameObject slotRoot;
  public long requiredAmount;
}

public struct ConsumeRequirement
{
  public int itemIdx;
  public long itemCount;

  public ConsumeRequirement(int itemIdx, long requiredAmount)
  {
    this.itemIdx = itemIdx;
    this.itemCount = requiredAmount;
  }
}

/// <summary>
/// 재화 소모 출력 모듈화 스크립트
/// </summary>
public class BundleConsume : MonoBehaviour
{
  public enum ConsumeTextType
  {
    //원하는 소모 재화만 출력
    Default,
    //원하는 소모 재화/내가 가지고 있는 재화 형식으로 출력
    Total
  }

  public enum ConsumeTimeType
  {
    Defualt,
    Time, //00:00:00 형식으로 출력
  }
  [SerializeField] private ConsumeTextType consumeTextType = ConsumeTextType.Default;
  [SerializeField] private ConsumeTimeType consumeTimeType = ConsumeTimeType.Defualt;

  [SerializeField] private ConsumeSlotUI[] consumeSlots;
  [SerializeField] private TextMeshProUGUI timeText;

  private int requiredTime;

  private int slotCount;

  private void InitConsumeSlot()
  {
    for (int i = 0; i < consumeSlots.Length; i++)
      consumeSlots[i].slotRoot.SetActive(false);
  }

  /// <summary>
  /// 요구 재화, 수량, 소모시간 세팅
  /// </summary>
  public void SetConsumeData(ConsumeRequirement[] requirements, int requiredTime = 0)
  {
    slotCount = requirements.Length;

    InitConsumeSlot();

    for (int i = 0; i < slotCount; i++)
    {
      int index = i;

      SetConsumeSlotData(requirements[i].itemIdx, requirements[i].itemCount, index);
    }

    this.requiredTime = requiredTime;
    Refresh();
  }

  public void SetConsumeData(ConsumeRequirement requirement, int requiredTime = 0)
  {
    slotCount = 1;

    InitConsumeSlot();

    SetConsumeSlotData(requirement.itemIdx, requirement.itemCount, 0);

    this.requiredTime = requiredTime;
    Refresh();

  }

  private void SetConsumeSlotData(int itemIdx, long itemCount, int slotIndex)
  {
    consumeSlots[slotIndex].itemIdx = itemIdx;
    consumeSlots[slotIndex].requiredAmount = itemCount;
    consumeSlots[slotIndex].slotRoot.SetActive(true);
  }

  /// <summary>
  /// UI 갱신 (내가 가진 재화와 필요 재화 비교)
  /// </summary>
  public void Refresh()
  {
    for (int i = 0; i < slotCount; i++)
    {
      var slot = consumeSlots[i];
      var itemData = ItemTable.getInstance.GetItemData(slot.itemIdx);

      if(slot.icon != null)
        slot.icon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, itemData.iconImage);

      long owned = CurrencyManager.getInstance.GetCurrencyAmount(slot.itemIdx);
      slot.valueText.text = consumeTextType == ConsumeTextType.Default ? $"{slot.requiredAmount}" : $"{owned}/{slot.requiredAmount}";
      slot.valueText.color = owned < slot.requiredAmount ? Color.red : Color.white;
    }

    if (timeText != null)
    {
      if(requiredTime > 0)
      {
        if (consumeTimeType == ConsumeTimeType.Defualt)
        {
          timeText.text = $"{requiredTime}초";
        }
        else if(consumeTimeType == ConsumeTimeType.Time)
        {
          timeText.text = FormatUtility.FormatHHMMSS(requiredTime);
        }
      }
      else
      {
        timeText.text = string.Empty;
      }
      
    }
  }

  /// <summary>
  /// 현재 소모 가능한지 여부 반환
  /// </summary>
  public bool CanConsume()
  {
    for (int i = 0; i < slotCount; i++)
    {
      var slot = consumeSlots[i];

      long owned = CurrencyManager.getInstance.GetCurrencyAmount(slot.itemIdx);
      if (owned < slot.requiredAmount)
        return false;
    }
    return true;
  }
}
