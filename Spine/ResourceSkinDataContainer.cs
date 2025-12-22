using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSkinDataContainer : MonoBehaviour
{
  [System.Serializable]
  public struct ResourceSkinData
  {
    public string boneName;
    public List<SpriteRenderer> spriteRenderer;
  }


  public ResourceSkinData[] weaponSkinDatas;
  public ResourceSkinData[] accessorySkinDatas;


  public ResourceSkinData[] GetResourceSkinDataArray(CostumePart costumePart) => costumePart switch
  { 
    CostumePart.Weapon => weaponSkinDatas,
    CostumePart.Accessory => accessorySkinDatas,
    _ => null,
  };

  public ResourceSkinData GetResourceSkinData(CostumePart costumePart, string boneName)
  {
    var resourceSkinDatas = GetResourceSkinDataArray(costumePart);

    foreach (var skinData in resourceSkinDatas)
    {
      if(skinData.boneName == boneName)
      {
        return skinData;
      }
       
    }

    return default;
  }

}
