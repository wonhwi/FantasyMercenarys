using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = FantasyMercenarys.Debug;
//ToDo
//제너릭 형태로 EventManager 상속받게 한다음 여러가지로 나눈다
//Enum 타입 형태로 갈거고, APIEventManager, InGameEventManager 등등으로 접근 할 예정
public static class EventManager<TEnum> where TEnum : Enum
{
  private static readonly string logColor = "FF6200";
  private static Dictionary<TEnum, Delegate> eventDictionary = new Dictionary<TEnum, Delegate>();

#if UNITY_EDITOR
  private static bool enableDebugLog = true;
#else
  private static bool enableDebugLog = false;
#endif


  #region Event AddListener

  public static void AddListener(TEnum eventType, Action listener)
  {
    AddInternal(eventType, listener);
  }

  public static void AddListener<T>(TEnum eventType, Action<T> listener)
  {
    AddInternal(eventType, listener);
  }

  public static void AddListener<T1, T2>(TEnum eventType, Action<T1, T2> listener)
  {
    AddInternal(eventType, listener);
  }

  private static void AddInternal(TEnum eventType, Delegate listener)
  {
    if (!eventDictionary.ContainsKey(eventType))
    {
      eventDictionary[eventType] = null;
    }
    eventDictionary[eventType] = Delegate.Combine(eventDictionary[eventType], listener);
    if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Added Listener to {eventType}");
  }

  #endregion


  #region Event RemoveListener
  public static void RemoveListener(TEnum eventType, Action listener)
  {
    RemoveInternal(eventType, listener);
  }

  public static void RemoveListener<T>(TEnum eventType, Action<T> listener)
  {
    RemoveInternal(eventType, listener);
  }

  public static void RemoveListener<T1, T2>(TEnum eventType, Action<T1, T2> listener)
  {
    RemoveInternal(eventType, listener);
  }

  private static void RemoveInternal(TEnum eventType, Delegate listener)
  {
    if (eventDictionary.ContainsKey(eventType))
    {
      eventDictionary[eventType] = Delegate.Remove(eventDictionary[eventType], listener);

      if (eventDictionary[eventType] == null)
      {
        eventDictionary.Remove(eventType);
        if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] All listeners removed for {eventType} (cleaned up)");
      }
      else
      {
        if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Removed Listener from {eventType}");
      }
    }
  }
  #endregion

  #region Event Trigger 실행
  public static void TriggerEvent(TEnum eventType)
  {
    if (eventDictionary.TryGetValue(eventType, out var del))
    {
      (del as Action)?.Invoke();
      if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Invoked {eventType} (no params)");
    }
  }

  public static void TriggerEvent<T>(TEnum eventType, T arg)
  {
    if (eventDictionary.TryGetValue(eventType, out var del))
    {
      (del as Action<T>)?.Invoke(arg);
      if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Invoked {eventType} with argument {arg}");
    }
  }

  public static void TriggerEvent<T1, T2>(TEnum eventType, T1 arg1, T2 arg2)
  {
    if (eventDictionary.TryGetValue(eventType, out var del))
    {
      (del as Action<T1, T2>)?.Invoke(arg1, arg2);
      if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Invoked {eventType} with arguments {arg1}, {arg2}");
    }
  }

  #endregion
  // 모든 이벤트 제거
  public static void ClearEvents()
  {
    eventDictionary.Clear();

    if (enableDebugLog) Debug.LogColor(logColor, $"[EventManager<{typeof(TEnum).Name}>] Cleared all listeners.");
  }

  public static void DebugPrint()
  {
    Debug.LogColor(logColor, $"==== EventManager<{typeof(TEnum).Name}> Registered Events ====");
    foreach (var pair in eventDictionary)
    {
      string delegateInfo = pair.Value != null ? pair.Value.GetType().Name : "None";
      Debug.LogColor(logColor, $"EventType: {pair.Key}, DelegateType: {delegateInfo}");
    }
  }

}