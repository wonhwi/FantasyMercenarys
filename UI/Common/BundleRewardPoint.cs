using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 보상 점수 획득 모듈화 스크립트
/// 기도원, 사육장에서 사용중
/// </summary>
public class BundleRewardPoint : MonoBehaviour
{
  public float positionY = 10f;

  private Vector3 originalPosition;
  public TextMeshProUGUI pointText;

#if UNITY_EDITOR
  public void Reset()
  {
    originalPosition = transform.position;
  }
#endif

  [ContextMenu("InitPosition")]
  public void InitPosition()
  {
    originalPosition = transform.position;
  }

  public void SetPoint(int point)
  {
    this.gameObject.SetActive(true);

    pointText.text = point.ToString();

    transform.DOLocalMoveY(originalPosition.y + positionY, 0.5f)
            .SetEase(Ease.OutQuad) // 부드러운 이동
            .OnComplete(() =>
            {
              // 2초 후에 UI 비활성화 및 원래 위치로 복귀
              DOVirtual.DelayedCall(2f, () =>
              {
                gameObject.SetActive(false); // UI 비활성화
                transform.localPosition = originalPosition; // 원래 위치로 복귀
              });
            });

  }

}
