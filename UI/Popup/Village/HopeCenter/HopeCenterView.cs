using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HopeCenterView : MonoBehaviour, ICurrencyUsable
{
  [SerializeField] private BundleCurrency bundleCurrency;
  [SerializeField] private GameObject uiSpineGroup;
  [SerializeField] private UISpineController uiSpineController;

  [SerializeField] private HopeCenterUISlot[] hopeUISlots;

  [SerializeField] private Button closeButton;

  public HopeCenterUISlot[] HopeUISlots=> this.hopeUISlots;

  public Action OnClosePopup;

  public Action OnScratchComplete;

  private void Awake()
  {
    closeButton.onClick.AddListener(() => OnClosePopup?.Invoke());

    uiSpineController.AddListenerAnimationComplete(OnAnimationComplete);
  }

  public void OnAnimationComplete(string animationName)
  {
    uiSpineGroup.SetActive(false);
    OnScratchComplete?.Invoke();
  }

  public void SetAnimation()
  {
    uiSpineGroup.SetActive(true);
    uiSpineController.SetAnimation("Lottery_scratch", loop:false);
  }

  public void EnableCurrency()
  {
    bundleCurrency.Enable();

  }
  public void DisableCurrency()
  {
    bundleCurrency.Disable();
  }
}
