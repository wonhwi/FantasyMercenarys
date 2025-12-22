using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 잠금 정보가 포함되어있는 Dropdown 모듈
/// </summary>
public class BundleUnlockDropdown : UIBaseUnlockCondition
{
  public TMP_Dropdown dropdown;

  public Action<int> OnSelect;

  private void Awake()
  {
    dropdown.onValueChanged.AddListener((index) => OnSelect?.Invoke(index));
  }

  public void SetDropdown(List<string> optionKeys)
  {
    dropdown.ClearOptions();

    dropdown.AddOptions(optionKeys);
  }

  public void OnValueChange(int index)
  {
    dropdown.onValueChanged?.Invoke(index);
    dropdown.SetValueWithoutNotify(index);
  }
}
