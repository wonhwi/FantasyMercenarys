using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SkeletonDataContainer : MonoBehaviour
{
  [System.Serializable]
  public struct SkeletonData
  {
    public CostumePart costumePart;
    public SkeletonAnimation[] skeletonAnimations;
  }

  public SkeletonData[] skeletonDataList;

  public SkeletonAnimation[] GetSkeletonAnimation(CostumePart costumePart)
  {
    for (int i = 0; i < skeletonDataList.Length; i++)
    {
      if(costumePart == skeletonDataList[i].costumePart)
      {
        return skeletonDataList[i].skeletonAnimations;
      }
    }

    return null;
  }

  public int GetSkeletonAnimationCount()
  {
    int skinCount = 0;

    for (int i = 0; i < skeletonDataList.Length; i++)
    {
      for (int j = 0; j < skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        skinCount++;
      }
    }

    return skinCount;
  }


}
