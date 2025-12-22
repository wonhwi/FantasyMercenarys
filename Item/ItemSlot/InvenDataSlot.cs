using Assets.ZNetwork.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using InvenData = FantasyMercenarys.Data.InvenData;

public class InvenDataSlot : ItemSlot
{
  [SerializeField] private InvenData invenData;
  [SerializeField] private long invenIdx;

  //미 수집 아이템 블러 처리 오브젝트
  public GameObject itemBlind;

  [Header("UI Component")]
  public GameObject valueImageBox;

  public InvenData GetInvenData() => this.invenData;
  public long GetInvenIndex() => this.invenIdx;
  public bool IsInvenItem() => invenIdx != 0;

  public virtual void SetInvenData(InvenData invenData)
  {
    this.invenData = invenData;
    this.invenIdx = invenData.invenIdx;

    ItemData itemData = ItemTable.getInstance.GetItemData(invenData.itemIdx);

    if (itemData.IsNull())
    {
      Debug.Log($"{invenData.itemIdx}의 ItemData가 존재하지 않습니다.");
      return;
    }

    base.SetItemData(itemData);

  }

  public virtual void SetInvenLvData(InvenData invenData)
  {
    this.SetInvenData(invenData);

    this.SetItemLvText(invenData);
  }

  public virtual void SetInvenCountData(InvenData invenData)
  {
    this.SetInvenData(invenData);

    this.SetItemCountText(invenData);
  }

  /// <summary>
  /// Override 형태로 상속받은 곳에서 사용
  /// Defualt는 아이템 레벨 출력되는 기능
  /// </summary>
  /// <param name="invenData"></param>
  public virtual void SetItemLvText(InvenData invenData)
  {
    if (valueText)
    {
      ActiveLevelBox(true);
      //아이템 타입에 따라 나오는걸 다르게
      valueText.text = ItemUtility.GetItemLevelBoxText(invenData);
    }
  }

  /// <summary>
  /// Override 형태로 상속받은 곳에서 사용
  /// Defualt는 아이템 레벨 출력되는 기능
  /// </summary>
  /// <param name="invenData"></param>
  public virtual void SetItemCountText(InvenData invenData)
  {
    if (valueText)
    {
      ActiveLevelBox(true);
      //아이템 타입에 따라 나오는걸 다르게
      valueText.text = invenData.itemCount.ToString();
    }
  }
  /// <summary>
  /// 레벨/갯수 출력 박스 활성화/비활성화 (하위 Text도 비활성화 됨)
  /// </summary>
  /// <param name="isActive"></param>
  public void ActiveLevelBox(bool isActive)
  {
    if (valueImageBox)
      valueImageBox.SetActive(isActive);
  }

  public void ActiveBlind(bool isActive)
  {
    if(this.itemBlind)
      this.itemBlind.SetActive(isActive);
  }

  public virtual void ClearReturnUIPool() { }

  public override void ClearData()
  {
    base.ClearData();

    this.invenData = null;
    this.invenIdx = 0;
  }

  public virtual void OnClickInvenDataEvent(Action<InvenData> action)
  {
    itemButton.onClick.RemoveAllListeners();
    itemButton.onClick.AddListener(() => action?.Invoke(GetInvenData()));
  }

}