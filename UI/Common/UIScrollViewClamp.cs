using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollViewClamp : MonoBehaviour, IDragHandler
{
  [SerializeField] private ScrollRect scrollRect;

  [SerializeField] private Vector2 minClamp = new Vector2(-500, -100);
  [SerializeField] private Vector2 maxClamp = new Vector2(500, 100);

  Vector2 lastPos;

  public void OnDrag(PointerEventData eventData)
  {
    Vector2 pos = scrollRect.content.anchoredPosition;
    pos.x = Mathf.Clamp(pos.x, minClamp.x, maxClamp.x);
    pos.y = Mathf.Clamp(pos.y, minClamp.y, maxClamp.y);

    lastPos = scrollRect.content.anchoredPosition = pos;
  }

}
