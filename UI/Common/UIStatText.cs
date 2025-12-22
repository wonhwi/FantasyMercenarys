using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

/// <summary>
/// 장비 관련 Stat 출력 Text
/// </summary>
public class UIStatText : MonoBehaviour
{
  StringBuilder sb = new StringBuilder();

  [SerializeField] private TextMeshProUGUI statNameText;
  [SerializeField] private TextMeshProUGUI statValueText;

  public void SetData(StatType statType, float statValue)
  {
    sb.Clear();

    statNameText.text = FormatUtility.GetStatTypeName(statType);
    statValueText.text = FormatUtility.GetStatValue(sb, statType, statValue);
  }

  
  public void SetCompareData(StatType statType, float statValue, float targetValue)
  {
    sb.Clear();

    statNameText.text = FormatUtility.GetStatTypeName(statType);
    statValueText.text = FormatUtility.GetStatCompareValue(sb, statType, statValue, targetValue);
  }

  public void SetCustomData(string statTypeName, StatType statType, float statValue)
  {
    sb.Clear();

    statNameText.text = statTypeName;
    statValueText.text = FormatUtility.GetStatValue(sb, statType, statValue);
  }

  public void SetCustomData(string statTypeName, string statValue)
  {
    sb.Clear();

    statNameText.text = statTypeName;
    statValueText.text = statValue;
  }
}
