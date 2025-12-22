using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤 데이터
/// </summary>
public class InvenData
{
  public long invenIdx;   //인벤 Index
  public int itemIdx;     //ItemIdx
  public int itemGroup;   //아이템 그룹
  public long itemCount;  //아이템 갯수
  public int itemLv;      //아이템 레벨
  public List<StatData> statDataList;
}
