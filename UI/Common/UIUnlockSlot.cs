using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIUnlockSlot : MonoBehaviour
{
  private SlotUnlockConditionType conditionType;
  private int conditionValue;

  public Button conditionLockSlot;

  private bool isLocked;

  protected virtual void Awake()
  {
    conditionLockSlot.onClick.AddListener(OnClickLockButton);
  }

  public bool IsLocked() => isLocked;

  public void SetConditionData(SlotUnlockConditionData slotUnlockConditionData)
  {
    this.SetConditionData((SlotUnlockConditionType)slotUnlockConditionData.unlockType, slotUnlockConditionData.unlockValue);
  }

  public virtual void SetConditionData(SlotUnlockConditionType conditionType, int conditionValue)
  {
    this.conditionType = conditionType;
    this.conditionValue = conditionValue;

    if (conditionType == SlotUnlockConditionType.None)
      return;

    int targetValue = 0;

    if (conditionType == SlotUnlockConditionType.Level)
    {
      targetValue = GameDataManager.getInstance.userInfoModel.GetPlayerLv();
    }
    else if (conditionType == SlotUnlockConditionType.BlessingStatueLv)
    {
      targetValue = GameDataManager.getInstance.userContentsData.blessingStatue.GetBlessingLv();
    }
    else if (conditionType == SlotUnlockConditionType.StageClear)
    {
      targetValue = GameDataManager.getInstance.stageIndex;
    }
    else if (conditionType == SlotUnlockConditionType.EquipmentGachaLv)
    {
      targetValue = LampLvInfoTable.getInstance.GetEquipmentGachaLevelData(GameDataManager.getInstance.userShopData.GetShopInfoData(ShopCategoryType.Equipment).shopLv).lampLv;
    }
    else if (conditionType == SlotUnlockConditionType.MerchantGuild)
    {
      isLocked = !GameDataManager.getInstance.userContentsData.merchantGuild.HasSkill(conditionValue);

      this.SetActiveLockButton();
      return;
    }

    isLocked = targetValue < conditionValue;

    this.SetActiveLockButton();
  }

  public virtual void SetActiveLockButton()
  {
    conditionLockSlot.gameObject.SetActive(isLocked);

  }

  public virtual void OnClickLockButton()
  {
    UIUtility.ShowToastMessagePopup($"{this.conditionType} {this.conditionValue} 달성 시 해금");
  }

}
