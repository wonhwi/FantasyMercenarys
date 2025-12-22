using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FantasyMercenarys.Data;

/// <summary>
/// 인벤토리 내 아이템 관련 세부 정보 출력 스크립트
/// </summary>
public class BundleInvenItemDetail : MonoBehaviour
{
  public InvenDataSlot invenDataSlot;

  [SerializeField] private TextMeshProUGUI itemGradeText;
  [SerializeField] private TextMeshProUGUI itemName;

  public void SetData(InvenData invenData)
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(invenData.itemIdx);

    invenDataSlot.SetInvenLvData(invenData);

    itemName.text = ItemUtility.GetItemName(invenData.itemIdx);
    itemGradeText.text = LanguageTable.getInstance.GetLanguage(string.Format(ConstantManager.ITEM_GRADE_TEXT_FORMAT, itemData.itemGrade));
  }

  public void ClearReturnUIPool()
  {
    invenDataSlot.ClearReturnUIPool();
  }
}
