using System;
using UnityEngine.Events;
using FantasyMercenarys.Data;
using System.Collections.Generic;

public static class UIUtility
{
  public static async void ShowMailRewardItemPopup(List<InvenData> rewardItemList)
  {
    if (rewardItemList != null && rewardItemList.Count > 0)
    {
      var rewardPopup = await NewUIManager.getInstance.Show<MailRewardPopup>(NewResourcePath.PREFAB_UI_POPUP_MAIL_ITEM_REWARD);

      rewardPopup.SetRewardData(rewardItemList);
    }
  }

  public static async void ShowRewardItemPopup(List<InvenData> rewardItemList)
  {
    var gachaPopup = await NewUIManager.getInstance.Show<ItemRewardPopup>(NewResourcePath.PREFAB_UI_POPUP_ITEM_REWARD);

    gachaPopup.SetRewardData(rewardItemList);
  }

  public static async void ShowToastMessagePopup(string message)
  {
    var popup = await NewUIManager.getInstance.Show<ToastMessagePopup>(NewResourcePath.PREFAB_UI_POPUP_TOASTMESSAGE, _setAutoOrder: false);

    popup.Open(message);
  }

  /// <summary>
  /// 팝업 호출
  /// </summary>
  /// <param name="title"></param>
  /// <param name="contents"></param>
  public static async void ShowNotificationNoticePopup(string title, string contents, Action confirmAction = null)
  {
    var popup = await NewUIManager.getInstance.Show<NotificationPopup>(NewResourcePath.PREFAB_UI_POPUP_NOTIFICATION, _setAutoOrder: false);

    popup.OpenConfirm(title, contents, confirmAction);
  }

  /// <summary>
  /// 팝업 호출
  /// </summary>
  /// <param name="title"></param>
  /// <param name="contents"></param>
  public static async void ShowNotificationPopup(string title, string contents, Action confirmAction = null)
  {
    var popup = await NewUIManager.getInstance.Show<NotificationPopup>(NewResourcePath.PREFAB_UI_POPUP_NOTIFICATION, _setAutoOrder: false);

    popup.OpenConfirmCancel(title, contents, confirmAction);
  }

  public static async void ShowNotificationConsumePopup(string title, string contents, int itemIdx, long targetValue, Action confirmAction = null)
  {
    var popup = await NewUIManager.getInstance.Show<NotificationConsumePopup>(NewResourcePath.PREFAB_UI_POPUP_CONSUME_NOTIFICATION, _setAutoOrder: false);

    popup.Open(title, contents, itemIdx, targetValue, confirmAction);
  }


  /// <summary>
  /// 팝업 호출
  /// </summary>
  /// <param name="title"></param>
  /// <param name="contents"></param>
  public static async void ShowNotificationThreePopup(string title, string contents, Action confirmAction = null, Action subAction = null, string confirmText = "확인", string cancelText = "취소", string subText = "서브")
  {
    var popup = await NewUIManager.getInstance.Show<NotificationPopup>(NewResourcePath.PREFAB_UI_POPUP_NOTIFICATION, _setAutoOrder: false);

    popup.OpenConfirmSubCancel(title, contents, confirmAction, subAction, confirmText, cancelText, subText);
  }



  public static async void ActiveItemInfo(ItemData itemData)
  {
    var itemInfoPopup = await NewUIManager.getInstance.Show<ItemInfoPopup>(NewResourcePath.PREFAB_UI_POPUP_ITEM_INFO);

    itemInfoPopup.SetData(itemData);
  }

  public static async void ActiveItemInfo(InvenData invenData, bool isMount = false)
  {
    var itemInfoPopup = await NewUIManager.getInstance.Show<ItemInfoPopup>(NewResourcePath.PREFAB_UI_POPUP_ITEM_INFO);

    itemInfoPopup.SetData(invenData, isMount);
  }


  public static async void ActiveItemInfoPopUp(InvenData invenData, bool isMount,
    UnityAction<InvenData> attachAction = null,
    UnityAction<InvenData> detachAction = null,
    UnityAction<InvenData> enhanceAction = null,
    UnityAction<InvenData> combineAction = null)
  {
    var itemInfoPopup = await NewUIManager.getInstance.Show<ItemInfoPopup>(NewResourcePath.PREFAB_UI_POPUP_ITEM_INFO);

    itemInfoPopup.SetDataPopUp(invenData, isMount,
          attachAction: (invenData) => attachAction?.Invoke(invenData),
          detachAction: (invenData) => detachAction?.Invoke(invenData),
          enhanceAction: (invenData) => enhanceAction?.Invoke(invenData),
          combineAction: (invenData) => combineAction?.Invoke(invenData));
  }
} 