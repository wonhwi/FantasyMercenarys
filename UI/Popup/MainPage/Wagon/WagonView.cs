using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WagonView : MonoBehaviour
{
  public enum ButtonState
  {
    Available = 0, //사용 가능
    Using = 1, //사용 중
    NotAvailable = 2, //사용 불가
  }

  [Serializable]
  public struct WagonInfo
  {
    public WagonType wagonType;
    public GameObject wagonObject;
  }
  [SerializeField] protected WagonInfo[] wagons;
  [SerializeField] GameObject skeletonCamera;
  [SerializeField] GameObject skeletonRoot;

  [Header("Cart Lv Range")]
  [SerializeField] private TextMeshProUGUI cartLvRange;

  [Header("BundleGaugeText")]
  [SerializeField] private BundleGaugeText bundleGaugeText;
  
  [Header("BundleCurrency")]
  [SerializeField] private BundleCurrency bundleCurrency;

  [Header("Name")]
  [SerializeField] private TextMeshProUGUI cartNameText;

  [Header("Level")]
  [SerializeField] private TextMeshProUGUI curLvText;
  [SerializeField] private TextMeshProUGUI nextLvText;

  [Header("StageReward")]
  [SerializeField] private TextMeshProUGUI curRewardText;
  [SerializeField] private TextMeshProUGUI nextRewardText;

  [Header("Heath Bonus")]
  [SerializeField] private TextMeshProUGUI curHeathText;
  [SerializeField] private TextMeshProUGUI nextHeathText;

  [Header("Attack Bonus")]
  [SerializeField] private TextMeshProUGUI curAttackText;
  [SerializeField] private TextMeshProUGUI nextAttackText;

  [Header("Defense Bonus")]
  [SerializeField] private TextMeshProUGUI curDefenseText;
  [SerializeField] private TextMeshProUGUI nextDefenseText;

  [Header("UI/Event")]
  [SerializeField] private Button beforeButton;       //이전 수레 스킨 정보
  [SerializeField] private Button nextButton;         //다음 수레 스킨 정보

  [SerializeField] private Button availableButton;    //사용 가능
  [SerializeField] private Button usingButton;        //사용 중
  [SerializeField] private Button notAvailableButton; //사용 불가
  [SerializeField] private Button upgradeButton;      //강화 버튼
  [SerializeField] private Button closeButton;        //닫기 버튼
  public Action OnBeforeSkin;
  public Action OnNextSkin;
  public Action OnClosePopup;
  public Action OnSelectSkin; //수레 외형 변경
  public Action OnUpgrade;

  private void Awake()
  {
    closeButton.onClick.AddListener(() => { 
      OnClosePopup?.Invoke();

      SetActiveSkeleton(false);
    });

    availableButton.onClick.AddListener(()     => OnSelectSkin?.Invoke());
    usingButton.onClick.AddListener(()         => Debug.Log("이미 사용중"));
    notAvailableButton.onClick.AddListener(()  => Debug.Log("사용 불가"));
    upgradeButton.onClick.AddListener(()       => OnUpgrade?.Invoke());

    beforeButton.onClick.AddListener(() => OnBeforeSkin?.Invoke());
    nextButton.onClick.AddListener(() => OnNextSkin?.Invoke());

  }

  public void SetActiveSkeleton(bool isActive)
  {
    skeletonCamera.SetActive(isActive);
    skeletonRoot.SetActive(isActive);
  }

  public void SetCartActive(WagonType wagonType)
  {
    for (int i = 0; i < wagons.Length; i++)
    {
      bool isMatch = wagons[i].wagonType == wagonType;

      wagons[i].wagonObject.SetActive(isMatch);
    }
  }

  public void SetCartName(string cartName)
  {
    cartNameText.text = LanguageTable.getInstance.GetLanguage(cartName);
  }

  public void SetCartLvRange(string lvRange)
  {
    cartLvRange.text = lvRange;
  }

  public void SetCartLv(int curLv, int nextLv)
  {
    curLvText.text = curLv.ToString();
    nextLvText.text = nextLv.ToString();
  }

  public void SetCartReward(float curReward, float nextReward)
  {
    curRewardText.text = $"+{curReward}%";
    nextRewardText.text = $"+{nextReward}%";
  }

  public void SetCartHeath(float curHeath, float nextHealth)
  {
    curHeathText.text = $"+{curHeath}%";
    nextHeathText.text = $"+{nextHealth}%";
  }

  public void SetCartAttack(float curAttack, float nextAttack)
  {
    curAttackText.text = $"+{curAttack}%";
    nextAttackText.text = $"+{nextAttack}%";
  }

  public void SetCartDefense(float curDefense, float nextDefense)
  {
    curDefenseText.text = $"+{curDefense}%";
    nextDefenseText.text = $"+{nextDefense}%";
  }

  
  public void SetBundleButton(ButtonState buttonState)
  {
    availableButton.gameObject.SetActive(buttonState == ButtonState.Available);
    usingButton.gameObject.SetActive(buttonState == ButtonState.Using);
    notAvailableButton.gameObject.SetActive(buttonState == ButtonState.NotAvailable);
  }

  public void SetBundleGaugeText(bool isMaxLv, int curValue, int targetValue)
  {
    if(isMaxLv)
      bundleGaugeText.SetCustomData(1, "Max");
    else
      bundleGaugeText.SetGaugeTextData(curValue, targetValue);
  }

  public void EnableCurrency()
  {
    bundleCurrency.Enable();

  }
  public void DisableCurrency()
  {
    bundleCurrency.Disable();
  }
}
