using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using System;
using System.Linq;
using Newtonsoft.Json;

public class PrayerUISlot : InvenDataSlot
{
  public Image gaugeImage;
  public TextMeshProUGUI itemCountText;
  public GameObject paryObject;

  private bool isMountPray = false;

  public bool IsMount() => isMountPray;

  public override void SetInvenLvData(InvenData invenData)
  {
    base.SetInvenLvData(invenData);

    int prayHP = PartnerTable.getInstance.GetPrayerCurrentHP(invenData);
    int prayMaxHP = PartnerTable.getInstance.GetPrayerMaxHP(invenData);

    this.isMountPray = GameDataManager.getInstance.userContentsData.prayerCenter.mountPrayerMap.Values.Any(n => n.partnerIdx == invenData.itemIdx);

    SetPrayIcon(this.isMountPray);

    gaugeImage.fillAmount = (float)prayHP / (float)prayMaxHP;
    itemCountText.text = $"{prayHP}/{prayMaxHP}";
  }

  public override void SetItemData(ItemData itemData)
  {
    base.SetItemData(itemData);

    gaugeImage.fillAmount = 0;

    itemCountText.text = $"???";
  }

  public void SetPrayIcon(bool isActive)
  {
    this.paryObject.SetActive(isActive);
  }

  public override void ClearData()
  {
    base.ClearData();
  }

}
