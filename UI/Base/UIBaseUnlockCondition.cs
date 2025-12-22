using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBaseUnlockCondition : MonoBehaviour
{
  private IEnumerable<SlotUnlockConditionData> slotUnlockConditionData;

  public virtual void SetData(SlotUnlockContentType contentType)
  {
    slotUnlockConditionData = SlotUnlockConditionTable.getInstance.GetSlotUnlockConditionDataList(contentType);
  }

  public SlotUnlockConditionData GetSlotUnlockConditionData(int index)
  {
    return slotUnlockConditionData.ElementAtOrDefault(index);
  }
}
