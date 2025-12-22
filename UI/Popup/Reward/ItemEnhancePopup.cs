using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬, 동료 팝업에서 강화를 했을 때 활성화 되는 팝업
/// </summary>
public class ItemEnhancePopup : UIBaseReward<PrefabUISlot>
{
  public UISkeletonAnimation SpineAnimation;
  
  protected override void Awake()
  {
    base.Awake();

    base.Init(NewResourcePath.PREFAB_UI_POPUP_SLOT, ConstantManager.ITEM_REWARD_MAX_COUNT);
  }

  public override void Show()
  {
    base.Show();

    SpineAnimation.SetAnimation(ConstantManager.ANIMATION_UI_SPINE_REWARD_PLAY, loop: false);
    SpineAnimation.AddAnimation(ConstantManager.ANIMATION_UI_SPINE_REWARD_IDLE, loop: true);
  }

  protected override void SetInvenAction(InvenData invenData, PrefabUISlot prefabUISlot)
  {
    //Debug.LogError(prefabUISlot.GetItemIndex());

    prefabUISlot.SetInvenData(invenData, false);

    prefabUISlot.ActiveAttachButton(false);
    prefabUISlot.ActiveDetachButton(false);
  }

  
}