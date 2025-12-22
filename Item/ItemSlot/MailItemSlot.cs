using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;

public class MailItemSlot : InvenDataSlot
{
  [SerializeField] private GameObject checkBox;

  public void SetCheckBox(bool isActive)
  {
    checkBox.SetActive(isActive);
  }
}
