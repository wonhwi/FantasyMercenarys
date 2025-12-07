using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProjectileSkill : ProjectileSkillBase
{
  protected override void UpdateSkill()
  {
    Vector3 direction = (endPos - transform.position).normalized;
    transform.position += skillData.speed * Time.deltaTime * direction;

    if (Vector2.Distance(transform.position, endPos) < 0.1f)
      base.ReleaseSkillData();
  }
}
