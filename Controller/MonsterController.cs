using Cysharp.Threading.Tasks;
using Global_Define;
using Newtonsoft.Json;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;



public class MonsterController : CombatUnitBase, IPoolable, IAnimationStateListener, ICrowdControllable
{
  //얘는 다중 스킨 사용할 가능성이 있어서 Interface기준으로 작업
  public SpineAnimationBase spineAnimation;
  public SpineSkinBase spineSkin;

  public DamageHandler damagerHandler;

  private NewInGameManager inGameManager;
  private GameDataManager gameDataManager;

  private MonsterData monsterData;


  [Header("CC Component")]
  //Movement 관련 단일책임 구조로 스크립트 분리 필요
  //해당 CC 관련 컴포넌트도 Movement에서 사용 가능하게 작업 필요
  private CrowdControlType ccType;

  private float slowSpeed = 1f;

  private Coroutine ccCoroutine;
  public CrowdControlType CCType => ccType;



  public void Awake()
  {
    inGameManager = NewInGameManager.getInstance;
    gameDataManager = GameDataManager.getInstance;
    damageCollider = GetComponent<BoxCollider2D>();

    damagerHandler.SetDamageable(this);
  }

  public override void Update()
  {
    if (inGameManager.PlayingWaveStage())
    {
      base.Update();
    }
  }


  /// <summary>
  /// 몬스터 초기화 함수.
  /// </summary>
  /// <param name="_initPosition">생성되는 위치Position</param>
  /// <param name="_targetTransform">플레이어 Transform</param>
  /// <param name="_hudBar">몬스터 HUDBar</param>
  public void SetMonster(Vector3 _initPosition, MonsterData data)
  {
    this.monsterData = data;

    float statWeight = IdleStageTable.getInstance.GetStageData(gameDataManager.stageIndex).weight;

    statBuffController.SetData(monsterData, statWeight);

    float randomX = Random.Range(_initPosition.x, _initPosition.x + 2f);
    float randomY = Random.Range(_initPosition.y - 1f, _initPosition.y + 1f);

    gameObject.transform.position = new Vector3(randomX, randomY, _initPosition.z);
    transform.localScale = Vector3.one * monsterData.monsterScale;

    spineSkin.SetSkin($"{data.monsterSpine.ToLower()}_{data.monsterSpineSkin:D2}");

    damagerHandler.SetHudBar(false);

    InitCombatUnit();

    damageCollider.enabled = true;

    spineAnimation.OnAnimationComplete = OnAnimationComplete;

    base.SetTargetUnit(GetTargetUnit());
  }

  public void OnAnimationComplete(string animationName)
  {
    // 특정 애니메이션이 종료될 때 실행할 함수
    if (animationName == MonsterAnimation.death.ToString())
    {
      ReleaseMonster();
    }
  }


  /// <summary>
  /// 몬스터, HUDBar ReturnPool
  /// </summary>
  public void ReleaseMonster()
  {
    transform.position = Vector3.zero;

    NewInGameManager.getInstance.ReturnObjectPoolTypeMonster(this, monsterData.monsterSpine);

    InitCCCoroutine();

    damagerHandler.ReleaseHUDBar();

  }

  public override void DeadAction()
  {
    NewSoundManager.Instance.PlaySFXSound("SFX_MONSTER_HIT_01_0");

    damageCollider.enabled = false;
    if (monsterData.monsterType == IdleMonsterType.normal)
      inGameManager.monsterKillCount++;

    DropGold();
    DropTicket();


    spineAnimation.SetAnimation(MonsterAnimation.death.ToString(), loop: false);

  }


  private void DropGold()
  {
    float rate = Random.Range(0f, 100f);

    if (rate <= monsterData.goldDropRate)
    {
      DropItem dropItem = (DropItem)inGameManager.dropItemPool.GetObject();

      if (dropItem != null)
      {
        dropItem.SetDropItemData(ConstantManager.ITEM_CURRENCY_GOLD, monsterData.gold);
        dropItem.UpdateUIPosition(transform.position);
        dropItem.OnActivate();
      }
    }
  }

  private void DropTicket()
  {
    float rate = Random.Range(0f, 100f);

    if (rate <= monsterData.monsterDropRate)
    {
      inGameManager.monsterDropTicket += monsterData.monsterDropCount;
    }

  }


  public void SetOrderInLayer(int _index)
  {
    spineSkin.SetLayer(_index);
  }

  public void OnActivate()
  {
    gameObject.SetActive(true);
  }

  public void OnDeactivate()
  {
    gameObject.SetActive(false);

  }

  public void TakeCrowdControl(CrowdControlData ccData)
  {
    InitCCCoroutine();

    if (IsDead())
      return;

    CrowdControlType ccType = ccData.ccType;
    float ccValue = ccData.ccValue;
    float ccTime  = ccData.ccTime;

    switch (ccType)
    {
      case CrowdControlType.Stun:         ccCoroutine = StartCoroutine(HandleStun(ccTime));            break;
      case CrowdControlType.Knockback:    ccCoroutine = StartCoroutine(HandleKnockback(ccValue));      break;
      case CrowdControlType.Slow:         ccCoroutine = StartCoroutine(HandleSlow(ccValue, ccTime));   break;
      case CrowdControlType.Immobilize:   ccCoroutine = StartCoroutine(HandleImmobilize(ccTime));      break;
      default:
        break;
    }
  }

  
  
  //Idle 처리?
  public IEnumerator HandleStun(float stunTime)
  {
    this.ccType = CrowdControlType.Stun;

    if (!spineAnimation.IsPlaying(MonsterAnimation.idle.ToString()))
      spineAnimation.SetAnimation(MonsterAnimation.idle.ToString());

    yield return YieldInstructionCache.WaitForSeconds(stunTime);

    this.ccType = CrowdControlType.None;
  }

  //Idle 처리
  public IEnumerator HandleImmobilize(float immobilizeTime)
  {
    this.ccType = CrowdControlType.Immobilize;

    if (!spineAnimation.IsPlaying(MonsterAnimation.idle.ToString()))
      spineAnimation.SetAnimation(MonsterAnimation.idle.ToString());

    yield return YieldInstructionCache.WaitForSeconds(immobilizeTime);

    this.ccType = CrowdControlType.None;
  }

  //단순 우측 이동
  public IEnumerator HandleKnockback(float knockbackValue)
  {
    this.ccType = CrowdControlType.Knockback;

    if (!spineAnimation.IsPlaying(MonsterAnimation.hit.ToString()))
      spineAnimation.SetAnimation(MonsterAnimation.hit.ToString(), loop: false);

    float knockbackTime = 0.3f;

    float velocity = (knockbackValue * 0.01f) / knockbackTime; // 전체 거리를 시간으로 나눔
    float elapsed = 0f;

    while (elapsed < knockbackTime)
    {
      float damping = Mathf.Lerp(1f, 0f, elapsed / knockbackTime); // 점점 느려짐
      transform.position += Vector3.right * velocity * damping * Time.deltaTime;
      elapsed += Time.deltaTime;
      yield return null;
    }


    this.ccType = CrowdControlType.None;
  }

  //단순 이동속도 제어
  public IEnumerator HandleSlow(float slowValue, float slowTime)
  {
    spineSkin.SetColor(ConstantManager.EFFECT_CC_SLOW_COLOR);

    slowSpeed = 1 - (slowValue * 0.01f);

    yield return YieldInstructionCache.WaitForSeconds(slowTime);

    spineSkin.SetColor(Color.white);
    slowSpeed = 1f;

  }

  private void InitCCCoroutine()
  {
    if (ccCoroutine != null)
    {
      StopCoroutine(ccCoroutine);
      ccCoroutine = null;
    }

    ccType = CrowdControlType.None;

    spineSkin.SetColor(Color.white);
    slowSpeed = 1f;
  }


  public override bool IsInAttackRange()
  {
    return transform.position.x - GetTargetUnit().GetDamagerPosition().x <= monsterData.attackRange;
  }


  public override Dictionary<StatType, float> GetTotalStats()
  {
    return statBuffController.GetTotalStats();
  }

  public override void TakeDamage(IAttackable attackable, SkillAttackType skillAttackType, float damagePer, SkillType skillType)
  {
    damagerHandler.TakeDamage(attackable, skillAttackType, damagePer, skillType);
  }



  

  protected override CombatUnitBase GetTargetUnit()
  {
    return inGameManager.player;
  }

  protected override void PlayIdle()
  {
    if (!spineAnimation.IsPlaying(MonsterAnimation.idle.ToString()))
      spineAnimation.SetAnimation(MonsterAnimation.idle.ToString());
  }

  protected override void PlayAttack()
  {
    spineAnimation.SetAnimation(MonsterAnimation.attack.ToString(), loop: false);
    spineAnimation.AddAnimation(MonsterAnimation.idle.ToString());
    targetUnit.TakeDamage(this, SkillAttackType.NormalAttack, 1f, SkillType.None);
  }

  protected override void PlayMove()
  {
    if(CCType == CrowdControlType.None)
    {
      transform.position += (unitSpeed * slowSpeed * Vector3.left) * Time.deltaTime;

      if (!spineAnimation.IsPlaying(MonsterAnimation.move.ToString()))
        spineAnimation.SetAnimation(MonsterAnimation.move.ToString());
    }
  }
}
  
