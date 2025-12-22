using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
/// 뽑기 확률 모듈화 스크립트
/// </summary>
public class BundleGachaInfo : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI[] gachaRateTextArray;

  [SerializeField] private TextMeshProUGUI gachaGradeTitle;
  [SerializeField] private Button nextLvButton;
  [SerializeField] private Button prevLvButton;

  private Dictionary<ItemGradeType, long> curGachaDict = new Dictionary<ItemGradeType, long>();
  
  private int maxShopLv = -1;
  private int curShopLv = -1;

  private ShopLvIndex shopLvIndex = ShopLvIndex.None;

  private void Awake()
  {
    this.nextLvButton?.onClick.AddListener(OnClickNextLvData);
    this.prevLvButton?.onClick.AddListener(OnClickPrevLvData);

  }

  public void SetData(ShopInfoData shopInfoData)
  {
    ShopCategoryType shopCategoryType = (ShopCategoryType)shopInfoData.shopCatIdx;

    this.shopLvIndex = GachaWeightTable.getInstance.GetShopLvIndex(shopCategoryType);
    this.maxShopLv = GachaWeightTable.getInstance.GetGachaMaxLevel(shopCategoryType);

    //지금 윤호님이 레벨이 아닌 레벨에 맞는 Index를 보내달라고 해서 그렇게 작업이 되어있어 예외처리
    if(shopCategoryType == ShopCategoryType.Equipment)
    {
      this.curShopLv = shopInfoData.shopLv - (int)this.shopLvIndex;
    }
    else
    {
      this.curShopLv = shopInfoData.shopLv;
    }

    SetCurrentGachaData();
    SetRateText();
  }

  /// <summary>
  /// 이전 레벨 버튼 눌렀을때
  /// </summary>
  private void OnClickPrevLvData()
  {
    if(curShopLv > 1)
    {
      curShopLv--;

      SetCurrentGachaData();
      SetRateText();
    }
  }

  /// <summary>
  /// 다음 레벨 버튼 눌렀을때
  /// </summary>
  private void OnClickNextLvData()
  {
    if(curShopLv < maxShopLv)
    {
      curShopLv++;

      SetCurrentGachaData();
      SetRateText();
    }
  }

  /// <summary>
  /// ShopLvIndex 기반 GachaData 삽입
  /// </summary>
  private void SetCurrentGachaData()
  {
    InitGachaData();

    var gachaWeightDataList = GachaWeightTable.getInstance.GetGachaWeightDataList((int)shopLvIndex + this.curShopLv);

    foreach (var curWeight in gachaWeightDataList)
    {
      long cleanWeight = Mathf.RoundToInt(curWeight.weight * 1000000);
      
      if (curGachaDict.ContainsKey((ItemGradeType)curWeight.grade))
      {
        curGachaDict[(ItemGradeType)curWeight.grade] += cleanWeight;
      }
      else
      {
        curGachaDict.Add((ItemGradeType)curWeight.grade, cleanWeight);
      }
    }

  }

  private void InitGachaData()
  {
    foreach (ItemGradeType key in System.Enum.GetValues(typeof(ItemGradeType)))
    {
      if (!curGachaDict.ContainsKey(key)) // 중복 방지
      {
        curGachaDict.Add(key, 0);
      }
      else
      {
        curGachaDict[key] = 0;
      }
    }
  }

  /// <summary>
  /// GachaData 기반 Text 확률 표시
  /// </summary>
  private void SetRateText()
  {
    gachaGradeTitle.text = $"뽑기 등급 {this.curShopLv}";

    for (int i = 0; i < gachaRateTextArray.Length; i++)
    {
      ItemGradeType gradeType = (ItemGradeType)(i + 1);

      if (curGachaDict.ContainsKey(gradeType))
      {
        // int 값을 퍼센트 형태로 변환 (13740000 -> 13.740000%)
        float percentage = curGachaDict[gradeType] / 1000000f;
        gachaRateTextArray[i].text = $"{percentage:F6}%";
      }
      else
        gachaRateTextArray[i].text = $"{0:F6}%";
    }

  }
}
