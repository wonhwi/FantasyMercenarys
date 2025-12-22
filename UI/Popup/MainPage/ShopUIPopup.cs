using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIPopup : UIBaseController
{
  [Header("MVP")]
  [SerializeField] private ShopUIView view;

  private ShopUIModel model;
  private ShopUIPresenter presenter;

  public void Open(ShopType shopType)
  {
    if (presenter == null)
    {
      model = new ShopUIModel();
      presenter = new ShopUIPresenter(model, view, this);
    }

    presenter.SetData(shopType);
  }
}
