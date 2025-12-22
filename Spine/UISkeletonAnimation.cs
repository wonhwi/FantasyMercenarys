using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkeletonAnimation : SpineAnimationBase
{
  [SerializeField] protected SkeletonGraphic skeletonGraphic;

  public override int AnimationCount => 1;

  public override Spine.AnimationState GetAnimationState()
  {
    return skeletonGraphic.AnimationState;
  }

}
