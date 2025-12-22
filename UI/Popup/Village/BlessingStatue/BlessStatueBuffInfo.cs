using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class BlessStatueBuffInfo : MonoBehaviour
{
  [System.Serializable]
  public struct BlessStatueBuffSlot
  {
    public GameObject bundleSlot;
    public Image buffGradeImage;
    public TextMeshProUGUI buffName;
    public TextMeshProUGUI buffValue;
    public TextMeshProUGUI buffPercent;
  }

  [Header("상단 가호 레벨")]
  public TextMeshProUGUI blessLevelText;

  [Header("버프 Grade Dropdown")]
  public TMP_Dropdown gradeDropdown;

  [Header("Buff Slot UI")]
  public RectTransform contents;
  public BlessStatueBuffSlot[] statueBuffSlotList;

  public Button closeButton;

  private Dictionary<GradeType, float> curGachaDict = new Dictionary<GradeType, float>();

  private void Awake()
  {
    gradeDropdown.onValueChanged.AddListener((index) => { 
      SetBuffSlot(index);
      gradeDropdown.captionImage.SetNativeSize();
    });
    closeButton.onClick.AddListener(Hide);
  }

  public void SetBuffData(int blessLv, Dictionary<GradeType, float> buffGachaDict)
  {
    this.gameObject.SetActive(true);
    contents.anchoredPosition = Vector3.zero;

    curGachaDict = buffGachaDict;

    blessLevelText.text = $"Lv.{blessLv}";

    SetDropdown();

    //여기서 자동으로 0번 인덱스 자동 선택하게
    //인덱스 선택시 그거에 맞는 데이터를 가져와서 슬롯들 설정하기
    gradeDropdown.onValueChanged?.Invoke(0);
    gradeDropdown.SetValueWithoutNotify(0);


    gradeDropdown.captionImage.SetNativeSize();
  }

  public void SetDropdown()
  {
    for (int i = 0; i < gradeDropdown.options.Count; i++)
    {
      float buffValue = 0f;

      if (curGachaDict.ContainsKey((GradeType)(i + 1)))
        buffValue = curGachaDict[(GradeType)(i + 1)];

      gradeDropdown.options[i].text = $"{buffValue:F2}%";
    }
  }

  public void SetBuffSlot(int index)
  {
    ClearBuffSlot();

    GradeType buffGradeType = (GradeType)(index + 1);

    float gradePercent = curGachaDict.ContainsKey(buffGradeType) ? curGachaDict[buffGradeType] : 0f;
    int buffTypeCount = BlessingStatueTable.getInstance.GetBlessingStatueDataCount(buffGradeType);

    float averagePer = gradePercent / buffTypeCount;

    int slotIndex = 0;

    foreach (var blessingStatueData in BlessingStatueTable.getInstance.GetBlessingStatueData(buffGradeType))
    {
      BlessStatueBuffSlot buffSlot = statueBuffSlotList[slotIndex];

      buffSlot.buffGradeImage.sprite = NewResourceManager.getInstance.LoadGradeSpirte((int)buffGradeType);
      buffSlot.buffGradeImage.SetNativeSize();

      buffSlot.buffName.text    = FormatUtility.GetStatTypeNameBlessing(blessingStatueData.blessingType);
      buffSlot.buffValue.text   = $"[{blessingStatueData.blessingMinValue:F2}%~{blessingStatueData.blessingMaxValue:F2}%]";
      buffSlot.buffPercent.text = $"{averagePer:F2}%";

      buffSlot.bundleSlot.SetActive(true);

      slotIndex++;
    }
  }

  private void ClearBuffSlot()
  {
    foreach (var buffSlot in statueBuffSlotList)
    {
      buffSlot.bundleSlot.SetActive(false);
    }
  }

  private void Hide()
  {
    ClearBuffSlot();

    this.gameObject.SetActive(false);
  }


}
