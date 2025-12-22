using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FantasyMercenarys.Data;

public class ShopUIView : MonoBehaviour, ICurrencyUpdateUsable
{
  private readonly string GACHA_ANIMATION     = "card_pick";
  private readonly string OPEN_ANIMATION      = "card_idle";
  private readonly string IDLE_LOOP_ANIMATION = "card_loop";
  private readonly string EFFECT_GACHA_ANIMATION = "draw_result";

  [Header("[Class Component]")]
  [SerializeField] private BundleCurrency bundleCurrency;          //우측 상단 재화 정보
  [SerializeField] private BundleContentLvInfo bundleContentInfo;  //하단 상점 레벨 정보

  [SerializeField] private BundleGachaInfo bundleGachaInfo;        //뽑기 확률 정보
  [SerializeField] private BundleGachaButton bundleGachaButton;    //뽑기 관련 버튼 정보

  [SerializeField] public ShopCategorySlot[] slotCategorySlots;

  [SerializeField] private UISkeletonAnimation UISpineController;
  [SerializeField] private UISkeletonAnimation UISpineGachaEffect;

  [Header("[UI Component]")]
  [SerializeField] private RectTransform selectImage;

  [SerializeField] private Button closeButton;

  [SerializeField] private GameObject blockObject;

  public Action OnClosePopup;
  public Action OnGachaComplete;    //뽑기 애니메이션 마무리 이후 아이템 획득 팝업 출력
  public Action OnActiveInfo;       //뽑기 확률 정보 출력


  private void Awake()
  {
    closeButton.onClick.AddListener(() =>
    {
      DisableCurrency();

      OnClosePopup?.Invoke();
    });

    bundleContentInfo.infoButton.onClick.AddListener(() =>
    {
      bundleGachaInfo.transform.parent.gameObject.SetActive(true);

      OnActiveInfo?.Invoke();
      
    });

    InitSpineEvent();
  }

  /// <summary>
  /// 스파인 애니메이션 이벤트
  /// </summary>
  private void InitSpineEvent()
  {
    //뽑기 애니메이션 실행 후 끝났을때 아래 함수 실행
    //뽑고 화려하게 연출되는 Spine Animation 실행
    //2번
    UISpineController.AddListenerHandleAnimationComplete((animationName) =>
    {
      if (animationName == GACHA_ANIMATION)
      {
        UISpineGachaEffect.gameObject.SetActive(true);
        UISpineGachaEffect.SetAnimation(EFFECT_GACHA_ANIMATION, loop: false);
      }
    });

    //화려하게 연출 되는 애니메이션이 끝나면 할당된 Action 실행
    //3번
    UISpineGachaEffect.AddListenerHandleAnimationComplete((animationName) =>
    {
      if (animationName == EFFECT_GACHA_ANIMATION)
      {
        UISpineGachaEffect.gameObject.SetActive(false);
        blockObject.SetActive(false);

        UISpineController.SetAnimation(IDLE_LOOP_ANIMATION);

        OnGachaComplete?.Invoke();
      }
    });
  }


  /// <summary>
  /// 상점 카테고리 진입 시 스파인 애니메이션 실행
  /// </summary>
  public void OnStartIdle()
  {
    UISpineController.SetAnimation(OPEN_ANIMATION, loop: false);
    UISpineController.AddAnimation(IDLE_LOOP_ANIMATION);
  }

  /// <summary>
  /// 뽑기 애니메이션 실행
  /// 1번
  /// </summary>
  public void OnStartGacha()
  {
    blockObject.SetActive(true);
    UISpineController.SetAnimation(GACHA_ANIMATION, loop: false);
  }


  public void ActiveContentInfo(bool isActive)
  {
    bundleContentInfo.gameObject.SetActive(isActive);
  }

  /// <summary>
  /// ShopItemData 기반 버튼들 설정
  /// </summary>
  /// <param name="shopItemDataList"></param>
  /// <param name="OnGachaShop"></param>
  public void SetBundleGachaButton(List<ShopItemData> shopItemDataList, Action<int> OnGachaShop)
  {
    bundleGachaButton.SetData(shopItemDataList, OnGachaShop);
  }

  /// <summary>
  /// 경험치 정보 쪽에 있는 Info 버튼 클릭시 실행되는 함수
  /// </summary>
  /// <param name="shopInfoData"></param>
  public void SetBundleGachaInfo(ShopInfoData shopInfoData)
  {
    bundleGachaInfo.SetData(shopInfoData);
  }

  /// <summary>
  /// 경험치 정보 출력
  /// </summary>
  /// <param name="shopInfoData"></param>
  public void SetBundleShopInfo(ShopInfoData shopInfoData)
  {
    if(shopInfoData != null)
      bundleContentInfo.SetData(shopInfoData);
  }

  /// <summary>
  /// 선택 Effect Object 위치 조절
  /// </summary>
  /// <param name="index"></param>
  public void SetSelectEffect(int index)
  {
    this.selectImage.parent = this.slotCategorySlots[index].transform;
    this.selectImage.localPosition = Vector2.zero;
  }

  /// <summary>
  /// 카테고리별 사용 재화가 다르기 때문에 좌측 카테고리 변경 시 마다 함수 호출
  /// </summary>
  /// <param name="itemIdx"></param>
  public void UpdateCurrency(int itemIdx)
  {
    bundleCurrency.UpdateCurrency(itemIdx);
  }

  public void DisableCurrency()
  {
    bundleCurrency.Disable();
  }
}
