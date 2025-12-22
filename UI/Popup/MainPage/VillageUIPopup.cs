using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RestPacket;

public class VillageUIPopup : UIBaseController
{
  public Button closeButton;

  [Header("Contents Button UI")]
  public Button prayerButton;
  public Button blessButton;
  public Button guildButton;
  public Button craftButton;
  public Button recruitButton;
  public Button merchantButton;
  public Button hopeCenterButton;
  public Button breedingGroundButton;



  protected override void Awake()
  {
    base.Awake();

    closeButton.onClick.AddListener(Hide);

    prayerButton.onClick.AddListener(DisplayPrayer);
    blessButton.onClick.AddListener(DisplayBlessStatue);
    guildButton.onClick.AddListener(DisplayAdventureGuild);
    craftButton.onClick.AddListener(DisplayItemCraft);
    recruitButton.onClick.AddListener(DisplayPartnerRecruitmentOffice);
    merchantButton.onClick.AddListener(DisplayMerchantGuild);
    hopeCenterButton.onClick.AddListener(DisplayHopeCenter);
    breedingGroundButton.onClick.AddListener(DisplayBreedingGround);
  }

  private async void DisplayPrayer()
  {
    await APIManager.getInstance.REQ_RefreshPartnerList<RES_RefreshPartnerList>(true);

    await APIManager.getInstance.REQ_BlessPrayerRcv<RES_BlessPrayerRcv>(OpenPrayer);

    async void OpenPrayer(RES_BlessPrayerRcv responseResult)
    {
      PrayerCenterUIPopup UIPopup = await NewUIManager.getInstance.Show<PrayerCenterUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_PRAYER_CENTER);

      UIPopup.Open(responseResult);
    }
  }

  private async void DisplayBlessStatue()
  {
    await APIManager.getInstance.REQ_BlessPrayerRcv<RES_BlessPrayerRcv>(null);

    BlessingStatueUIPopup UIPopup = await NewUIManager.getInstance.Show<BlessingStatueUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_BLESSING_STATUE);

    UIPopup.Open();

  }

  private async void DisplayAdventureGuild()
  {
    await APIManager.getInstance.REQ_AdventureGuildQuestValueRefresh<RES_AdventureGuildQuestValueRefresh>();

    AdventureGuildUIPopup UIPopup = await NewUIManager.getInstance.Show<AdventureGuildUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_ADVENTURE_GUILD);

    UIPopup.Open();
  }

  private async void DisplayItemCraft()
  {
    ItemCraftUIPopup UIPopup = await NewUIManager.getInstance.Show<ItemCraftUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_ITEM_CRAFT);

    UIPopup.Open();
  }

  private async void DisplayPartnerRecruitmentOffice()
  {
    RecruitmentUIPopup UIPopup = await NewUIManager.getInstance.Show<RecruitmentUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_PARTNER_RECRUITMENT);

    UIPopup.Open();
  }

  private async void DisplayMerchantGuild()
  {
    MerchantGuildUIPopup UIPopup = await NewUIManager.getInstance.Show<MerchantGuildUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_MERCHANT_GUILD);

    UIPopup.Open();

  }

  private async void DisplayHopeCenter()
  {
    HopeCenterUIPopup UIPopup = await NewUIManager.getInstance.Show<HopeCenterUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_HOPE_CENTER);

    UIPopup.Open();

  }

  private async void DisplayBreedingGround()
  {
    BreedingGroundUIPopup UIPopup = await NewUIManager.getInstance.Show<BreedingGroundUIPopup>(NewResourcePath.PREFAB_UI_POPUP_VILLAGE_BREEDING_GROUND);

    UIPopup.Open();

  }
}
