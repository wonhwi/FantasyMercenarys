using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System;

public class PlayerStatUIPopup : UIBaseController
{
  [Serializable]
  public struct UIStatSlot
  {
    public UIStatText uiStatText;
    public Button statDetailButton;
  }

  [SerializeField] private RectTransform contentsParent;
  [SerializeField] private Button closeButton;

  [Header("[Class Component]")]
  [SerializeField] private PlayerStatDetail playerStatDetail;
  [SerializeField] private UIStatSlot[] UIStatSlotList;

  public Action OnClickStatType;

  [ContextMenu("1")]
  public void InsetData()
  {
    for (int i = 0; i < contentsParent.childCount; i++)
    {
      UIStatSlot statSlot = new UIStatSlot();

      statSlot.uiStatText = contentsParent.GetChild(i).GetComponent<UIStatText>();
      statSlot.statDetailButton = contentsParent.GetChild(i).GetChild(3).GetComponent<Button>();

      UIStatSlotList[i] = statSlot;
    }
  }

  protected override void Awake()
  {
    base.Awake();

    closeButton.onClick.AddListener(Hide);
  }

  public void Open()
  {
    StatType[] playerStats = ConstantManager.STAT_TYPES_PLAYER_ALL_STAT;

    contentsParent.anchoredPosition = Vector2.zero;

    var totalStat = PlayerStatManager.getInstance.playerTotalStats;

    for (int i = 0; i < UIStatSlotList.Length; i++)
    {
      UIStatSlotList[i].uiStatText.gameObject.SetActive(false);
    }

    for (int i = 0; i < playerStats.Length; i++)
    {
      UIStatSlot statSlot = UIStatSlotList[i];

      StatType statType = playerStats[i];
      
      totalStat.TryGetValue(statType, out float statValue);

      statSlot.uiStatText.SetData(statType, statValue);
      statSlot.uiStatText.gameObject.SetActive(true);

      statSlot.statDetailButton.onClick.RemoveAllListeners();
      statSlot.statDetailButton.onClick.AddListener(() => playerStatDetail.SetData(statType));
    }
  }

  public override void Hide()
  {
    base.Hide();

    playerStatDetail.gameObject.SetActive(false);
  }

}
