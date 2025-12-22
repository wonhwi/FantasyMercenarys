using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUISlot : InvenDataSlot
{
  private EquipmentItemData equipmentItemData;
  private UISpineGrade uISpineGrade;

  public EquipmentItemData GetEquipmentItemData()
    => equipmentItemData;

  public virtual void SetEquipmentData(EquipmentItemData equipmentItemData)
  {
    this.equipmentItemData = equipmentItemData;

    ItemData itemData = ItemTable.getInstance.GetItemData(equipmentItemData.equipItemIdx);

    base.SetItemData(itemData);
  }


  protected override void SetItemGrade(ItemData itemData)
  {
    if(!itemData.itemGradeSpine)
    {
      base.SetItemGrade(itemData);
    
      itemGrade.enabled = true;
      return;
    }

    itemGrade.enabled = false;

    if(!uISpineGrade)
    {
      uISpineGrade = UIPoolManager.Instance.GetPool<UISpineGrade>(UIPoolType.EquipmentItemGrade);
    }

    uISpineGrade.OnSetParent(itemGrade.transform);
    uISpineGrade.SetAnimation($"{itemData.itemGrade:D2}_{((ItemGradeType)itemData.itemGrade)}");
  }

  public override void SetItemLvText(InvenData invenData)
  {
    if (valueText)
    {
      ActiveLevelBox(true);
      //아이템 타입에 따라 나오는걸 다르게
      valueText.text = ItemUtility.GetItemLevelText(invenData); //갯수 또는 레벨
    }

  }

  public override void ClearReturnUIPool()
  {
    if (uISpineGrade != null)
    {
      UIPoolManager.Instance.ClearReturnUIPool<UISpineGrade>(UIPoolType.EquipmentItemGrade, itemGrade.transform);
      uISpineGrade = null;
    }
  }


}
