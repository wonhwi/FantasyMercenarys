using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundCreature : MonoBehaviour
{
  private readonly string animationName = "CreatureRunning";

  [SerializeField] private Animator animator;

  [SerializeField] private Image creatureIcon;

  private float startNormalizedTime;

  /// <summary>
  /// 크리쳐 이미지 설정
  /// </summary>
  /// <param name="creatureImage"></param>
  public void SetCreatureImage(string creatureImage)
  {
    this.gameObject.SetActive(true);

    creatureIcon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, creatureImage);
  }

  /// <summary>
  /// 애니메이션 실행 (랜덤 정규화 시작 시간, 애니메이션 속도 설정)
  /// </summary>
  /// <param name="creatureSpeed"></param>
  public void SetCreautureAnimation(float creatureSpeed)
  {
    // 랜덤 시작 지점: 0.0 ~ 1.0
    startNormalizedTime = 0.1f * (this.transform.GetSiblingIndex() + 1);  //UnityEngine.Random.Range(0f, 1f);

    //속도 설정
    animator.speed = 1 / creatureSpeed;

    // 애니메이션 실행 (정규화 시간 기반)
    animator.Play(animationName, 0, startNormalizedTime);
  }

  public void InitCreature()
  {
    this.gameObject.SetActive(false);
  }


}
