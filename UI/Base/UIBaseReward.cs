using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIBaseReward<T> : UIBaseController where T : InvenDataSlot
{
  [SerializeField] protected Inventory<T> inventory = new Inventory<T>();

  [Header("[UI Component]")]
  [SerializeField] protected RectTransform contentsParent;

  [SerializeField] private Button closeBackGround;

  protected override void Awake()
  {
    base.Awake();

    closeBackGround.onClick.AddListener(Hide);
  }

  public override void Hide()
  {
    base.Hide();

    ClearItemSlots();
  }

  protected void Init(string prefabPath, int count)
  {
    inventory.Init(prefabPath, this.contentsParent, count);
  }

  protected void ClearItemSlots()
  {
    inventory.ClearItemSlots();
  }

  protected abstract void SetInvenAction(InvenData invenData, T invenDataSlot);

  public virtual void SetRewardData(List<InvenData> rewardItemList)
  {
    this.contentsParent.anchoredPosition = Vector2.zero;

    for (int i = 0; i < rewardItemList.Count; i++)
    {
      T invenDataSlot = inventory.GetItemSlot();

      SetInvenAction(rewardItemList[i], invenDataSlot);
      invenDataSlot.OnClickItemDataEvent((itemData) => UIUtility.ActiveItemInfo(itemData));
      invenDataSlot.gameObject.SetActive(true);

      
    }
  }
}
