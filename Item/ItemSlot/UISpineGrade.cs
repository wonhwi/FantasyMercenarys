using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpineGrade : UISkeletonAnimation, IPoolableUI
{
  public override int AnimationCount => 1;

  public override Spine.AnimationState GetAnimationState()
  {
    return skeletonGraphic.AnimationState;
  }

  public void OnActivate()
  {
    this.gameObject.SetActive(true);
  }

  public void OnDeactivate()
  {
    this.gameObject.SetActive(false);
  }

  public void OnSetParent(Transform parent)
  {
    this.transform.SetParent(parent, false);

    (this.transform as RectTransform).anchoredPosition = Vector2.zero;
  }
}
