using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonPressDownScale : UIButtonAnimation
{
  protected override void Awake()
  {
    base.Awake();

    OnPointerDownAction += () => ApplyScale(pressedScale);
    OnPointerUpAction   += () => ApplyScale(1f);
  }

  private void ApplyScale(float targetScale)
  {
    targetTransform.DOKill();
    targetTransform.DOScale(targetScale, animDuration).SetEase(Ease.OutQuad);
  }
}
