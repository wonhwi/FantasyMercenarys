using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemBase : CombatUIBase
{
  protected NewInGameManager inGameManager;
  protected GameDataManager gameDataManager;

  [SerializeField] private Image itemIcon;
  protected int itemIdx;
  protected long itemCount;

  [SerializeField] private float delayBeforeMove = 1.5f;
  [SerializeField] private float moveDuration = 0.5f;

  protected Transform targetTransform;

  protected override void Awake()
  {
    base.Awake();

    inGameManager = NewInGameManager.getInstance;
    gameDataManager = GameDataManager.getInstance;

  }


  /// <summary>
  /// 아이템 정보 삽입
  /// </summary>
  /// <param name="itemCount"></param>
  public void SetDropItemData(int itemIdx, long itemCount = 0)
  {
    this.targetTransform = inGameManager.dropGoldTransform;
    
    this.itemIdx = itemIdx;
    this.itemCount = itemCount;

    SetItemImage();
  }

  private void SetItemImage()
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    itemIcon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON, itemData.iconImage);
  }

  public override void UpdateUIPosition(Vector3 position)
  {
    base.UpdateUIPosition(position);

    StartDropMove();
  }
  
  public void StartDropMove()
  {
    DOVirtual.DelayedCall(delayBeforeMove, MoveToTarget, false);
  }

  private void MoveToTarget()
  {
    this.targetTransform ??= inGameManager.dropGoldTransform;

    thisRectTransform.DOMove(targetTransform.position, moveDuration)
        .SetEase(Ease.Linear)
        .OnComplete(OnDropArrived);
  }

  protected virtual void OnDropArrived()
  {
    DOTween.Kill(targetTransform);
    targetTransform.DOScale(0.8f, 0.25f)
            .OnComplete(() => {
              targetTransform.DOScale(1f, 0.25f)
                  .SetEase(Ease.OutBack);
            });
  }
}
