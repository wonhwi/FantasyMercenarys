using Spine;
using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class SpineSkinBase : MonoBehaviour, ISpineSkin
{
  [HideInInspector] public Skin skin { get; set; }

  public abstract int SkinCount { get; }

  public virtual void SetSkin(string skinName) { }
  public virtual void SetSkin(CostumePart costumePart, string skinName) { }

  public virtual void SetSkin(Skeleton skeleton, string skinName)
  {
    skin = skeleton.Data.FindSkin(skinName);

    if (skin == null)
      skin = skeleton.Data.DefaultSkin;

    SetInitialSkin(skinName);

    skeleton.SetSkin((Skin)null);
    skeleton.SetSkin(skin);
    skeleton.SetSlotsToSetupPose();
  }

  public abstract void SetColor(Color color);

  public abstract void SetLayer(int orderLayer);
  protected abstract void SetInitialSkin(string skinName);

}