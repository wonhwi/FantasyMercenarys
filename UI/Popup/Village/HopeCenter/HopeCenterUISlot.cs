using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HopeCenterUISlot : MonoBehaviour
{
  [SerializeField] private BundleConsume bundleConsume;
  [SerializeField] private BundleCountDownText bundleCountDownText;

  [SerializeField] private TextMeshProUGUI lotteryNameText;
  [SerializeField] private Image lotteryImage;
  [SerializeField] private TextMeshProUGUI rewardText; //획득 가능 등급

  [SerializeField] private TextMeshProUGUI purchaseLimitCount; //구매 가능 수량
  [SerializeField] private Button purchaseButton; //구매 버튼


  public Action OnPurchase;
  public Action OnCountComplete;

  private void Awake()
  {
    purchaseButton.onClick.AddListener(() => OnPurchase?.Invoke());
  }

  public void SetLotteryNameText(string lotteryName)
  {
    lotteryNameText.text = LanguageTable.getInstance.GetLanguage(lotteryName);
  }

  public void SetLotteryImage(string imageName)
  {
    lotteryImage.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_HOPE_CENTER, imageName);
  }

  public void SetLotteryRewardText(string text)
  {
    rewardText.text = LanguageTable.getInstance.GetLanguage(text);
  }

  public void SetPurchaseLimitText(string limitText)
  {
    purchaseLimitCount.text = limitText;
  }

  public void SetConsumeData(ConsumeRequirement[] requirements)
  {
    bundleConsume.SetConsumeData(requirements);
  }

  public bool GetConsumeIsEnought()
  {
    return bundleConsume.CanConsume();
  }

  public void InitCountDown()
  {
    bundleCountDownText.InitCountDown();
  }

  public void SetCountDown(int totalSeconds)
  {
    bundleCountDownText.OnComplete = OnCountComplete;

    bundleCountDownText.SetCountDown(totalSeconds);
  }


}
