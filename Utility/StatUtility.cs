using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StatUtility
{
  public static int GetStatTotalPower(Dictionary<StatType, float> statDataDic)
  {
    float statPower = 0;
    foreach (var statData in statDataDic)
    {
      statPower += GetStatPower(statData.Key, statData.Value);
    }
    return (int)statPower;

  }

  public static float GetStatPower(StatType statType, float statValue)
  {
    float powerValue = GetBaseStatPowerValue((int)statType);

    return statValue * powerValue;
  }

  /// <summary>
  /// BaseTable의 스텟 전투력계수 반환
  /// </summary>
  /// <param name="statType"></param>
  /// <returns></returns>
  public static float GetBaseStatPowerValue(int statType)
  {
    return BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_STAT_POWER_MULTIPLY + statType);
  }

  public static void CombineStatDic(Dictionary<StatType, float> origin, Dictionary<StatType, float> targetDic)
  {
    foreach (var dic in targetDic)
    {
      if (!origin.ContainsKey(dic.Key))
        origin.Add(dic.Key, dic.Value);
      else
        origin[dic.Key] += dic.Value;
    }
  }

  public static void CalculateTotalStat(Dictionary<StatType, float> dictTotalStat, Dictionary<StatType, float> dictBonusStat)
  {
    StatType[] multiplierStats = ConstantManager.STAT_TYPES_TOTAL_MULTIPLIER_STAT;

    foreach (var stat in dictBonusStat)
    {
      StatType statType = GetConvertStatType(stat.Key);
      float statValue = stat.Value;

      //곱연산 할놈들
      if (multiplierStats.Contains(statType))
      {
        dictTotalStat[statType] = GetStatValue(statType, dictTotalStat[statType] * (1f + (statValue * 0.01f)));
      }
      //합연산 할 놈들
      else
      {
        dictTotalStat.TryGetValue(statType, out float tryValue);
        dictTotalStat[statType] = GetStatValue(statType, tryValue + statValue);
      }
      
    }
    
  }

  public static float GetStatValue(StatType statType, float statValue)
  {
    StatType[] integerStats = ConstantManager.STAT_TYPES_INTEGER_STAT;

    if(integerStats.Contains(statType))
    {
      return Mathf.RoundToInt(statValue);
    }
    else
    {
      return Mathf.Round(statValue * 1000f) * 0.001f;
    }
  }

  /// <summary>
  /// 최종 스텟 계산시 보너스 스텟의 타겟 설정
  /// </summary>
  /// <param name="statType"></param>
  /// <returns></returns>
  public static StatType GetConvertStatType(StatType statType) => statType switch
  { 
    StatType.HealthPer      => StatType.Health,
    StatType.AttackPowerPer => StatType.AttackPower,
    StatType.DefensePer     => StatType.Defense,

    _ => statType
  };

  /// <summary>
  /// 스텟 출력 시 보너스 스텟의 값 반환
  /// </summary>
  /// <param name="statType"></param>
  /// <returns></returns>
  public static StatType GetConvertBonusStatType(StatType statType) => statType switch
  {
    StatType.Health => StatType.HealthPer,
    StatType.AttackPower => StatType.AttackPowerPer,
    StatType.Defense => StatType.DefensePer,

    _ => statType
  };


}