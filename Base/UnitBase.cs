using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
  [SerializeField] protected FactionType factionType;

  [SerializeField] protected UnitType unitType = UnitType.None;
  [SerializeField] protected float unitSpeed;
  
  
}

// 공격 가능한 유닛의 기본 클래스
public abstract class AttackableUnitBase : UnitBase, IAttackable
{
  public Transform projectilePoint;

  public CombatUnitBase targetUnit;

  public StatBuffController statBuffController = new StatBuffController();

  public float attackInterval = 2f;
  public float attackTime = 0f;

  public FactionType GetFactionType()
    => factionType;

  public UnitType GetUnitType()
    => unitType;

  protected virtual void InitAttackUnit()
  {
    attackTime = attackInterval = 1 / GetStatValue(StatType.AttackSpeed);
  }

  // 공통 스탯 관련 메서드
  public abstract Dictionary<StatType, float> GetTotalStats();
  public float GetStatValue(StatType statType)
  {
    GetTotalStats().TryGetValue(statType, out float value);
    return value;
  }

  protected abstract CombatUnitBase GetTargetUnit();

  protected virtual void SetTargetUnit(CombatUnitBase targetUnit)
  {
    this.targetUnit = targetUnit;
  }

  public abstract bool IsInAttackRange();

  public Vector3 GetAttackerProjectilePoint()
  {
    return projectilePoint.position;
  }

  public Vector3 GetAttackerForwardPoint()
  {
    return this.transform.position;
  }

  /// <summary>
  /// 공격 가능 한지
  /// </summary>
  /// <returns></returns>
  protected bool CanAttack()
  {
    attackTime += Time.deltaTime;

    if (attackTime >= attackInterval)
    {
      attackTime = 0f;
      return true;
    }

    return false;
  }


  //여기 Attack 코드 넣고 Override 해서 애니메이션 실행
  protected abstract void PlayIdle();
  protected abstract void PlayAttack();
  protected abstract void PlayMove();
}


/// <summary>
/// 전투 유닛
/// </summary>
public abstract class CombatUnitBase : AttackableUnitBase, IDamageable
{
  [SerializeField] protected BoxCollider2D damageCollider;
  public float currentHP;
  public float maxHp = 10f;

  public float CurrentHP
  {
    get =>
      currentHP;
    set
    {
      currentHP = value;

      if (currentHP < 0)
        currentHP = 0;
    }
  }

  public float MaxHP
  {
    get =>
      maxHp;
    private set
    {
      maxHp = value;
    }
  }

  /// <summary>
  /// 유닛 초기화
  /// </summary>
  protected virtual void InitCombatUnit()
  {
    base.InitAttackUnit();

    currentHP = maxHp = GetStatValue(StatType.Health);
  }

  public virtual void Update()
  {
    //GetState Abstract해서 State별로 함수 실행하게 To Do
    if (!IsDead())
    {
      if (IsInAttackRange())
      {
        if (targetUnit.IsDead())
        {
          PlayIdle();
          return;
        }


        if (CanAttack())
          PlayAttack();
      }
      else
      {
        PlayMove();
      }
    }
  }

  

  
  public bool IsDead()
  {
    return CurrentHP <= 0f;
  }


  public Vector3 GetDamagerPosition()
  {
    return this.transform.position;
  }

  public abstract void DeadAction();

  public abstract void TakeDamage(IAttackable attackable, SkillAttackType skillAttackType, float damagePer, SkillType skillType);
}


