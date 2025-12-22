using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIButtonAnimation : UIButton
{
#if UNITY_EDITOR
  protected override void Reset()
  {
    base.Reset();

    targetTransform = GetComponent<Transform>();
  }
#endif

  [Header("필수 요소")]
  [SerializeField] protected Transform targetTransform; // 애니메이션 적용 대상 (예: Button.transform or Text)

  [Header("설정값")]
  [SerializeField] protected float pressedScale = 0.8f;
  [SerializeField] protected float animDuration = 0.1f;

}
