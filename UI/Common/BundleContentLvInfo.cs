using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using DTT.Utils.Extensions;


/// <summary>
/// ShopCategory 기반 콘텐츠의 현재레벨, 경험치량 출력 모듈 스크립트
/// </summary>
public class BundleContentLvInfo : MonoBehaviour
{
  public enum ContentLvType
  {
    Lv,
    Grade
  }
  public ContentLvType contentLvType = ContentLvType.Lv;


  [Header("[UI Component]")]
  public Button infoButton;
  public TextMeshProUGUI shopLvText;
  public BundleGaugeText bundleGaugeText;


  //여기 format Type 만들어서 Lv.할건지 등급으로 표시할건지 추가를 하자
  public void SetData(ShopCategoryType categoryType, int level, int curExp)
  {
    int currentExp = ShopLvInfoTable.getInstance.GetCurRequireLvExp(categoryType, level);

    shopLvText.text = string.Format(GetFormat(), level);

    bundleGaugeText.SetGaugeTextData(curExp, currentExp);
  }


  public void SetData(ShopInfoData shopInfoData)
  {
    int currentExp = ShopLvInfoTable.getInstance.GetCurRequireLvExp((ShopCategoryType)shopInfoData.shopCatIdx, shopInfoData.shopLv);

    shopLvText.text = string.Format(GetFormat(), shopInfoData.shopLv);

    bundleGaugeText.SetGaugeTextData(shopInfoData.buyCount, currentExp);
  }

  private string GetFormat() => contentLvType switch
  {
    ContentLvType.Lv    => "Lv.{0}",
    ContentLvType.Grade => "{0}등급",
  };

  /// <summary>
  /// 해당 상점이 최대 상태일때
  /// </summary>
  /// <param name="level">상점 Text 출력 내용</param>
  public void SetMaxData(int level, string maxText = "Max")
  {
    shopLvText.text = string.Format(GetFormat(), level);

    bundleGaugeText.SetCustomData(1f, maxText);
  }
}
