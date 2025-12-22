using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점에서 아이템을 뽑기했을 때 활성화 되는 팝업
/// </summary>
public class ItemGachaPopup : UIBaseReward<InvenDataSlot>
{
  [Header("[Class Component]")]
  [SerializeField] private BundleGachaButton bundleGachaButton;

  protected override void Awake()
  {
    base.Awake();

    base.Init(NewResourcePath.PREFAB_UI_INVEN_DATA_SLOT, ConstantManager.ITEM_REWARD_MAX_COUNT);
  }


  protected override void SetInvenAction(InvenData invenData, InvenDataSlot invenDataSlot)
  {
    invenDataSlot.SetInvenData(invenData);
    invenDataSlot.ActiveLevelBox(false);
  }

  public override void SetRewardData(List<InvenData> rewardItemList)
  {
    base.SetRewardData(rewardItemList);

    bundleGachaButton.SetClose(Hide);

  }
}

