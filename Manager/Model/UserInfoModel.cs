using FantasyMercenarys.Data;

public class UserInfoModel
{
  public UserInfoData userInfoData;

  public int GetPlayerLv() 
    => userInfoData.accLv;
  public long GetPlayerExp() 
    => userInfoData.accExp;
  public string GetPlayerName() 
    => userInfoData.userNick;

  public void PlayerLevelUp()
    => userInfoData.accLv++;

  public long GetGold()
    => userInfoData.gold;

  public long GetGem()
    => userInfoData.freeDia;

  public int GetJobCode()
    => userInfoData.jobCode;

  public long GetBlessingPoint()
    => userInfoData.blessingPoint;

  public void AddGold(long itemCount)
  {
    userInfoData.gold += itemCount;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_GOLD, userInfoData.gold);
  }

  public void SetGold(long itemCount)
  {
    userInfoData.gold = itemCount;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_GOLD, userInfoData.gold);
  }

  public void AddGem(long itemCount)
  {
    userInfoData.freeDia += itemCount;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_GEM, userInfoData.freeDia);
  }

  public void SetGem(long itemCount)
  {
    userInfoData.freeDia = itemCount;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_GEM, userInfoData.freeDia);
  }

  public void AddBlessingPoint(long blessingPoint)
  {
    userInfoData.blessingPoint += blessingPoint;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_FAITH, userInfoData.blessingPoint);

  }

  public void SetBlessingPoint(long blessingPoint)
  {
    userInfoData.blessingPoint = blessingPoint;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_FAITH, userInfoData.blessingPoint);

  }

  public void SetJobCode(int jobCode)
  {
    userInfoData.jobCode = jobCode;
  }

  public bool IsCommoner()
  {
    return userInfoData.jobCode == ConstantManager.JOB_DEFAULT_INDEX;
  }
}
