using System;


public enum InGameEventType
{
  GameTimeChange,
  UpdatePlayerStat,
  //GameStart,
  //PlayerDeath,
}

/// <summary>
/// EventManager Wrapper Class
/// </summary>
public static class InGameEventManager
{
  public static void AddListener(InGameEventType eventType, Action listener)
      => EventManager<InGameEventType>.AddListener(eventType, listener);

  public static void AddListener<T>(InGameEventType eventType, Action<T> listener)
      => EventManager<InGameEventType>.AddListener(eventType, listener);

  public static void AddListener<T1, T2>(InGameEventType eventType, Action<T1, T2> listener)
      => EventManager<InGameEventType>.AddListener(eventType, listener);

  // RemoveListener 오버로드
  public static void RemoveListener(InGameEventType eventType, Action listener)
      => EventManager<InGameEventType>.RemoveListener(eventType, listener);

  public static void RemoveListener<T>(InGameEventType eventType, Action<T> listener)
      => EventManager<InGameEventType>.RemoveListener(eventType, listener);

  public static void RemoveListener<T1, T2>(InGameEventType eventType, Action<T1, T2> listener)
      => EventManager<InGameEventType>.RemoveListener(eventType, listener);

  // Invoke 오버로드
  public static void TriggerEvent(InGameEventType eventType)
      => EventManager<InGameEventType>.TriggerEvent(eventType);

  public static void TriggerEvent<T>(InGameEventType eventType, T arg)
      => EventManager<InGameEventType>.TriggerEvent(eventType, arg);

  public static void TriggerEvent<T1, T2>(InGameEventType eventType, T1 arg1, T2 arg2)
      => EventManager<InGameEventType>.TriggerEvent(eventType, arg1, arg2);

  // 유틸리티
  public static void Clear()
      => EventManager<InGameEventType>.ClearEvents();

  public static void DebugPrint()
      => EventManager<InGameEventType>.DebugPrint();
}
