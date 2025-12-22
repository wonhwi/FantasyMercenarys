using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUIPopup : UIBaseController
{
  public EquipmentUISlot[] equipmentUISlotArray;

  [SerializeField] private Button closeButton;

  private List<InvenData> equipmentInvenList = new List<InvenData>();

  protected override void Awake()
  {
    base.Awake();

    closeButton.onClick.AddListener(Hide);
  }

  public void Open()
  {
    equipmentInvenList = GameDataManager.getInstance.GetPresetInvenDataList(PresetType.Equipment);

    int itemCount = equipmentInvenList.Count;

    for (int i = 0; i < itemCount; i++)
    {
      int index = i;

      EquipmentMountType mountType = (EquipmentMountType)(index + 1);

      EquipmentUISlot equipmentUISlot = equipmentUISlotArray[index];

      InvenData invenData = equipmentInvenList[index];

      if (invenData == null)
      {
        equipmentUISlot.itemButton.onClick.RemoveAllListeners();
        //장비 아이콘 출력 (아이템 말고)
        equipmentUISlot.itemButton.image.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, $"fm_equipment_{mountType.ToString().ToLower()}_icon");
        equipmentUISlot.itemGrade.enabled = false;
        equipmentUISlot.valueText.text = string.Empty;
        continue;
      }


      EquipmentItemData equipmentItemData = EquipmentItemTable.getInstance.GetEquipmentItemData(invenData.itemIdx);
      
      equipmentUISlot.SetInvenLvData(invenData);

      equipmentUISlot.OnClickInvenDataEvent(OnClickItemSlot);
    }
  }

  private async void OnClickItemSlot(InvenData invenData)
  {
    var popup = await NewUIManager.getInstance.Show<EquipmentItemUIPopup>(NewResourcePath.PREFAB_UI_POPUP_EQUIPMENT_ITEM_INFO);
    popup.Open(invenData);
  }

  public override void Hide()
  {
    base.Hide();

    equipmentInvenList.Clear();

    for (int i = 0; i < equipmentUISlotArray.Length; i++)
    {
      equipmentUISlotArray[i].ClearReturnUIPool();
    }

  }
}
