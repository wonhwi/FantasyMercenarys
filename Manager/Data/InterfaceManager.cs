using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public interface ICurrencyUpdateUsable
{
  void UpdateCurrency(int itemIdx);
  void DisableCurrency();
}
public interface ICurrencyUsable
{
  void EnableCurrency();
  void DisableCurrency();
}

public interface IStatable
{
  public abstract Dictionary<StatType, float> GetTotalStats();

  public float GetStatValue(StatType statType);

  FactionType GetFactionType();

  UnitType GetUnitType();
}
public interface IAttackable : IStatable
{
  /// <summary>
  /// 공격 범위에 들어왔는지 판단
  /// </summary>
  /// <returns></returns>
  bool IsInAttackRange();
  
  Vector3 GetAttackerProjectilePoint();

  Vector3 GetAttackerForwardPoint();

}
public interface IDamageable : IStatable
{
  public Vector3 GetDamagerPosition();

  public bool IsDead();

  public void DeadAction();

  public float CurrentHP { get; set; }
  public float MaxHP { get; }

  /// <summary>
  /// 데미지 받는 함수
  /// </summary>
  /// <param name="attackable">공격자</param>
  /// <param name="damagePer">스킬 계수</param>
  public void TakeDamage(IAttackable attackable, SkillAttackType skillAttackType, float damagePer, SkillType skillType);
}

public interface ICrowdControllable
{
  IEnumerator HandleStun(float stunTime);
  IEnumerator HandleImmobilize(float immobilizeTime);
  IEnumerator HandleKnockback(float knockbackValue);
  IEnumerator HandleSlow(float slowValue, float slowTime);

  CrowdControlType CCType { get; }

  void TakeCrowdControl(CrowdControlData ccData);
}


public interface IAnimationHandler
{
  void PlayIdle();
  void PlayAttack();
  void PlayMove();
  void PlayDead();
}

public interface ISpineAnimation
{
  Spine.AnimationState GetAnimationState();
  int AnimationCount { get; }
  void SetAnimation(string animationName, int trackIndex = 0, bool loop = true, float timeScale = 1f);
  void AddAnimation(string animationName, int trackIndex = 0, bool loop = true, float delay = 0f);
  bool IsPlaying(string animationName);
  void ClearTrack(int trackIndex);
  void ClearAllTracks();

  Action<string> OnAnimationStart { get; set; }
  Action<string> OnAnimationComplete { get; set; }

  //float GetAnimationDuration(string animationName);
}

public interface ISpineSkin
{
  int SkinCount { get; }
  void SetSkin(string skinName);
  void SetSkin(Skeleton skeleton, string skinName);
  void SetLayer(int orderLayer);
}
public interface IAnimationStateListener
{
  void OnAnimationComplete(string animationName);
}

public interface IPoolableUI
{
  void OnActivate();
  void OnDeactivate();
  void OnSetParent(Transform parent);
}

public interface IPoolable
{
  void OnActivate();
  void OnDeactivate();
}
