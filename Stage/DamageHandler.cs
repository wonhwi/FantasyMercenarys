using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DamageHandler : MonoBehaviour
{
  private IDamageable damageable;

  //체력 바 
  [SerializeField] private HUDBar hudBar;

  //피격 관련 Effect 생성 위치
  public Transform hitEffectRoot;

  //피격 관련 Text 위치
  [SerializeField] public Vector3 damageTextOffset;
  [SerializeField] public Vector3 hudBarOffset;

  public void SetDamageable(IDamageable damageable)
  {
    this.damageable = damageable;
  }

  public void SetHudBar(bool isPlayer)
  {
    if(this.hudBar == null)
    {
      this.hudBar = (HUDBar)NewInGameManager.getInstance.hudBarPool.GetObject();
    }

    this.hudBar.handler = this;

    this.hudBar.SetProgressBarSprite(isPlayer);
    this.hudBar.OnActivate();
  }


  public void LateUpdate()
  {
    if (hudBar != null)
    {
      if(damageable != null)
        hudBar.UpdateUIPosition(damageable.GetDamagerPosition() + (hudBarOffset * this.transform.localScale.y));
    }
  }

  public void TakeDamage(IAttackable attackable, SkillAttackType skillAttackType, float damagePer, SkillType skillType)
  {
    var damageText = (DamageText)NewInGameManager.getInstance.damageTextPool.GetObject();

    Vector3 damagePosition = damageable.GetDamagerPosition() + (damageTextOffset * this.transform.localScale.y);

    damageText.UpdateUIPosition(damagePosition);

    
    float hitChance = Random.Range(0f, 100f);

    float accuracyValue = attackable.GetStatValue(StatType.AccuracyRate); //적중률
    float dodgeValue = damageable.GetStatValue(StatType.DodgeRate);       //회피율


    bool isHit = hitChance <= (accuracyValue - dodgeValue);

    //MISS 처리
    if (!isHit)
    {
      damageText?.SetMiss(damagePosition);
      return;
    }

    (float totalDamage, bool isCritical) = CombatUtility.GetDamage(attackable, damageable, skillAttackType, damagePer, skillType);

    totalDamage = Mathf.Clamp(totalDamage, 1f, totalDamage);

    damageable.CurrentHP -= totalDamage;

    CalculationDamageFillAmount(damageable.CurrentHP, damageable.MaxHP);
    

    damageText?.SetDamage(totalDamage, damagePosition, isCritical);

    if (damageable.IsDead())
    {
      CreateDeathEffectAnimation();

      damageable.DeadAction();
    }
    else
    {
      CreateHitEffectAnimation();
    }
  }


  /// <summary>
  /// 몬스터 죽었을때 실행
  /// </summary>
  public void ReleaseHUDBar()
  {
    if (hudBar != null)
    {
      NewInGameManager.getInstance.ReturnObjectPoolTypeHUDBar(hudBar);
      hudBar = null; // 참조 해제
    }
  }

  public void CalculationDamageFillAmount(float _currentHP, float _maxHP)
  {
    hudBar.CalculationDamageFillAmount(_currentHP, _maxHP);
  }

  public void CreateHitEffectAnimation()
  {
    HitEffect effect = (HitEffect)NewInGameManager.getInstance.hitEffectPool.GetObject();

    if (effect != null)
    {
      effect.PlayHitEffectAnimation(hitEffectRoot);
    }
  }

  public void CreateDeathEffectAnimation()
  {
    DeathEffect effect = (DeathEffect)NewInGameManager.getInstance.deathEffectPool.GetObject();

    if (effect != null)
    {
      effect.PlayDeathEffectAnimation(hitEffectRoot);
    }
  }

}
