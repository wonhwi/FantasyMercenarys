using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class ForwardAreaSkill : AreaSkillBase
{
  public override void InitSkill(GameSkillData gameSkillData, Vector3 targetPos)
  {
    base.InitSkill(gameSkillData, targetPos);

    startPos = IAttacker.GetAttackerForwardPoint() + Vector3.up * gameSkillData.startOffsetPos;
    endPos = targetPos;
  }
}
