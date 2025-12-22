using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailboxUIPopup : UIBaseController
{
  [Header("MVP")]
  [SerializeField] private MailboxView view;

  private MailboxModel model;
  private MailboxPresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new MailboxModel();
      presenter = new MailboxPresenter(model, view, this);
    }

    presenter.OnMailLoad();
  }

}
