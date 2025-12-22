using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
public class BreedingGroundUIPopup : UIBaseInventory<BreedingUISlot>
{
  [Header("MVP")]
  [SerializeField] private BreedingGroundView view;

  private BreedingGroundModel model;
  private BreedingGroundPresenter presenter;

  public void Open()
  {
    if (presenter == null)
    {
      model = new BreedingGroundModel();
      presenter = new BreedingGroundPresenter(model, view, this);
    }

    presenter.SetData();
  }

  public void OnLoadInven()
  {
    LoadInven(ItemGroup.Creature);
  }

  public void OnCloseInven()
  {
    CloseInven();
  }

  public int GetSelectItemIdx()
  {
    return GetSelectSlot().GetItemIndex();
  }

  protected override void OnClickItemSlot(BreedingUISlot itemSlot)
  {
    base.OnClickItemSlot(itemSlot);

    view.OnViewInven?.Invoke(itemSlot.GetItemIndex());
  }

  
}
