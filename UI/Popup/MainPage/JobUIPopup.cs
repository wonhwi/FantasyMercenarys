using Core.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RestPacket;


public class JobUIPopup : UIBaseController
{
  [Header("Class")]
  [SerializeField] private UIJobSkillInfo jobSkillInfo;

  [Header("UI Text Component")]
  [SerializeField] private TextMeshProUGUI playerLevelText;           //레벨
  [SerializeField] private TextMeshProUGUI playerWeaponText;          //무기
  [SerializeField] private TextMeshProUGUI playerCharacteristicsText; //특징  
  [SerializeField] private TextMeshProUGUI playerNoticeUpgrade;       //진화 가능 진행도 보여주기 



  [Header("UI Button Component")]
  [SerializeField] private Button JobPreviewButton;   //직업 보기
  [SerializeField] private Button JobInitButton;      //직업 초기화
  [SerializeField] private Button JobUpgradeButton;   //직업 진화

  [SerializeField] private Button closeButton;


  private JobData jobData;
  private JobTable jobTable;
  private SkillTable skillTable;
  private NewInGameManager inGameManager;
  private GameDataManager gameDataManager;
  private NewUIManager uiManager;

  private StringBuilder stringBuilder = new StringBuilder();

  private bool isCanChangeGrade = false;

  //직업 버튼 클릭시 내 정보를 받아와서 (임의값) 해당 데이터 테이블에서 긁어와서 보여주는 작업하면될듯

  protected override void Awake()
  {
    base.Awake();

    inGameManager = NewInGameManager.getInstance;
    gameDataManager = GameDataManager.getInstance;
    jobTable = JobTable.getInstance;
    uiManager = NewUIManager.getInstance;
    skillTable = SkillTable.getInstance;

    JobPreviewButton.onClick.AddListener(OnClickJobRoadMapView);
    JobInitButton.onClick.AddListener(OnClickJobInit);
    JobUpgradeButton.onClick.AddListener(OnClickSelectJob);
    closeButton.onClick.AddListener(OnClickClose);

    APIEventManager.AddListener<int>(APIEventType.UpdateJob, InitJopPanel);
  }

  /// <summary>
  /// 초기화
  /// </summary>
  public void InitJopPanel(int playerJobIndex)
  {
    jobData = jobTable.GetJobData(playerJobIndex);

    this.SetUI(jobData);

    jobSkillInfo.InitUIJobSkillInfo(jobData);
  }

  private void SetUI(JobData jobData)
  {
    int playerLevel = gameDataManager.userInfoModel.GetPlayerLv();
    int jobChangeValue = jobData.jobChangeValue;

    bool isFinalJob = jobData.jobChangeGrade == ConstantManager.JOB_FINAL_CHANGE_GRADE_VALUE;

    //직업 진화 가능 여부
    this.isCanChangeGrade = playerLevel >= jobChangeValue;


    stringBuilder.Clear();
    stringBuilder.Append(((JobType)jobData.jobType).ToString().ToLower());

    playerLevelText.text = playerLevel.ToString();
    playerWeaponText.text = LanguageTable.getInstance.GetLanguage($"{stringBuilder}_weapon");
    playerCharacteristicsText.text = skillTable.GetChracteristics(jobData);

    
    if(isFinalJob)
    {
      playerNoticeUpgrade.text = "";
    }
    else if(isCanChangeGrade)
      playerNoticeUpgrade.text = "진화가능!!";
    else
      playerNoticeUpgrade.text = $"계정 레벨 {jobChangeValue}달성 시 진화 가능 [{playerLevel}/{jobChangeValue}]";

    JobUpgradeButton.gameObject.SetActive(!isFinalJob);

  }


  #region Button Event

  /// <summary>
  /// 직업 보기
  /// </summary>
  private async void OnClickJobRoadMapView()
  {
    UIJobRoadView job = await uiManager.Show<UIJobRoadView>(NewResourcePath.PREFAB_UI_POPUP_JOB_ROADVIEW);
    job.Init();



  }

  /// <summary>
  /// 직업 초기화
  /// </summary>
  private void OnClickJobInit()
  {
    if (gameDataManager.userInfoModel.IsCommoner())
    {
      UIUtility.ShowNotificationPopup(
      "알림",
      "전직을 하지 않은 상태입니다."
      );

      return;
    }

    UIUtility.ShowNotificationPopup(
      "알림", 
      "캐릭터의 직업 초기화를 진행 합니까?<br>초기화 후 이전 직업까지 사용 된 진화 아이템은 모두 반환됩니다.", 
      confirmAction : () => APIManager.getInstance.REQ_JobReset<RES_JobReset>(null)
      );

  }

  /// <summary>
  /// 직업 진화
  /// </summary>
  private async void OnClickSelectJob()
  {
    if (!this.isCanChangeGrade)
    {
      Debug.Log("진화 가능한 레벨이 아닙니다");
      return;
    }

    var UIJobSelect = await uiManager.Show<UIJobSelect>(NewResourcePath.PREFAB_UI_POPUP_JOB_SELECT_POPUP);

    UIJobSelect.Init(jobData);
  }

  private void OnClickClose()
  {
    base.Hide();

    this.isCanChangeGrade = false;

    jobSkillInfo.Close();
  }

  #endregion


}
