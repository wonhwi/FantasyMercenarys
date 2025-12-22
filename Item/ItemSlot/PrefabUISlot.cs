using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabUISlot : InvenDataSlot
{
  private UISpineSelect uISpineSelect;
  public Image gaugeImage;
  public TextMeshProUGUI itemCountText;
  public Button attachButton;
  public Button detachButton;

  public void SetInvenData(InvenData invenData, bool isMount)
  {
    base.SetInvenLvData(invenData);

    long itemCount = invenData.itemCount;
    int enhanceTarget = ItemUtility.GetEnhanceTargetCount(invenData);
    int combineTarget = ItemUtility.GetCombineTargetCount(invenData);
    bool isMaxItemLevel = ItemUtility.IsMaxItemLevel(invenData);

    if(isMaxItemLevel)
    {
      gaugeImage.fillAmount = ((float)itemCount / (float)combineTarget);
      itemCountText.text = $"{itemCount}/{combineTarget}";
    }
    else
    {
      gaugeImage.fillAmount = ((float)itemCount / (float)enhanceTarget);
      itemCountText.text = $"{itemCount}/{enhanceTarget}";
    }

    attachButton?.gameObject.SetActive(!isMount);
    detachButton?.gameObject.SetActive(isMount);

    if(isMount)
    {
      if(uISpineSelect == null)
      {
        uISpineSelect = UIPoolManager.Instance.GetPool<UISpineSelect>(UIPoolType.ItemSelect);
        uISpineSelect.OnSetParent(itemGrade.transform);
      }
    }
    else
      UIPoolManager.Instance.ClearReturnUIPool<UISpineSelect>(UIPoolType.ItemSelect, itemGrade.transform);
  }

  public void ActiveAttachButton(bool active)
    => attachButton.gameObject.SetActive(active);

  public void ActiveDetachButton(bool active)
    => detachButton.gameObject.SetActive(active);


  public override void SetItemData(ItemData itemData)
  {
    base.SetItemData(itemData);

    gaugeImage.fillAmount = 0;

    itemCountText.text = $"0/1";

    attachButton?.gameObject.SetActive(false);
    detachButton?.gameObject.SetActive(false);
  }

  public override void ClearData()
  {
    base.ClearData();

    UIPoolManager.Instance.ClearReturnUIPool<UISpineSelect>(UIPoolType.ItemSelect, itemGrade.transform);

    uISpineSelect = null;
  }

  public void OnClickAttachEvent(Action<InvenData> action)
  {
    attachButton.onClick.RemoveAllListeners();
    attachButton.onClick.AddListener(() => action?.Invoke(GetInvenData()));
  }

  public void OnClickDetachEvent(Action<InvenData> action)
  {
    detachButton.onClick.RemoveAllListeners();
    detachButton.onClick.AddListener(() => action?.Invoke(GetInvenData()));
  }
}
