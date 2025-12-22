using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewResourceManager : LazySingleton<NewResourceManager>
{
  [SerializeField] private SerializableDictionary<string, Object> cachedResources = new SerializableDictionary<string, Object>();
  [SerializeField] private SerializableDictionary<string, int> cachedReferences = new SerializableDictionary<string, int>();

  public AudioClip LoadAudioClip(string path, string audioName)
    => LoadResource<AudioClip>(GetResourcePath(path, audioName));

  public Sprite LoadMapSprite(string mapName)
    => LoadResource<Sprite>(GetResourcePath(NewResourcePath.PATH_MAP, mapName));

  public Sprite LoadItemGradeSprite(int grade)
    => LoadResource<Sprite>(string.Format(NewResourcePath.PATH_UI_ICON_ITEM_GRADE, grade,((ItemGradeType)grade).ToString().ToLower()));

  public Sprite LoadGradeSpirte(int grade)
    => LoadResource<Sprite>(string.Format(NewResourcePath.PATH_UI_ICON_GRADE, ((GradeType)grade).ToString().ToLower()));

  public Sprite LoadResourceSkin(string skinName)
    => LoadSprite(NewResourcePath.PATH_UI_ICON_COSTUME, skinName);

  public Sprite LoadItemSprite(ItemGroup itemGroup, string resourceName) => itemGroup switch
  {
    ItemGroup.Equipment => LoadSprite(NewResourcePath.PATH_UI_ICON_EQUIPMENT, resourceName),
    ItemGroup.Partner   => LoadSprite(NewResourcePath.PATH_UI_ICON_PARTNER  , resourceName),
    ItemGroup.Skill     => LoadSprite(NewResourcePath.PATH_UI_ICON_SKILL    , resourceName),
    ItemGroup.Creature  => LoadSprite(NewResourcePath.PATH_UI_ICON_CREATURE , resourceName),

    ItemGroup.Gold 
    or ItemGroup.Gem 
    or ItemGroup.AGExp
    or ItemGroup.Equipment
    or ItemGroup.Ticket_Partner
    or ItemGroup.Ticket_Skill
    or ItemGroup.RandomBoxGold
    or ItemGroup.RandomBoxSilver
                        => LoadSprite(NewResourcePath.PATH_UI_ICON, resourceName),

    _ => LoadSprite(NewResourcePath.PATH_UI_ICON, resourceName),

  };

  /// <summary>
  /// 가장 기본적인 Sprite Load 함수
  /// </summary>
  /// <param name="path"></param>
  /// <param name="resourceName"></param>
  /// <returns></returns>
  public Sprite LoadSprite(string path, string resourceName)
  {
    if (string.IsNullOrEmpty(resourceName))
      return LoadResource<Sprite>(NewResourcePath.ICON_NULL);

    Sprite sprite = LoadResource<Sprite>(GetResourcePath(path, resourceName));

    if (sprite == null)
      return LoadResource<Sprite>(NewResourcePath.ICON_NULL);

    return sprite;
  }



  private string GetResourcePath(string path, string resourceName)
    => string.Format("{0}{1}", path, resourceName);

  /// <summary>
  /// 리소스 형식인지, 에셋번들인지, Addressable인지에 따라 코드 수정 필요 지금은 단순 Resource를 불러오는 방식
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="resourceName"></param>
  /// <returns></returns>
  public T LoadResource<T>(string resourceName) where T : Object
  {
    T obj = null;

    ///리소스가 캐싱 되어 있다면
    if (cachedResources.ContainsKey(resourceName))
    {
      obj = cachedResources[resourceName] as T;

      if (!cachedReferences.ContainsKey(resourceName))
      {
        cachedReferences[resourceName] = 0;
      }
      cachedReferences[resourceName]++;
    }
    ///
    else
    {
      obj = Resources.Load<T>(resourceName);

      if (obj != null)
      {
        cachedResources.Add(resourceName, obj);
        cachedReferences.Add(resourceName, 1);
      }
    }

    return obj;
  }

  public void UnloadResource<T>(string resourceName) where T : Object
  {
    if (cachedReferences.ContainsKey(resourceName))
    {
      cachedReferences[resourceName]--;

      if (cachedReferences[resourceName] <= 0)
      {
        Resources.UnloadAsset(cachedResources[resourceName]);
        cachedResources.Remove(resourceName);
        cachedReferences.Remove(resourceName);

        //cachedReferences 삭제 가능성 높음 일일히 UnLoadResource를 해줘야되니까
        //Resources.UnloadUnusedAssets(); // 필요 시 메모리 해제 , 이 부분은 씬 전환시 두개 같이 쓰면될듯
        //System.GC.Collect()
      }
    }
  }

  public bool IsResourceInUse(string resourceName)
  {
    return cachedReferences.ContainsKey(resourceName) && cachedReferences[resourceName] > 0;
  }

  
}

public class NewResourcePath
{

  #region Path
  public const string PATH_SKILL_PREFAB   = "Prefabs/FantasyMercenary/Effect/Skill/Skill{0}";
  
  public const string PATH_PARTNER_PREFAB = "Prefabs/FantasyMercenary/Unit/Partner/Partner_{0}";
  public const string PATH_UI_SPINE_PARTNER_PREFAB = "Prefabs/FantasyMercenary/UI/Unit/Partner/Partner_{0}_Contents";

  public const string PATH_SOUND_BGM = "Sounds/BGM/";
  public const string PATH_SOUND_SFX = "Sounds/SFX/";
  public const string PATH_SOUND_UI_SFX = "Sounds/UI_SFX/";

  public const string PATH_MAP = "Map/";



  #endregion
  public const string ICON_NULL = "UIAsset/FantasyMercenary/NullDataIcon";

  public const string PATH_UI_ICON = "UIAsset/FantasyMercenary/Fm_Icon/";
  public const string PATH_UI_ICON_BUTTON = "UIAsset/FantasyMercenary/Fm_Button/";

  public const string PATH_UI_ICON_CURRENCY = "UIAsset/FantasyMercenary/Fm_Icon/fm_{0}_icon";

  public const string PATH_UI_ICON_COMMON = "UIAsset/FantasyMercenary/Fm_common/";

  public const string PATH_UI_ICON_COSTUME = "UIAsset/FantasyMercenary/Fm_costume/";

  





  public const string PATH_UI_ICON_SKILL     = "UIAsset/FantasyMercenary/Fm_Item/Icon_Skill/";
  public const string PATH_UI_ICON_EQUIPMENT = "UIAsset/FantasyMercenary/Fm_Item/Icon_Equipment/";
  public const string PATH_UI_ICON_PARTNER   = "UIAsset/FantasyMercenary/Fm_Item/Icon_Partner/";
  public const string PATH_UI_ICON_CREATURE   = "UIAsset/FantasyMercenary/Fm_Item/Icon_Creature/";


  public const string PATH_UI_ICON_ITEM_GRADE = "UIAsset/FantasyMercenary/Fm_grade/fm_itemslot_{0}_{1}";
  public const string PATH_UI_ICON_GRADE = "UIAsset/FantasyMercenary/Fm_grade/fm_rating_{0}";

  public const string PATH_UI_JOB_ICON = "UIAsset/FantasyMercenary/Fm_job/";
  public const string PATH_UI_ICON_PRYAERCENTER = "UIAsset/FantasyMercenary/Fm_contents/Prayer/";
  public const string PATH_UI_ICON_MERCHANT_GUILD = "UIAsset/FantasyMercenary/Fm_contents/MerchantGuild/";
  public const string PATH_UI_ICON_HOPE_CENTER = "UIAsset/FantasyMercenary/Fm_contents/HopeCenter/";
  public const string PATH_UI_ICON_BREEDING_GROUND = "UIAsset/FantasyMercenary/Fm_contents/BreedingGround/";



  #region Battle
  public const string PATH_UI_BATTLE = "UIAsset/FantasyMercenary/Fm_battle/fm_{0}_hp_gauge";
  #endregion

  #region Prefab
  #region Item Slot

  public const string PREFAB_UI_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot";
  public const string PREFAB_UI_ITEM_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot/ItemSlot";
  public const string PREFAB_UI_INVEN_DATA_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot/InvenDataSlot";

  public const string PREFAB_UI_POPUP_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot/PrefabUISlot";

  public const string PREFAB_UI_ITEM_GRADE = "Prefabs/FantasyMercenary/UI/ItemSlot/UISpineGrade";
  public const string PREFAB_UI_ITEM_SELECT = "Prefabs/FantasyMercenary/UI/ItemSlot/UISpineSelect";


  public const string PREFAB_UI_EQUIPMENT_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot/EquipmentUISlot";


  public const string PREFAB_UI_MAILBOX_SLOT = "Prefabs/FantasyMercenary/Popup/Main/Mailbox/MailSlot";
  public const string PREFAB_UI_MAILBOX_ITEM_SLOT = "Prefabs/FantasyMercenary/UI/ItemSlot/MailItemSlot";


  #endregion


  public const string PREFAB_WAGON_PATH = "Prefabs/FantasyMercenary/Unit/Wagon/{0}Wagon";

  public const string PREFAB_SOUND_BGM = "Prefabs/FantasyMercenary/Sound/BGMSound";
  public const string PREFAB_SOUND_SFX = "Prefabs/FantasyMercenary/Sound/SFXSound";


  #region Prefab PopUp

  public const string PREFAB_UI_POPUP_NOTIFICATION = "FantasyMercenary/Popup/Notification/NotificationPopup";
  public const string PREFAB_UI_POPUP_CONSUME_NOTIFICATION = "FantasyMercenary/Popup/Notification/NotificationConsumePopup";
  public const string PREFAB_UI_POPUP_TOGGLE_NOTIFICATION = "FantasyMercenary/Popup/Notification/NotificationTogglePopup";

  public const string PREFAB_UI_POPUP_ENHANCE_ITEM_REWARD = "FantasyMercenary/Popup/Reward/ItemEnhancePopup";
  public const string PREFAB_UI_POPUP_COMBINE_ITEM_REWARD = "FantasyMercenary/Popup/Reward/ItemCombinePopup";
  public const string PREFAB_UI_POPUP_GACHA_ITEM_REWARD = "FantasyMercenary/Popup/Reward/ItemGachaPopup";
  public const string PREFAB_UI_POPUP_ITEM_REWARD = "FantasyMercenary/Popup/Reward/ItemRewardPopup";
  public const string PREFAB_UI_POPUP_MAIL_ITEM_REWARD = "FantasyMercenary/Popup/Reward/MailRewardPopup";



  public const string PREFAB_UI_POPUP_TOASTMESSAGE = "FantasyMercenary/Popup/ToastMessagePopup"; //토스트 메시지 팝업
  public const string PREFAB_UI_POPUP_ITEM_INFO = "FantasyMercenary/Popup/ItemInfoPopup";
  public const string PREFAB_UI_POPUP_LOBBY_TEST = "FantasyMercenary/Popup/LobbyTestPopup";

  #region Main UI 버튼 
  //좌측 하단
  public const string PREFAB_UI_POPUP_EQUIPMENT = "FantasyMercenary/Popup/Main/EquipmentUIPopup";
  public const string PREFAB_UI_POPUP_JOB = "FantasyMercenary/Popup/Main/JobUIPopup";
  public const string PREFAB_UI_POPUP_SKILL = "FantasyMercenary/Popup/Main/SkillUIPopup";
  public const string PREFAB_UI_POPUP_PARTNER = "FantasyMercenary/Popup/Main/PartnerUIPopup";
  public const string PREFAB_UI_POPUP_WAGON = "FantasyMercenary/Popup/Main/WagonUIPopup";
  public const string PREFAB_UI_POPUP_ITEM_SHOP = "FantasyMercenary/Popup/Main/ShopUIPopup";

  //우측 하단
  public const string PREFAB_UI_POPUP_CHALLENGE = "FantasyMercenary/Popup/Main/ChallengeUIPopup";

  //우측 상단
  public const string PREFAB_UI_POPUP_MAILBOX = "FantasyMercenary/Popup/Main/MailboxUIPopup";

  public const string PREFAB_UI_POPUP_PLAYER_STAT = "FantasyMercenary/Popup/Main/PlayerStatUIPopup";

  #endregion

  #region Equipment Main UI 관련 팝업
  public const string PREFAB_UI_POPUP_EQUIPMENT_COSTUME = "FantasyMercenary/Popup/Equipment/EquipmentCostumePopup";
  public const string PREFAB_UI_POPUP_EQUIPMENT_GACHA_SETTING = "FantasyMercenary/Popup/Equipment/EquipmentAutoGachaSettingPopup";
  public const string PREFAB_UI_POPUP_EQUIPMENT_AUTO_GACHA_LIST_POPUP = "FantasyMercenary/Popup/Equipment/EquipmentAutoGachaListPopup";
  public const string PREFAB_UI_POPUP_EQUIPMENT_ITEM_INFO = "FantasyMercenary/Popup/Equipment/EquipmentItemUIPopup";
  public const string PREFAB_UI_POPUP_EQUIPMENT_ITEM_COMPARE_INFO = "FantasyMercenary/Popup/Equipment/EquipmentItemCompareUIPopup";



  #endregion

  #region Village UI 팝업
  public const string PREFAB_UI_POPUP_VILLAGE = "FantasyMercenary/Popup/Main/VillageUIPopup";

  public const string PREFAB_UI_POPUP_VILLAGE_PRAYER_CENTER = "FantasyMercenary/Popup/Village/PrayerCenterUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_BLESSING_STATUE = "FantasyMercenary/Popup/Village/BlessingStatueUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_ADVENTURE_GUILD = "FantasyMercenary/Popup/Village/AdventureGuildUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_ITEM_CRAFT = "FantasyMercenary/Popup/Village/ItemCraftUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_PARTNER_RECRUITMENT = "FantasyMercenary/Popup/Village/RecruitmentUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_MERCHANT_GUILD = "FantasyMercenary/Popup/Village/MerchantGuildUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_HOPE_CENTER = "FantasyMercenary/Popup/Village/HopeCenterUIPopup";
  public const string PREFAB_UI_POPUP_VILLAGE_BREEDING_GROUND = "FantasyMercenary/Popup/Village/BreedingGroundUIPopup";







  #endregion

  #endregion


  #region 직업 관련 팝업 프리팹
  public const string PREFAB_UI_POPUP_JOB_SELECT_POPUP = "FantasyMercenary/Popup/Job/UIJobSelectPopup";
  public const string PREFAB_UI_POPUP_JOB_INFO = "FantasyMercenary/Popup/Job/UIJobInfoPopup";
  public const string PREFAB_UI_POPUP_JOB_ROADVIEW = "FantasyMercenary/Popup/Job/UIJobRoadViewPopup";
  #endregion

  #endregion


}
