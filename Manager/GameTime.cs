using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime
{
  private static float _timeScale = 1f;
  public static float TimeScale 
  {
    get
    {
      return _timeScale;
    }
    private set
    {
      _timeScale = value;

      InGameEventManager.TriggerEvent(InGameEventType.GameTimeChange, _timeScale);
    }
  }

  public static float deltaTime => Time.deltaTime * TimeScale;

  public static float unscaledDeltaTime => Time.unscaledDeltaTime;

  public static float FixedDeltaTime => Time.fixedDeltaTime * TimeScale;

  public static void SetTimeScale(float timeScale)
  {
    TimeScale = timeScale;
  }
}
