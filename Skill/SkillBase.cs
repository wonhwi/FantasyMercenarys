using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour, IPoolable
{
  [Tooltip("인게임 내 SkillData")]
  [SerializeField] protected GameSkillData skillData;

  [Tooltip("스킬 시전자 정보")]
  protected IAttackable IAttacker;

  private void Update()
  {
    UpdateSkill();
  }

  /// <summary>
  /// 스킬 사용하기전 기본 스킬에 대한 설정
  /// </summary>
  /// <param name="gameSkillData"></param>
  /// <param name="targetPos"></param>
  public abstract void InitSkill(GameSkillData gameSkillData, Vector3 targetPos);

  /// <summary>
  /// 스킬 시작 시 실행하는 함수
  /// 스킬 Sound Play, 위치 조절
  /// </summary>
  public abstract void StartSkill();

  protected virtual void UpdateSkill() { }

  /// <summary>
  /// 스킬 사용 종료시 실행하는 함수
  /// </summary>
  public abstract void ReleaseSkillData();

  /// <summary>
  /// 스킬 시전자 정보 설정 (데미지 스텟 참조 용)
  /// </summary>
  /// <param name="attackable"></param>
  public abstract void SetAttackerUnit(IAttackable attackable);

  /// <summary>
  /// 대미지 전달
  /// </summary>
  /// <param name="idamageable"></param>
  protected abstract void TakeDamageEvent(IDamageable idamageable);

  protected void TakeDamage(IDamageable idamageable)
  {
    idamageable.TakeDamage(IAttacker, GetSkillAttackType(), skillData.skillDmgPer, (SkillType)skillData.skillType);

    // CC 효과 적용
    if (idamageable is ICrowdControllable crowdControllable)
    {
      CrowdControlData ccData = skillData.ccData; //skillData.GetKnockbackData();


      if (ccData.ccType == CrowdControlType.None)
        return;

      // 예시: 스킬에 따라 CC 타입/값 결정
      crowdControllable.TakeCrowdControl(ccData);
    }
  }

  public virtual void OnActivate()
  {
    gameObject.SetActive(true);
  }

  public virtual void OnDeactivate()
  {
    gameObject.SetActive(false);
  }

  protected SkillAttackType GetSkillAttackType() => skillData.skillCategory switch
  {
    SkillCategory.Job or SkillCategory.Partner
      => SkillAttackType.NormalAttack,

    _ => SkillAttackType.SkillAttack,
  };
}
