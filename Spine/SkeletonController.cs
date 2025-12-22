using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Animation, Skin 둘다 사용하는 Controller
/// 플레이어, 동료, 코스튬
/// </summary>
public class SkeletonController : MonoBehaviour
{
  [SerializeField] protected SpineAnimationBase spineAnimation;
  [SerializeField] protected SpineSkinBase spineSkin;

  public void SetSkin(string skinName)
  {
    spineSkin.SetSkin(skinName);
  }

  public void SetLayer(int orderLayer)
  {
    spineSkin.SetLayer(orderLayer);
  }

  public void SetAnimation(string animationName, int trackIndex = 0, bool loop = true)
  {
    spineAnimation.SetAnimation(animationName, loop: loop);
  }

  public void AddAnimation(string animationName, int trackIndex = 0, bool loop = true)
  {
    spineAnimation.AddAnimation(animationName, loop: loop);
  }

  public void AddListenerAnimationComplete(Action<string> action)
  {
    spineAnimation.AddListenerHandleAnimationComplete(action);
  }
}
