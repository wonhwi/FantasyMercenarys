using Assets.ZNetwork.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
  [SerializeField] protected ItemData itemData;
  [SerializeField] protected int itemIndex;

  [Header("UI Common Component")]
  [SerializeField] public Button itemButton;
  [SerializeField] public Image itemGrade;

  [Header("UI Component")]
  [SerializeField] public TextMeshProUGUI valueText; //갯수, 레벨이 될 수 있음

  public ItemData GetItemData() => this.itemData;
  public int GetItemIndex() => this.itemIndex;

  public virtual void SetItemData(ItemData itemData)
  {
    if (itemData.IsNull())
    {
      Debug.Log($"ItemData가 존재하지 않습니다.");
      return;
    }

    this.itemData = itemData;
    this.itemIndex = itemData.itemIdx;

    this.SetItemImage(itemData);
    this.SetItemGrade(itemData);
  }

  public void SetItemCountData(ItemData itemData, int itemCount)
  {
    this.SetItemData(itemData);

    valueText.text = itemCount.ToString();
  }

  public void SetItemLvData(ItemData itemData, int itemLv)
  {
    this.SetItemData(itemData);

    valueText.text = $"Lv. {itemLv}";
  }


  protected virtual void SetItemImage(ItemData itemData)
  {
    this.itemButton.image.sprite = NewResourceManager.getInstance.LoadItemSprite((ItemGroup)itemData.itemGroup, ItemUtility.GetItemIcon(itemData.itemIdx));
  }

  /// <summary>
  /// 아이템 등급이 Spine 형태로 구성되어 있는 경우도 있기 때문에 따로 빼었습니다.
  /// </summary>
  protected virtual void SetItemGrade(ItemData itemData)
  {
    this.itemGrade.sprite = NewResourceManager.getInstance.LoadItemGradeSprite(itemData.itemGrade);
  }

  public virtual void ClearData()
  {
    this.itemButton.image.sprite = null;
    this.itemGrade.sprite = null;

    this.itemData = default;
    this.itemIndex = -1;

    this.gameObject.SetActive(false);
  }

  public virtual void OnClickEvent(Action action)
  {
    itemButton.onClick.RemoveAllListeners();
    itemButton.onClick.AddListener(() => action?.Invoke());
  }

  public virtual void OnClickItemDataEvent(Action<ItemData> action)
  {
    itemButton.onClick.RemoveAllListeners();
    itemButton.onClick.AddListener(() => action?.Invoke(GetItemData()));
  }
}






