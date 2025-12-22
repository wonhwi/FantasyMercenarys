using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class FormatUtility
{
  private static readonly Dictionary<int, string> romanValueMap = new Dictionary<int, string>()
  {
    //{ 1000, "M" }, {900, "CM"}, {500, "D"}, {400, "CD"},
    //{100, "C"}, {90, "XC"}, {50, "L"}, {40, "XL"},
    {10, "X"}, {9, "IX"}, {5, "V"}, {4, "IV"}, {1, "I"}
  };

  /// <summary>
  /// 가호 석상에서 사용하는 스텟 Type Naming
  /// </summary>
  /// <param name="statType"></param>
  /// <returns></returns>
  public static string GetStatTypeNameBlessing(int statType) => (StatType)statType switch
  {
    StatType.AttackPower           => "전체 공격력",
    StatType.Defense               => "전체 방어력",
    StatType.Health                => "전체 체력",
    StatType.CriticalDamage        => "치명타 피해",
    StatType.SkillDamageIncrease   => "스킬 피해",
    StatType.BossDamageIncrease    => "보스 피해",
    StatType.PartnerDamageIncrease => "동료 피해",
    StatType.AccuracyRate          => "명중률",
    _ => "[None]",
  };


  /// <summary>
  /// StatTypeName 반환
  /// </summary>
  /// <param name="statType"></param>
  /// <returns></returns>
  public static string GetStatTypeName(StatType statType) => statType switch
  {
    StatType.Health                 => "체력",
    StatType.AttackPower            => "공격력",
    StatType.Defense                => "방어력",
    StatType.CriticalChance         => "크리티컬 확률",
    StatType.CriticalDamage         => "크리티컬 배율",
    StatType.DodgeRate              => "회피율",
    StatType.AccuracyRate           => "명중률",
    StatType.AttackSpeed            => "공격 속도",
    StatType.DamageReduction        => "피해 감소",
    StatType.SkillDamageIncrease    => "스킬 피해 증가",
    StatType.SkillCriticalChance    => "스킬 치명타 확률",
    StatType.SkillCriticalDamage    => "스킬 치명타 피해",
    StatType.BossDamageIncrease     => "보스 피해 증가",
    StatType.BossDamageReduction    => "보스 피해 감소",
    StatType.PartnerDamageIncrease  => "동료 피해 증가",
    StatType.PartnerDamageReduction => "동료 피해 감소",
    StatType.PartnerAttackSpeed     => "동료 기본 공속 증가",

    StatType.SkillAreaDamageIncrease => "범위 스킬 피해 증가",
    StatType.DamageReductionPer      => "받는 피해 감소",
    StatType.DamageReflection        => "받는 데미지 반사",
    StatType.PartyAttackPowerUpPer   => "팀 전원 공격력 증가"
  };

  /// <summary>
  /// StatType에 따른 단위 반환
  /// </summary>
  /// <param name="statType"></param>
  /// <param name="statValue"></param>
  /// <returns></returns>
  public static string GetStatTypeUnit(StatType statType) => statType switch
  {
    StatType.CriticalChance or
    StatType.CriticalDamage or
    StatType.DodgeRate or
    StatType.AccuracyRate or
    StatType.SkillDamageIncrease or
    StatType.SkillCriticalChance or
    StatType.SkillCriticalDamage or
    StatType.BossDamageIncrease or
    StatType.BossDamageReduction or
    StatType.PartnerDamageIncrease or
    StatType.PartnerDamageReduction or
    StatType.PartnerAttackSpeed or
    StatType.SkillAreaDamageIncrease or
    StatType.DamageReductionPer or
    StatType.DamageReflection or
    StatType.PartyAttackPowerUpPer
      => "%",

    StatType.Health or
    StatType.Defense or
    StatType.AttackSpeed or
    StatType.AttackPower or
    StatType.DamageReduction or
      _ => "",

  };

  /// <summary>
  /// StatType에 따른 Value Formatting
  /// </summary>
  /// <param name="statType"></param>
  /// <param name="statValue"></param>
  /// <returns></returns>
  public static string GetStatValue(StringBuilder sb, StatType statType, float statValue)
  {
    sb.Append(StatUtility.GetStatValue(statType, statValue));
    sb.Append(GetStatTypeUnit(statType));

    return sb.ToString();
  }

  /// <summary>
  /// 능력치 비교
  /// </summary>
  /// <param name="sb"></param>
  /// <param name="statType"></param>
  /// <param name="statValue">내 스텟</param>
  /// <param name="targetValue">비교 대상 스텟</param>
  /// <returns></returns>
  public static string GetStatCompareValue(StringBuilder sb, StatType statType, float statValue, float targetValue)
  {
    float viewStatValue   = StatUtility.GetStatValue(statType, statValue);
    float viewTargetValue = StatUtility.GetStatValue(statType, targetValue);

    float compareStat = viewStatValue - viewTargetValue;

    bool isIntValue = compareStat % 1 == 0;
    int decimals    = isIntValue ? 0 : 3;

    if (compareStat > 0)
    {
      sb.Append($"{viewStatValue.ToString($"F{decimals}")}<color=#00AA00>(+{compareStat.ToString($"F{decimals}")})</color>{GetStatTypeUnit(statType)}");
      sb.Append("<sprite name=fm_up_arrow>");

    }
    else if (compareStat < 0)
    {
      sb.Append($"{viewStatValue.ToString($"F{decimals}")}<color=#AA0000>({compareStat.ToString($"F{decimals}")})</color>{GetStatTypeUnit(statType)}");
      sb.Append("<sprite name=fm_down_arrow>");

    }
    else
    {
      sb.Append($"{viewStatValue.ToString($"F{decimals}")}{GetStatTypeUnit(statType)}");
    }

    return sb.ToString();

  }


  /// <summary>
  /// 이후 CurrencyType에 따라 계산 방식을 여기에 추가할 예정 if문으로
  /// </summary>
  /// <param name="currencyValue"></param>
  /// <returns></returns>
  public static string GetCurencyValue(long currencyValue)
  {
    if (currencyValue < 10000)
    {
      return currencyValue.ToString();
    }
    else
    {
      int thousands = (int)currencyValue / 1000;
      int remainder = (int)currencyValue % 1000;

      // 10,000 이상부터는 하위 3자리 중 100 미만이면 소수점 이하를 표시하지 않음
      if (remainder < 100)
      {
        return thousands.ToString() + "k";
      }
      else
      {
        // value/1000을 계산한 후, 소수점 첫째자리까지 내림 처리
        float result = Mathf.Floor((currencyValue / 1000f) * 10f) / 10f;
        return result.ToString("0.#") + "k";
      }
    }
  }


  /// <summary>
  /// 정수를 Roman 숫자로 변환
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static string ConvertIntToRoman(StringBuilder sb, int number)
  {
    if (number <= 0 || number > 3999)
      return string.Empty;

    sb.Clear();

    foreach (var romanValue in romanValueMap)
    {
      while (number >= romanValue.Key)
      {
        sb.Append(romanValue.Value);
        number -= romanValue.Key;
      }
    }

    return sb.ToString();
  }


  public static string FormatHHMMSS(int totalSeconds)
  {
    int hours = totalSeconds / 3600;
    int minutes = (totalSeconds % 3600) / 60;
    int seconds = totalSeconds % 60;

    if (hours > 0)
      return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    else
      return $"{minutes:D2}:{seconds:D2}";
  }
}
