using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스탯 데이터
/// </summary>
public struct StatData
{
  public int statType;
  public float statValue;

  public StatData(int _type, float _value)
  {
    statType = _type;
    statValue = _value;
  }
}
