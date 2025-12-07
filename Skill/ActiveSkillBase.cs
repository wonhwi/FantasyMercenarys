using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkillBase : SkillBase
{
  [SerializeField] protected SpineAnimationBase spineAnimation;
  [SerializeField] protected SpineSkinBase spineSkin;

  [SerializeField] public bool isUseLifeTime = false;   //생존시간 사용하는지
  [SerializeField] protected bool isTriggerSkill = true;   //트리거를 사용하는 스킬인지

  [Tooltip("스킬 시작 위치")]
  protected Vector3 startPos;

  [Tooltip("스킬 타겟 위치")]
  protected Vector3 endPos;

  [Tooltip("최대 공격 가능 객채 갯수")]
  [SerializeField] protected int skillHitEnemyCount = 1;

  [Tooltip("투사체 : 생존시간, 기타 : 지속 시간")]
  [SerializeField] protected float lifeTime = ConstantManager.SKILL_DEFAULT_LIFT_TIME;

  public override void InitSkill(GameSkillData gameSkillData, Vector3 targetPos)
  {
    this.skillData = gameSkillData;
    this.skillHitEnemyCount = gameSkillData.skillHitEnemyCount;
    this.lifeTime = gameSkillData.lifeTime;

    string skinName = gameSkillData.skillEffectSpine;

    //나중에 삭제할 내용 임시 방어 코드
    if (skinName.Equals("0"))
      skinName = (SkillType)gameSkillData.skillType == SkillType.Projectile ? ConstantManager.SKILL_PROJECTILE_DEFAULT_SKIN : ConstantManager.SKILL_AREA_DEFAULT_SKIN;

    SetSkin(skinName);
    SetAnimation(skinName);
  }

  protected virtual void SetSkin(string skinName)
  {
    spineSkin.SetSkin(skinName);
  }

  protected abstract void SetAnimation(string skinName);


  public override void StartSkill()
  {
    NewSoundManager.Instance.PlaySFXSound(skillData.sound);

    transform.position = startPos;
  }

  /// <summary>
  /// Skill Object 초기화 함수
  /// </summary>
  public override void ReleaseSkillData()
  {
    SkillType skillType = (SkillType)skillData.skillType;

    NewInGameManager.getInstance.ReturnObjectPoolTypeSkill(this, skillType);
    SkillManager.getInstance.skillActorList.Remove(this);

    transform.position = Vector3.zero;
    startPos = endPos = Vector3.zero;
  }

  public override void SetAttackerUnit(IAttackable iAttacker)
  {
    IAttacker = iAttacker;
  }

  protected void OnTriggerEnter2D(Collider2D collision)
  {
    if (!isTriggerSkill)
      return;

    //비활성화 될 때 Trigger 인식이 되는 경우도 있어 Return
    if (!this.gameObject.activeInHierarchy)
      return;

    if (collision.TryGetComponent(out IDamageable damageable))
    {
      //아군 스킬이면 무시
      if (damageable.GetFactionType() == skillData.factionType)
        return;

      TakeDamageEvent(damageable);
    }
  }

}
