using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct ConsumeMaterialData
{
  public int itemIdx;
  public int itemCount;

  public ConsumeMaterialData(int itemIdx, int itemCount)
  {
    this.itemIdx = itemIdx;
    this.itemCount = itemCount;
  }
}

/// <summary>
/// 아이템 제작 팝업에 사용되는 재화 소모 모듈
/// 추후 BundleConsume으로 통합 예정
/// </summary>
public class BundleConsumeMaterial : MonoBehaviour
{
  private GameDataManager gameDataManager;


  [System.Serializable]
  public struct ConsumeMaterialUI
  {
    public GameObject container;
    public Image materialImage;
    public TextMeshProUGUI countText;
    public int itemIdx;
    public long itemCount;
  }
  [SerializeField] private ConsumeMaterialUI[] consumeHasMaterials; //소모 재화 보유량
  [SerializeField] private ConsumeMaterialUI[] consumeMaterials; //소모 재화

  [SerializeField] private TextMeshProUGUI requiredTimeText; //소모 시간

  //재화 소모가 충분히 있다
  private bool isEnoughtState = true;

  private void SetDefaultState()
  {
    this.isEnoughtState = true;

    if (gameDataManager == null)
      gameDataManager = GameDataManager.getInstance;

    for (int i = 0; i < consumeHasMaterials.Length; i++)
      consumeHasMaterials[i].container.SetActive(false);

    for (int i = 0; i < consumeMaterials.Length; i++)
      consumeMaterials[i].container.SetActive(false);
  }

  public void SetConsumeMaterial(ConsumeMaterialData[] consumeMaterialDatas, int count)
  {
    SetDefaultState();

    for (int i = 0; i < count; i++)
    {
      int index = i;

      ConsumeMaterialData consumeMaterialData = consumeMaterialDatas[index];

      SetConsumeData(consumeMaterialData, index);
    }
  }

  public void SetConsumeMaterial(ConsumeMaterialData consumeMaterialData)
  {
    SetDefaultState();

    SetConsumeData(consumeMaterialData);
  }

  private void SetConsumeData(ConsumeMaterialData consumeMaterialData, int index = 0)
  {
    UpdateHasConsumeData(consumeMaterialData.itemIdx, index);
    UpdateConsumeData(consumeMaterialData, index);
    UpdateEnought(index);
  }

  public void UpdateHasConsumeData(int itemIdx, int index = 0)
  {
    ConsumeMaterialUI consumeHasMaterialUI = consumeHasMaterials[index];
    consumeHasMaterialUI.itemCount = gameDataManager.GetConsumeValue(itemIdx);
    consumeHasMaterials[index] = consumeHasMaterialUI;

    SetConsumeUIData(consumeHasMaterialUI, itemIdx);

  }

  public void UpdateConsumeData(ConsumeMaterialData consumeData, int index = 0)
  {
    ConsumeMaterialUI consumeMaterialUI = consumeMaterials[index];
    consumeMaterialUI.itemCount = consumeData.itemCount;
    consumeMaterials[index] = consumeMaterialUI;

    SetConsumeUIData(consumeMaterialUI, consumeData.itemIdx);
  }

  private void UpdateEnought(int index = 0)
  {
    long hasCount     = consumeHasMaterials[index].itemCount;
    long consumeCount = consumeMaterials[index].itemCount;

    bool isEnought = consumeCount <= hasCount;

    if (!isEnought)
      this.isEnoughtState = false;

    consumeMaterials[index].countText.color = isEnought ? Color.white : Color.red;
  }

  public void SetConsumeUIData(ConsumeMaterialUI consumeMaterialUI, int itemIdx)
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    consumeMaterialUI.materialImage.sprite = NewResourceManager.getInstance.LoadItemSprite((ItemGroup)itemData.itemGroup, itemData.iconImage);
    consumeMaterialUI.countText.text = FormatUtility.GetCurencyValue(consumeMaterialUI.itemCount);
    consumeMaterialUI.container.SetActive(true);
  }

  public void SetRequiredTimeTotalSeconds(int totalSeconds)
  {
    requiredTimeText.text = FormatUtility.FormatHHMMSS(totalSeconds);
  }

  public void SetRequiredTimeMinute(int minute)
  {
    requiredTimeText.text = $"{minute} 분";
  }

  public bool IsEnought()
  {
    return this.isEnoughtState;
  }

}
