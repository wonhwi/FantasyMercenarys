using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MultipleSkeletonAnimation : SpineAnimationBase
{
  [SerializeField] private SkeletonDataContainer skeletonDataContainer;

  public SkeletonDataContainer SkeletonDataContainer 
  {
    get
    {
      return skeletonDataContainer;
    } 
    set
    {
      RemoveOnAnimationComplete();

      skeletonDataContainer = value;

      AddOnAnimationComplete();


    } 
  }

  public override int AnimationCount => SkeletonDataContainer.GetSkeletonAnimationCount();

  public override Spine.AnimationState GetAnimationState()
  {
    return SkeletonDataContainer.skeletonDataList[0].skeletonAnimations[0].AnimationState;
  }


  public override void SetAnimation(string animationName, int trackIndex = 0, bool loop = true, float timeScale = 1f)
  {
    Debug.LogError(animationName);
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].AnimationState.SetAnimation(trackIndex, animationName, loop);
        
      }
    }

    SetTimeScale(timeScale);
  }

  public override void AddAnimation(string animationName, int trackIndex = 0, bool loop = true, float delay = 0f)
  {
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].AnimationState.AddAnimation(trackIndex, animationName, loop, delay);
      }
    }

  }

  public override void SetTimeScale(float timeScale)
  {
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].AnimationState.TimeScale = timeScale * GameTime.TimeScale;
      }
    }
  }

  public override void ClearAllTracks()
  {
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].AnimationState.ClearTracks();
      }
    }
  }
}
