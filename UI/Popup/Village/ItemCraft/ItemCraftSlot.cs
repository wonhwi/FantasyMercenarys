using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using FantasyMercenarys.Data;

public class ItemCraftSlot : UIUnlockSlot
{
  [SerializeField] private BundleCountDownText bundleCountDownText;

  public Button selectButton;
  public Button completeButton;

  public ItemSlot itemSlot;

  public Action OnClickInven;
  public Action OnClickCancel;
  public Action OnClickComplete;



  protected override void Awake()
  {
    base.Awake();

    itemSlot.OnClickEvent(() => OnClickCancel?.Invoke());
    selectButton.onClick.AddListener(() => OnClickInven?.Invoke());
    completeButton.onClick.AddListener(() => OnClickComplete?.Invoke());

    
  }

  public void SetData(PSCraftData craftData)
  {
    SetDefaultButton();

    if(craftData.craftItemIdx != 0)
    {
      ItemData itemData = ItemTable.getInstance.GetItemData(craftData.craftItemIdx);

      itemSlot.SetItemData(itemData);
      itemSlot.gameObject.SetActive(true);

      if(craftData.isCrafting)
        bundleCountDownText.OnComplete = () => completeButton.transform.parent.gameObject.SetActive(true);
    }
  }

  public void SetCountDown(int remainSeconds)
  {
    bundleCountDownText.InitCountDown();

    bundleCountDownText.SetCountDown(remainSeconds);
  }

  private void SetDefaultButton()
  {
    selectButton.gameObject.SetActive(true);
    completeButton.transform.parent.gameObject.SetActive(false);
  }

  public void ClearData()
  {
    SetDefaultButton();

    itemSlot.ClearData();

    bundleCountDownText.InitCountDown();

  }

}
