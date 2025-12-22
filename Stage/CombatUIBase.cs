using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIBase : MonoBehaviour, IPoolable
{
  protected RectTransform thisRectTransform;

  protected virtual void Awake()
  {
    thisRectTransform = GetComponent<RectTransform>();
  }

  public virtual void UpdateUIPosition(Vector3 position)
  {
    Vector3 transform = Camera.main.WorldToScreenPoint(position);
    float screenScaleFactor = Screen.height / (float)Camera.main.pixelHeight;
    transform.x *= screenScaleFactor;
    transform.y *= screenScaleFactor;
    thisRectTransform.position = transform;
  }

  public virtual void OnActivate()
  {
    gameObject.SetActive(true);

  }

  public virtual void OnDeactivate()
  {
    thisRectTransform.anchoredPosition = Vector3.zero;

    gameObject.SetActive(false);
  }
}
