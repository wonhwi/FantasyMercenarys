using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeUIPopup : UIBaseController
{
  [SerializeField] private ChallengeView view;

  private ChallengeModel model;
  private ChallengePresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new ChallengeModel();
      presenter = new ChallengePresenter(model, view, this);
    }

    presenter.SetData();
  }
}
