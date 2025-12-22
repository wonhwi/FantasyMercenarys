using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static RestPacket;
using Newtonsoft.Json;
using System;
using Cysharp.Threading.Tasks;

public class ItemCraftUIPopup : UIBaseInventory<InvenDataSlot>
{

  private ContentPartnerSkillCraft contentPartnerSkillCraft;

  [Header("[UI Inventory Component]")]
  [SerializeField] private BundleConsumeMaterial bundleConsumeMaterial;
  [SerializeField] private ConsumeMaterialData[] consumeMeterials = new ConsumeMaterialData[2];
  [SerializeField] private Button invenCloseButton;
  [SerializeField] private Button invenApplyButton;

  [Header("[UI ETC]")]
  [SerializeField] private Button closeButton;

  [SerializeField] private BundleUnlockSlot bundlePartnerUnlockSlot;
  [SerializeField] private BundleUnlockSlot bundleSkillUnlockSlot;

  [SerializeField] private ItemCraftSlot[] partnerCraftSlots;
  [SerializeField] private ItemCraftSlot[] skillCraftSlots;

  [SerializeField] private GameObject noticeText;

  public Action OnClickMount;

  protected override void Awake()
  {
    base.Awake();

    closeButton.onClick.AddListener(Hide);
    invenCloseButton.onClick.AddListener(CloseInven);
    invenApplyButton.onClick.AddListener(() => OnClickMount?.Invoke());

    InitSlotEvent();

  }

  /// <summary>
  /// 슬롯 이벤트 
  /// </summary>
  private void InitSlotEvent()
  {
    bundlePartnerUnlockSlot.SetSlot(partnerCraftSlots);
    bundleSkillUnlockSlot.SetSlot(skillCraftSlots);

    bundlePartnerUnlockSlot.SetData(SlotUnlockContentType.PartnerCraft);
    bundleSkillUnlockSlot.SetData(SlotUnlockContentType.SkillCraft);
  }


  public void Open()
  {
    if (contentPartnerSkillCraft == null)
      contentPartnerSkillCraft = GameDataManager.getInstance.userContentsData.partnerSkillCraft;

    UpdateCraftSlots();

    bundlePartnerUnlockSlot.UpdateData();
    bundleSkillUnlockSlot.UpdateData();
  }

  

  private void UpdateCraftSlots()
  {
    List<PSCraftData> craftPartnerList = contentPartnerSkillCraft.GetCraftData(ItemGroup.Partner);
    List<PSCraftData> craftSkillList = contentPartnerSkillCraft.GetCraftData(ItemGroup.Skill);

    for (int i = 0; i < craftPartnerList.Count; i++)
      UpdateCraftSlot(craftPartnerList[i]);

    for (int i = 0; i < craftSkillList.Count; i++)
      UpdateCraftSlot(craftSkillList[i]);

  }

  private void UpdateCraftSlot(PSCraftData craftData)
  {
    ItemGroup itemGroup = (ItemGroup)craftData.craftType;

    int slotIndex = craftData.slotNo - 1;
    int totalSecend = CodeUtility.GetTotalSeconds(craftData.endDate);

    ItemCraftSlot craftSlot = GetCraftSlot(itemGroup, slotIndex);

    craftSlot.ClearData();

    craftSlot.OnClickInven   = () => { 
      LoadInven(itemGroup);
      OnClickMount = () => OnMountSlot(itemGroup, craftData.slotNo);
    };
    craftSlot.OnClickCancel   = () => OnUnMountSlot(itemGroup, craftData.slotNo);
    craftSlot.OnClickComplete = () => OnCompleteSlot(itemGroup, craftData.slotNo);

    craftSlot.SetData(craftData);

    if(craftData.isCrafting)
      craftSlot.SetCountDown(totalSecend);
  }


  private ItemCraftSlot[] GetCraftSlots(ItemGroup itemGroup)
  {
    if (itemGroup == ItemGroup.Partner)
      return partnerCraftSlots;
    else if (itemGroup == ItemGroup.Skill)
      return skillCraftSlots;
    else
      return null;
  }

  private ItemCraftSlot GetCraftSlot(ItemGroup itemGroup, int slotIndex)
  {
    return GetCraftSlots(itemGroup)[slotIndex];
  }


  private async void OnMountSlot(ItemGroup itemGroup, int slotNo)
  {
    if (!bundleConsumeMaterial.IsEnought())
    {
      UIUtility.ShowToastMessagePopup("제작 등록에 필요한 재화 부족");
      return;
    }
      

    var invenDataSlot = GetSelectSlot();

    await APIManager.getInstance.REQ_PartnerSkillCraftService<RES_PartnerSkillCraftService>((int)itemGroup, slotNo, invenDataSlot.GetItemIndex(), true, (responseResult) =>
    {
      UpdateCraftSlot(responseResult.psCraftData);
      CloseInven();
    });
  }

  private async void OnUnMountSlot(ItemGroup itemGroup, int slotNo)
  {
    int slotIndex = slotNo - 1;

    ItemCraftSlot itemCraftSlot = GetCraftSlot(itemGroup, slotIndex);
    ItemSlot itemSlot = itemCraftSlot.itemSlot;

    UIUtility.ShowNotificationThreePopup(
      "알림",
      "제작 취소 또는 제작 가속을 하시겠습니까?",

      confirmAction: async () =>
      {

        await UniTask.DelayFrame(1);

        UIUtility.ShowNotificationPopup(
        "알림",
        "제작을 취소 하시겠습니까?<br>취소 시, 사용한 재료는 반환 되지 않습니다.",
        async () => await APIManager.getInstance.REQ_PartnerSkillCraftService<RES_PartnerSkillCraftService>((int)itemGroup, slotNo, itemSlot.GetItemIndex(), false,
        (responseResult) =>
        {
          UpdateCraftSlot(responseResult.psCraftData);
        }));

      },
      subAction: async () => {
        var popup = await NewUIManager.getInstance.Show<EquipmentFastTimePopupController>("FantasyMercenary/Popup/Equipment/EquipmentFastTimePopup");
        popup.SetLeftTime(10000);
      },
      confirmText : "제작 취소",
      subText : "가속",
      cancelText : "닫기"
      );
  }

  private async void OnCompleteSlot(ItemGroup itemGroup, int slotNo)
  {
    int slotIndex = slotNo - 1;

    ItemCraftSlot itemCraftSlot = GetCraftSlot(itemGroup, slotIndex);
    ItemSlot itemSlot = itemCraftSlot.itemSlot;


    await APIManager.getInstance.REQ_PartnerSkillCraftComplete<RES_PartnerSkillCraftComplete>((int)itemGroup, slotNo, async (responseResult) =>
    {
      UpdateCraftSlot(responseResult.psCraftData);

      UIUtility.ShowRewardItemPopup(responseResult.rewardList);
    });
  }


  protected override void InitItemFilter()
  {
    base.partnerFilter = n => n.isCraft;
    base.skillFilter   = n => n.isCraft;
  }

  protected override void SetInvenData(InvenDataSlot prefabUISlot, InvenData invenData)
  {
    prefabUISlot.SetInvenData(invenData);

    prefabUISlot.OnClickInvenDataEvent((_) => OnClickInvenSlot(prefabUISlot));
  }

  protected override void LoadInven(ItemGroup itemGroup)
  {
    base.LoadInven(itemGroup);

    bool isValid = IsValidItem();

    noticeText.SetActive(!isValid);
    invenApplyButton.interactable = isValid;

    if (!isValid)
    {
      var conditionData = PartnerSkillCraftConditionTable.getInstance.GetCraftConditionDefaultData(itemGroup);

      SetBundleConsumeData(conditionData, false);
    }
  }

  protected override void OnClickInvenSlot(InvenDataSlot itemSlot)
  {
    base.OnClickInvenSlot(itemSlot);

    ItemData itemData = itemSlot.GetItemData();

    var conditionData = PartnerSkillCraftConditionTable.getInstance.GetCraftConditionData((ItemGroup)itemData.itemGroup, 
                                                                                          (ItemGradeType)itemData.itemGrade);

    SetBundleConsumeData(conditionData, true);
  }

  private void SetBundleConsumeData(PartnerSkillCraftConditionData conditionData, bool isHasItem)
  {
    int dataCount = 0;

    if (conditionData.materialIdx1 != 0)
    {
      consumeMeterials[0] = new ConsumeMaterialData(conditionData.materialIdx1, isHasItem ? conditionData.materialCount1 : 0);
      dataCount++;
    }

    if (conditionData.materialIdx2 != 0)
    {
      consumeMeterials[1] = new ConsumeMaterialData(conditionData.materialIdx2, isHasItem ? conditionData.materialCount2 : 0);
      dataCount++;
    }

    bundleConsumeMaterial.SetConsumeMaterial(consumeMeterials, dataCount);
    bundleConsumeMaterial.SetRequiredTimeMinute(isHasItem ? conditionData.craftTime : 0);
  }
  
}
