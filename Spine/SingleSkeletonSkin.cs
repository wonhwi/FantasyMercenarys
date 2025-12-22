using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SingleSkeletonSkin : SpineSkinBase
{
  [SerializeField] protected SkeletonAnimation skeletonAnimation;

  public override int SkinCount => 1;

  public override void SetColor(Color color)
  {
    skeletonAnimation.skeleton.SetColor(color);
  }

  public override void SetLayer(int orderLayer)
  {
    skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = orderLayer;
  }

  public override void SetSkin(string skinName)
  {
    base.SetSkin(skeletonAnimation.skeleton, skinName);
  }

  protected override void SetInitialSkin(string skinName)
  {
    skeletonAnimation.initialSkinName = skinName;
  }

#if UNITY_EDITOR
  private void Reset()
  {
    skeletonAnimation = GetComponent<SkeletonAnimation>();
  }
#endif
}
