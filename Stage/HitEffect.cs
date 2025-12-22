using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : SkeletonController, IPoolable, IAnimationStateListener
{
  public void Awake()
  {
    AddListenerAnimationComplete(OnAnimationComplete);
  }

  public void PlayHitEffectAnimation(Transform _transform)
  {
    transform.position = _transform.position;
    OnActivate();

    SetAnimation("HitEffect", loop : false);
  }

  private void ReleaseHitEffect()
  {
    NewInGameManager.getInstance.ReturnObjectPoolTypeHitEffect(this);
  }

  public void OnActivate()
  {
    gameObject.SetActive(true);
  }

  public void OnDeactivate()
  {
    gameObject.SetActive(false);
  }

  public void OnAnimationComplete(string animationName)
  {
    ReleaseHitEffect();
  }
}
