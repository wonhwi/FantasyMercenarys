using Assets.ZNetwork;
using Assets.ZNetwork.Data;
using Cysharp.Threading.Tasks;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static RestPacket;

/// <summary>
/// 단순 Request 시 async void 사용
/// Response 받아야 할때는 async UniTask 형태로 await 이후 데이터 기반 후 작업
/// </summary>
public class APIManager : LazySingleton<APIManager>
{
  public async UniTask<T> REQ_AuthLogin<T>(int platformType, string platformId, string countryCd, string platformToken) where T : RestResponseBase
  {
    REQ_AuthLogin requestPacket = new REQ_AuthLogin();

    requestPacket.platformType = platformType;
    requestPacket.platformId = platformId;
    requestPacket.countryCd = countryCd;
    requestPacket.plaftformToken = platformToken;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.authLogin, requestPacket);

    return responseResult;
  }


  #region 장비 Gacha
  /// <summary>
  /// 램프 장비 뽑기 [ProtocolType = 8]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_LampGacha<T>(REQ_LampGacha requestPacket, UnityAction<T> action) where T : RestResponseBase
  {
    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.lampGacha, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 램프 가챠 뽑기 시작 [ProtocolType = 45]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_LampGachaStart<T>(int useLampTicket, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_LampGachaStart requestPacket = new REQ_LampGachaStart();

    requestPacket.useLampTicket = useLampTicket;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.lampGachaStart, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 램프 가챠 아이템 획득 및 판매 [ProtocolType = 46]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_LampGachaItemUpdate<T>(REQ_LampGachaItemUpdate requestPacket, UnityAction<T> action = null) where T : RestResponseBase
  {
    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.lampGachaItemUpdate, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 램프 가챠 종료 [ProtocolType = 47]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_LampGachaEnd<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_LampGachaEnd requestPacket = new REQ_LampGachaEnd();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.lampGachaEnd, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 레벨업, 레벨업 시 콘텐츠 잠금 해제 API 모음
  /// <summary>
  /// 유저 레벨업
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_UserLevelUp<T>(UnityAction action) where T : RestResponseBase
  {
    REQ_UserLevelUp requestPacket = new REQ_UserLevelUp();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.userLevelUp, requestPacket);

    if (responseResult.result.IsSuccess())
    {
      action?.Invoke();

      UpdateSlotUnlock();
    }
  }


  private void UpdateSlotUnlock()
  {
    int playerLv = GameDataManager.getInstance.userInfoModel.GetPlayerLv();

    //내 현재 레벨과 동일한 Value들을 반환
    var slotUnlockDataList = SlotUnlockConditionTable.getInstance.GetSlotUnlockConditionDataList(SlotUnlockConditionType.Level)
      .Where(n => n.unlockValue == playerLv);

    foreach (var slotUnlockData in slotUnlockDataList)
    {
      switch ((SlotUnlockContentType)slotUnlockData.slotType)
      {
        case SlotUnlockContentType.Skill:
          break;
        case SlotUnlockContentType.Partner:
          break;
        case SlotUnlockContentType.Prayer:
          REQ_PrayerSlotOpen<RES_PrayerSlotOpen>(slotUnlockData.slotNumber, null).Forget();
          break;
          //얘는 슬롯을 클릭해서 API를 호출하는 것이기 때문에 따로 API 호출할 필요 없음
        case SlotUnlockContentType.BlessingStatueSlot:
          break;
      }
    }
  }

  public async UniTask REQ_PrayerSlotOpen<T>(int openSlot, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_PrayerSlotOpen requestPacket = new REQ_PrayerSlotOpen();

    requestPacket.openSlot = openSlot;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.prayerSlotOpen, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  public async UniTask REQ_BlessingStatueSlotOpen<T>(int unlockPresetNo, int unlockSlotNo, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_BlessingStatueSlotOpen requestPacket = new REQ_BlessingStatueSlotOpen();
    requestPacket.unlockPresetNo = unlockPresetNo;
    requestPacket.unlockSlotNo   = unlockSlotNo;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.blessingStatueSlotOpen, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion


  /// <summary>
  /// 상점 아이템 구매 [ProtocolType = 10]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="shopItemIdx"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BuyShopItem<T>(int shopItemIdx, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_BuyShopItem requestPacket = new REQ_BuyShopItem();

    requestPacket.shopItemIdx = shopItemIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.buyShopItem, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  #region 아이템 장착, 강화, 합성, 코스튬
  /// <summary>
  /// 아이템 장착(동료, 스킬) [ProtocolType = 12]
  /// 팝업 닫을때 실행시켜주는 함수
  /// 이 친구는 Response패킷으로 추가 작업을 하는게 없고 서버로 패킷만 쏴주면 되는 부분이라 사용하는 클래스에서 Await삭제
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="presetNum"></param>
  /// <param name="mountType"></param>
  /// <param name="mountSlotList"></param>
  /// <param name="action"></param>
  public async void REQ_MountItem<T>(int presetNum, int mountType, List<MountPacketData> mountSlotList, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_MountItem requestPacket = new REQ_MountItem();
    requestPacket.presetNum = 1;
    requestPacket.mountType = mountType;
    requestPacket.mountSlotList = mountSlotList;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mountItem, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 아이템 강화(동료, 스킬) [ProtocolType = 13]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="groupType"></param>
  /// <param name="itemList"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_EnhanceItem<T>(int groupType, List<int> itemList , UnityAction<T> action) where T : RestResponseBase
  {
    REQ_Enhancement requestPacket = new REQ_Enhancement();
    requestPacket.groupType = groupType;
    requestPacket.itemList = itemList;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.enhancement, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 아이템 합성(동료, 스킬) [ProtocolType = 14] 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="groupType"></param>
  /// <param name="itemList"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_CombineItem<T>(int groupType, List<int> itemList, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_Combine requestPacket = new REQ_Combine();
    requestPacket.groupType = groupType;
    requestPacket.itemList = itemList;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.combine, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  public async void REQ_CostumeChange<T>(int mountType, int itemIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_CostumeChange requestPacket = new REQ_CostumeChange();
    requestPacket.mountType = mountType;
    requestPacket.itemIdx = itemIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.costumeChange, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  #endregion



  #region 직업 전직, 초기화
  /// <summary>
  /// 직업 전직 [ProtocolType = 16]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="jobCode"></param>
  /// <param name="action"></param>
  public async void REQ_JobChange<T>(int jobCode, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_JobChange requestPacket = new REQ_JobChange();

    requestPacket.jobCode = jobCode;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.jobChange, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }


  /// <summary>
  /// 직업 초기화 [ProtocolType = 17]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  public async void REQ_JobReset<T>(UnityAction<T> action) where T : RestResponseBase
  {
    REQ_JobReset requestPacket = new REQ_JobReset();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.jobReset, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }
  #endregion

  #region 일일 팝업 정보
  public async void REQ_RemoveDailyPopupInfo<T>(DailyPopupType popupType, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_RemoveDailyPopupInfo requestPacket = new REQ_RemoveDailyPopupInfo();
    
    requestPacket.popupType = (int)popupType;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.removeDailyPopupInfo, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 기도원
  /// <summary>
  /// 파트너 기도원 배치 [ProtocolType = 20]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="slotNum"></param>
  /// <param name="invenIdx"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MountPrayerSlot<T>(int slotNum, long invenIdx, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_MountPrayerSlot requestPacket = new REQ_MountPrayerSlot();

    requestPacket.slotNum = slotNum;
    requestPacket.invenIdx = invenIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mountPrayerSlot, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  /// <summary>
  /// 기도원 신성력 획득 [ProtocolType = 21]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BlessPrayerRcv<T>(UnityAction<T> action) where T : RestResponseBase
  {
    REQ_BlessPrayerRcv requestPacket = new REQ_BlessPrayerRcv();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.blessPrayerRcv, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  /// <summary>
  /// 파트너 체력 등 갱신 [ProtocolType = 27]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="isPrayer"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_RefreshPartnerList<T>(bool isPrayer, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_RefreshPartnerList requestPacket = new REQ_RefreshPartnerList();
    requestPacket.isPrayer = isPrayer;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.refreshPartnerList, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }
  #endregion

  #region 가호 석상
  /// <summary>
  /// 가호 석상 축복 실행
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_UseBlessing<T>(int usePresetNumber, UnityAction<T> action) where T : RestResponseBase
  {

    REQ_UseBlessing requestPacket = new REQ_UseBlessing();
    requestPacket.usePresetNumber = usePresetNumber;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.useBlessing, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  /// <summary>
  /// 가호 및 기도 로드
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_LoadBlessingAndPrayer<T>(UnityAction<T> action) where T : RestResponseBase
  {
    REQ_LoadBlessingAndPrayer requestPacket = new REQ_LoadBlessingAndPrayer();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.loadBlessingAndPrayer, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  public async UniTask REQ_BlessingStatuePresetUpdate<T>(int type, int presetNo, int slotNo, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_BlessingStatuePresetUpdate requestPacket = new REQ_BlessingStatuePresetUpdate();

    requestPacket.type     = type;
    requestPacket.presetNo = presetNo;
    requestPacket.slotNo   = slotNo;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.blessingStatuePresetUpdate, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  #endregion

  #region 모험가 길드
  public async UniTask REQ_AdventureGuildQuestStatusEditor<T>(int questOrder, int updatedStatus, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_AdventureGuildQuestStatusEditor requestPacket = new REQ_AdventureGuildQuestStatusEditor();

    requestPacket.questOrder    = questOrder;
    requestPacket.updatedStatus = updatedStatus;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.adventureGuildQuestStatusEditor, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  public async UniTask REQ_AdventureGuildQuestReward<T>(int questOrder, UnityAction<T> action) where T : RestResponseBase
  {
    REQ_AdventureGuildQuestReward requestPacket = new REQ_AdventureGuildQuestReward();

    requestPacket.questOrder = questOrder;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.adventureGuildQuestReward, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  public async UniTask REQ_AdventureGuildQuestValueRefresh<T>() where T : RestResponseBase
  {
    REQ_AdventureGuildQuestRefresh requestPacket = new REQ_AdventureGuildQuestRefresh();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.adventureGuildQuestValueRefresh, requestPacket);

  }
  #endregion

  #region 동료, 스킬 제작소
  public async UniTask REQ_PartnerSkillCraftService<T>(int itemGroupType, int slotNo, int craftItemIdx, bool isCrafting, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_PartnerSkillCraftService requestPacket = new REQ_PartnerSkillCraftService();
    
    requestPacket.itemGroupType = itemGroupType;
    requestPacket.slotNo = slotNo;
    requestPacket.craftItemIdx = craftItemIdx;
    requestPacket.isCrafting = isCrafting;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.partnerSkillCraftService, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }
  
  public async UniTask REQ_PartnerSkillCraftComplete<T>(int itemGroupType, int slotNo, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_PartnerSkillCraftComplete requestPacket = new REQ_PartnerSkillCraftComplete();

    requestPacket.itemGroupType = itemGroupType;
    requestPacket.slotNo = slotNo;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.partnerSkillCraftComplete, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 동료 모집소
  public async UniTask REQ_RecruitmentStart<T>(int slotNo, IEnumerable<long> mountPartnerList, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_RecruitmentStart requestPacket = new REQ_RecruitmentStart();

    requestPacket.slotNo = slotNo;
    requestPacket.mountPartnerList = mountPartnerList;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.recruitmentStart, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);

  }

  public async UniTask REQ_RecruitmentUpdateStatus<T>(int slotNo, int updateStatus, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_RecruitmentUpdateStatus requestPacket = new REQ_RecruitmentUpdateStatus();

    requestPacket.slotNo = slotNo;
    requestPacket.updateStatus = updateStatus;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.recruitmentUpdateStatus, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 상인 협회

  /// <summary>
  /// 로그인 시, 팝업 창 내에서 연구 시간 다 끝날떄까지 기다렸을때 실행 [41]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MerchantGuildSkillTreeLoad<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MerchantGuildSkillTreeLoad requestPacket = new REQ_MerchantGuildSkillTreeLoad();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.merchantGuildSkillTreeLoad, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 상인협회 스킬 업그레이드 [42]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MerchantGuildSkillTreeUpgrade<T>(int skillIdx, int skillLv, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MerchantGuildSkillTreeUpgrade requestPacket = new REQ_MerchantGuildSkillTreeUpgrade();
    requestPacket.skillIdx = skillIdx;
    requestPacket.skillLv = skillLv;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.merchantGuildSkillTreeUpgrade, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  #endregion

  #region 희망센터

  /// <summary>
  /// 희망센터 정보 로드 [43]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_HopeCenterLoad<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_HopeCenterLoad requestPacket = new REQ_HopeCenterLoad();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.hopeCenterLoad, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }


  /// <summary>
  /// 희망센터 복권 긁기 [44]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="lotteryIdx"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_HopeCenterRcv<T>(int lotteryIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_HopeCenterRcv requestPacket = new REQ_HopeCenterRcv();
    requestPacket.lotteryIdx = lotteryIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.hopeCenterRcv, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 사육장
  /// <summary>
  /// 사육장 정보 로드 [48]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BreedingGroundsLoad<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_BreedingGroundsLoad requestPacket = new REQ_BreedingGroundsLoad();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.breedingGroundsLoad, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 사육장 슬롯 오픈 [49]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BreedingSlotOpen<T>(int slotNo, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_BreedingSlotAction requestPacket = new REQ_BreedingSlotAction();

    requestPacket.actionType = 1;
    requestPacket.slotNo = slotNo;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.breedingSlotAction, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 사육장 슬롯 장착/해제 [49]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BreedingSlotMount<T>(int slotNo, int creatureIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_BreedingSlotAction requestPacket = new REQ_BreedingSlotAction();

    requestPacket.actionType = 2;
    requestPacket.slotNo = slotNo;
    requestPacket.creatureIdx = creatureIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.breedingSlotAction, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 사육장 포인트 획득 [49]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BreedingPointGain<T>(int addType, int addValue, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_BreedingPointGain requestPacket = new REQ_BreedingPointGain();

    requestPacket.addType = addType;
    requestPacket.addValue = addValue;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.breedingPointGain, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 사육장 업그레이드(금고) [50]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_BreedingGroundsUpgrade<T>(int updateLv, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_BreedingGroundsUpgrade requestPacket = new REQ_BreedingGroundsUpgrade();

    requestPacket.updateLv = updateLv;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.breedingGroundsUpgrade, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  #endregion

  #region 수레
  /// <summary>
  /// 마차 코스튬 변경 [61]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="changeCartIdx">변경할 마차 idx . 0으로 보내면  지금 레벨의 마차 모습</param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_CartChangeCostume<T>(int changeCartIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_CartChangeCostume requestPacket = new REQ_CartChangeCostume();

    requestPacket.changeCartIdx = changeCartIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.cartChangeCostume, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 마차 업그레이드 [62]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="upgradeLv">업그레이드 마차 레벨</param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_CartUpgrade<T>(int upgradeLv, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_CartUpgrade requestPacket = new REQ_CartUpgrade();

    requestPacket.upgradeLv = upgradeLv;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.cartUpgrade, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  #endregion

  #region 우편함
  /// <summary>
  /// 메일 로드 [63]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MailLoad<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MailLoad requestPacket = new REQ_MailLoad();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mailLoad, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 메일 보상 전체 수령 [64]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_ClaimAllMail<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MailRewardRcv requestPacket = new REQ_MailRewardRcv();

    requestPacket.isBulkReceive = true;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mailRewardRcv, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 메일 보상 개별 수령 [64]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="mailIdx"></param>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_ClaimMail<T>(long mailIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MailRewardRcv requestPacket = new REQ_MailRewardRcv();

    requestPacket.isBulkReceive = false;
    requestPacket.mailIdx = mailIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mailRewardRcv, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 메일 삭제 [65]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MailDelete<T>(UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MailDelete requestPacket = new REQ_MailDelete();

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mailDelete, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }

  /// <summary>
  /// 메일 상세 보기 [66]
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="action"></param>
  /// <returns></returns>
  public async UniTask REQ_MailDetail<T>(long mailIdx, UnityAction<T> action = null) where T : RestResponseBase
  {
    REQ_MailDetail requestPacket = new REQ_MailDetail();

    requestPacket.mailIdx = mailIdx;

    T responseResult = await ZNetworkManager.Instance.SendAsyncRestObject<T>(RestProtocolType.mailDetail, requestPacket);

    if (responseResult.result.IsSuccess())
      action?.Invoke(responseResult);
  }
  #endregion

}
