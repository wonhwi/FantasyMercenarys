using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailRewardPopup : UIBaseReward<InvenDataSlot>
{
  protected override void Awake()
  {
    base.Awake();

    base.Init(NewResourcePath.PREFAB_UI_MAILBOX_ITEM_SLOT, 10);
  }


  protected override void SetInvenAction(InvenData invenData, InvenDataSlot invenDataSlot)
  {
    invenDataSlot.SetInvenCountData(invenData);
  }

  public override void SetRewardData(List<InvenData> rewardItemList)
  {
    this.contentsParent.anchoredPosition = Vector2.zero;

    for (int i = 0; i < rewardItemList.Count; i++)
    {
      InvenDataSlot invenDataSlot = inventory.GetItemSlot();

      SetInvenAction(rewardItemList[i], invenDataSlot);

      //아직 버튼 눌렀을떄 어떻게 처리할지 모르겠어
      invenDataSlot.gameObject.SetActive(true);
    }

  }
}
