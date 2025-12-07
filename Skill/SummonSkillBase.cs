using System.Collections;
using UnityEngine;

public class SummonSkillBase : ActiveSkillBase
{
  public GameSkillData subSkillData = new GameSkillData();

  protected float attackInterval = 1f;

  private Coroutine attackCoroutine;


  public override void InitSkill(GameSkillData gameSkillData, Vector3 targetPos)
  {
    base.InitSkill(gameSkillData, targetPos);

    //SubSkill 설정
    this.SetSubSkill(gameSkillData);
    //내 스킬 생성 위치 설정
    startPos = IAttacker.GetAttackerForwardPoint();

    startPos.x += -1.5f;
    startPos.y += 3f;
  }

  public virtual void SetSubSkill(GameSkillData gameSkillData)
  {
    subSkillData.factionType = IAttacker.GetFactionType();
    subSkillData.skillIdx    = gameSkillData.skillIdx;

    subSkillData.skillEffectSpine = gameSkillData.subSkillEffectSpine;
    subSkillData.skillDmgPer      = gameSkillData.skillDmgPer;

    subSkillData.sound            = gameSkillData.sound;

    subSkillData.skillHitEnemyCount = gameSkillData.skillHitEnemyCount;

    subSkillData.skillType          = gameSkillData.subSkillType;
    subSkillData.startOffsetPos     = gameSkillData.startOffsetPos;
  }

  public override void StartSkill()
  {
    base.StartSkill();

    attackCoroutine = StartCoroutine(AttackRoutine());
  }

  private IEnumerator AttackRoutine()
  {
    float elapsed = 0f;

    while (elapsed < lifeTime)
    {
      Attack();

      yield return YieldInstructionCache.WaitForSeconds(attackInterval);
      elapsed += attackInterval;
    }

    ReleaseSkillData();
  }

  /// <summary>
  /// 공격 실행
  /// </summary>
  private void Attack()
  {
    if (!CombatUtility.ValidEnemyTarget())
      return;
    
    ActiveSkillBase skill = NewInGameManager.getInstance.SkillTypeGetObject(subSkillData, (SkillType)subSkillData.skillType);
    
    if (skill != null)
    {
      skill.SetAttackerUnit(IAttacker);
      skill.InitSkill(subSkillData, CombatUtility.GetEnemyTarget(this.transform));
    
      skill.OnActivate();
      skill.StartSkill();
    
      SkillManager.getInstance.skillActorList.Add(skill);
    }

  }

  public override void ReleaseSkillData()
  {
    attackCoroutine = null;

    base.ReleaseSkillData();
  }

  protected override void SetAnimation(string skinName)
  {
    spineAnimation.SetAnimation("Item_skill_character");
  }

  protected override void TakeDamageEvent(IDamageable idamageable) { }

}
