using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDBar : CombatUIBase
{
  [SerializeField] private Image fillAmountImage;
  public DamageHandler handler;

  public void SetProgressBarSprite(bool isPlayer)
  {
    fillAmountImage.sprite = NewResourceManager.getInstance.LoadResource<Sprite>(string.Format(
      NewResourcePath.PATH_UI_BATTLE, isPlayer ? "player" : "monster")
      );
  }

  /// <summary>
  /// Image FillAmount 계산 함수.
  /// </summary>
  /// <param name="_currentHP">현재 HP</param>
  /// <param name="_maxHP">최대 HP</param>
  public void CalculationDamageFillAmount(float _currentHP, float _maxHP)
  {
    fillAmountImage.fillAmount = _currentHP / _maxHP;
  }

  public override void OnDeactivate()
  {
    base.OnDeactivate();

    fillAmountImage.fillAmount = 1f;

  }
}
