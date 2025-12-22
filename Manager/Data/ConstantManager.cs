using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantManager
{
  public const int DATA_NONE_INTEGER_VALUE = 0;
  public const string DATA_NONE_STRING_VALUE = "0";

  #region Base
  //스킬 레벨에 따른 데미지 계수 증가량을 참조하는 베이스 Index
  public const int BASE_INDEX_SKILL_LEVEL_DAMAGE_PERCENT = 100;

  //스텟에 따른 전투력 계산에 필요한 계수 값을 참조하는 베이스 Index
  public const int BASE_INDEX_STAT_POWER_MULTIPLY = 200;

  public const int BASE_INDEX_SKILL_ENHANCE_MULTIPLY = 311;
  public const int BASE_INDEX_PARTNER_ENHANCE_MULTIPLY = 312;

  public const int BASE_INDEX_BLEESING_STATUE_SLOT_BLESSING_COST = 511;

  public const int BASE_INDEX_ADVENTURE_GUILD_REFRESH_COOLTIME = 600;

  //해당 인덱스는 모험가 레벨을 더해줘 601부터 시작
  public const int BASE_INDEX_ADVENTURE_GUILD_LIMIT_QUEST = 600;

  public const int BASE_INDEX_BREEDING_CREATURE_TRACK_LENGTH = 802;
  public const int BASE_INDEX_BREEDING_CREATURE_REWARD_RESEARCHFUNDS = 804;



  public const int BASE_INDEX_BREEDING_CREATURE_CONDITION = 810;



  #endregion

  #region Job
  public const int JOB_FINAL_CHANGE_GRADE_VALUE = 5;

  //기본 직업 Index
  public const int JOB_DEFAULT_INDEX = 10001;

  public const int JOB_COMMON_ATTACK_SKILL_INDEX = 2001001;
  public const int PARTNER_COMMON_ATTACK_SKILL_INDEX = 4001001;


  #endregion

  #region Currency
  public const int ITEM_CURRENCY_GOLD = 90000001; //골드
  public const int ITEM_CURRENCY_GEM  = 90000002; //잼
  public const int ITEM_CURRENCY_FAITH = 90000004; //신앙심
  public const int ITEM_CURRENCY_RESEARCH_FUNDS = 90000006; //연구비
  public const int ITEM_CURRENCY_LUCK_SHARD_01 = 90000007; //복권재료1
  public const int ITEM_CURRENCY_LUCK_SHARD_02 = 90000008; //복권재료2
  public const int ITEM_CURRENCY_CART_ENHANCE = 90000009; //복권재료2

  public const int ITEM_CURRENCY_TICKET_EQUIPMENT = 80100000; //장비 뽑기 티켓
  public const int ITEM_CURRENCY_TICKET_PARTNER = 80200000; //동료 뽑기 티켓
  public const int ITEM_CURRENCY_TICKET_SKILL = 80300000; //스킬 뽑기 티켓
  public const int ITEM_CURRENCY_BOOST_TIME = 80400000; //시간 가속권





  public static readonly int[] CURRENCY_ITEM_GROUPS = new int[]
  {
    801,802,803,804,
    901,902,904,905,906,907
  };

  #endregion

  //값 비교시 색상
  public const string COMPARE_VALUE_MINUS_COLOR = "#FF3030";
  public const string COMPARE_VALUE_PLUS_COLOR  = "#00AA00";


  #region UI Font Sprite
  public const string TEXT_SPRITE_NAME = "<sprite name=fm_sprite_damage_0{0}_{1}>";
  public const string TEXT_MISS_SPRITE_NAME = "<sprite name=fm_sprite_damage_MISS>";

  #endregion

  #region Spine
  public const string DEFAULT_SKIN_NAME = "default_{0}";
  #endregion

  #region Animation

  public const string ANIMATION_UI_SPINE_REWARD_PLAY = "reward_play";
  public const string ANIMATION_UI_SPINE_REWARD_IDLE = "reward_idle";

  #endregion

  #region Item
  public const string ITEM_MAX_LEVEL_TEXT = "MAX";
  //아이템 최대 획득갯수?
  public const int ITEM_REWARD_MAX_COUNT = 50;

  public const string ITEM_GRADE_TEXT_FORMAT = "grade_0{0}";

  #endregion

  #region UI
  public const string UI_BUTTON_DEFAULT_IMAGE = "fm_gray_btn";
  public const string UI_BUTTON_ACTIVE_IMAGE  = "fm_green_btn";



  #endregion

  #region Effect
  public static readonly Color EFFECT_CC_SLOW_COLOR = new Color(0.5f, 0.5f, 1f, 1f);
  #endregion


  #region Sound

  public const string SFX_UI_DEFAULT_CLICK = "SFX_UI_BUTTON_CLICK_0";

  //최대 활성화 할 SFX AudioSource
  public const int SFX_MAX_SIZE = 30;
  public const float SFX_MAX_SOUND_Delay = 0.15f;  //최대 딜레이 시간


  public const int TEST_BGM_INDEX = 12414;

  #endregion

  #region Partner

  public static readonly (int orderLayer, Vector3 pos)[] PARTNER_SPAWN_POSITION = 
  {
    //플레이어 레이어 11 ~ 20 사이
    (10 ,new Vector3(-6.19f,  -2.08f, 0)),    
    (20, new Vector3(-7.37f,  -3.62f, 0)),  
    (21, new Vector3(-8.77f,  -4.14f, 0)),    
    (10, new Vector3(-8.59f,  -1.64f, 0)),    
    (20, new Vector3(-9.32f,  -2.98f, 0)),    
    (21, new Vector3(-10.96f, -4.17f, 0)),   


  };
  #endregion

  #region Skill


  //데미지를 받았을때 출력되는 Text 색상
  public const string SKILL_HIT = "#FFFF00";
  public const string SKILL_HIT_MISS = "#FFFFFF";
  public const string SKILL_HIT_CRITICAL = "#FF0000";
  public const string SKILL_PRINT_STRING = "<color={0}>{1}</color>";

  public const float SKILL_DEFAULT_LIFT_TIME = 5f;

  public const int SKILL_MOUNT_INVEN_SLOT_COUNT = 5;

  //광역 스킬의 스킨이 없는 경우 DefaultSkin
  public const string SKILL_AREA_DEFAULT_SKIN = "bat_boom";
  //투사체 스킬의 스킨이 없는 경우 DefaultSkin
  public const string SKILL_PROJECTILE_DEFAULT_SKIN = "ProjectTile-normal";

  /// <summary>
  /// SkillDMG와 곱해지는 값
  /// </summary>
  public const float SKILL_DAMAGE_MULTIPLIER = 0.01f;

  /// <summary>
  /// 플레이어가 소유하는 스텟 종류들
  /// </summary>
  public static readonly StatType[] STAT_TYPES_PLAYER_ALL_STAT = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense, 
    StatType.CriticalChance, StatType.CriticalDamage, StatType.DodgeRate, StatType.AccuracyRate,
    StatType.AttackSpeed, StatType.DamageReduction, StatType.SkillDamageIncrease, StatType.SkillCriticalChance,
    StatType.SkillCriticalDamage, StatType.BossDamageIncrease, StatType.PartnerDamageIncrease, StatType.PartnerAttackSpeed,
    StatType.SkillAreaDamageIncrease, StatType.DamageReductionPer, StatType.DamageReflection, StatType.PartyAttackPowerUpPer
  };

  /// <summary>
  /// 스킬 스텟에서 해당 스텟들이 있으면 기본 스텟에 합산
  /// </summary>
  public static readonly StatType[] STAT_TYPES_PASSIVE_SKILL_ADD = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense
  };


  /// <summary>
  /// 정수 형태로 사용되는 Stat들
  /// </summary>
  public static readonly StatType[] STAT_TYPES_INTEGER_STAT = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense
  };

  /// <summary>
  /// 최종 스텟 결정시 보너스 스텟들이 곱연산으로 사용되는 StatType들 (이것 제외는 다 합연산이다)
  /// </summary>
  public static readonly StatType[] STAT_TYPES_TOTAL_MULTIPLIER_STAT = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense
  };

  /// <summary>
  /// 보유 아이템들의 올라가는 스텟 Stat Array
  /// </summary>
  public static readonly StatType[] STAT_TYPES_ITEM_HAS_VALUE = new StatType[]
  {
    StatType.HealthPer, StatType.AttackPowerPer, StatType.DefensePer
  };

  /// <summary>
  /// 직업 기본 스텟 정보
  /// </summary>
  public static readonly StatType[] STAT_TYPES_PLAYER_JOB = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense, StatType.CriticalChance, StatType.CriticalDamage,
    StatType.DodgeRate, StatType.AccuracyRate, StatType.AttackSpeed
  };

  /// <summary>
  /// 동료 기본 스텟 정보
  /// </summary>
  public static readonly StatType[] STAT_TYPES_PARTNER = new StatType[]
  {
    //아래 세개는 데이터 설정할때 따로 연산해서 넣어줄거라 뻈습니다.
    //StatType.AttackPower, StatType.AccuracyRate, StatType.AttackSpeed,
    StatType.CriticalChance, StatType.CriticalDamage,
    StatType.SkillDamageIncrease, StatType.SkillCriticalChance,
    StatType.SkillCriticalDamage, StatType.BossDamageIncrease, StatType.PartnerDamageIncrease

  };

  /// <summary>
  /// 몬스터 기본 스텟 정보
  /// </summary>
  public static readonly StatType[] STAT_TYPES_MONSTER = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense, StatType.CriticalChance, StatType.CriticalDamage,
    StatType.DodgeRate, StatType.AccuracyRate, StatType.AttackSpeed
  };

  /// <summary>
  /// 몬스터 기본 스텟 중 (Weight 영향 받는 Type들)
  /// </summary>
  public static readonly StatType[] STAT_TYPES_MONSTER_WEIGHT = new StatType[]
  {
    StatType.Health, StatType.AttackPower, StatType.Defense
  };

  #endregion
}
