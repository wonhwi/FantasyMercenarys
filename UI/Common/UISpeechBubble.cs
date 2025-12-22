using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class UISpeechBubble : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
  private float offsetY = 10f;
  public TextMeshProUGUI bubbleText;

  private GameObject pointerTarget;


  public void SetData(ItemData itemData, RectTransform target)
  {
    this.gameObject.SetActive(true);

    IdleSkillData skillData = SkillTable.getInstance.GetSkillData(itemData.itemIdx);

    bubbleText.text = LanguageTable.getInstance.GetLanguage(skillData.descRecordCd);

    this.transform.position = target.position + Vector3.up * (target.sizeDelta.y * 0.5f + offsetY);
  }

  public bool IsActive()
    => this.gameObject.activeSelf && pointerTarget == null;

  public void Close()
  {
    pointerTarget = null;
    this.gameObject.SetActive(false);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (eventData.pointerEnter == this.gameObject)
      pointerTarget = eventData.pointerEnter;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (eventData.pointerEnter == this.gameObject)
      pointerTarget = null;
  }
}
