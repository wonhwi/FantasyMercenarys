using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentCart
{
  public CartInfoData cartInfoData { get; private set; } = new CartInfoData();

  public void LoadCartData(CartInfoData cartInfoData)
  {
    UpdateCartData(cartInfoData);
  }

  public void UpdateCartData(CartInfoData cartInfoData)
  {
    this.cartInfoData = cartInfoData;

    CartData cartData = CartTable.getInstance.GetCartData(this.cartInfoData.cartLv);

    PlayerStatManager.getInstance.UpdateCartStats(cartData);
  }

  public int GetCartLv()
  {
    return this.cartInfoData.cartLv;
  }

  public int GetCartExp()
  {
    return this.cartInfoData.cartExp;
  }

  public int GetCartCostumeIdx()
  {
    return this.cartInfoData.selectedCostumeIdx;
  }

}
