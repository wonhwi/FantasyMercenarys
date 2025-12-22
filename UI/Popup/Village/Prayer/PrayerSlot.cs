using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using FantasyMercenarys.Data;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using Assets.ZNetwork.Data;
using static RestPacket;
using Newtonsoft.Json;

public class PrayerSlot : MonoBehaviour
{
  //해당 슬롯 현재 상태
  public PrayerSlotType prayerSlotType = PrayerSlotType.Empty;
  public PartnerSpineType partnerSpineType;

  public Button slotButton;   //슬롯 버튼
  public Transform spinePoint;  //UI Spine Parent
  public BundleGaugeText bundleGaugeText;
  public GameObject drainEffect;

  public Action<RES_BlessPrayerRcv> OnUpdateBless;

  private UISpineController uISpineController;

  private DateTime? dateTime;
  private bool isWorkingTask = false;

  private PrayerData prayerData;

  public BundleRewardPoint bundleRewardPoint;

  /// <summary>
  /// UI Spine 생성
  /// </summary>
  /// <param name="partnerIdx"></param>
  public void SetSpine(int partnerIdx)
  {
    PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(partnerIdx);

    PartnerSpineType spineType = (PartnerSpineType)partnerData.groupSpine;

    this.partnerSpineType = spineType;

    this.uISpineController = UIPoolManager.Instance.GetPool<UISpineController>(spineType);
    
    this.uISpineController.OnSetParent(this.spinePoint);
    this.uISpineController.SetAnimation("Pray");
    this.uISpineController.SetSkin(partnerData.partnerSpine);
    this.uISpineController.transform.localScale = Vector3.one * 0.5f;
  }

  /// <summary>
  /// 체력에 따른 효과 적용
  /// </summary>
  /// <param name="prayerData"></param>
  public void SetData(PrayerData prayerData)
  {
    this.prayerData = prayerData;

    if(prayerData.hp > 0)
    {
      this.SetSlot(PrayerSlotType.Using);
      this.uISpineController?.SetColor(Color.white);

      drainEffect.SetActive(false);
    }
    else
    {
      this.SetSlot(PrayerSlotType.Drain);
      this.uISpineController?.SetColor(Color.gray);

      drainEffect.SetActive(true);
    }

  }

  public void SetBlessTime(DateTime? dateTime)
  {
    this.isWorkingTask = true;

    this.dateTime = dateTime;
  }

  /// <summary>
  /// 클라이언트 내 자체적으로 신앙심 갱신 API 호출
  /// </summary>
  /// <param name="dateTime"></param>
  /// <param name="token"></param>
  public void StartBlessUpdateTask(DateTime? dateTime , CancellationToken token)
  {
    SetBlessTime(dateTime);
    BlessUpdateTask(token);

  }

  private async void BlessUpdateTask(CancellationToken token)
  {
    try
    {
      while (true)
      {
        if (token.IsCancellationRequested)
        {
          break;
        }

        if (!isWorkingTask)
        {
          Debug.Log("WorkingTask 종료");
          break;
        }

        TimeSpan? diff = DateTime.Now - dateTime;


        //현재시간과 최근 갱신기간 차이가 1분이상 차이나면 Update API 실행
        //따로 종료가 없는 이유는 밑에 API 실행 이후 dateTime을 업데이트 해주기 때문
        if (diff.Value.TotalMinutes >= 1)
        {
          await APIManager.getInstance.REQ_BlessPrayerRcv<RES_BlessPrayerRcv>((responseResult) =>
          {
            OnUpdateBless?.Invoke(responseResult);
            bundleRewardPoint.SetPoint(PartnerTable.getInstance.GetPartnerFaithMin(prayerData.partnerIdx, prayerData.lv));
          });
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
      }
    }
    catch (OperationCanceledException)
    {
      // 토큰에 의한 취소로 인해 OperationCanceledException 예외가 발생
      Debug.Log("OperationCanceledException 예외가 발생하여 작업을 종료합니다.");
    }
    catch (ObjectDisposedException)
    {
      // 토큰이 Dispose된 후 접근하여 ObjectDisposedException 예외가 발생
      Debug.Log("ObjectDisposedException 예외가 발생하여 작업을 종료합니다.");
    }
    catch (Exception ex)
    {
      // 기타 발생 가능한 예외에 대해 에러 로그 출력
      Debug.LogError("ClassB: 예외 발생 - " + ex.Message);
    }
  }

  /// <summary>
  /// 체력바 UI 설정
  /// </summary>
  /// <param name="prayerData"></param>
  public void SetGaugeText(PrayerData prayerData)
  {
    if (!prayerData.IsNull())
    {
      bundleGaugeText.SetGaugeTextData(prayerData.hp, prayerData.maxHp);
    }
  }

  public void CancelTask()
  {
    isWorkingTask = false;
  }

  /// <summary>
  /// 슬롯 설정
  /// </summary>
  public void SetSlot(PrayerSlotType slotType)
  {
    this.prayerSlotType = slotType;

    slotButton.image.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_PRYAERCENTER, GetPrayerSlotImageName(slotType));
  }


  public void ClearPool()
  {
    CancelTask();

    bundleGaugeText.ActiveBundle(false);
    drainEffect.SetActive(false);

    uISpineController = null;
    prayerData = default;

    UIPoolManager.Instance.ClearReturnUIPool<UISpineController>(partnerSpineType, spinePoint);
  }


  public string GetPrayerSlotImageName(PrayerSlotType slotType) => slotType switch
  {
    PrayerSlotType.Empty => "fm_prayer_cha_panel_slot",
    PrayerSlotType.Using => "fm_prayer_cha_panel_on",
    PrayerSlotType.Drain => "fm_prayer_cha_panel_off",
    PrayerSlotType.Lock  => "fm_prayer_cha_panel_lock",
  };
}
