using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum APIEventType
{
  /// <summary>
  /// 직업 교체시 실행
  /// 1. UISkillController에서 액티브 스킬 자체 쿨타임 돌리는 용도
  /// 2. PlayerSkillController에서 직업 스킬 Skill Update하는 용도
  /// 3. Player의 StatData를 Update하는 용도
  /// 4. PlayerSkillController에서의 버프 효과(액티브, 패시브) Update 용도
  /// </summary>
  UpdateJob,      //int
  CostumeChange,

  PlayerLevelUp,
}

/// <summary>
/// EventManager Wrapper Class
/// </summary>
public class APIEventManager
{
  public static void AddListener(APIEventType eventType, Action listener)
    => EventManager<APIEventType>.AddListener(eventType, listener);

  public static void AddListener<T>(APIEventType eventType, Action<T> listener)
      => EventManager<APIEventType>.AddListener(eventType, listener);

  public static void AddListener<T1, T2>(APIEventType eventType, Action<T1, T2> listener)
      => EventManager<APIEventType>.AddListener(eventType, listener);

  // RemoveListener 오버로드
  public static void RemoveListener(APIEventType eventType, Action listener)
      => EventManager<APIEventType>.RemoveListener(eventType, listener);

  public static void RemoveListener<T>(APIEventType eventType, Action<T> listener)
      => EventManager<APIEventType>.RemoveListener(eventType, listener);

  public static void RemoveListener<T1, T2>(APIEventType eventType, Action<T1, T2> listener)
      => EventManager<APIEventType>.RemoveListener(eventType, listener);

  // Invoke 오버로드
  public static void TriggerEvent(APIEventType eventType)
      => EventManager<APIEventType>.TriggerEvent(eventType);

  public static void TriggerEvent<T>(APIEventType eventType, T arg)
      => EventManager<APIEventType>.TriggerEvent(eventType, arg);

  public static void TriggerEvent<T1, T2>(APIEventType eventType, T1 arg1, T2 arg2)
      => EventManager<APIEventType>.TriggerEvent(eventType, arg1, arg2);

  // 유틸리티
  public static void Clear()
      => EventManager<APIEventType>.ClearEvents();

  public static void DebugPrint()
      => EventManager<APIEventType>.DebugPrint();
}
