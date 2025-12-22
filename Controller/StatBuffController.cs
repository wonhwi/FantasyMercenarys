using DTT.AreaOfEffectRegions.Shaders;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Threading;

/// <summary>
/// 플레이어, 몬스터들의 기본 스텟 정보들을 저장하는 Class이고
/// 추후 버프, 디버프 스킬이 추가 될때 이 Class로 Controller 할 예정
/// 이 클래스에 있는 데이터들도 Total 데미지 연산에 포함이 됨
/// 여기에 데미지를 받는 로직을 이동할 예정
/// </summary>
public class StatBuffController
{
  /// <summary>
  /// 기본 스텟 정보 (플레이어, 몬스터, 동료)
  /// </summary>
  private Dictionary<StatType, float> unitStats = new Dictionary<StatType, float>();

  /// <summary>
  /// 스킬 패시브 스텟 (플레이어)
  /// </summary>
  private Dictionary<StatType, float> passiveStats = new Dictionary<StatType, float>();

  /// <summary>
  /// 스킬 버프/디버프 스텟 (플레이어)
  /// </summary>
  private Dictionary<StatType, float> passiveBuffDebuffStats = new Dictionary<StatType, float>();

  /// <summary>
  /// 현재 객체에 부여된 버프/디버프 효과 정보 (플레이어, 몬스터) - 동료는 플레이어 스텟 따라가게?
  /// </summary>
  private Dictionary<StatType, float> activeBuffDebuffStats = new Dictionary<StatType, float>();


  /// <summary>
  /// Unit Total Stat 상태
  /// </summary>
  private Dictionary<StatType, float> totalStats = new Dictionary<StatType, float>();


  public Dictionary<StatType, float> GetUnitStats()
  {
    return unitStats;
  }

  public Dictionary<StatType, float> GetPassiveStats()
  {
    return passiveStats;
  }

  /// <summary>
  /// 최종 스텟에 대한 정보 (버프. 디버프 미포함)
  /// </summary>
  /// <returns></returns>
  public virtual Dictionary<StatType, float> GetTotalStats()
  {
    totalStats.Clear();

    foreach (var stat in unitStats)
    {
      SetStatData(totalStats, stat.Key, stat.Value);
    }

    foreach (var stat in passiveStats)
    {
      SetStatData(totalStats, stat.Key, stat.Value);
    }

    return totalStats;
  }

  public float GetStatValue(StatType statType)
  {
    unitStats.TryGetValue(statType, out float value);

    return value;
  }
  

  private void SetStatData(Dictionary<StatType, float> targetStat, StatType statType, float statValue)
  {
    if (statType != StatType.None)
      if (!targetStat.ContainsKey(statType))
        targetStat[statType] = statValue;
      else
        targetStat[statType] += statValue;
  }

  /// <summary>
  /// 직업 Class 기본 스텟 설정
  /// 여기에 직업 기본 Passive Stat 정보도 추가해주기
  /// </summary>
  /// <param name="jobData"></param>
  public void SetData(JobData jobData)
  {
    unitStats.Clear();
    passiveStats.Clear();

    StatType[] statTypes = ConstantManager.STAT_TYPES_PLAYER_JOB;

    float[] statValues = new float[] {
      jobData.health,
      jobData.attackDmg,
      jobData.defence,
      jobData.criticalRate,
      jobData.criticalDmg,
      jobData.dodge,
      jobData.accuracy,
      jobData.attackSpeed,
    };

    for (int i = 0; i < statTypes.Length; i++)
    {
      StatType statType = statTypes[i];
      float statValue = statValues[i];

      SetStatData(unitStats, statType, statValue);
    }

    //여기에 패시브 정보도 추가
    var passiveSkillList = SkillTable.getInstance.GetJobPassiveSkill(jobData.jobIdx);

    foreach (var skillData in passiveSkillList)
    {
      StatType statType1 = (StatType)skillData.statType1;
      StatType statType2 = (StatType)skillData.statType2;
      StatType statType3 = (StatType)skillData.statType3;

      SetStatData(passiveStats, statType1, skillData.statValue1);
      SetStatData(passiveStats, statType2, skillData.statValue2);
      SetStatData(passiveStats, statType3, skillData.statValue3);
    }

  }

  /// <summary>
  /// 몬스터 기본 스텟 설정
  /// </summary>
  /// <param name="monsterData"></param>
  /// <param name="weight"></param>
  public void SetData(MonsterData monsterData, float weight)
  {
    unitStats.Clear();

    StatType[] statTypes = ConstantManager.STAT_TYPES_MONSTER;
    StatType[] statTypesWeight = ConstantManager.STAT_TYPES_MONSTER_WEIGHT;

    float[] statValues = new float[] {
      monsterData.health,
      monsterData.attackDmg,
      monsterData.defence,
      monsterData.criticalRate,
      monsterData.criticalDmg,
      monsterData.dodge,
      monsterData.accuracy,
      monsterData.attackSpeed,
    };

    //기본 스텟 설정
    for (int i = 0; i < statTypes.Length; i++)
    {
      StatType statType = statTypes[i];
      float statValue = statValues[i];

      SetStatData(unitStats, statType, statValue);
    }

    //Weight만큼 곱하기
    for (int i = 0; i < statTypesWeight.Length; i++)
    {
      StatType statType = statTypesWeight[i];

      if (unitStats.ContainsKey(statType))
        unitStats[statType] = StatUtility.GetStatValue(statType, unitStats[statType] * weight);
    }
  }

  /// <summary>
  /// 동료 기본 스텟 설정
  /// </summary>
  /// <param name="partnerData"></param>
  public void SetData(PartnerData partnerData)
  {
    unitStats.Clear();

    StatType[] statTypes = ConstantManager.STAT_TYPES_PARTNER;

    var playerTotalStat = PlayerStatManager.getInstance.playerTotalStats;

    int playerPower = (int)playerTotalStat[StatType.AttackPower];
    float partnerAttackSpeed = playerTotalStat[StatType.PartnerAttackSpeed];

    for (int i = 0; i < statTypes.Length; i++)
    {
      StatType statType = statTypes[i];

      SetStatData(unitStats, statType, playerTotalStat[statType]);
    }

    unitStats[StatType.AttackPower]  = Mathf.RoundToInt(partnerData.damageMultiplier * playerPower);
    unitStats[StatType.AccuracyRate] = 100;
    unitStats[StatType.AttackSpeed]  = partnerData.attackSpeed * (1f + (partnerAttackSpeed * 0.01f));

  }

}
