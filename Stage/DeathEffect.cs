using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : SkeletonController, IPoolable, IAnimationStateListener
{
  public void Awake()
  {
    AddListenerAnimationComplete(OnAnimationComplete);
  }

  public void PlayDeathEffectAnimation(Transform _transform)
  {
    transform.position = _transform.position;
    OnActivate();

    SetAnimation("DeathEffect", loop: false);
  }

  private void ReleaseDeathEffect()
  {
    NewInGameManager.getInstance.ReturnObjectPoolTypeDeathEffect(this);
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
    ReleaseDeathEffect();
  }
}
