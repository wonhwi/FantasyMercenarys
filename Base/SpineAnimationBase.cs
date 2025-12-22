using Spine;
using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class SpineAnimationBase : MonoBehaviour, ISpineAnimation
{
  private Dictionary<string, float> _animationDurationCache = new Dictionary<string, float>();

  public abstract int AnimationCount { get; }
  public Action<string> OnAnimationStart { get; set; }
  public Action<string> OnAnimationComplete { get; set; }
  public Action<string> OnAnimationEvent { get; set; }

  public abstract Spine.AnimationState GetAnimationState();

  protected void Start()
  {
    AddOnAnimationComplete();
  }


  protected void OnDestroy()
  {
    RemoveOnAnimationComplete();
  }

  protected virtual void AddOnAnimationComplete()
  {
    var animState = GetAnimationState();
    if (animState != null)
    {
      animState.Start    += HandleAnimationStart;
      animState.Event    += HandleAnimationEvent;
      animState.Complete += HandleAnimationComplete;
    }
  }

  protected virtual void RemoveOnAnimationComplete()
  {
    var animState = GetAnimationState();
    if (animState != null)
    {
      animState.Start    -= HandleAnimationStart;
      animState.Event    -= HandleAnimationEvent;
      animState.Complete -= HandleAnimationComplete;
    }
  }


  protected virtual void HandleAnimationStart(TrackEntry trackEntry)
  {
    OnAnimationStart?.Invoke(trackEntry.Animation.Name);
  }

  protected virtual void HandleAnimationComplete(TrackEntry trackEntry)
  {
    OnAnimationComplete?.Invoke(trackEntry.Animation.Name);
  }
  protected virtual void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
  {
    OnAnimationEvent?.Invoke(e.Data.Name);
  }


  public virtual void AddListenerHandleAnimationStart(Action<string> action)
  {
    OnAnimationStart = action;
  }

  public virtual void AddListenerHandleAnimationComplete(Action<string> action)
  {
    OnAnimationComplete = action;
  }

  public virtual void AddListenerHandleAnimationEvent(Action<string> action)
  {
    OnAnimationEvent = action;
  }

  /// <summary>
  /// Animation 실행 여부 판단
  /// </summary>
  /// <param name="animationName"></param>
  /// <returns></returns>
  public virtual bool IsPlaying(string animationName)
  {
    var current = GetAnimationState().GetCurrent(0);
    return current != null && current.Animation.Name == animationName;
  }

  public virtual void SetAnimation(string animationName, int trackIndex = 0, bool loop = true, float timeScale = 1f)
  {
    GetAnimationState().SetAnimation(trackIndex, animationName, loop);
    SetTimeScale(timeScale);
  }

  public virtual void AddAnimation(string animationName, int trackIndex = 0, bool loop = true, float delay = 0f)
  {
    GetAnimationState().AddAnimation(trackIndex, animationName, loop, delay);
  }

  public virtual void SetTimeScale(float timeScale)
  {
    GetAnimationState().TimeScale = timeScale * GameTime.TimeScale;
  }

  public virtual void ClearTrack(int trackIndex)
  {
    GetAnimationState().ClearTrack(trackIndex);
  }

  public virtual void ClearAllTracks()
  {
    GetAnimationState().ClearTracks();
  }

  public virtual string GetAnimation()
  {
    return GetAnimationState().GetCurrent(0)?.Animation.Name;
  }

  public virtual float GetAnimationDuration(string animationName)
  {
    if (_animationDurationCache.TryGetValue(animationName, out float duration))
      return duration;
  
    var animation = GetAnimationState().Data.SkeletonData.FindAnimation(animationName);
    if (animation != null)
    {
      duration = animation.Duration;
      _animationDurationCache[animationName] = duration;
      return duration;
    }
    return 0f;
  }

}