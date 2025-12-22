using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentUISlot : MonoBehaviour
{
  [SerializeField] public InvenDataSlot invenDataSlot;

  [SerializeField] private Button registButton;

  public Action OnClickInven;

  protected void Awake()
  {
    registButton.onClick.AddListener(() => OnClickInven?.Invoke());
  }

  public void SetData(InvenData invenData)
  {
    if (invenData != null)
    {
      registButton.gameObject.SetActive(false);

      invenDataSlot.SetInvenData(invenData);

      invenDataSlot.gameObject.SetActive(true);
    }


  }

  public void ClearData()
  {
    registButton.gameObject.SetActive(true);
    invenDataSlot.ClearData();
  }

}
