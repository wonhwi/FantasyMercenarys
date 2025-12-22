using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class BaseSlotUI
{
  public Image icon;
  public TextMeshProUGUI valueText;
  public int itemIdx;
  public bool useScaleAnimation;
}

[Serializable]
public class CurrencySlotUI : BaseSlotUI
{
  [NonSerialized] public Action<long> callback; // 콜백 인스턴스 저장
}

/// <summary>
/// 일반적인 상황 또는 팝업에 출력되는 재화 모듈화 스크립트
/// 옵저버 이벤트로 구성되어 있으며 재화 변경시 Event를 받아 재화 업데이트 됩니다.
/// </summary>
public class BundleCurrency : MonoBehaviour
{
  [SerializeField] private CurrencySlotUI[] currencySlots;

  /// <summary>
  /// 지속적으로 재화 정보가 바뀔 경우 이 함수 실행, 팝업 닫힐때 Disable 실행시켜주면 됩니다.
  /// </summary>
  /// <param name="itemIdx"></param>
  /// <param name="slotIndex"></param>
  public void UpdateCurrency(int itemIdx, int slotIndex = 0)
  {
    DisableIndex(slotIndex);

    currencySlots[slotIndex].itemIdx = itemIdx;

    EnableIndex(slotIndex);
  }

  public void Enable()
  {
    for (int i = 0; i < currencySlots.Length; i++)
    {
      int index = i;

      EnableIndex(index);
    }
  }


  public void Disable()
  {
    for (int i = 0; i < currencySlots.Length; i++)
    {
      int index = i;

      DisableIndex(index);
    }
  }
  private void EnableIndex(int index)
  {
    CurrencySlotUI slotUI = currencySlots[index];

    //없으면 새로 생성
    if (slotUI.callback == null)
      slotUI.callback = (value) => OnCurrencyChanged(index, value);

    CurrencyManager.getInstance.Subscribe(slotUI.itemIdx, slotUI.callback);
    RefreshSlot(slotUI);
  }

  private void DisableIndex(int index)
  {
    if (currencySlots[index].callback != null)
      CurrencyManager.getInstance.Unsubscribe(currencySlots[index].itemIdx, currencySlots[index].callback);
  }

  private void RefreshSlot(CurrencySlotUI slot)
  {
    var itemData = ItemTable.getInstance.GetItemData(slot.itemIdx);

    if(slot.icon != null)
      slot.icon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, itemData.iconImage);

    slot.valueText.text = FormatUtility.GetCurencyValue(CurrencyManager.getInstance.GetCurrencyAmount(slot.itemIdx));

    if (slot.useScaleAnimation)
      TweeningUtility.UpdateCurrency(slot.valueText.transform);
  }

  private void OnCurrencyChanged(int slotIndex, long newValue)
  {
    var slot = currencySlots[slotIndex];
    slot.valueText.text = FormatUtility.GetCurencyValue(newValue);

    if(slot.useScaleAnimation)
      TweeningUtility.UpdateCurrency(slot.valueText.transform);
  }



}
