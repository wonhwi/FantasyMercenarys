using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkeletonSkin : SpineSkinBase
{
  [SerializeField] protected SkeletonGraphic skeletonGraphic;

  public override int SkinCount => 1;

  public override void SetLayer(int orderLayer) { }

  public override void SetSkin(string skinName)
  {
    base.SetSkin(skeletonGraphic.Skeleton, skinName);
  }

  protected override void SetInitialSkin(string skinName)
  {
    skeletonGraphic.initialSkinName = skinName;
  }

  public override void SetColor(Color color)
  {
    skeletonGraphic.color = color;
  }
}
