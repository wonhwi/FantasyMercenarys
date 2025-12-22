using Assets.ZNetwork.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 스킬의 범위나 랜덤 함수들을 통해 스킬의 변수 관여
/// 여기에 스킬 사용 가능 변수도 만들어 놓자
/// 스킬 데미지 연산도 Controller에 작업, SkillActor 에 데미지 실어서 날리자
/// </summary>
public abstract class SkillController : MonoBehaviour
{
  public IAttackable IAttacker;
  protected NewInGameManager inGameManager = NewInGameManager.getInstance;
  protected GameDataManager gameDataManager = GameDataManager.getInstance;
  protected SkillManager skillManager = SkillManager.getInstance;
  protected JobTable jobTable = JobTable.getInstance;

  

  //[SkillController에서 ActiveJobSkillList를 가지고 있다.]
  //1. 직업 스킬
  //
  //[SkillController에서 GameSkillList를 가지고 있다.]
  //
  //1. 위의 GameSkillList는 인벤토리에서 업데이트 하거나 할때 삽입해준다 - SkillTable과 InvenData의 ItemLevel들을 참조한다.
  //
  //2. 스킬 사용시 아래 BuffSkillList 사용해 GameSkillList를 가공한다.여기서 최종 데미지 , 치명타 피해 등등의 데이터도 같이 실어준다
  //   플레이어일 시 스킬, 동료 보유 효과량 데이터와, 플레이어 직업의 패시브 버프 스킬 데이터들을 참조해야 한다.
  //
  //2.1 버프형 스킬 사용시 BuffSkillList에 추가를 해주고 지속 시간이 끝나면 삭제 해 준다.
  //
  //3. 가공한 데이터를 기반으로 스킬에 데이터를 넣고 사용한다.
  //
  //[SkillController에서 BuffSkillList를 가지고 있다.]
  //
  //1. 캐릭터 직업 진화시, 캐릭터 Load시 직업기반으로 해당 데이터를 가지고 있는다.PlayerController로 이동 할 수도 있음.

  /// <summary>
  /// 가공한 통일 데이터 List Player - InvenData, Boss - BossData 기반 가공 데이터 데미지 관련 데이터만 포함
  /// skillList 추가 시 각종 버프 관련 데이터를 포함해서 SkillData를 가공 시킬거야
  /// </summary>
  [SerializeField] protected List<GameSkillData> gameSkillDataList = new List<GameSkillData>();

  /// <summary>
  /// 플레이어 - 직업 전용 액티브 스킬
  /// 보스 - 보스 전용 액티브 스킬
  /// </summary>
  private GameSkillData uniqueSkillData = new GameSkillData();

  /// <summary>
  /// 공용 일반 평타 스킬 공격 - 쿨타임은 JobData 공격속도를 따라간다
  /// </summary>
  private GameSkillData commonSkillData = new GameSkillData();

  //스킬 자동 사용에 사용되는 토큰
  private CancellationTokenSource cenceltoken;


  protected virtual void Awake() { }

  protected virtual void Start() { }


  [SerializeField] private SerializableDictionary<int, float> skillCoolTimer = new SerializableDictionary<int, float>();
  [SerializeField] private Queue<int> skillCoolDownkeys = new Queue<int>();

  protected void Update() 
  {
    skillCoolDownkeys.Clear();

    foreach (var key in skillCoolTimer.Keys)
      skillCoolDownkeys.Enqueue(key);

    while (skillCoolDownkeys.Count > 0)
    {
      int key = skillCoolDownkeys.Dequeue();

      if (skillCoolTimer.ContainsKey(key))
      {
        skillCoolTimer[key] -= Time.deltaTime;

        if (skillCoolTimer[key] <= 0)
        {
          skillCoolTimer.Remove(key);
        }
      }
    }
  }

  protected bool IsCoolDown(int skillIndex)
  {
    return !skillCoolTimer.ContainsKey(skillIndex);
  }

  public void SetAttacker(IAttackable attackable)
  {
    this.IAttacker = attackable;
  }

  public GameSkillData GetGameSkillData(int index) => gameSkillDataList[index];
  public GameSkillData GetUniqueSkillData() => uniqueSkillData;
  public GameSkillData GetCommonSkillData() => commonSkillData;


  public List<GameSkillData> GetGameSkillDataList() => gameSkillDataList;

  /// <summary>
  /// 얘는 공속기반으로 작동하는거라 따로 처리하는 부분이 없습니다.
  /// </summary>
  /// <param name="gameSkillData"></param>
  protected void UpdateCommonSkillData(GameSkillData gameSkillData)
  {
    commonSkillData = gameSkillData;
  }
  protected void UpdateJobSkillData(GameSkillData gameSkillData)
  {
    if (uniqueSkillData != null)
      this.RemoveCoolTime(uniqueSkillData);

    uniqueSkillData = gameSkillData;
  }

  protected void RemoveCoolTime(GameSkillData gameSkillData)
  {
    if (skillCoolTimer.ContainsKey(gameSkillData.skillIdx))
      skillCoolTimer.Remove(gameSkillData.skillIdx);
  }

  protected void ClearGameSkillDataList()
  {
    for (int i = 0; i < gameSkillDataList.Count; i++)
    {
      if (gameSkillDataList[i] != null)
      {
        RemoveCoolTime(gameSkillDataList[i]);
      }
    }

    gameSkillDataList.Clear();
  }



  /// <summary>
  /// 스킬 데이터 초기화 및 설정
  /// </summary>
  protected abstract void InitSkill();

  public abstract bool IsValidTarget();

  public abstract Vector3 GetTarget();

  protected abstract GameSkillData CalculateSkillData(IdleSkillData idleSkillData);

  #region 스킬 상태 확인
  /// <summary>
  /// 스킬 사용 가능 상태 확인
  /// </summary>
  protected virtual bool IsCanUseSkill(int skillIdx)
  {
    if (skillIdx.Equals(0))
      return false;

    return IsCoolDown(skillIdx);
  }

  #endregion

  #region 스킬 사용

  /// <summary>
  /// 고유 스킬 사용
  /// 플레이어 - 직업 스킬
  /// 보스 - 보스 스킬
  /// </summary>
  public virtual void ExcuteUniqueSkill()
  {
    ExcuteSkill(uniqueSkillData);
  }

  public void ExcuteUISkill(int index)
  {
    ExcuteSkill(GetGameSkillData(index));
  }

  /// <summary>
  /// 위의 ExcuteSkill 를 각 SkillController에서 Override를 진행
  /// 그리고 추가적인 함수들을 사용하고
  /// 이 함수를 최종으로 사용하게됨
  /// </summary>
  /// <param name="gameSkillData"></param>
  /// <param name="statDic"></param>
  public virtual void ExcuteSkill(GameSkillData gameSkillData)
  {
    if (gameSkillData == null)
      return;

    skillCoolTimer.Add(gameSkillData.skillIdx, gameSkillData.coolTime);

    SkillType skillType = (SkillType)gameSkillData.skillType;

    ActiveSkillBase skill = inGameManager.SkillTypeGetObject(gameSkillData, skillType);
    

    if (skill != null)
    {
      skill.SetAttackerUnit(IAttacker);
      skill.InitSkill(gameSkillData, GetTarget());

      skill.OnActivate();
      skill.StartSkill();


      skillManager.skillActorList.Add(skill);
    }
  }

  /// <summary>
  /// 공용 평타 스킬 사용
  /// 이 부분 Enemy와 같이 사용하게 된다면, 내 위치, Target 파라미터로 넘겨주면 될듯 싶습니다.
  /// 얘는 유닛의 공격 애니메이션 호출시 실행되는거라 유닛의 공격속도에 영향을 받음
  /// </summary>
  public void ExcuteCommonSkill()
  {
    ActiveSkillBase skill = inGameManager.SkillTypeGetObject(commonSkillData, SkillType.Projectile);

    if (skill != null)
    {
      skill.SetAttackerUnit(IAttacker);
      skill.InitSkill(commonSkillData, GetTarget()); 
      skill.OnActivate();
      skill.StartSkill();


      skillManager.skillActorList.Add(skill);
    }
  }

  

  
  #endregion

  #region 자동 스킬 사용 관련

  /// <summary>
  /// 스킬 자동 사용 시작
  /// </summary>
  /// <param name="isUseForce">강제 시작 여부 판단</param>
  public virtual void ExcuteAutoSkill()
  {
    Debug.Log("ExcuteAutoSkill");

    PlayerPrefs.SetInt("AutoSkill", 1);

    cenceltoken = new CancellationTokenSource();
    UpdateAutoSkill(cenceltoken.Token).Forget();
  }

  /// <summary>
  /// 스킬 자동 사용 취소
  /// </summary>
  public virtual void CancelAutoSkill()
  {
    PlayerPrefs.SetInt("AutoSkill", 0);

    Debug.Log("CancelAutoSkill");

    cenceltoken?.Cancel();
  }



  /// <summary>
  /// 어차피 보스도 스킬을 사용할거면 Auto기능이 내장되어 있어야 하지 않나?
  /// 배속 기능 추가 해야함
  /// </summary>
  /// <param name="cenceltoken"></param>
  /// <returns></returns>
  private async UniTaskVoid UpdateAutoSkill(CancellationToken cenceltoken)
  {
    while (true)
    {
      if(uniqueSkillData != null)
      {
        if(IsCanUseSkill(uniqueSkillData.skillIdx))
          ExcuteUniqueSkill();
      }

      for (int i = 0; i < gameSkillDataList.Count; i++)
      {
        if (gameSkillDataList[i] != null)
        {
          if (IsCanUseSkill(gameSkillDataList[i].skillIdx))
            ExcuteSkill(gameSkillDataList[i]);
        }
      }

      // 0.5초 대기 후 다음 업데이트 관련 상수는 따로 빼던지 기메타데이터 받아서 하던지 하자
      await UniTask.Delay(100, cancellationToken: cenceltoken);
    }
  }

  

  #endregion


  /// <summary>
  /// 컨트롤러 Destroy
  /// </summary>
  public virtual void DestroySkillController()
  {
    this.gameSkillDataList.Clear();

    cenceltoken?.Cancel();

  }

  private void OnDestroy()
  {
    DestroySkillController();
  }




}
