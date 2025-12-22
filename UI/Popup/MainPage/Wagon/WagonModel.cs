using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ButtonState = WagonView.ButtonState;

public class WagonModel
{
  private CartTable cartTable => CartTable.getInstance;

  private ContentCart contentModel => GameDataManager.getInstance.userContentsData.contentCart;

  #region Table Data
  /// <summary>
  /// 레벨 기반 CartData 반환
  /// </summary>
  /// <param name="cartLv"></param>
  /// <returns></returns>
  public CartData GetCartData(int cartLv)
  {
    return cartTable.GetCartData(cartLv);
  }

  /// <summary>
  /// CartIdx 기반 CartData 반환
  /// </summary>
  /// <param name="cartIdx"></param>
  /// <returns></returns>
  public CartData GetCartDataIndex(int cartIdx)
  {
    return cartTable.GetCartDataIndex(cartIdx);
  }

  /// <summary>
  /// Index 기반 해당 CartData 반환 (코스튬 정보)
  /// </summary>
  /// <returns></returns>
  public CartData GetCostumeData(int index)
  {
    return cartTable.GetCostumeData(index);
  }

  public WagonType GetWagonType(CartData cartData)
  {
    return cartTable.GetWagonType(cartData);
  }

  public int GetCostumeCount()
  {
    return cartTable.cartCostumeList.Count;
  }

  /// <summary>
  /// Spine기반 현재 Index 구함
  /// </summary>
  /// <param name="cartSpine"></param>
  /// <returns></returns>
  public int GetCostumeIdx(string cartSpine)
  {
    return cartTable.GetCostumeIdx(cartSpine);
  }

  /// <summary>
  /// Spine기반 해당 레벨 범위 출력
  /// </summary>
  /// <param name="cartSpine"></param>
  /// <returns></returns>
  public string GetLevelRange(string cartSpine)
  {
    return cartTable.GetLevelRangeText(cartSpine);
  }

  public int GetCartMaxLv()
  {
    return cartTable.cartMaxLv;
  }

  public bool IsCartMaxLv()
  {
    int cartLv = GetCartLv();
    int cartMaxLv = GetCartMaxLv();

    return cartLv == cartMaxLv;
  }


  #endregion

  #region User Data

  //사용 가능 한 경우
  //현재 내가 사용중인 경우
  //내가 해당 레벨을 달성하지 못했을경우
  public ButtonState GetButtonState(int selectIdx)
  {
    ButtonState buttonState = ButtonState.Available;

    int cartLv = GetCartLv();

    CartData targetCarData = GetCostumeData(selectIdx);
    CartData currentCarData = GetCurrentCartData();

    var levelRange = cartTable.GetLevelRange(targetCarData.cartSpine);

    //동일한 스킨일 경우 사용중
    if (targetCarData.cartSpine == currentCarData.cartSpine)
    {
      buttonState = ButtonState.Using;
    }
    else if(levelRange.minLv > cartLv)
    {
      buttonState = ButtonState.NotAvailable;
    }

    return buttonState;
  }

  /// <summary>
  /// 현재 내 카트 정보 반환
  /// </summary>
  /// <returns></returns>
  public CartData GetCurrentCartData()
  {
    int cartLv = GetCartLv();

    CartData cartData = GetCartData(cartLv);

    bool isCostume = IsCostumeState();

    if (isCostume)
    {
      int costumeIdx = GetCartCostumeIdx();

      cartData = GetCartDataIndex(costumeIdx);
    }

    return cartData;
  }

  public bool IsEnoughUpgradeItem()
  {
    return GameDataManager.getInstance.GetConsumeValue(ConstantManager.ITEM_CURRENCY_CART_ENHANCE) > 0;
  }

  public int GetCartLv()
  {
    return contentModel.GetCartLv();
  }

  public int GetCartExp()
  {
    return contentModel.GetCartExp();
  }

  public int GetCartCostumeIdx()
  {
    return contentModel.GetCartCostumeIdx();
  }

  /// <summary>
  /// 수레 코스튬 상태
  /// </summary>
  /// <returns></returns>
  public bool IsCostumeState()
  {
    return GetCartCostumeIdx() != 0;
  }

  #endregion
}
