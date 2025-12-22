using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SkillState = MerchantGuildSkillUISlot.SkillState;

public class MerchantGuildView : MonoBehaviour, ICurrencyUsable
{
  [System.Serializable]
  public struct MerchantSkillUISlot
  {
    public string slotTagName;
    public MerchantGuildSkillUISlot slot;
    public GameObject[] roadLines;
  }

  public BundleCurrency bundleCurrency;
  public BundleConsume bundleConsume;
  public BundleCountDownText bundleCountDownText;

  [Header("중앙 슬롯 정보")]
  [SerializeField] MerchantSkillUISlot[] skillUISlots;
  public MerchantGuildSkillUISlot selectSlot;
  [SerializeField] private Transform roadLineArrowRoot;
  [SerializeField] private Transform roadLineRoot;
  [SerializeField] private ScrollRect scrollRect;
  [SerializeField] private RectTransform contentRoot;

  public RectTransform selectHighlight;

  [Header("하단 아이템 정보")]
  [SerializeField] MerchantGuildSkillUISlot previewSlot;
  [SerializeField] TextMeshProUGUI skillNameText;
  [SerializeField] TextMeshProUGUI curDetailText;
  [SerializeField] TextMeshProUGUI nextDetailText;

  [Header("우측 하단 정보")]
  [SerializeField] GameObject researchingText;

  [SerializeField] private Button researchButton;   //연구 시작 버튼
  [SerializeField] private Button speedBoostButton; //가속권 버튼
  [SerializeField] private Button maxLevelButton;   

  [SerializeField] private Button closeButton;


  public MerchantSkillUISlot[] SkillUISlots => this.skillUISlots;

  public MerchantGuildSkillUISlot PreviewSlot => previewSlot;

  public Action OnClickResearch;
  public Action OnClickSpeedBoost;
  public Action OnClickMaxLv;


  public Action OnClosePopup;

  private void Awake()
  {
    closeButton.onClick.AddListener(() => 
    {
      InitSelectImage();
      OnClosePopup?.Invoke(); 
    });

    researchButton.onClick.AddListener(() => OnClickResearch?.Invoke());
    speedBoostButton.onClick.AddListener(() => OnClickSpeedBoost?.Invoke());
    maxLevelButton.onClick.AddListener(() => OnClickMaxLv?.Invoke());

  }

  private void InitSelectImage()
  {
    selectHighlight.gameObject.SetActive(false);
  }

  public void InitContentPosition(int slotIndex)
  {
    Canvas.ForceUpdateCanvases();

    // HorizontalLayoutGroup 정보
    float paddingLeft = 100f;
    float spacing = 130f;

    // Viewport와 Content 정보
    float viewportWidth = scrollRect.viewport.rect.width;
    float contentWidth = contentRoot.rect.width;

    Transform slotParent = skillUISlots[slotIndex].slot.transform.parent;
    // 캐시된 슬롯의 RectTransform 사용
    RectTransform targetItem = slotParent as RectTransform;

    int slotParentIndex = slotParent.GetSiblingIndex() - 1;

    // 아이템의 너비
    float itemWidth = targetItem.rect.width;

    // 타겟 아이템까지의 거리 계산
    float targetPosition = paddingLeft + (spacing + itemWidth) * slotParentIndex;

    // 중앙 정렬을 위한 오프셋 계산
    float centerOffset = (viewportWidth - itemWidth) * 0.5f;

    // 최종 스크롤 위치 계산
    float scrollPosition = targetPosition - centerOffset;

    // 스크롤 범위 제한
    float maxScroll = contentWidth - viewportWidth;
    scrollPosition = Mathf.Clamp(scrollPosition, 0f, maxScroll);

    // Content 위치 설정
    contentRoot.anchoredPosition = new Vector2(-scrollPosition, 0);
  }

  public void InitRoadLine()
  {
    for (int i = 0; i < roadLineArrowRoot.childCount; i++)
      roadLineArrowRoot.GetChild(i).gameObject.SetActive(false);

    for (int j = 0; j < roadLineRoot.childCount; j++)
      roadLineRoot.GetChild(j).gameObject.SetActive(false);
  }

  public void SetSkillNameText(string skillName)
  {
    skillNameText.text = LanguageTable.getInstance.GetLanguage(skillName);
  }

  public void SetCurDetailText(string skillRecordCd, float value)
  {
    curDetailText.text = string.Format(LanguageTable.getInstance.GetLanguage(skillRecordCd), value);
  }

  public void SetNextDetailText(string skillRecordCd, float value)
  {
    nextDetailText.text = string.Format(LanguageTable.getInstance.GetLanguage(skillRecordCd), value);
  }

  public void SetButtonState(SkillState skillState)
  {
    researchButton.gameObject.SetActive(skillState == SkillState.Activate || skillState == SkillState.DeActive);

    researchButton.image.sprite = NewResourceManager.getInstance.LoadSprite(
        NewResourcePath.PATH_UI_ICON_BUTTON,
        skillState == SkillState.Activate ? ConstantManager.UI_BUTTON_ACTIVE_IMAGE : ConstantManager.UI_BUTTON_DEFAULT_IMAGE
      );

    speedBoostButton.gameObject.SetActive(skillState == SkillState.Researching);
    maxLevelButton.gameObject.SetActive(skillState == SkillState.MaxLevel);


    researchingText.SetActive(skillState == SkillState.Researching);
  }

  public void EnableCurrency()
  {
    bundleCurrency.Enable();

  }
  public void DisableCurrency()
  {
    bundleCurrency.Disable();

  }

  public void SetConsumeData(ConsumeRequirement requirement, int requiredTime)
  {
    bundleConsume.SetConsumeData(requirement, requiredTime);
  }

  public bool GetConsumeIsEnought()
  {
    return bundleConsume.CanConsume();
  }
}
