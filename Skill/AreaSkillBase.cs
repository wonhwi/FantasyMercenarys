using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class AreaSkillBase : ActiveSkillBase, IAnimationStateListener
{
  public override void StartSkill()
  {
    base.StartSkill();

    spineAnimation.OnAnimationComplete += OnAnimationComplete;
  }
  public override void ReleaseSkillData()
  {
    spineAnimation.OnAnimationComplete -= OnAnimationComplete;

    base.ReleaseSkillData();
  }

  protected override void SetAnimation(string skinName)
  {
    spineAnimation.SetAnimation(skinName, loop: false);
  }

  protected override void TakeDamageEvent(IDamageable idamageable)
  {
    if (skillHitEnemyCount <= 0)
      return;

    TakeDamage(idamageable);

    skillHitEnemyCount--;
  }

  public void OnAnimationComplete(string animationName)
  {
    ReleaseSkillData();
  }
}