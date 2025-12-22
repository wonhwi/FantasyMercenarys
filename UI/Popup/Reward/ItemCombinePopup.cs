using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬, 동료 팝업에서 합성을 했을 때 활성화 되는 팝업
/// </summary>
public class ItemCombinePopup : UIBaseReward<InvenDataSlot>
{
  public UISkeletonAnimation SpineAnimation;

  protected override void Awake()
  {
    base.Awake();

    base.Init(NewResourcePath.PREFAB_UI_INVEN_DATA_SLOT, ConstantManager.ITEM_REWARD_MAX_COUNT);
  }

  public override void Show()
  {
    base.Show();

    //이거 상수로 빼자
    SpineAnimation.SetAnimation(ConstantManager.ANIMATION_UI_SPINE_REWARD_PLAY, loop: false);
    SpineAnimation.AddAnimation(ConstantManager.ANIMATION_UI_SPINE_REWARD_IDLE, loop: true);
  }

  protected override void SetInvenAction(InvenData invenData, InvenDataSlot invenDataSlot)
  {
    invenDataSlot.SetInvenCountData(invenData);
  }
}
