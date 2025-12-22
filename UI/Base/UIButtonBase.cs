using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIButtonBase : Button
{
  [Header("Interaction Control")]
  [SerializeField] private bool disableOnClick = false;
  [SerializeField] private float clickCooldown = 0f;

  protected Action OnClickAction;
  protected Action OnPointerDownAction;
  protected Action OnPointerUpAction;
  protected Action OnPointerEnterAction;
  protected Action OnPointerExitAction;

  private bool isCooldown = false;

  public sealed override void OnPointerClick(PointerEventData eventData)
  {
    base.OnPointerClick(eventData);

    if (isCooldown) return;

    if (clickCooldown > 0f)
      StartCoroutine(ClickCooldownCoroutine());

    OnClickAction?.Invoke();
  }

  private IEnumerator ClickCooldownCoroutine()
  {
    if (disableOnClick)
      interactable = false;

    isCooldown = true;
    yield return new WaitForSeconds(clickCooldown);
    isCooldown = false;
    interactable = true;
  }

  public override void OnPointerDown(PointerEventData eventData)
  {
    base.OnPointerDown(eventData);

    OnPointerDownAction?.Invoke();
  }

  public override void OnPointerUp(PointerEventData eventData)
  {
    base.OnPointerUp(eventData);

    OnPointerUpAction?.Invoke();
  }

  public override void OnPointerEnter(PointerEventData eventData)
  {
    base.OnPointerEnter(eventData);

    OnPointerEnterAction?.Invoke();
  }

  public override void OnPointerExit(PointerEventData eventData)
  {
    base.OnPointerExit(eventData);

    OnPointerExitAction?.Invoke();
  }

  public void SimulateClick()
  {
    onClick?.Invoke();

    OnClickAction?.Invoke();
  }
}
