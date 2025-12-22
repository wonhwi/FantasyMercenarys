using UnityEngine;
using FantasyMercenarys.Data;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class CombatUtility
{
  public static bool ValidEnemyTarget()
  {
    bool valid = false;

    List<MonsterController> monsterControllerList = NewInGameManager.getInstance.GetMosterControllerList();

    for (int i = 0; i < monsterControllerList.Count; i++)
    {
      MonsterController monster = monsterControllerList[i];

      if (!monster.IsDead() && monster.gameObject.activeInHierarchy)
      {
        valid = true;
      }
    }

    return valid;
  }

  /// <summary>
  /// 적 유닛 위치 받아 오는 함수
  /// </summary>
  /// <param name="transform"></param>
  /// <returns></returns>
  public static Vector3 GetEnemyTarget(Transform transform)
  {
    List<MonsterController> monsterController = NewInGameManager.getInstance.GetMosterControllerList();
    
    float closestDistance = float.MaxValue;
    Vector3 closestPosition = Vector3.zero;
    
    for (int i = 0; i < monsterController.Count; i++)
    {
      MonsterController monster = monsterController[i];

      if (!monster.IsDead() && monster.gameObject.activeInHierarchy)
      {
        float distance = Vector3.Distance(transform.position, monster.transform.position);
        
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestPosition = monster.GetDamagerPosition();
        }
      }
    }
    
    return closestPosition;
  }


  /// <summary>
  /// 데미지 연산
  /// </summary>
  /// <param name="attackable">데미지 주는 객체</param>
  /// <param name="damageable">데미지 받는 객체</param>
  /// <param name="skillDmgPer">스킬 계수</param>
  /// <param name="skillType">스킬 타입</param>
  /// <returns></returns>
  public static (float totalDamage, bool isCritical) GetDamage(IAttackable attackable, IDamageable damageable, SkillAttackType skillAttackType, float skillDmgPer, SkillType skillType)
  {
    float criticalRandomRate = Random.Range(0f, 100f);

    float attackValue  = attackable.GetStatValue(StatType.AttackPower);  //공격자의 공격력
    float defenceValue = damageable.GetStatValue(StatType.Defense);      //피해자의 방어력

    float criticalRate        = attackable.GetStatValue(StatType.CriticalChance);       //일반 치명타 확률
    float skillCriticalRate   = attackable.GetStatValue(StatType.SkillCriticalChance);  //스킬 치명타 확률

    float skillDamageIncrease = attackable.GetStatValue(StatType.SkillDamageIncrease);  //스킬 피해 증가

    float criticalDamage      = 0f;  //일반 치명타 피해
    float skillCriticalDamage = 0f;  //스킬 치명타 피해
    
    float damageDecrease      = damageable.GetStatValue(StatType.DamageReduction);      //피해 감소
    float damageDecreasePer   = damageable.GetStatValue(StatType.DamageReductionPer);   //피해 감소


    //동료피해, 보스 피해 등등 추가 데미지 퍼센트 적용
    float bonusDamagePercent   = GetBonusDamagePercent(attackable, damageable, skillType);
    float bonusDecreasePercent = GetBonusDecreasePercent(attackable, damageable, skillType);

    //Debug.LogError(attackable.GetUnitType().ToString() +  JsonConvert.SerializeObject(attackable.GetTotalStats(), Formatting.Indented));


    bool isCritical = criticalRandomRate < criticalRate;
    bool isSkillCritical = false;

    float totalDamage = 0;

    if (isCritical)
    {
      criticalDamage = attackable.GetStatValue(StatType.CriticalDamage);
    }

    if(skillAttackType == SkillAttackType.SkillAttack)
    {
      float skillCriticalRandomRate = Random.Range(0f, 100f);

      isSkillCritical = skillCriticalRandomRate < skillCriticalRate;

      if (isSkillCritical)
      {
        skillCriticalDamage = attackable.GetStatValue(StatType.SkillCriticalDamage);
      }
    }

    totalDamage = (attackValue - defenceValue) * 
      (1 + ((criticalDamage + skillDamageIncrease + skillCriticalDamage + bonusDamagePercent) * 0.01f)) * 
      (skillDmgPer) *
      (1 - ((damageDecreasePer + bonusDecreasePercent) * 0.01f))
      - damageDecrease;


    totalDamage = Mathf.Clamp(totalDamage, 1f, totalDamage);

    return (totalDamage, isCritical || isSkillCritical);
  }

  /// <summary>
  /// 보너스 피해량 계수
  /// </summary>
  private static float GetBonusDamagePercent(IAttackable attacker, IDamageable damageable, SkillType skillType)
  {
    float bonusDamagePercent = 0f;

    FactionType factionType = attacker.GetFactionType();
    UnitType attackerType   = attacker.GetUnitType();
    UnitType damageableType = damageable.GetUnitType();

    //아군이 공격했을때
    if(factionType == FactionType.Friendly)
    {
      //보스 피해 증가 (아군 - 플레이어, 동료)
      if (damageableType == UnitType.BossMonster)
      {
        bonusDamagePercent += attacker.GetStatValue(StatType.BossDamageIncrease);
        //Debug.LogError($"보스 피해 증가 보너스 + {attacker.GetStatValue(StatType.BossDamageIncrease)}");
      }

      //동료 피해 증가
      if (attackerType == UnitType.Partner)
      {
        bonusDamagePercent += attacker.GetStatValue(StatType.PartnerDamageIncrease);
        //Debug.LogError($"동료 피해 증가 보너스 + {attacker.GetStatValue(StatType.PartnerDamageIncrease)}");
      }

      //범위 피해 증가
      if(skillType == SkillType.Area || skillType == SkillType.ForwardArea)
      {
        bonusDamagePercent += attacker.GetStatValue(StatType.SkillAreaDamageIncrease);
        //Debug.LogError($"범위 피해 증가 보너스 + {attacker.GetStatValue(StatType.SkillAreaDamageIncrease)}");
      }
    }

    return bonusDamagePercent;
  }

  /// <summary>
  /// 보너스 피해 감소량 계수
  /// </summary>
  private static float GetBonusDecreasePercent(IAttackable attacker, IDamageable damageable, SkillType skillType)
  {
    float bonusDecreasePercent = 0f;

    FactionType factionType = attacker.GetFactionType();
    UnitType attackerType = attacker.GetUnitType();
    UnitType damageableType = damageable.GetUnitType();

    //적군이 공격했을때 (플레이어 대미지 받았을때)
    if (factionType == FactionType.Enemy)
    {
      //보스 피해 감소
      if(attackerType == UnitType.BossMonster)
      {
        if (damageableType == UnitType.Player)
        {
          bonusDecreasePercent += attacker.GetStatValue(StatType.BossDamageReduction);
          //Debug.LogError($"보스 피해 감소 보너스 + {attacker.GetStatValue(StatType.BossDamageReduction)}");
        }
      }
    }

    return bonusDecreasePercent;
  }
}