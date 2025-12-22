using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonClickScale : UIButtonAnimation
{
  protected override void Awake()
  {
    base.Awake();

    OnClickAction += OnClickButton;
  }

  public void OnClickButton()
  {
    // 1. 트윈 초기화
    DOTween.Kill(targetTransform);
    targetTransform.localScale = Vector3.one;

    // 2. Scale 애니메이션
    targetTransform.DOScale(pressedScale, animDuration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
          targetTransform.DOScale(1f, animDuration).SetEase(Ease.OutBack);
        });
  }
}
