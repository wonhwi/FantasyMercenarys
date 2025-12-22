using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCenterUIPopup : UIBaseController
{
  [SerializeField] private HopeCenterView view;

  private HopeCenterModel model;
  private HopeCenterPresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new HopeCenterModel();
      presenter = new HopeCenterPresenter(model, view, this);
    }

    presenter.SetData();
  }
}
