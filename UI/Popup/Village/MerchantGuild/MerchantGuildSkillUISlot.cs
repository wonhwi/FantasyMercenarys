using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MerchantGuildSkillUISlot : MonoBehaviour
{
  public enum SkillState
  {
    Activate = 0,       //선행 버프 스킬 조건 만족
    Researching = 1,    //연구 진행 중
    DeActive = 2,       //선행 버프 스킬 조건을 만족하지 못한 상태
    MaxLevel = 3,       //최대 레벨일경우
  }

  [SerializeField] GameObject researchTextRoot;
  [SerializeField] GameObject maxLevelRoot;
  [SerializeField] CanvasGroup canvasGroup;
  

  [Header("UI Common Component")]
  [SerializeField] public Button itemButton;

  [Header("UI Component")]
  [SerializeField] public TextMeshProUGUI valueText;

  public Action OnClickSlot;

  private void Awake()
  {
    itemButton.onClick.AddListener(() => OnClickSlot?.Invoke());
  }

  public void SetSlotState(SkillState state)
  {
    SetIsReseachingText(state == SkillState.Researching);
    SetMaxLevelText(state == SkillState.MaxLevel);
    SetBlackOpacity(state == SkillState.DeActive);
  }

  public void SetItemImage(string itemImage)
  {
    this.itemButton.image.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_MERCHANT_GUILD, itemImage);
  }

  public void SetLvText(int skillMaxLevel, int skillLv)
  {
    valueText.text = $"{skillLv}/{skillMaxLevel}";
  }

  /// <summary>
  /// 연구 중 Text 출력 함수
  /// </summary>
  /// <param name="active"></param>
  public void SetIsReseachingText(bool active)
  {
    researchTextRoot.SetActive(active);
  }

  /// <summary>
  /// MaxLevel 출력 이미지
  /// </summary>
  /// <param name="active"></param>
  public void SetMaxLevelText(bool active)
  {
    maxLevelRoot.SetActive(active);
  }

  /// <summary>
  /// 비활성화 시
  /// </summary>
  /// <param name="active"></param>
  public void SetBlackOpacity(bool active)
  {
    canvasGroup.alpha = active ? 0.3f : 1f;
  }


}
