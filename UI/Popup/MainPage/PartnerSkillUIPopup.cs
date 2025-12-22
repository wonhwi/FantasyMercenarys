using Assets.ZNetwork;
using Assets.ZNetwork.Data;
using Cysharp.Threading.Tasks;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UI;
using static RestPacket;

/// <summary>
/// 파트너 , 스킬 팝업을 같이 사용하는 PopUp
/// </summary>
public abstract class PartnerSkillUIPopup : UIBaseController
{
  public enum MountSlotState
  {
    Empty = 0,
    Mount = 1,
    Lock = 2,
  }

  [System.Serializable]
  public struct MountItemSlot
  {
    public InvenDataSlot mountInvenSlot;
    public Button unMountButton;            //장착 해제 버튼
    public GameObject[] bundleMountState; //Skll Mount 상태시 활성화 Bundle GameObject
    public GameObject bundleEmptyState;
    public GameObject bundleLockState;    //Lock 상태시 활성화 Bundle GameObject
  }

  [SerializeField] protected UITotalStat uiTotalStat;

  protected ItemTable itemTable;
  protected GameDataManager gameDataManager;
  protected NewInGameManager inGameManager;

  protected List<InvenData> presetInvenDataList = new();
  protected List<InvenData> mountInvenDataList = new();             //장착 중인 InvenDataList
  protected Dictionary<long, InvenData> invenDataList = new();      //현재 내가 가지고 있는 InvenDataList
  

  protected List<InvenData> invenEnhanceList = new();   //강화 List
  protected List<InvenData> invenCombineList = new();   //합성 List

  private List<int> enhanceItemList = new List<int>();
  private List<int> combineItemList = new List<int>();


  protected List<int> itemTableKeyList = new();

  protected Action<InvenData> OnClickMount;
  protected Action<InvenData> OnClickUnMount;
  protected Action<InvenData> OnClickEnhance;
  protected Action<InvenData> OnClickCombine;


  [SerializeField] protected Inventory<PrefabUISlot> inventory = new Inventory<PrefabUISlot>();

  [SerializeField] protected PresetType presetType;
  [SerializeField] protected ItemGroup popUpItemGroup;

  //추후 레벨업에 따른 슬롯 개방 변수 필요
  private int limitMountCount = 10;//4;

  [Header("UI Component")]
  [Header("UI Left Component")]
  [SerializeField] protected MountItemSlot[] mountItemSlots;

  [Header("UI Right Component")]
  [SerializeField] protected RectTransform contentsParent;
  [SerializeField] private ScrollRect contentsScroll;

  [Header("UI Item Interaction")]
  [SerializeField] private Button mountItemButton;   
  [SerializeField] private Button enhanceItemButton;
  [SerializeField] private Button combineItemButton;

  [SerializeField] private Button closeButton;

  protected override void Awake()
  {
    base.Awake();

    itemTable = ItemTable.getInstance;
    gameDataManager = GameDataManager.getInstance;
    inGameManager = NewInGameManager.getInstance;

    mountItemButton  .onClick.AddListener(OnClickMountAllItem);
    closeButton      .onClick.AddListener(Hide);

    OnClickMount    += this.OnClickMountItem;
    OnClickUnMount  += this.OnClickUnMountItem;
    OnClickEnhance  += this.OnClickEnhanceItem;
    OnClickCombine  += this.OnClickCombineItem;

    combineItemButton.onClick.AddListener(() => OnClickCombineAllItem(this.invenCombineList));
    enhanceItemButton.onClick.AddListener(() => OnClickEnhanceAllItem(this.invenEnhanceList));


    SetButtonEvent();

    SetItemTableTarget();

    inventory.Init(NewResourcePath.PREFAB_UI_POPUP_SLOT, contentsParent, itemTableKeyList.Count);
  }

  private void SetButtonEvent()
  {
    for (int i = 0; i < mountItemSlots.Length; i++)
    {
      int index = i;
      mountItemSlots[index].unMountButton.onClick.RemoveAllListeners();
      mountItemSlots[index].unMountButton.onClick.AddListener(() => 
      {
        OnClickUnMount?.Invoke(mountItemSlots[index].mountInvenSlot.GetInvenData());

      });

    }
  }

  /// <summary>
  /// 팝업 별 Target Table
  /// </summary>
  protected abstract void SetItemTableTarget();

  /// <summary>
  /// 인벤데이터 설정
  /// 하위 클래스에서 설정 가능하게
  /// </summary>
  protected abstract void GetInvenData();
  
  /// <summary>
  /// 아이템 장착시 실행하는 Action Call
  /// </summary>
  /// <param name="index"></param>
  /// <param name="invenData"></param>
  protected abstract void OnClickMountAction(int index, InvenData invenData);

  /// <summary>
  /// 아이템 장착 해제 시 실행하는 Action Call
  /// </summary>
  /// <param name="index"></param>
  /// <param name="invenData"></param>
  protected abstract void OnClickUnMountAction(int index, InvenData invenData);

  public virtual void Init()
  {
    this.contentsParent.anchoredPosition = Vector2.zero;

    this.GetInvenData();

    this.LoadMountInventory();
    this.LoadItemCollection();
    this.LoadItemInventory();
    this.SetItemInteractionUI();

    this.SetItemGroupHasValue();

  }

  /// <summary>
  /// 좌측 하단 총 아이템 보유 효과
  /// </summary>
  private void SetItemGroupHasValue()
  {
    uiTotalStat.SetItemGroupHasValue(this.popUpItemGroup);
  }

  /// <summary>
  /// 내가 소지하지않은 아이템 도감 출력
  /// </summary>
  protected void LoadItemCollection()
  {
    this.ClearInventory();

    for (int i = 0; i < itemTableKeyList.Count; i++)
    {
      ItemData itemData = itemTable.GetItemData(itemTableKeyList[i]);

      PrefabUISlot prefabUISlot = inventory.GetItemSlot();

      prefabUISlot.SetItemData(itemData);

      prefabUISlot.ActiveBlind(true);
      prefabUISlot.OnClickItemDataEvent((itemData) => UIUtility.ActiveItemInfo(itemData));
      prefabUISlot.gameObject.SetActive(true);
    }

  }

  /// <summary>
  /// 내가 소지하고 있는 모든 인벤토리 아이템들을 출력
  /// </summary>
  protected void LoadItemInventory()
  {
    foreach (var invenData in invenDataList)
    {
      UpdateInventory(invenData.Value);
    }

  }

  /// <summary>
  /// 특정 아이템들을 업데이트
  /// </summary>
  /// <param name="invenDataList"></param>
  protected void UpdateInventory(IEnumerable<InvenData> invenDataList)
  {
    foreach (var item in invenDataList)
    {
      UpdateInventory(item);
    }
  }


  /// <summary>
  /// 특정 인벤토리 업데이트
  /// </summary>
  /// <param name="invenData"></param>
  protected void UpdateInventory(InvenData invenData)
  {
    if (invenData != null)
    {
      PrefabUISlot prefabUISlot = inventory.FindItemSlot(n => n.GetItemIndex().Equals(invenData.itemIdx));

      bool isMount = mountInvenDataList.Exists(n => n != null && n.invenIdx.Equals(invenData.invenIdx));

      prefabUISlot.SetInvenData(invenData, isMount);

      prefabUISlot.ActiveBlind(false);
      prefabUISlot.OnClickInvenDataEvent((data) =>
      {
        UIUtility.ActiveItemInfoPopUp(
            data, 
            isMount,
            attachAction  : (invenData) => OnClickMount?.Invoke(invenData),
            detachAction  : (invenData) => OnClickUnMount?.Invoke(invenData),
            enhanceAction : (invenData) => OnClickEnhance?.Invoke(invenData),
            combineAction : (invenData) => OnClickCombine?.Invoke(invenData)
          );

      });
      prefabUISlot.OnClickAttachEvent(OnClickMount);
      prefabUISlot.OnClickDetachEvent(OnClickUnMount);
      prefabUISlot.gameObject.SetActive(true);

    }
  }
  

  /// <summary>
  /// 내가 현재 장착한 아이템들의 대한 정보 토대로 출력
  /// </summary>
  protected void LoadMountInventory()
  {

    for (int index = 0; index < mountItemSlots.Length; index++)
    {
      MountItemSlot mountItemSlot = mountItemSlots[index];

      MountSlotState mountSlotState = MountSlotState.Empty;


      if (index >= limitMountCount)
      {
        mountSlotState = MountSlotState.Lock;
      }
      else
      {
        if(index < mountInvenDataList.Count)
        {
          InvenData invenData = mountInvenDataList[index];

          if (invenData != null)
          {
            mountSlotState = MountSlotState.Mount;

            mountItemSlot.mountInvenSlot.SetInvenLvData(invenData);
            mountItemSlot.mountInvenSlot.OnClickInvenDataEvent(
              (data) =>

              UIUtility.ActiveItemInfoPopUp(
                data,
                isMount: true,
                detachAction  : (invenData) => OnClickUnMount?.Invoke(invenData),
                enhanceAction : (invenData) => OnClickEnhance?.Invoke(invenData),
                combineAction : (invenData) => OnClickCombine?.Invoke(invenData)
              ));
          }
        }

      }

      SetMountUI(mountItemSlot, mountSlotState);
    }

    void SetMountUI(MountItemSlot mountItemSlot, MountSlotState mountSlotState)
    {
      MountSlotState state = mountSlotState;

      for (int i = 0; i < mountItemSlot.bundleMountState.Length; i++)
        mountItemSlot.bundleMountState[i].SetActive(state.Equals(MountSlotState.Mount));

      mountItemSlot.bundleLockState.SetActive(state.Equals(MountSlotState.Lock));
      mountItemSlot.bundleEmptyState.SetActive(state.Equals(MountSlotState.Empty) || state.Equals(MountSlotState.Lock));

    }
  }


  /// <summary>
  /// 아이템 상호 작용 관련 버튼 설정
  /// </summary>
  private void SetItemInteractionUI()
  {
    bool isEnhanceItems = this.IsCanEnhanceItem();
    bool isCombineItems = this.IsCanCombineItem();

    this.combineItemButton.gameObject.SetActive(isCombineItems);
    this.enhanceItemButton.gameObject.SetActive(isEnhanceItems);
  }

  /// <summary>
  /// 자동으로 전투력이 높은 아이템들을 판단해서 반환해 주는 함수
  /// </summary>
  /// <returns></returns>
  protected IEnumerable<InvenData> GetAutoMountItem()
  {
    IEnumerable<InvenData> mountItemList = invenDataList.Values
      .OrderByDescending(n => itemTable.GetItemData(n.itemIdx).itemGrade)
      .ThenByDescending(m => m.itemLv)
      .ThenByDescending(k => k.itemIdx)
      .Take(mountItemSlots.Length);

    return mountItemList;
  }


  protected void ClearInventory()
  {
    inventory.ClearItemSlots();
  }

  /// <summary>
  /// 인벤데이터가 현재 MountSlot의 몇 번째 인덱스에 있는지
  /// -1 = 장착이 되어있지 않은 상태
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  protected int GetMountSlotInvenIndex(InvenData invenData)
  {
    int findIndex = mountInvenDataList.FindIndex(n => n != null && n.invenIdx == invenData.invenIdx);

    return findIndex;
  }

  /// <summary>
  /// 반응형 장착한 슬롯 데이터 업데이트 함수
  /// </summary>
  /// <param name="invenData"></param>
  protected virtual void UpdateMountSlotData(InvenData invenData)
  {
    int findIndex = GetMountSlotInvenIndex(invenData);

    if (!findIndex.Equals(-1))
    {
      mountInvenDataList[findIndex] = invenData;
    }
  }

  /// <summary>
  /// 아이템 장착
  /// </summary>
  protected void OnClickMountItem(InvenData invenData)
  {
    //현재 MountSlot에 invenData가 존재하면 Return 잘못된 flow
    int findIndex = mountInvenDataList.FindIndex(n => n == null);

    if (findIndex.Equals(-1))
      return;


    mountInvenDataList[findIndex] = invenData;

    this.LoadMountInventory();

    this.UpdateInventory(invenData);

    OnClickMountAction(findIndex, invenData);

    NewSoundManager.Instance.PlayUISFXSound("SFX_UI_SLOT_ON_0");
  }

  /// <summary>
  /// 아이템 장착 해제
  /// </summary>
  protected void OnClickUnMountItem(InvenData invenData)
  {
    int findIndex = GetMountSlotInvenIndex(invenData);

    if (findIndex.Equals(-1))
      return;

    mountInvenDataList[findIndex] = null;

    this.LoadMountInventory();

    this.UpdateInventory(invenData);

    OnClickUnMountAction(findIndex, invenData);

    NewSoundManager.Instance.PlayUISFXSound("SFX_UI_SLOT_OFF_0");
  }

  /// <summary>
  /// 아이템 일괄 장착
  /// </summary>
  protected virtual void OnClickMountAllItem()
  {
    //기존 장착한 mount정보와 같은지?
    bool isSameData = true;

    IEnumerable<InvenData> invenAllMountDataList = this.GetAutoMountItem();

    int index = 0;

    if (invenAllMountDataList.Count() == mountInvenDataList.Count)
    {
      foreach (var invenMountData in invenAllMountDataList)
      {
        if (mountInvenDataList[index] != invenMountData)
          isSameData = false;

        index++;
      }

      if (isSameData)
      {
        Debug.Log($"이미 최고 스킬들을 일괄 장착 하였습니다.");
        return;
      }
    }

    mountInvenDataList.Clear();

    foreach (var invenData in invenAllMountDataList)
      mountInvenDataList.Add(invenData);

    this.LoadMountInventory();
    this.LoadItemInventory();

    NewSoundManager.Instance.PlayUISFXSound("SFX_UI_SLOT_ON_0");
  }

  /// <summary>
  /// 강화 할 수 있는 아이템이 존재하는지 판단 및 강화 가능 InvenDataList 반환
  /// </summary>
  /// <returns></returns>
  protected bool IsCanEnhanceItem()
  {
    this.invenEnhanceList.Clear();

    foreach (var invenData in invenDataList.Values)
    {
      if (ItemUtility.IsUseEnhanceItem(invenData))
        this.invenEnhanceList.Add(invenData);
    }

    if (!this.invenEnhanceList.Count.Equals(0))
      return true;

    return false;
  }

  /// <summary>
  /// 합성 할 수 있는 아이템이 존재하는지 판단 및 합성 가능 InvenDataList 반환
  /// </summary>
  /// <returns></returns>
  protected bool IsCanCombineItem()
  {
    this.invenCombineList.Clear();
    
    foreach (var invenData in invenDataList.Values)
    {
      if(ItemUtility.IsUseCombineItem(invenData))
        this.invenCombineList.Add(invenData);
    }

    if (!this.invenCombineList.Count.Equals(0))
      return true;

    return false;
  }

  #region APIManager로 이동
  /// <summary>
  /// 선택 강화
  /// </summary>
  private async void OnClickEnhanceItem(InvenData invenData)
  {
    enhanceItemList.Clear();
    enhanceItemList.Add(invenData.itemIdx);

    await APIManager.getInstance.REQ_EnhanceItem<RES_Enhancement>((int)popUpItemGroup, enhanceItemList, EnhanceSucess);

  }

  /// <summary>
  /// 일괄 강화
  /// </summary>
  /// <param name="invenDataList"></param>
  private async void OnClickEnhanceAllItem(IEnumerable<InvenData> invenDataList)
  {
    enhanceItemList.Clear();

    foreach (var invenData in invenDataList)
      enhanceItemList.Add(invenData.itemIdx);

    await APIManager.getInstance.REQ_EnhanceItem<RES_Enhancement>((int)popUpItemGroup, enhanceItemList, EnhanceSucess);
  }

  /// <summary>
  /// 선택 합성
  /// </summary>
  private async void OnClickCombineItem(InvenData invenData)
  {
    combineItemList.Clear();
    combineItemList.Add(invenData.itemIdx);

    await APIManager.getInstance.REQ_CombineItem<RES_Combine>((int)popUpItemGroup, combineItemList, CombineSucess);
  }

  /// <summary>
  /// 일괄 합성
  /// </summary>
  /// <param name="invenDataList"></param>
  private async void OnClickCombineAllItem(IEnumerable<InvenData> invenDataList)
  {
    combineItemList.Clear();

    foreach (var invenData in invenDataList)
      combineItemList.Add(invenData.itemIdx);

    await APIManager.getInstance.REQ_CombineItem<RES_Combine>((int)popUpItemGroup, combineItemList, CombineSucess);
  }

  /// <summary>
  /// 강화 성공
  /// </summary>
  /// <param name="responseResult"></param>
  private async void EnhanceSucess(RES_Enhancement responseResult)
  {
    var popup = await NewUIManager.getInstance.Show<ItemEnhancePopup>(NewResourcePath.PREFAB_UI_POPUP_ENHANCE_ITEM_REWARD);
    popup.SetRewardData(responseResult.rewardList);

    foreach (InvenData invenData in responseResult.updateItemList)
    {
      if (invenData != null)
        UpdateMountSlotData(invenData);
    }


    this.SetItemGroupHasValue();

    this.LoadMountInventory();
    this.LoadItemInventory();

    this.SetItemInteractionUI();

  }

  /// <summary>
  /// 합성 성공
  /// </summary>
  /// <param name="responseResult"></param>
  private async void CombineSucess(RES_Combine responseResult)
  {
    var popup = await NewUIManager.getInstance.Show<ItemCombinePopup>(NewResourcePath.PREFAB_UI_POPUP_COMBINE_ITEM_REWARD);
    popup.SetRewardData(responseResult.rewardList);

    foreach (InvenData invenData in responseResult.updateItemList)
    {
      if (invenData != null)
        UpdateMountSlotData(invenData);
    }

    this.SetItemGroupHasValue();

    this.LoadMountInventory();
    this.LoadItemInventory();

    this.SetItemInteractionUI();


  }

  /// <summary>
  /// 클라이언트 내에서 장착, 장착해제 작업을 한 이후
  /// 프리셋과, 현재 장착 데이터가 서로 다를경우 해당 패킷을 서버에 쏴줌
  /// </summary>
  /// <returns></returns>
  private void SendMountPacket()
  {
    List<MountPacketData> mountPacketDataList = new List<MountPacketData>();

    int slotIndex = 0;

    foreach (var invenMountData in presetInvenDataList)
    {
      if (slotIndex >= mountInvenDataList.Count)
        break;

      if (mountInvenDataList[slotIndex] != invenMountData)
      {
        MountPacketData mountPacketData = new MountPacketData();

        mountPacketData.mountSlot = slotIndex + 1;
        mountPacketData.mountInvenIdx = mountInvenDataList[slotIndex] != null ? (int)mountInvenDataList[slotIndex].invenIdx : 0;

        mountPacketDataList.Add(mountPacketData);
      }
      slotIndex++;
    }

    if (mountPacketDataList.Count == 0)
    {
      Debug.Log("변경된 값이 없어 Packet을 전송하지 않습니다.");
      return;
    }

    APIManager.getInstance.REQ_MountItem<RES_MountItem>(1, (int)presetType, mountPacketDataList, null);
  }

  #endregion

  public override void Hide()
  {
    //여기에 비활성화시 서버 연결
    SendMountPacket();

    base.Hide();

    ClearInventory();
  }

}
