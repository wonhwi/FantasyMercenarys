using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SingleSkeletonAnimation : SpineAnimationBase
{
  [SerializeField] protected SkeletonAnimation skeletonAnimation;

  public override int AnimationCount => 1;

  public override Spine.AnimationState GetAnimationState()
  {
    return skeletonAnimation.AnimationState;
  }

#if UNITY_EDITOR
  private void Reset()
  {
    skeletonAnimation = GetComponent<SkeletonAnimation>();
  }
#endif
}
