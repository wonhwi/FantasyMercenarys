using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GroundObject : MonoBehaviour
{
  [SerializeField] private Button objectButton;
  [SerializeField] private BundleRewardPoint bundleRewardPoint;

  public Action OnPointGain;

  private void Awake()
  {
    objectButton.onClick.AddListener(() => OnPointGain?.Invoke());
  }

  public int GetSiblingIndex()
  {
    return this.transform.GetSiblingIndex();
  }

  public bool IsActive()
  {
    return objectButton.gameObject.activeInHierarchy;
  }

  public void SetData(GroundObjectType groundObjectType)
  {
    objectButton.gameObject.SetActive(true);

    objectButton.image.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, $"fm_breeding_{groundObjectType.ToString().ToLower()}");
    objectButton.image.SetNativeSize();
  }

  public void SetRewardPoint(int point)
  {
    if(point > 0)
      bundleRewardPoint.SetPoint(point);

    objectButton.gameObject.SetActive(false);
  }

  public void InitObject()
  {
    objectButton.gameObject.SetActive(false);
    bundleRewardPoint.gameObject.SetActive(false);
  }
    
}
