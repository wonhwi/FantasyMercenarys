using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreedingUISlot : InvenDataSlot
{
  public Image conditionImage;

  public override void SetItemData(ItemData itemData)
  {
    base.SetItemData(itemData);

    CreatureData creatureData = GameDataManager.getInstance.userContentsData.breedingGround.GetCreatureData(itemData.itemIdx);

    bool hasCreature = creatureData != null;

    if (hasCreature)
      conditionImage.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, $"fm_condition_{creatureData.conditionValue}");
    else
      conditionImage.sprite = null;

    conditionImage.gameObject.SetActive(hasCreature);
  }
}
