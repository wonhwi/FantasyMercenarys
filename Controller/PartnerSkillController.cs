using Assets.ZNetwork.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FantasyMercenarys.Data;

public class PartnerSkillController : SkillController
{
  private PartnerController partnerController; //동료의 스텟을 가져올때 사용


  protected override void Awake()
  {
    base.Awake();

    this.partnerController = GetComponent<PartnerController>();
    
    this.SetAttacker(this.partnerController);
  }

  protected override void Start()
  {
    base.Start();

    InitSkill();
  }

  protected override void InitSkill()
  {
    InitCommonSkill();
  }

  private void InitCommonSkill()
  {
    IdleSkillData idleSkillData = SkillTable.getInstance.GetSkillData(ConstantManager.PARTNER_COMMON_ATTACK_SKILL_INDEX);

    base.UpdateCommonSkillData(CalculateSkillData(idleSkillData));
  }


  public override void ExcuteSkill(GameSkillData gameSkillData)
  {
    throw new NotImplementedException();
  }

  public override Vector3 GetTarget()
  {
    return CombatUtility.GetEnemyTarget(this.transform);
  }

  protected override GameSkillData CalculateSkillData(IdleSkillData idleSkillData)
    => new GameSkillData(idleSkillData, FactionType.Friendly);

  public override bool IsValidTarget()
  {
    return CombatUtility.ValidEnemyTarget();
  }
}
