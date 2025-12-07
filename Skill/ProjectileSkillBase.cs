using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체인스킬로 확장되면 다른 타겟을 향해 이동하는 Update 부분 수정 할 수 있게
/// </summary>
public class ProjectileSkillBase : ActiveSkillBase
{
  public override void InitSkill(GameSkillData gameSkillData, Vector3 targetPos)
  {
    base.InitSkill(gameSkillData, targetPos);

    startPos = IAttacker.GetAttackerProjectilePoint() + Vector3.up * gameSkillData.startOffsetPos;
    endPos = targetPos;
  }

  protected override void SetAnimation(string skinName) 
  {
    spineAnimation.SetAnimation("Item_active_skill");
  }

  protected override void TakeDamageEvent(IDamageable idamageable)
  {
    skillHitEnemyCount--;

    TakeDamage(idamageable);

    if (skillHitEnemyCount <= 0)
    {
      ReleaseSkillData();
    }
  }
}
