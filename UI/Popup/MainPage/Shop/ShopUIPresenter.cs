using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;
using static ShopUIView;
public class ShopUIPresenter
{
  private ShopUIModel model;
  private ShopUIView view;
  private ShopUIPopup popUp;

  public ShopUIPresenter(ShopUIModel model, ShopUIView view, ShopUIPopup popUp)
  {
    this.model = model;
    this.view = view;
    this.popUp = popUp;

    InitEvent();
  }

  private void InitEvent()
  {
    view.OnClosePopup = popUp.Hide;
  }


  /// <summary>
  /// 해당 ShopType Value 값 기반 상점 활성화
  /// </summary>
  /// <param name="shopType"></param>
  public void SetData(ShopType shopType)
  {
    SetCategory(shopType);
  }

  /// <summary>
  /// ShopType 데이터 정보 기반 좌측 카테고리 활성화
  /// </summary>
  private void SetCategory(ShopType shopType)
  {
    var categorySlots = view.slotCategorySlots;
    List<ShopCategoryData> shopCategoryDataList = model.GetShopCategoryDataList((int)shopType);

    for (int i = 0; i < categorySlots.Length; i++)
      categorySlots[i].gameObject.SetActive(false);

    for (int i = 0; i < shopCategoryDataList.Count; i++)
    {
      int slotIdx = i;

      ShopCategoryData shopCategoryData = shopCategoryDataList[i];
      
      var slot = categorySlots[slotIdx];

      int categoryIdx = shopCategoryData.categoryIdx;
      string shopDesc  = shopCategoryData.shopDesc;

      bool useShopLvInfo = shopCategoryData.isLv == 1;

      slot.OnClickSlot = () => {
        
        view.SetSelectEffect(slotIdx);

        view.ActiveContentInfo(useShopLvInfo);

        OnClickCategorySlot(categoryIdx);
      };

      slot.SetText(shopDesc);
      slot.gameObject.SetActive(true);

      if(slotIdx == 0)
        categorySlots[slotIdx].OnClickSlot?.Invoke();
    }
  }

  private void OnClickCategorySlot(int categoryIdx)
  {
    ShopCategoryType shopCategoryType = (ShopCategoryType)categoryIdx;

    ShopInfoData shopInfoData = model.GetShopInfoData(shopCategoryType);

    List<ShopItemData> shopItemDataList = model.GetShopCategoryItemDataList(categoryIdx);

    int consumeItemIdx = model.GetConsumeItemIdx(shopCategoryType);

    view.OnActiveInfo = () => view.SetBundleGachaInfo(shopInfoData);
    
    view.UpdateCurrency(consumeItemIdx);
    view.OnStartIdle();

    view.SetBundleShopInfo(shopInfoData);
    view.SetBundleGachaButton(shopItemDataList, OnGachaShop);
  }

  #region API
  private async void OnGachaShop(int shopItemIdx)
  {
    Debug.Log("OnGachaShop" + shopItemIdx);

    ShopItemData shopItemData = model.GetShopItemData(shopItemIdx);

    bool isEnough = model.IsEnoughItem(shopItemData.costItemIdx, shopItemData.costPay);

    if(!isEnough)
    {
      UIUtility.ShowToastMessagePopup("상점 뽑기 재료가 부족합니다.");
      return;
    }

    await APIManager.getInstance.REQ_BuyShopItem<RES_BuyShopItem>(shopItemIdx, (responseResult) =>
    {
      view.OnGachaComplete = async () =>
      {
        var gachaPopup = await NewUIManager.getInstance.Show<ItemGachaPopup>(NewResourcePath.PREFAB_UI_POPUP_GACHA_ITEM_REWARD);

        gachaPopup.SetRewardData(responseResult.rewardList);
      };

      view.OnStartGacha();

      view.SetBundleShopInfo(responseResult.updateshopInfo);
    });
  }
  #endregion
}
