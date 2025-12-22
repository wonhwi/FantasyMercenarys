using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;
using ButtonState = WagonView.ButtonState;

public class WagonPresenter 
{
  private WagonModel model;
  private WagonView view;
  private WagonUIPopup popUp;

  private int selectIdx = 0;

  public WagonPresenter(WagonModel model, WagonView view, WagonUIPopup popUp)
  {
    this.model = model;
    this.view = view;
    this.popUp = popUp;

    InitData();
  }

  private void InitData()
  {
    
    view.OnClosePopup = () => { 
      popUp.Hide();
      view.DisableCurrency();
    };

    view.OnSelectSkin = OnChangeCostume;
    view.OnUpgrade = OnUpgrade;

    view.OnBeforeSkin = SetBeforeCart;
    view.OnNextSkin = SetNextCart;

  }

  public void SetData()
  {
    view.EnableCurrency();

    view.SetActiveSkeleton(true);

    SetDefaultCart();

    SetCartStatData();
    SetGaugeText();
  }

  /// <summary>
  /// 현재 수레 정보 기반 수레 정보 설정
  /// </summary>
  private void SetDefaultCart()
  {
    CartData cartData = model.GetCurrentCartData();

    this.selectIdx = model.GetCostumeIdx(cartData.cartSpine);

    SetCartDetail();
    SetCartButton();
  }


  /// <summary>
  /// 이전 수레 정보 기반 수레 정보 설정
  /// </summary>
  private void SetBeforeCart()
  {
    if(this.selectIdx > 0)
    {
      this.selectIdx--;

      SetCartDetail();
      SetCartButton();
    }
  }

  /// <summary>
  /// 다음 수레 정보 기반 수레 정보 설정
  /// </summary>
  private void SetNextCart()
  {
    if(this.selectIdx < model.GetCostumeCount() - 1)
    {
      this.selectIdx++;

      SetCartDetail();
      SetCartButton();
    }
  }

  private void SetCartDetail()
  {
    CartData cartData = model.GetCostumeData(this.selectIdx);

    string lvRange = model.GetLevelRange(cartData.cartSpine);

    WagonType wagonType = model.GetWagonType(cartData);

    view.SetCartActive(wagonType);

    view.SetCartName(cartData.cartRecordCd);
    view.SetCartLvRange(lvRange);
  }

  private void SetCartButton()
  {
    ButtonState buttonState = model.GetButtonState(this.selectIdx);

    view.SetBundleButton(buttonState);
  }
  
  /// <summary>
  /// 수레 우측 상세 정보 설정
  /// </summary>
  private void SetCartStatData()
  {
    int cartLv = model.GetCartLv();

    bool isMaxLv = model.IsCartMaxLv();

    CartData cartData = model.GetCartData(cartLv);
    CartData nextCartData = model.GetCartData(isMaxLv ? cartLv : cartLv + 1);

    view.SetCartLv(cartData.cartLevel, nextCartData.cartLevel);
    view.SetCartReward(cartData.stageBonusReward, nextCartData.stageBonusReward);
    view.SetCartHeath(cartData.bonusHp, nextCartData.bonusHp);
    view.SetCartAttack(cartData.bonusAtk, nextCartData.bonusAtk);
    view.SetCartDefense(cartData.bonusDef, nextCartData.bonusDef);
  }

  private void SetGaugeText()
  {
    int cartLv = model.GetCartLv();
    int cartExp = model.GetCartExp();
    bool isMaxLv = model.IsCartMaxLv();
    
    CartData cartData = model.GetCartData(cartLv);

    view.SetBundleGaugeText(isMaxLv, cartExp, cartData.requiredExp);
  }

  #region API
  private async void OnChangeCostume()
  {
    Debug.Log("OnChangeCostume");

    CartData cartData = model.GetCostumeData(this.selectIdx);

    await APIManager.getInstance.REQ_CartChangeCostume<RES_CartChangeCostume>(cartData.cartIdx, (responseResult) =>
    {
      SetCartButton();

      int cartIdx = responseResult.userCartInfo.selectedCostumeIdx;

      CartData cartData = model.GetCartDataIndex(cartIdx);

      WagonType wagonType = model.GetWagonType(cartData);

      string animationName = NewInGameManager.getInstance.currentWagon.spineAnimation.GetAnimation();

      NewInGameManager.getInstance.SetWagonController(wagonType);

      if(animationName == "idle")
        NewInGameManager.getInstance.currentWagon.SetAnimation(WagonAnimation.idle);
      else if (animationName == "move")
        NewInGameManager.getInstance.currentWagon.SetAnimation(WagonAnimation.move);
      else 
        NewInGameManager.getInstance.currentWagon.SetAnimation(WagonAnimation.death, false);

    });
  }

  private async void OnUpgrade()
  {
    Debug.Log("OnUpgrade");

    int cartLv = model.GetCartLv();
    bool isMaxLv = model.IsCartMaxLv();
    bool isEnough = model.IsEnoughUpgradeItem();

    if (isMaxLv)
    {
      UIUtility.ShowToastMessagePopup("현재 최대 레벨입니다.");
      return;
    }

    if(!isEnough)
    {
      UIUtility.ShowToastMessagePopup("강화 재료가 부족합니다.");
      return;
    }

    await APIManager.getInstance.REQ_CartUpgrade<RES_CartUpgrade>(cartLv + 1, (responseResult) =>
    {
      SetCartStatData();
      SetGaugeText();

      //여기에 스텟 업데이트 해주는 로직 추가

    });
    
  }
  #endregion
}
