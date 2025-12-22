using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : LazySingleton<CurrencyManager>
{
  // 재화별 콜백 관리
  private Dictionary<int, Action<long>> currencyChanged = new Dictionary<int, Action<long>>();
  
  /// <summary>
  /// GameDataMager에서 재화 업데이트시 아래 함수 호출
  /// </summary>
  /// <param name="itemIdx"></param>
  /// <param name="value"></param>
  public void SetCurrencyData(int itemIdx, long value)
  {
    if (currencyChanged.TryGetValue(itemIdx, out var action))
    {
      action?.Invoke(value);
    }
  }

  /// <summary>
  /// 재화 값 반환 (GameDataManager에서 우선 조회, 없으면 내부 캐시)
  /// </summary>
  public long GetCurrencyAmount(int itemIdx)
  {
    return GameDataManager.getInstance.GetConsumeValue(itemIdx);
  }

  /// <summary>
  /// 특정 재화 변화 구독
  /// </summary>
  public void Subscribe(int itemIdx, Action<long> callback)
  {
    if (currencyChanged.ContainsKey(itemIdx))
      currencyChanged[itemIdx] += callback;
    else
      currencyChanged[itemIdx] = callback;
  }

  /// <summary>
  /// 특정 재화 변화 구독 해제
  /// </summary>
  public void Unsubscribe(int itemIdx, Action<long> callback)
  {
    if (currencyChanged.ContainsKey(itemIdx))
      currencyChanged[itemIdx] -= callback;
  }
}