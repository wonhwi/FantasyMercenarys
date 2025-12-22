using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static RestPacket;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using Assets.ZNetwork.Data;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using System.Threading;
using System;


public enum PrayerSlotType
{
  Empty = 0,
  Using = 1,
  Drain = 2,
  Lock = 3,
}


public class PrayerCenterUIPopup : UIBaseInventory<PrayerUISlot>
{
  private UserContentsData userContentsData;

  [System.Serializable]
  public class PartnerPrayerInfo
  {
    public ItemSlot partnerSlot;
    public TextMeshProUGUI faithValue;
    public BundleGaugeText bundleGaugeText;

    public void SetData(ItemData itemData)
    {
      partnerSlot.SetItemData(itemData);

      faithValue.text = "0";

      bundleGaugeText.SetCustomData(0f, "???");
    }

    public void SetData(InvenData invenData)
    {
      ItemData itemData = ItemTable.getInstance.GetItemData(invenData.itemIdx);

      partnerSlot.SetItemData(itemData);

      faithValue.text = PartnerTable.getInstance.GetPartnerFaithMin(invenData.itemIdx, invenData.itemLv).ToString();

      bundleGaugeText.SetGaugeTextData(
        PartnerTable.getInstance.GetPrayerCurrentHP(invenData),
        PartnerTable.getInstance.GetPrayerMaxHP(invenData)
        );
    }
  }

  [Header("[인벤토리에 좌측에 사용되는 파트너 상세 정보]")]
  public PartnerPrayerInfo partnerPrayerInfo;

  [Header("[콘텐츠 재화]")]
  [SerializeField] private BundleCurrency bundleCurrency;

  [Header("[신앙심 분당 획득량]")]
  public TextMeshProUGUI faithPerText;  //신앙심 초당 획득 Text

  [Header("[기도원 슬롯]")]
  public PrayerSlot[] prayerSlots;

  [Header("[UI Component]")]
  public Button closeButton;
  public Button invenCloseButton;
  public Button invenApplyButton;

  private int slotIndex = -1;

  private CancellationTokenSource cancelTokenSource;

  protected override void Awake()
  {
    base.Awake();

    this.userContentsData = GameDataManager.getInstance.userContentsData;

    closeButton.onClick.AddListener(Hide);
    invenCloseButton.onClick.AddListener(CloseInven);
    invenApplyButton.onClick.AddListener(OnClickApply);

    InitPrayerSlotEvent();
  }

  /// <summary>
  /// Slot별 이벤트 등록
  /// </summary>
  private void InitPrayerSlotEvent()
  {
    for (int i = 0; i < prayerSlots.Length; i++)
    {
      int index = i;

      prayerSlots[index].OnUpdateBless = UpdateBlessPrayer;
      prayerSlots[index].slotButton.onClick.AddListener(() => {
        
        this.slotIndex = index;
        InitPrayerSlot(prayerSlots[index], index);

      });
    }
  }

  private void InitPrayerSlot(PrayerSlot prayerSlot, int slotIndex)
  {
    PrayerSlotType type = prayerSlot.prayerSlotType;

    switch (type)
    {
      case PrayerSlotType.Empty:
        LoadInven(ItemGroup.Partner);
        break;
      case PrayerSlotType.Using:
      case PrayerSlotType.Drain:
        OnUnMountSlot();
        break;
      case PrayerSlotType.Lock:

        var (unlockConditionType, unLockValue) = SlotUnlockConditionTable.getInstance.GetSlotUnlockConditionData(SlotUnlockContentType.Prayer, slotIndex + 1);

        UIUtility.ShowToastMessagePopup($"플레이어 Lv.{unLockValue} 달성 시 해제");
        break;
    }
  }


  /// <summary>
  /// 기도원 Canvas 활성화시 처음 실행시켜주는 함수
  /// </summary>
  /// <param name="responseResult"></param>
  public void Open(RES_BlessPrayerRcv responseResult)
  {
    cancelTokenSource = new CancellationTokenSource();

    var slotUnlockDataList = SlotUnlockConditionTable.getInstance.GetSlotUnlockConditionDataList(SlotUnlockContentType.Prayer);

    int userLevel = GameDataManager.getInstance.userInfoModel.GetPlayerLv();

    foreach ( var slotUnlockData in slotUnlockDataList)
    {
      int slotIndex = slotUnlockData.slotNumber - 1;

      if(userLevel >= slotUnlockData.unlockValue)
      {
        prayerSlots[slotIndex].SetSlot(PrayerSlotType.Empty);
      }
      else
      {
        prayerSlots[slotIndex].SetSlot(PrayerSlotType.Lock);
      }
    }

    foreach (var prayerData in responseResult.prayerDataList)
      SetMountSlot(prayerData);


    SetMountTotalFaith();

    bundleCurrency.Enable();
  }

  /// <summary>
  /// 신앙심 갱신 및 현재 동료들 상태 업데이트
  /// </summary>
  /// <param name="responseResult"></param>
  public void UpdateBlessPrayer(RES_BlessPrayerRcv responseResult)
  {
    foreach (var prayerData in responseResult.prayerDataList)
    {
      SetUpdateMountSlot(prayerData);
    }

    SetMountTotalFaith();
  }

  /// <summary>
  /// 기도원 진입 시, 슬롯 장착 시
  /// </summary>
  /// <param name="prayerData"></param>
  public void SetMountSlot(PrayerData prayerData)
  {
    int index = prayerData.slot - 1;

    this.prayerSlots[index].SetSpine(prayerData.partnerIdx);
    this.prayerSlots[index].SetData(prayerData);
    this.prayerSlots[index].SetGaugeText(prayerData);

    if (prayerData.hp > 0)
      this.prayerSlots[index].StartBlessUpdateTask(prayerData.lastFaithDate, cancelTokenSource.Token);
  }

  /// <summary>
  /// 기도원에 배치된 동료들의 상태 업데이트 할때 사용
  /// </summary>
  /// <param name="prayerData"></param>
  public void SetUpdateMountSlot(PrayerData prayerData)
  {
    int index = prayerData.slot - 1;

    this.prayerSlots[index].SetData(prayerData);
    this.prayerSlots[index].SetGaugeText(prayerData);
    
    if(prayerData.hp > 0 )
      this.prayerSlots[index].SetBlessTime(prayerData.lastFaithDate);
    else
      this.prayerSlots[index].CancelTask();
  }

  /// <summary>
  /// 현재 배치 되어진 동료들의 신앙심 총 값
  /// </summary>
  public void SetMountTotalFaith()
  {
    int totalValue = 0;

    foreach (PrayerData prayerData in userContentsData.prayerCenter.mountPrayerMap.Values)
    {
      if (prayerData.hp <= 0)
        continue;

      totalValue += PartnerTable.getInstance.GetPartnerFaithMin(prayerData.partnerIdx, prayerData.lv);
    }

    this.faithPerText.text = totalValue.ToString();
  }


  /// <summary>
  /// 인벤토리 활성화 이후 등록 버튼 눌렀을때 실행 함수
  /// </summary>
  public async void OnClickApply()
  {
    if (this.slotIndex == -1)
      return;

    PrayerUISlot prayerUISlot = GetSelectSlot();

    if (prayerUISlot == null)
      return;

    if(!prayerUISlot.IsInvenItem())
    {
      UIUtility.ShowToastMessagePopup("보유한 동료 등록 가능");
      return;
    }

    if(prayerUISlot.IsMount())
    {
      UIUtility.ShowToastMessagePopup("이미 등록 된 동료");
      return;
    }

    if (!PartnerTable.getInstance.IsCanMountPary(prayerUISlot.GetInvenData()))
    {
      int minHP = (int)BaseTable.getInstance.GetBaseValue((int)MetaBaseType.REQUIRED_MINIMUM_PRAYER_SLOT_FOR_PRAYER_HOUSE_HP);

      UIUtility.ShowToastMessagePopup($"최소 체력 {minHP}이상 등록 가능");
      return;
    }

    OnMountSlot(prayerUISlot);
  }


  private async void OnMountSlot(PrayerUISlot prayerUISlot)
  {
    await APIManager.getInstance.REQ_MountPrayerSlot<RES_MountPrayerSlot>(
      slotNum  : this.slotIndex + 1,
      invenIdx : prayerUISlot.GetInvenIndex(),
      action   : (responseResult) =>
      {
        PrayerData prayerData = responseResult.prayerDataList.FirstOrDefault();

        SetMountSlot(prayerData);

        SetMountTotalFaith();

        CloseInven();
      });
  }

  private async void OnUnMountSlot()
  {
    await APIManager.getInstance.REQ_MountPrayerSlot<RES_MountPrayerSlot>(
      slotNum  : this.slotIndex + 1,
      invenIdx : 0,
      action   : (responseResult) =>
      {
        prayerSlots[this.slotIndex].SetSlot(PrayerSlotType.Empty);
        prayerSlots[this.slotIndex].ClearPool();

        SetMountTotalFaith();
      });
  }


  /// <summary>
  /// 기도원 진입 후 클라이언트 내 자동 체력 회복 시스템
  /// 인벤토리 활성화 시 마다 판단해서 체력 올려놓고 인벤토리 마저 오픈
  /// </summary>
  private void RecoverPrayerPartner()
  {
    foreach (PartnerPrayData partnerPrayData in userContentsData.prayerCenter.dictPartnerPray.Values)
    {
      if (partnerPrayData.mountPrayerSlot != 0)
      {
        continue;
      }

      PartnerTable.getInstance.CalcRecoveryHp(partnerPrayData);
    }
  }
  

  protected override void LoadInven(ItemGroup itemGroup)
  {
    this.RecoverPrayerPartner();

    base.LoadInven(itemGroup);
  }


  /// <summary>
  /// 미 수집한 아이템 클릭시 발생하는 함수
  /// </summary>
  /// <param name="itemSlot"></param>
  protected override void OnClickItemSlot(PrayerUISlot itemSlot)
  {
    base.OnClickItemSlot(itemSlot);

    partnerPrayerInfo.SetData(itemSlot.GetItemData());
  }

  /// <summary>
  /// 인벤 아이템 클릭시 발생하는 함수
  /// </summary>
  /// <param name="invenSlot"></param>
  protected override void OnClickInvenSlot(PrayerUISlot invenSlot)
  {
    base.OnClickInvenSlot(invenSlot);

    partnerPrayerInfo.SetData(invenSlot.GetInvenData());
  }

  public override async void Hide()
  {
    //await APIManager.getInstance.REQ_LoadBlessingAndPrayer<RES_LoadBlessingAndPrayer>(null);

    base.Hide();

    slotIndex = -1;

    foreach (var prayerSlot in prayerSlots)
    {
      prayerSlot.ClearPool();
    }

    if (!cancelTokenSource.IsCancellationRequested)
    {
      cancelTokenSource.Cancel();
    }
    cancelTokenSource.Dispose();
    cancelTokenSource = null;

    bundleCurrency.Disable();
  }
}
