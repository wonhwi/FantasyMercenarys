using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : UIButtonBase
{
  [SerializeField] private bool useSound = true;
  [SerializeField] private string sfxSoundName = ConstantManager.SFX_UI_DEFAULT_CLICK;

  protected override void Awake()
  {
    base.Awake();

    OnClickAction += OnPlayUISFX;
  }

  private void OnPlayUISFX()
  {
    if (useSound)
      NewSoundManager.Instance.PlayUISFXSound(sfxSoundName);
  }

}
