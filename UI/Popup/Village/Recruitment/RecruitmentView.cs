using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecruitmentView : MonoBehaviour 
{
  [SerializeField] private BundleUnlockSlot bundlePartnerUnlockSlot;

  [SerializeField] private RecruitmentSlot[] recruitSlots;

  [SerializeField] private Button closeButton;

  public Action OnCloseUIPopup;             //팝업 닫기
  public Action OnCloseCountDown;           //쿨타임 로직 다 비활성화
  public Action<InvenData> OnClickMount;    //인벤토리 에서 적용 버튼 눌렀을때 이벤트
  public Action<Predicate<PartnerData>> OnLoadInven;  //조건에 맞는 인벤토리 출력
  public RecruitmentSlot[] RecruitSlots => recruitSlots;


  private void Awake()
  {
    closeButton.onClick.AddListener(() => {
      OnCloseUIPopup?.Invoke();
      OnCloseCountDown?.Invoke();
    });
  }


  /// <summary>
  /// 잠금 조건 이벤트 설정
  /// </summary>
  public void InitUnlockSlot()
  {
    bundlePartnerUnlockSlot.SetSlot(recruitSlots);

    bundlePartnerUnlockSlot.SetData(SlotUnlockContentType.Recruitment);
  }

  public void OnApplyItem(InvenData invenData)
  {
    OnClickMount?.Invoke(invenData);
  }

}
