using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RestPacket;

public class AdventureGuildUIPopup : UIBaseController
{
  [SerializeField] private AdventureGuildView adventureGuildView;
  private AdventureGuildPresenter presenter;

  public void Open()
  {
    if (presenter == null)
      presenter = new AdventureGuildPresenter(this, adventureGuildView);

    presenter.SetData();
  }
}
