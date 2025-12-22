using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreedingGroundView : MonoBehaviour
{
  [Header("[Base Inventory Component]")]
  [SerializeField] private Image itemIcon;
  [SerializeField] private Image ItemConditionIcon;
  [SerializeField] private TextMeshProUGUI itemNameText;
  [SerializeField] private TextMeshProUGUI itemSpeedText;
  [SerializeField] private TextMeshProUGUI itemMaxText;
  [SerializeField] private TextMeshProUGUI itemConditionText;

  [SerializeField] private Button invenCloseButton;
  [SerializeField] private Button invenApplyButton;

  [Header("연구비 관련")]
  [SerializeField] private TextMeshProUGUI researchPerText; //분당 연구비 획득
  [SerializeField] private TextMeshProUGUI researchText;    //현재 연구비/최대 소지 연구비 출력

  [Header("금고")]
  [SerializeField] public BundleCountDownText bundleCountDownText;
  [SerializeField] private BundleConsume bundleConsume;
  [SerializeField] private Button upgradeButton;
  [SerializeField] public Button upgradingButton;
  [SerializeField] private Button maxLvButton;

  [Header("생명체, 바닥 쓰레기")]
  [SerializeField] private BreedingCreatureSlot[] slotArray;
  [SerializeField] private GroundCreature[] creatureUIArray;
  [SerializeField] private GroundObject[] objectUIArray;

  [SerializeField] private Button closeButton;
  public Action OnClosePopup;

  public Action OnUpgradeVault;
  public Action OnBoostVault;
  public Action OnMaxVault;


  public Action OnCloseInven;       //인벤토리 닫기
  public Action<int> OnViewInven;   //인벤토리에서 아이템 클릭 시
  public Action<int> OnSelectInven; //인벤토리에서 아이템 클릭 후 등록 버튼 눌렀을때 API 호출
  public Action OnSelect;           //인벤토리에서 아이템 클릭 후 등록 버튼 눌렀을때


  public int selectSlotIdx;

  public GroundCreature[] GroundCreatures => creatureUIArray;

  public GroundObject[] GroundObjects => objectUIArray;

  public BreedingCreatureSlot[] CreatureSlots => slotArray;


  private List<int> objectIndexList = new List<int>();
  private Coroutine timerCoroutine;

  private void Awake()
  {
    closeButton.onClick.AddListener(() =>
    {
      StopCorutine();
      OnClosePopup?.Invoke();
    });

    invenCloseButton.onClick.AddListener(() => OnCloseInven?.Invoke());
    invenApplyButton.onClick.AddListener(() => OnSelect?.Invoke());

    upgradeButton.onClick.AddListener(() => OnUpgradeVault?.Invoke());
    upgradingButton.onClick.AddListener(() => OnBoostVault?.Invoke());
    maxLvButton.onClick.AddListener(() => OnMaxVault?.Invoke());
  }

  public void RunCoroutine(IEnumerator coroutine)
  {
    timerCoroutine = StartCoroutine(coroutine);
  }

  private void StopCorutine()
  {
    if (timerCoroutine != null)
    {
      StopCoroutine(timerCoroutine);
      timerCoroutine = null;
    }
  }

  /// <summary>
  /// 트랙을 돌고 있는 크리쳐들 비활성화
  /// </summary>
  public void InitCreature()
  {
    for (int i = 0; i < creatureUIArray.Length; i++)
    {
      creatureUIArray[i].InitCreature();
    }
  }

  /// <summary>
  /// 하단 크리쳐 Slot들 초기화
  /// </summary>
  public void InitCreatureSlot()
  {
    for (int i = 0; i < slotArray.Length; i++)
    {
      slotArray[i].InitCreatureSlot();
    }
  }

  /// <summary>
  /// 트랙에 배치되어있는 오브젝트 비활성화
  /// </summary>
  public void InitObject()
  {
    for (int i = 0; i < objectUIArray.Length; i++)
    {
      objectUIArray[i].InitObject();
    }
  }

  public GroundObject GetGroundObject()
  {
    objectIndexList.Clear();

    for (int i = 0; i < objectUIArray.Length; i++)
    {
      int index = i;

      if (!objectUIArray[i].IsActive())
      {
        objectIndexList.Add(index);
      }
    }

    int randomIdx = UnityEngine.Random.Range(0, objectIndexList.Count);

    return objectUIArray[objectIndexList[randomIdx]];
  }

  public int GetActiveObject()
  {
    int count = 0;

    for (int i = 0; i < objectUIArray.Length; i++)
    {
      if(objectUIArray[i].IsActive())
        count++;
    }

    return count;
  }

  public void SetInventoryIcon(string iconImage)
  {
    itemIcon.sprite = NewResourceManager.getInstance.LoadItemSprite(ItemGroup.Creature, iconImage);
  }
  public void SetInventoryDetail(BreedingCreatureData creatureMetaData)
  {
    itemNameText.text = LanguageTable.getInstance.GetLanguage(creatureMetaData.descRecordCd);
    itemSpeedText.text = creatureMetaData.creatureSpeed.ToString();
    itemMaxText.text = creatureMetaData.maxResearchPoints.ToString();
  }

  public void SetInventoryCondition(int conditionValue)
  {
    ItemConditionIcon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, $"fm_condition_{conditionValue}");
    itemConditionText.text = LanguageTable.getInstance.GetLanguage($"breeding_creature_condition_{conditionValue}");
  }

  public void SetInventoryConditionActive(bool hasCreature)
  {
    ItemConditionIcon.gameObject.SetActive(hasCreature);
    itemConditionText.gameObject.SetActive(hasCreature);
  }

  public void SetResearchPerText(float value)
  {
    researchPerText.text = value.ToString();
  }

  public void SetResearchPoint(int maxResearchPoints)
  {
    long researchCurrency = CurrencyManager.getInstance.GetCurrencyAmount(ConstantManager.ITEM_CURRENCY_RESEARCH_FUNDS);

    researchText.text = $"{FormatUtility.GetCurencyValue(researchCurrency)}/{FormatUtility.GetCurencyValue(maxResearchPoints)}";
  }

  public void SetVaultState(bool isUpgrade, bool isMaxLv)
  {
    upgradeButton.gameObject.SetActive(!isMaxLv && !isUpgrade);
    upgradingButton.gameObject.SetActive(!isMaxLv && isUpgrade);
    maxLvButton.gameObject.SetActive(isMaxLv);
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
