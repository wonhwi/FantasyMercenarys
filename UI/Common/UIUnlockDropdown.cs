using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnlockDropdown : UIUnlockSlot
{
  public BundleUnlockDropdown bundleDropdown;

  private void Start()
  {
    this.SetConditionData(bundleDropdown.GetSlotUnlockConditionData(transform.GetSiblingIndex() - 1));
  }

}
