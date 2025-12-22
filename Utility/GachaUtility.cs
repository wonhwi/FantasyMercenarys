using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GachaUtility
{
  public static bool HasEnoughGachaTickets(int itemIdx, int targetCount)
  {
    long ticketCount = GameDataManager.getInstance.GetConsumeValue(itemIdx);

    if (ticketCount >= targetCount)
      return true;

    return false;
  }
}
