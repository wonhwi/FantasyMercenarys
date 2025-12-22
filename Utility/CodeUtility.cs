using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

public static class CodeUtility
{
  public static int GetTotalSeconds(long targetUnixTime)
  {
    DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(targetUnixTime).UtcDateTime;

    TimeSpan timeDifference = dateTime - DateTime.UtcNow;

    return (int)timeDifference.TotalSeconds;
  }

  public static int GetDateDiffTotalSeconds(DateTime targetDateTime)
  {
    TimeSpan timeDifference = targetDateTime - DateTime.UtcNow.AddHours(9);

    return (int)timeDifference.TotalSeconds;
  }

}

namespace FantasyMercenarys.Extenstions
{
  public static class ExtensionsUtility
  {
    /// <summary>
    /// string to enum (string 값이 잘못되었을때는 0번째에 해당하는 enum을 반납)
    /// </summary>
    public static T ToEnum<T>(this string enumString)
    {
      if (Enum.IsDefined(typeof(T), enumString))
        return (T)Enum.Parse(typeof(T), enumString);
      else
        return Enum.GetNames(typeof(T))[0].ToEnum<T>();
    }

    public static int GetEnumCount<T>(this T _) where T : Enum
    {
      return Enum.GetValues(typeof(T)).Length;
    }


    #region Enum Flags
    // 플래그 전체 초기화(0, None)
    public static T ClearFlags<T>(this T enumValue) where T : struct, Enum
    {
      return (T)Enum.ToObject(typeof(T), 0);
    }

    // 플래그 추가
    public static T AddFlag<T>(this T enumValue, T flag) where T : struct, Enum
    {
      int result = ToInt(enumValue) | ToInt(flag);
      return (T)Enum.ToObject(typeof(T), result);
    }

    // 플래그 제거
    public static T RemoveFlag<T>(this T enumValue, T flag) where T : struct, Enum
    {
      int result = ToInt(enumValue) & ~ToInt(flag);
      return (T)Enum.ToObject(typeof(T), result);
    }

    // 플래그 포함 여부 (None도 지원)
    public static bool HasFlagEx<T>(this T enumValue, T flag) where T : struct, Enum
    {
      int value = ToInt(enumValue);
      int flagValue = ToInt(flag);

      if (flagValue == 0)
        return value == 0;

      return (value & flagValue) != 0;
    }

    // int 변환 (박싱 최소화)
    private static int ToInt<T>(T value) where T : struct, Enum
    {
      return (int)(object)value;
    }
    #endregion
  }
}


