using Newtonsoft.Json;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkill : AreaSkillBase
{
  public override void InitSkill(GameSkillData gameSkillData, Vector3 targetPos)
  {
    base.InitSkill(gameSkillData, targetPos);

    startPos = endPos = targetPos + Vector3.up * gameSkillData.startOffsetPos;
  }
}
