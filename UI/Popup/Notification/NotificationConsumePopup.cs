using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NotificationConsumePopup : UIBaseNotification
{
  [Header("[UI Component]")]
  public BundleConsume bundleConsume;

  public void Open(string title, string message, int itemIdx, long targetValue, Action confirmAction = null)
  {
    base.OpenNotice(title, message);

    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    ConsumeRequirement consumeRequirement = new ConsumeRequirement(itemIdx, targetValue);

    bundleConsume.SetConsumeData(consumeRequirement);

    SetActiveButton(confirmButton, true);
    SetActiveButton(cancelButton, true);

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => {

      if(!bundleConsume.CanConsume())
      {
        UIUtility.ShowToastMessagePopup("재화가 부족합니다");
        return;
      }

      confirmAction?.Invoke();
      Hide();
    });
  }
}
