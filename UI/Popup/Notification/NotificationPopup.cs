using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIBaseNotification : UIBaseController
{
  [SerializeField] private TextMeshProUGUI titleText;
  [SerializeField] private TextMeshProUGUI messageText;

  [SerializeField] protected Button confirmButton;
  [SerializeField] protected Button subButton;
  [SerializeField] protected Button cancelButton;

  [SerializeField] private TextMeshProUGUI confirmText;
  [SerializeField] private TextMeshProUGUI subText;
  [SerializeField] private TextMeshProUGUI cancelText;

  protected override void Awake()
  {
    base.Awake();

    cancelButton.onClick.AddListener(Hide);
  }

  protected virtual void SetText(string title, string message)
  {
    this.titleText.text = LanguageTable.getInstance.GetLanguage(title);
    this.messageText.text = LanguageTable.getInstance.GetLanguage(message);
  }

  protected void SetButtonText(string confirmText = "확인", string cancelText = "취소", string subText = "")
  {
    this.confirmText.text = confirmText;
    this.subText.text = subText;
    this.cancelText.text = cancelText;
  }

  public virtual void OpenNotice(string title, string message)
  {
    InitActiveButton();

    SetText(title, message);
  }

  private void InitActiveButton()
  {
    SetActiveButton(confirmButton, false);
    SetActiveButton(subButton,     false);
    SetActiveButton(cancelButton,  false);
  }

  public void SetActiveButton(Button button, bool setActive)
  {
    if (button != null)
      button.gameObject.SetActive(setActive);
  }
}


public class NotificationPopup : UIBaseNotification
{
  public void OpenConfirm(string title, string message, Action confirmAction = null, string confirmText = "확인", string cancelText = "취소")
  {
    base.OpenNotice(title, message);

    SetActiveButton(confirmButton, true);

    SetButtonText(confirmText, cancelText);

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => {
      confirmAction?.Invoke();
      Hide();
    });
  }

  public void OpenConfirmCancel(string title, string message, Action confirmAction = null, string confirmText = "확인", string cancelText = "취소")
  {
    base.OpenNotice(title, message);

    SetActiveButton(confirmButton, true);
    SetActiveButton(cancelButton, true);

    SetButtonText(confirmText, cancelText);


    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => {
      confirmAction?.Invoke();
      Hide();
    });

  }

  public void OpenConfirmSubCancel(string title, string message, Action confirmAction = null, Action subAction = null, string confirmText = "확인", string cancelText = "취소", string subText = "서브")
  {
    base.OpenNotice(title, message);

    SetActiveButton(confirmButton, true);
    SetActiveButton(subButton, true);
    SetActiveButton(cancelButton, true);

    SetButtonText(confirmText, cancelText, subText);

    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => {
      confirmAction?.Invoke();
      Hide();
    });

    subButton.onClick.RemoveAllListeners();
    subButton.onClick.AddListener(() => {
      subAction?.Invoke();
      Hide();
    });


  }

}
