using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopCategorySlot : MonoBehaviour
{
  [SerializeField] private Button slotButton;
  [SerializeField] TextMeshProUGUI categoryText;

  public Action OnClickSlot;

  private void Awake()
  {
    slotButton.onClick.AddListener(() => OnClickSlot?.Invoke());
  }

  public void SetText(string text)
  {
    categoryText.text = text;
  }
}
