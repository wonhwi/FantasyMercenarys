using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해금 정보가 들어있는 Slot 모듈
/// </summary>
public class BundleUnlockSlot : UIBaseUnlockCondition
{
  private UIUnlockSlot[] unlockSlotList;

  public void SetSlot(UIUnlockSlot[] unlockSlotList)
  {
    this.unlockSlotList = unlockSlotList;
  }

  public override void SetData(SlotUnlockContentType contentType)
  {
    base.SetData(contentType);

    UpdateData();
  }

  /// <summary>
  /// 드랍다운과 다르게 Slot은 새로 생성이 되지 않아 주기적으로 호출해서 맞는지 체크해줘야함
  /// </summary>
  public void UpdateData()
  {
    for (int i = 0; i < unlockSlotList.Length; i++)
    {
      int index = i;
      unlockSlotList[index].SetConditionData(base.GetSlotUnlockConditionData(index));
    }
  }
}