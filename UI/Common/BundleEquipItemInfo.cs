using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 정보, 스텟 출력 모듈화 스크립트
/// </summary>
public class BundleEquipItemInfo : MonoBehaviour
{
  [SerializeField] private BundleInvenItemDetail bundleInvenItemDetail;
  [SerializeField] private BundleStatText bundleStatText;

  public void SetData(InvenData invenData)
  {
    bundleInvenItemDetail.SetData(invenData);
    bundleStatText.SetData(invenData.statDataList);
  }

  public void SetCompareData(InvenData invenData, InvenData targetInvenData)
  {
    bundleInvenItemDetail.SetData(invenData);

    if(targetInvenData != null)
      bundleStatText.SetCompareData(invenData.statDataList, targetInvenData.statDataList);
    else
      bundleStatText.SetCompareEmptyData(invenData.statDataList);
  }

  public void ClearReturnUIPool()
  {
    bundleInvenItemDetail.ClearReturnUIPool();
  }
}
