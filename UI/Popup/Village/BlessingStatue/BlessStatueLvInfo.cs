using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BlessStatueLvInfo : MonoBehaviour
{
  [System.Serializable]
  public struct BlessStatueLvInfoSlot
  {
    public TextMeshProUGUI curGachaPerText;
    public TextMeshProUGUI nextGachaPerText;
  }

  public BlessStatueBuffInfo blessStatueBuffInfo;

  [Header("관련 슬롯 UI")]
  public BlessStatueLvInfoSlot[] blessStatueLvinfoSlotList;

  [Header("현재 레벨 관련 UI")]
  public TextMeshProUGUI currentLvText;  //현재 레벨
  public Button curLvInfoBtn;

  [Header("다음 레벨 관련 UI ")]
  public TextMeshProUGUI nextLvText;     //다음 레벨
  public Button nextLvInfoBtn;

  public Button closeButton;

  private Dictionary<GradeType, float> curGachaDict = new Dictionary<GradeType, float>();
  private Dictionary<GradeType, float> nextGachaDict = new Dictionary<GradeType, float>();

  private int currentLv;
  private int nextLv;

  private void Awake()
  {
    curLvInfoBtn.onClick.AddListener(() => blessStatueBuffInfo.SetBuffData(currentLv, curGachaDict));
    nextLvInfoBtn.onClick.AddListener(() => blessStatueBuffInfo.SetBuffData(nextLv, nextGachaDict));

    closeButton.onClick.AddListener(Hide);
  }

  public void SetLvData(int blessLv)
  {
    this.ClearSlot();

    int statueMaxLevel = GachaWeightTable.getInstance.GetGachaMaxLevel(ShopCategoryType.BlessStatue);

    this.currentLv = blessLv;
    this.nextLv    = blessLv + 1;

    if(this.nextLv > statueMaxLevel)
      this.nextLv = blessLv;

    currentLvText.text = $"Lv.{currentLv}";
    nextLvText.text = $"Lv.{nextLv}";

    int gachaLvIdx     = (int)ShopLvIndex.BlessStatue + this.currentLv;
    int nextGachaLvIdx = (int)ShopLvIndex.BlessStatue + this.nextLv;

    //모바일 환경으로 인해 Linq ToDictionary ToList 최소화
    UpdateGachaData(curGachaDict,  gachaLvIdx);
    UpdateGachaData(nextGachaDict, nextGachaLvIdx);

    //Dictionary 데이터 기반 Text 값 삽입
    for (int i = 0; i < blessStatueLvinfoSlotList.Length; i++)
    {
      GradeType buffGradeType = (GradeType)(i + 1);

      if (curGachaDict.ContainsKey(buffGradeType))
        blessStatueLvinfoSlotList[i].curGachaPerText.text = $"{curGachaDict[buffGradeType]:F2}%";

      if (nextGachaDict.ContainsKey(buffGradeType))
        blessStatueLvinfoSlotList[i].nextGachaPerText.text = $"{nextGachaDict[buffGradeType]:F2}%";
    }

    
    this.gameObject.SetActive(true);
  }

  /// <summary>
  /// ShopLvIdx 기반 GachaDict (현재 레벨의 뽑기 등급 별 확률 Dictionary) 데이터 업데이트
  /// </summary>
  /// <param name="shopLvIdx"></param>
  private void UpdateGachaData(Dictionary<GradeType, float> gachaDict, int shopLvIdx)
  {
    foreach (var curWeight in GachaWeightTable.getInstance.GetGachaWeightDataList(shopLvIdx))
    {
      if (gachaDict.ContainsKey((GradeType)curWeight.grade))
      {
        gachaDict[(GradeType)curWeight.grade] += curWeight.weight;
      }
      else
      {
        gachaDict.Add((GradeType)curWeight.grade, curWeight.weight);
      }
    }
  }

  public void ClearSlot()
  {
    foreach (var blessStatueLvinfoSlot in blessStatueLvinfoSlotList)
    {
      blessStatueLvinfoSlot.curGachaPerText.text = $"0.00%";
      blessStatueLvinfoSlot.nextGachaPerText.text = $"0.00%";
    }
  }

  private void Hide()
  {
    this.currentLv = 0;

    curGachaDict.Clear();
    nextGachaDict.Clear();

    this.gameObject.SetActive(false);
  }
}
