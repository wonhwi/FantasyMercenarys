using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ResourceSkinDataContainer;

public class ResourceSkin : MonoBehaviour
{
  [SerializeField] private ResourceSkinDataContainer resourceSkinDataContainer;


  public ResourceSkinDataContainer ResourceSkinDataContainer
  {
    get
    {
      return resourceSkinDataContainer;
    }
    set
    {
      resourceSkinDataContainer = value;
    }

  }

  public void SetSkin(CostumePart costumePart, string skinName)
  {
    ResourceSkinData[] resourceSkinDataArray = resourceSkinDataContainer.GetResourceSkinDataArray(costumePart);
    ResourceSkinData resourceSkinData = resourceSkinDataContainer.GetResourceSkinData(costumePart, GetFirstToken(skinName).ToString());
    
    for (int i = 0; i < resourceSkinDataArray.Length; i++)
    {
      for (int j = 0; j < resourceSkinDataArray[i].spriteRenderer.Count; j++)
      {
        resourceSkinDataArray[i].spriteRenderer[j].enabled = false;
      }
    }

    SpineLayerData spineLayerData = SpineLayerTable.getInstance.GetSpineLayerData(skinName);

    resourceSkinData.spriteRenderer[0].enabled = true;
    resourceSkinData.spriteRenderer[0].sprite = NewResourceManager.getInstance.LoadResourceSkin(spineLayerData.front);

    if (spineLayerData.back != "0")
    {
      if(resourceSkinData.spriteRenderer[1] == null)
      {
        Debug.Log("잘못된 접근입니다.");
        return;
      }
      resourceSkinData.spriteRenderer[1].enabled = true;
      resourceSkinData.spriteRenderer[1].sprite = NewResourceManager.getInstance.LoadResourceSkin(spineLayerData.back);
    }


  }

  private ReadOnlySpan<char> GetFirstToken(string input)
  {
    var span = input.AsSpan();
    int index = span.IndexOf('_');
    return (index == -1) ? span : span.Slice(0, index);
  }


}
