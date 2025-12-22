using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : CombatUIBase, IPoolable
{
  [SerializeField] private TextMeshProUGUI damageText;
  public StringBuilder stringBuilder = new StringBuilder();

  [SerializeField] private float moveSpeed = 40f;
  private float damageTextDisplayTime = 1f;

  private void Update()
  {
    // 데미지 텍스트 1초동안 보여지며 위로 올라가는 로직.
    damageTextDisplayTime -= Time.deltaTime;
    thisRectTransform.anchoredPosition += (Vector2.up * moveSpeed) * Time.deltaTime;
    if (damageTextDisplayTime <= 0f)
    {
      damageTextDisplayTime = 1f;
      ReleaseDamageText();
    }
  }

  public void SetMiss(Vector3 _position)
  {
    UpdateUIPosition(_position);

    stringBuilder.Clear();
    stringBuilder.Append(ConstantManager.TEXT_MISS_SPRITE_NAME);

    damageText.text = stringBuilder.ToString();

    OnActivate();
  }

  /// <summary>
  /// DamageText 셋팅 함수.
  /// </summary>
  /// <param name="_damage">데미지</param>
  /// <param name="_position">데미지 텍스트 생성되는 위치</param>
  /// <param name="_isCritial">크리티컬 여부</param>
  public void SetDamage(float _damage, Vector3 _position, bool _isCritial = false)
  {
    UpdateUIPosition(_position);

    int damage = Mathf.RoundToInt(_damage);

    SetFontSprite(_isCritial ? DamageFontType.Critial : DamageFontType.Normal, damage);

    damageText.text = stringBuilder.ToString();

    OnActivate();
  }

  private void SetFontSprite(DamageFontType damageFontType, int damage)
  {
    stringBuilder.Clear();

    string numString = damage.ToString();
    int numCount = numString.Length;

    for (int i = 0; i < numCount; i++)
    {
      stringBuilder.Append(string.Format(ConstantManager.TEXT_SPRITE_NAME, (int)damageFontType, numString[i]));
    }
  }

  /// <summary>
  /// 데미지 텍스트 데이터 초기화 및 Pool반환 함수.
  /// </summary>
  private void ReleaseDamageText()
  {
    NewInGameManager.getInstance.ReturnObjectPoolTypeDamageText(this);
  }

  public override void OnDeactivate()
  {
    base.OnDeactivate();

    damageText.text = string.Empty;
  }

}
