using FantasyMercenarys.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailboxView : MonoBehaviour
{
  public enum SlotState
  {
    NewMail = 0,   //새로운 메일
    ReadMail = 1,  //확인 완료
    ClaimMail = 2, //보상 받음
  }

  public Action<long> OnMailSlotClicked;
  public Action<long> OnClaimMail; //메일 수령

  public Action OnDeleteReadMail;   //읽은 우편 삭제
  public Action OnClaimAllRewards;  //모든 보상 획득
  public Action OnCloseClicked;     //확인 (닫기 버튼)

  [Header("Mail Detail Class")]
  [SerializeField] public MailboxDetail mailDetail;

  [SerializeField] private readonly List<MailboxSlot> activeSlots = new();

  [SerializeField] public RectTransform contentsParent;
  [SerializeField] private Button closeButton;
  [SerializeField] private Button deleteMailButton;
  [SerializeField] private Button claimAllButton;


  private void Awake()
  {
    closeButton.onClick.AddListener(() => {
      OnCloseClicked?.Invoke();
    });

    deleteMailButton.onClick.AddListener(() => OnDeleteReadMail?.Invoke());
    claimAllButton.onClick.AddListener(() => OnClaimAllRewards?.Invoke());
  }

  public void SetSlots(List<MailboxSlot> mailboxSlotList, List<MailData> mailList)
  {
    contentsParent.anchoredPosition = Vector2.zero;

    // 기존 슬롯 비활성화
    foreach (var slot in activeSlots)
      slot.gameObject.SetActive(false);

    activeSlots.Clear();

    for (int i = 0; i < mailList.Count; i++)
    {
      var slot = mailboxSlotList[i];

      slot.SetData(mailList[i], OnMailSlotClicked);
      slot.gameObject.SetActive(true);
      activeSlots.Add(slot);
    }
  }

  public void ShowDetail(MailData mailData)
  {
    mailDetail.SetData(mailData, OnClaimMail);
  }
}
