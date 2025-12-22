using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//동료는 평타만 치냐??
public class PartnerController : AttackableUnitBase, IPoolable
{

  private NewInGameManager inGameManager;
  
  [SerializeField] public SpineAnimationBase spineAnimation;
  [SerializeField] public SpineSkinBase spineSkin;

  [SerializeField] private PartnerSkillController skillController;     //공격시 스킬 사용 용도

  public PartnerData partnerData;

  [SerializeField] private Vector3 partnerPos;

  private bool eventAwake = false;

  public void Awake()
  {
    inGameManager = NewInGameManager.getInstance;
  }

  /// <summary>
  /// 동료 설정
  /// </summary>
  public void InitPartner(PartnerData partnerData, Vector3 pos)
  {
    this.partnerData = partnerData;
    this.partnerPos = pos;

    InitPosition();
    SetPartnerData();
  }

  public void InitPosition()
  {
    this.transform.position = this.partnerPos;
  }

  //이거를 플레이어 스텟 업데이트시 업데이트 할 수 있도록 해야합니다.
  public void SetPartnerData()
  {
    statBuffController.SetData(this.partnerData);

    InitAttackUnit();

  }

  public void Update()
  {
    //Debug.LogError(inGameManager.currentStageState + " : " + inGameManager.currentWaveState);

    attackTime += Time.deltaTime;

    switch (inGameManager.currentStageState)
    {
      case StageState.StageStart:
        InitPosition();

        spineAnimation.SetAnimation(PartnerAnimation.Idle.ToString());
        break;

      case StageState.StagePlaying:

        if(inGameManager.currentWaveState == WaveState.WaveStart)
        {
          if (!spineAnimation.IsPlaying(PartnerAnimation.Idle.ToString()))
            spineAnimation.SetAnimation(PartnerAnimation.Idle.ToString());
        }
        else if(inGameManager.currentWaveState == WaveState.WaveEnd)
        {
          attackTime = 0f;

          if(!spineAnimation.IsPlaying(PartnerAnimation.Move.ToString()))
            spineAnimation.SetAnimation(PartnerAnimation.Move.ToString());
        }
        else
        {

          if(inGameManager.player.IsDead())
          {
            if (!spineAnimation.IsPlaying(PartnerAnimation.Run.ToString()))
              spineAnimation.SetAnimation(PartnerAnimation.Run.ToString());
            return;
          }
          
          if (attackTime >= attackInterval)
          {
            if (!inGameManager.player.IsDead())
            {
              if (inGameManager.isPlayerPossibleAttack)
              {
                if (skillController.IsValidTarget())
                {

                  spineAnimation.SetAnimation(PartnerAnimation.Attack.ToString(), loop: false, timeScale : 1 / attackInterval);

                  skillController.ExcuteCommonSkill();

                  attackTime = 0f;
                }
              }
            }

          }
        }
        break;
    }

    
  }



  public void OnActivate()
  {
    gameObject.SetActive(true);

    InGameEventManager.AddListener(InGameEventType.UpdatePlayerStat, SetPartnerData);
  }

  public void OnDeactivate()
  {
    gameObject.SetActive(false);

    if(eventAwake)
    {
      InGameEventManager.RemoveListener(InGameEventType.UpdatePlayerStat, SetPartnerData);
    }

    eventAwake = true;
    
  }

  public override Dictionary<StatType, float> GetTotalStats()
  {
    return statBuffController.GetTotalStats();
  }

  public override bool IsInAttackRange()
  {
    return true;
  }

  protected override CombatUnitBase GetTargetUnit()
  {
    return null;
  }

  protected override void PlayIdle()
  {
    if (!spineAnimation.IsPlaying(PartnerAnimation.Idle.ToString()))
      spineAnimation.SetAnimation(PartnerAnimation.Idle.ToString());
  }

  protected override void PlayAttack()
  {
    spineAnimation.SetAnimation(PartnerAnimation.Attack.ToString(), loop: false);

    skillController.ExcuteCommonSkill();
  }

  protected override void PlayMove()
  {
    if (!spineAnimation.IsPlaying(PartnerAnimation.Move.ToString()))
      spineAnimation.SetAnimation(PartnerAnimation.Move.ToString());
  }

}
