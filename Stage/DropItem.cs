using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : DropItemBase, IPoolable
{
  protected override void OnDropArrived()
  {
    base.OnDropArrived();

    if(itemIdx == ConstantManager.ITEM_CURRENCY_GOLD)
    {
      gameDataManager.waveTotalGold += (int)itemCount;
      gameDataManager.userInfoModel.AddGold(itemCount);
    }

    ReleaseItem();
  }


  /// <summary>
  /// 이거 OnDeactivate랑 중복된다 어떻게 처리할지 고민
  /// </summary>
  private void ReleaseItem()
  {
    thisRectTransform.anchoredPosition = Vector2.zero;

    inGameManager.ReturnObjectPoolTypeDropItem(this);
  }

}
