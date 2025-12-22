using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 공통으로 아이템을 뽑았을때 활성화 하는 팝업
/// </summary>
public class ItemRewardPopup : UIBaseReward<InvenDataSlot>
{
  private float maxSize = 670f;

  [SerializeField] private RectTransform rect;

  protected override void Awake()
  {
    base.Awake();

    base.Init(NewResourcePath.PREFAB_UI_INVEN_DATA_SLOT, 10);
  }


  protected override void SetInvenAction(InvenData invenData, InvenDataSlot invenDataSlot)
  {
    invenDataSlot.SetInvenCountData(invenData);
  }

  public override void SetRewardData(List<InvenData> rewardItemList)
  {
    if(rewardItemList == null)
    {
      Debug.Log("보상 정보가 없습니다.");

      base.Hide();
      return;
    }

    for (int i = 0; i < rewardItemList.Count; i++)
    {
      InvenDataSlot invenDataSlot = inventory.GetItemSlot();

      SetInvenAction(rewardItemList[i], invenDataSlot);

      //아직 버튼 눌렀을떄 어떻게 처리할지 모르겠어
      //invenDataSlot.OnClickItemDataEvent((itemData) => CodeUtility.ActiveItemInfo(itemData));
      invenDataSlot.gameObject.SetActive(true);
    }

    StartCoroutine(ApplyContentHeightNextFrame());
  }

  IEnumerator ApplyContentHeightNextFrame()
  {
    yield return null; // 1 프레임 대기

    float sizeDeltaY = contentsParent.sizeDelta.y;
    float height = Mathf.Clamp(sizeDeltaY, sizeDeltaY, maxSize);

    this.rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
    this.contentsParent.anchoredPosition = new Vector2(contentsParent.anchoredPosition.x, 0f);


  }
}
