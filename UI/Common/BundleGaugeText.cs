using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게이지 관련 스크립트
/// </summary>
public class BundleGaugeText : MonoBehaviour
{
  public Image progressBar;
  public TextMeshProUGUI valueText;

  public void SetGaugeTextData(int curValue, int maxValue)
  {
    progressBar.fillAmount = (float)curValue / (float)maxValue;

    valueText.text = $"{curValue} / {maxValue}";

    ActiveBundle(true);
  }

  public void SetCustomData(float fillAmount, string value)
  {
    progressBar.fillAmount = fillAmount;

    valueText.text = value;

    ActiveBundle(true);
  }

  public void ActiveBundle(bool isActive)
  {
    this.gameObject.SetActive(isActive);
  }
}
