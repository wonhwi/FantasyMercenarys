using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 뽑기 관련 버튼들을 모듈화 시켜놓은 스크립트
/// </summary>
public class BundleGachaButton : MonoBehaviour
{
  public enum ShopSaleType
  {
    Item = 1,   //1.특정 아이템  (티켓, 이벤트 재화, 정수 등등 )
    Gold = 2,   //2.골드?
    Dia = 3,    //3.다이아(무료, 유료 다이아)
    AD = 4,     //4.광고
    Cash = 5,   //5.현금
  }

  //이거 클래스도 따로 빼버리자
  [System.Serializable]
  public struct ShopGachaButtonUI
  {
    public Button gachaButton;
    public Image ticketImage;
    public TextMeshProUGUI buttonText;  //버튼 이름
    public TextMeshProUGUI consumeText; //소모 갯수
  }

  [SerializeField] private ShopGachaButtonUI[] gachaButtonUIArray;
  [SerializeField] private Button closeButton;

  private void SetDefaultButton()
  {
    for (int i = 0; i < gachaButtonUIArray.Length; i++)
      gachaButtonUIArray[i].gachaButton.gameObject.SetActive(false);
  }
  
  public void SetData(List<ShopItemData> shopItemDataList, Action<int> OnAction, Action OnClose = null)
  {
    SetDefaultButton();

    for (int i = 0; i < shopItemDataList.Count; i++)
    {
      int index = i;

      ShopItemData shopItemData = shopItemDataList[index];

      ShopGachaButtonUI gachaButtonUI = gachaButtonUIArray[index];

      SetButtonData(shopItemData, index);

      gachaButtonUI.gachaButton.onClick.RemoveAllListeners();
      gachaButtonUI.gachaButton.onClick.AddListener(() => OnAction?.Invoke(shopItemData.shopItemIdx));

      gachaButtonUI.gachaButton.gameObject.SetActive(true);
    }


    SetCloseButton(OnClose);
  }

  public void SetClose(Action OnClose)
  {
    SetDefaultButton();

    SetCloseButton(OnClose);
  }


  private void SetCloseButton(Action OnClose)
  {
    closeButton.gameObject.SetActive(OnClose != null);

    if (OnClose != null)
    {
      closeButton.onClick.RemoveAllListeners();
      closeButton.onClick.AddListener(() => OnClose?.Invoke());
    }
  }

  private void SetButtonData(ShopItemData shopItemData, int index)
  {
    ShopSaleType shopSaleType = (ShopSaleType)shopItemData.salesType;

    ShopGachaButtonUI gachaButtonUI = gachaButtonUIArray[index];

    gachaButtonUI.ticketImage.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, shopItemData.costIconImage);
    gachaButtonUI.buttonText.text = shopItemData.itemDesc;

    bool isLimitProduct = shopSaleType == ShopSaleType.AD || shopItemData.purchaseLimit > 0;

    if (isLimitProduct)
    {
      gachaButtonUI.consumeText.text = $"{shopItemData.purchaseLimit}/{shopItemData.purchaseLimit}";
    }
    else
    {
      gachaButtonUI.consumeText.text = shopItemData.costPay.ToString();
    }
      
  }
    
}

