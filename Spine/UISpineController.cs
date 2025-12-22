using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// UI Animation, UI Skin 둘다 사용하는 Controller
/// 추후 IPoolableUI 삭제하고 동료 UI Spine UISpineController 상속받게 구조 설계 해야될듯
/// </summary>
public class UISpineController : MonoBehaviour, IPoolableUI
{
  [SerializeField] protected UISkeletonAnimation spineAnimation;
  [SerializeField] protected UISkeletonSkin spineSkin;

  public void AddListenerAnimationComplete(Action<string> action)
  {
    spineAnimation.AddListenerHandleAnimationComplete(action);
  }

  public void SetAnimation(string animation, bool loop = true)
  {
    spineAnimation.SetAnimation(animation, loop : loop);
  }

  public void AddAnimation(string animation, bool loop = true)
  {
    spineAnimation.AddAnimation(animation, loop: loop);
  }

  public void SetSkin(string skinName)
  {
    spineSkin.SetSkin(skinName);
  }

  public void SetColor(Color color)
  {
    spineSkin.SetColor(color);
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
    this.transform.parent = parent;

    (this.transform as RectTransform).anchoredPosition = Vector2.zero;
  }
}
