using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 전반적인 스킬에 관여하고 생성 및 스킬 관련 데이터 수치에 관여
/// </summary>
public class SkillManager : LazySingleton<SkillManager>
{
  /// <summary>
  /// 현재 활성화된 이펙트 List들
  /// </summary>
  public List<ActiveSkillBase> skillActorList = new List<ActiveSkillBase>();

  //현재 활성화 되어있는 SkillControll List들
  //플레이어, 보스, 파트너
  public List<SkillController> skillControllList = new List<SkillController>();

  /// <summary>
  /// 스킬 Effect Visual Enum 형태
  /// </summary>
  public SkillVisualType visualType = SkillVisualType.All;

  public void SetVisualSkill(SkillVisualType visualType)
    => this.visualType = visualType;

  /// <summary>
  /// 현재 활성화된 스킬 Return Clear
  /// </summary>
  public void ClearSkillList()
  {
    for (int i = skillActorList.Count - 1; i >= 0; i--)
    {
      if (skillActorList[i] && !skillActorList[i].isUseLifeTime)
        skillActorList[i].ReleaseSkillData();
    }
  }

  /// <summary>
  /// 스킬 리스소 해제 및 초기상태
  /// </summary>
  public void Release()
  {
    ClearSkillList();

    skillControllList.Clear();
  }
}