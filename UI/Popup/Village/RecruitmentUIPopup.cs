using FantasyMercenarys.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentUIPopup : UIBaseInventory<InvenDataSlot>
{
  [Header("[Base Inventory Component]")]
  [SerializeField] private Button invenCloseButton;
  [SerializeField] private Button invenApplyButton;

  [SerializeField] private GameObject noticeText;


  [Header("[MVP]")]
  [SerializeField] private RecruitmentView view;
  private RecruitmentPresenter presenter;
  private RecruitmentModel model;

  protected override void Awake()
  {
    base.Awake();

    view.OnCloseUIPopup += Hide;
    view.OnLoadInven = LoadInven;


    invenCloseButton.onClick.AddListener(CloseInven);
    invenApplyButton.onClick.AddListener(OnApplyItem);
  }

  public void Open()
  {
    if (presenter == null)
    {
      model = new RecruitmentModel();
      presenter = new RecruitmentPresenter(model, view);
    }

    presenter.SetData();
  }


  private void OnApplyItem()
  {
    view.OnApplyItem(GetSelectSlot().GetInvenData());
    CloseInven();
  }


  public void LoadInven(Predicate<PartnerData> predicate)
  {
    base.partnerFilter = predicate;

    LoadInven(ItemGroup.Partner);
  }

  protected override void LoadInven(ItemGroup itemGroup)
  {
    base.LoadInven(itemGroup);

    bool isValid = IsValidItem();

    noticeText.SetActive(!isValid);
    invenApplyButton.interactable = isValid;

  }
}
