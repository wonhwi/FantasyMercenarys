using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using System;
using SlotState = MailboxView.SlotState;

public class MailboxDetail : MonoBehaviour
{
  [SerializeField] protected Inventory<MailItemSlot> inventory = new Inventory<MailItemSlot>();

  [SerializeField] private TextMeshProUGUI titleText;    //우편 제목
  [SerializeField] private TextMeshProUGUI dateText;     //우편 시간
  [SerializeField] private TextMeshProUGUI contentsText; //상세 내용

  [SerializeField] RectTransform contentsParent;
  [SerializeField] private GameObject rewardRoot;

  [Header("[UI Buttons]")]
  [SerializeField] Button claimButton;      //보상 받기 버튼
  [SerializeField] Button claimedButton;    //보상 받은 상태
  [SerializeField] Button backGroundButton; //뒷 배경 닫기 버튼

  public void Awake()
  {
    backGroundButton.onClick.AddListener(Hide);

    inventory.Init(NewResourcePath.PREFAB_UI_MAILBOX_ITEM_SLOT, contentsParent, 30);
  }

  public void SetData(MailData mailData, Action<long> onClaim)
  {
    this.gameObject.SetActive(true);

    contentsParent.anchoredPosition = Vector2.zero;

    SetTitleText(mailData.mailTitle);
    SetDateText(mailData.regDt);
    SetContentText(mailData.mailContents);
    SetRewardItem(mailData.rewardList);
    SetButton(mailData);

    claimButton.onClick.RemoveAllListeners();
    claimButton.onClick.AddListener(() => onClaim?.Invoke(mailData.mailIdx));
  }

  private void SetTitleText(string text)
  {
    titleText.text = text;
  }

  private void SetDateText(long time)
  {
    DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime.AddHours(9);

    dateText.text = dateTime.ToString("yyyy'/'M'/'d H:mm");
  }

  private void SetContentText(string text)
  {
    contentsText.text = text;
  }

  private void SetRewardItem(List<InvenData> rewardItemList)
  {
    if(rewardItemList != null)
    {
      for (int i = 0; i < rewardItemList.Count; i++)
      {
        MailItemSlot mailItemSlot = inventory.GetItemSlot();

        int itemIdx = rewardItemList[i].itemIdx;
        long itemCount = rewardItemList[i].itemCount;

        ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

        mailItemSlot.SetItemCountData(itemData, (int)itemCount);
        mailItemSlot.gameObject.SetActive(true);
      }
    }
  }

  private void SetButton(MailData mailData)
  {
    //단순 메시지 우편 여부
    bool isRewardMail = mailData.rewardList != null && mailData.rewardList.Count > 0;

    rewardRoot.SetActive(isRewardMail);

    if (isRewardMail)
    {
      SlotState slotState = (SlotState)mailData.mailState;

      claimButton.gameObject.SetActive(slotState == SlotState.ReadMail);
      claimedButton.gameObject.SetActive(slotState == SlotState.ClaimMail);
    }
    else
    {
      claimButton.gameObject.SetActive(false);
      claimedButton.gameObject.SetActive(false);
    }

  }

  public void Hide()
  {
    this.gameObject.SetActive(false);

    inventory.ClearItemSlots();
  }
}
