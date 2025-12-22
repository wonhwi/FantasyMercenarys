using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SlotState = MailboxView.SlotState;

public class MailboxSlot : MonoBehaviour
{
  private const string newMailIcon  = "fm_menu_new_mail_icon";
  private const string readMailIcon = "fm_menu_read_mail_icon";

  [SerializeField] private Image mailIcon;
  [SerializeField] private Button mailButton;

  [SerializeField] private TextMeshProUGUI titleText;   //우편 제목
  [SerializeField] private TextMeshProUGUI dateText;    //우편 시간

  [SerializeField] private MailItemSlot mailItemSlot;

  public void SetData(MailData mailData, Action<long> onClick)
  {
    SlotState slotState = (SlotState)mailData.mailState;

    List<InvenData> rewardItemList = mailData.rewardList;

    SetMailState(slotState);
    SetTitleText(mailData.mailTitle);
    SetDateText(mailData.regDt);
    SetMailReward(slotState, rewardItemList);

    mailButton.onClick.RemoveAllListeners();
    mailButton.onClick.AddListener(() => onClick?.Invoke(mailData.mailIdx));
  }

  public void SetMailState(SlotState slotState)
  {
    mailIcon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, 
      slotState == SlotState.NewMail ? newMailIcon : readMailIcon);
  }
  
  public void SetTitleText(string text)
  {
    titleText.text = text;
  }

  public void SetDateText(long time)
  {
    DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime.AddHours(9);

    dateText.text = dateTime.ToString("yyyy'/'M'/'d H:mm");
  }

  public void SetMailReward(SlotState slotState, List<InvenData> rewardItemList)
  {
    bool isValidReward = rewardItemList != null && rewardItemList.Count > 0;

    if (isValidReward)
    {
      ItemData itemData = ItemTable.getInstance.GetItemData(rewardItemList[0].itemIdx);

      mailItemSlot.SetItemCountData(itemData, (int)rewardItemList[0].itemCount);
      mailItemSlot.gameObject.SetActive(true);
    }
    else
    {
      mailItemSlot.ClearData();
    }

    mailItemSlot.SetCheckBox(slotState == SlotState.ClaimMail);
  }
    
}
