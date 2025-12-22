using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WagonUIPopup : UIBaseController
{
  [Header("MVP")]
  [SerializeField] private WagonView view;

  private WagonModel model;
  private WagonPresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new WagonModel();
      presenter = new WagonPresenter(model, view, this);
    }

    presenter.SetData();
  }

  

}
