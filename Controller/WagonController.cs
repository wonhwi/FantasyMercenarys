using Core.Manager;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonController : UnitBase
{
  public SingleSkeletonAnimation spineAnimation;
  public void SetAnimation(WagonAnimation animationType, bool loop = true)
  {
    spineAnimation.SetAnimation(animationType.ToString(), loop : loop);
  }
}
