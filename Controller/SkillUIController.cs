using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//스킬 팝업창 관련은 따로 만들거다 이거는 인게임 스킬 버튼 할당
//스킬 팝업창 설정하면 이놈이 스킬 버튼들 이미지 등등 바꿔주는 역할 할듯
public class SkillUIController : MonoBehaviour
{
  /// <summary>
  /// Skill UI 관련 Struct
  /// </summary>
  [Serializable]
  public struct SkillUIData
  {
    public Button skillButton;
    public Image coolTimeImage;
  }

  [Header("Manager")]
  NewInGameManager inGameManager;

  [Header("SkillController")]
  private PlayerSkillController skillController;

  [Header("UI Component")]
  [SerializeField] private SkillUIData jobSkillUIData;       //유저 직업의 Active Skill
  [SerializeField] private SkillUIData[] mountSkillUIDatas;  //장착한 스킬 0번째는 직업 스킬
  public Toggle autoSkillToggle;


  public void Awake()
  {
    inGameManager = NewInGameManager.getInstance;    
  }

  private void Start()
  {
    this.skillController = inGameManager.playerSkillController;

    this.skillController.OnExcuteJobSkill += UpdateJobSkillCoolTime;
    this.skillController.OnExcuteSkill += UpdateSkillCoolTime;

    APIEventManager.AddListener<int>(APIEventType.UpdateJob, (jobIndex) => UpdateJobSkill(jobIndex));

    inGameManager.OnUpdateSkillIUI += (index, data) => UpdateSkill(index, data);

    this.InitUIEvent();
    this.InitUIData();
    this.StartAutoSkill();
  }


  #region Init Method (초기화, 초기 데이터 삽입)

  /// <summary>
  /// UI 버튼들에 함수 실행 이벤트 추가
  /// 1.SkillController의 ExcuteSkill, ExcuteAutoSkill 함수 실행
  /// </summary>
  private void InitUIEvent()
  {
    // Skill Button UI Event
    jobSkillUIData.skillButton.onClick.RemoveAllListeners();
    jobSkillUIData.skillButton.onClick.AddListener(() => skillController?.ExcuteUniqueSkill());

    for (int i = 0; i < mountSkillUIDatas.Length; i++)
    {
      int index = i;

      mountSkillUIDatas[index].skillButton.onClick.RemoveAllListeners();
      mountSkillUIDatas[index].skillButton.onClick.AddListener(() => skillController?.ExcuteUISkill(index));
    }

    // Auto Skill Button Event
    autoSkillToggle.onValueChanged.RemoveAllListeners();
    autoSkillToggle.onValueChanged.AddListener((isOn) =>
    {
      if (isOn)
        skillController?.ExcuteAutoSkill();
      else
        skillController?.CancelAutoSkill();
    });

    
  }


  /// <summary>
  /// SkillController의 Data 기반 쿨타임, 이미지 데이터 최초 설정
  /// 캐릭터 생성이 늦다 보니 처음 Init을 해줘야합니다.
  /// </summary>
  public void InitUIData()
  {
    this.UpdateJobSkill(GameDataManager.getInstance.userInfoModel.GetJobCode(), false);

    this.InitSkill(skillController.GetGameSkillDataList());

  }

  private void StartAutoSkill()
  {
    bool isAutoSkill = Convert.ToBoolean(PlayerPrefs.GetInt("AutoSkill", 0));

    autoSkillToggle.isOn = isAutoSkill;
  }
  #endregion


  #region UI Update 함수

  /// <summary>
  /// 스킬 업데이트 시 아이콘, 쿨타임 돌도록 하는 함수
  /// </summary>
  /// <param name="gameSkillDataList"></param>
  /// <param name="forceUpdate">강제로 쿨타임 돌게 할 건지?</param>
  private void InitSkill(IEnumerable<GameSkillData> gameSkillDataList)
  {
    int index = 0;
    foreach (GameSkillData gameSkillData in gameSkillDataList)
    {
      UpdateSkill(index, gameSkillData, false);
      index++;
    }
  }

  public void UpdateSkill(int index, GameSkillData gameSkillData, bool forceUpdate = true)
  {
    string iconImage = "";

    if (gameSkillData != null)
    {
      IdleSkillData idleSkillData = SkillTable.getInstance.GetSkillData(gameSkillData.skillIdx);

      iconImage = idleSkillData.iconImage;

      if (forceUpdate)
        UpdateSkillCoolTime(index, idleSkillData.coolTime);
    }

    SetUISlotIcon(mountSkillUIDatas[index], iconImage);
  }

  /// <summary>
  /// 스킬 교채시 자체적으로 쿨타임이 돌아야 함
  /// UpdateJobSkillCoolTime 추가
  /// </summary>
  /// <param name="jobIndex"></param>
  private void UpdateJobSkill(int jobIndex, bool forceUpdate = true)
  {
    IdleSkillData idleSkillData = SkillTable.getInstance.GetJobActiveSkill(jobIndex);

    SetUISlotIcon(jobSkillUIData, idleSkillData.iconImage);

    if (forceUpdate)
      UpdateJobSkillCoolTime(idleSkillData.coolTime);
  }

  private void SetUISlotIcon(SkillUIData skillUIData, string iconImage)
  {
    if (!string.IsNullOrEmpty(iconImage))
    {
      skillUIData.skillButton.image.sprite = NewResourceManager.getInstance.LoadItemSprite(ItemGroup.Skill, iconImage);
      skillUIData.skillButton.gameObject.SetActive(true);
    }
    else
    {
      skillUIData.coolTimeImage.DOKill(true);
      skillUIData.skillButton.gameObject.SetActive(false);
    }
  }


  /// <summary>
  /// PlayerSkillController 의 직업 ActiveSkill CoolTime 함수
  /// </summary>
  /// <param name="coolTime"></param>
  public void UpdateJobSkillCoolTime(float coolTime)
  {
    SkillCoolTime(jobSkillUIData, coolTime);
  }

  /// <summary>
  /// SkillController가 스킬을 실행했을때 실행시켜줘야해 
  /// 1. Controller의 ExcuteSkill 실행시 아래 함수를 실행시켜줘야해
  /// TimeScale 기반 작업 필요 -> 0(정지) ~ 2(배속)
  /// </summary>
  /// <param name="index">SkillController가 가지고있는 SkillList Index</param>
  public void UpdateSkillCoolTime(int index, float coolTime)
  {
    DOTween.Kill(mountSkillUIDatas[index].coolTimeImage, true);

    SkillCoolTime(mountSkillUIDatas[index], coolTime);

  }

  private void SkillCoolTime(SkillUIData skillUIData, float coolTime)
  {
    

    skillUIData.coolTimeImage.gameObject.SetActive(true);
    skillUIData.coolTimeImage.fillAmount = 1;
    skillUIData.coolTimeImage.DOFillAmount(0, coolTime) 
               .SetEase(Ease.Linear)
               .OnComplete(() =>
               {
                 skillUIData.coolTimeImage.gameObject.SetActive(false);

               });
  }




  #endregion



}