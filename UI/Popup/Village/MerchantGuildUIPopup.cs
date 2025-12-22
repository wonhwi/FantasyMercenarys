using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantGuildUIPopup : UIBaseController
{
  [SerializeField] private MerchantGuildView view;
  
  private MerchantGuildModel model;
  private MerchantGuildPresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new MerchantGuildModel();
      presenter = new MerchantGuildPresenter(model, view, this);
    }

    presenter.SetData();
  }
}
