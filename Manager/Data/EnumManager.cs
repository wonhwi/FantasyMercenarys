using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Table Enum

public enum BaseDataUnit
{
  Integer = 1,      //정수
  Float = 2,        //소수
  Percentage = 3,   //백분율
}

public enum BossTypes
{
  MiddleBoss,
  StageBoss,
}

public enum MapType
{
  GrassField = 1,   //초원
  Lake = 2,         //호수
  Swamp = 3,        //늪지
  SnowField = 4,    //설원
  Desert = 5,       //사막
  LavaField = 6,    //용암지대
}

#endregion

#region Spine/ Animation

public enum WagonAnimation
{
  death = 0,
  idle = 1,
  move = 2
}
public enum PartnerAnimation
{
  Attack = 0,
  Idle = 1,
  Move = 2,
  Run = 3,
}

public enum PlayerAnimation
{
  idle,
  move,
  attack1,
  attack2,
  death
}
public enum MonsterAnimation
{
  idle = 0,
  move = 1,
  attack = 2,
  death = 3,
  hit = 4,
}

public enum CostumePart
{
  None = 0,
  Weapon = 1,
  Hat = 2,
  Accessory = 3,
  Body = 4,
}

public enum EquipmentMountType
{
  None = 0,
  Weapon = 1,       //무기
  Helmet = 2,       //투구
  Glasses = 3,      //안경
  Armor = 4,        //갑옷
  Pauldron = 5,     //견갑
  Gloves = 6,       //장갑
  Vambraces = 7,    //완갑
  Belt = 8,         //벨트
  Shoes = 9,        //신발
  Greaves = 10,     //슬갑
}

public enum PlayerSpinePart
{
  BackHair = 0,
  Clothes = 1,
  Head = 2,
  FrontHair = 3,
  Hat = 4,
  Weapon = 5,
  Max = 6
}

public enum MonsterSkin
{
  Blue = 0,
  Green = 1,
  Orange = 2,
  Red = 3,
  Yellow = 4,
  Max = 5
}


public enum JobChangeType
{
  Commoner = 0,     //무명 용사
  FirstChange = 1,  //1차 전직
  SecondChange = 2, //2차 전직
  ThirdChange = 3,  //3차 전직
  FourthChange = 4, //4차 전직
  FifthChange = 5,  //5차 전직
}

#endregion


#region Shop

/// <summary>
/// GachaTable에서 해당 상점 최대 수치를 판단하는 근거 Index
/// </summary>
public enum ShopLvIndex
{
  None = 0,
  Skill = 15000,
  Partner = 16000,
  Equipment = 17000,
  BlessStatue = 18000,
  AdventureGuild = 19000,
}

public enum ShopCategoryType
{
  Skill = 1001,
  Partner = 1002,
  LimitedProduct = 1003, //한정 상품
  DiaProduct = 1004, //다이아 상품

  Equipment = 2001,
  BlessStatue = 3001,
  AdventureGuild = 4001,
}

public enum ShopType
{
  None = 0,
  NormalShop = 5001, //일반 뽑기 상점 (스킬, 동료)
}
#endregion

public enum DailyPopupType
{
  Partner_Gacha = 1,
  Skill_Gacha = 2,
  BlessingStatue_Statue = 3, //높은 등급 돌릴때 알림 팝업
}

#region Item

public enum ItemGroup
{
  Equipment = 1,  //장비
  Partner = 2,    //동료
  Skill = 3,      //스킬
  Creature = 4,

  RandomBoxGold = 100,      //랜덤 박스 뽑기 (골드)
  RandomBoxSilver = 101,    //랜덤 박스 뽑기 (실버)

  Ticket_Equipment = 801,   //장비 뽑기 티켓
  Ticket_Partner = 802,     //동료 뽑기 티켓
  Ticket_Skill = 803,       //스킬 뽑기 티켓

  Gold = 901,     //골드
  Gem = 902,       //보석
  Exp = 903,        //경험치
  BlessPoint = 904, //신성력
  AGExp = 905,     //모험가 길드 경험치
  ResearchFunds = 906, //연구비
  LotteryTicket = 907,
  CartEnhance = 909, //수레 강화 재료
  
}

public enum ItemGradeType
{
  Common = 1,     //일반
  Advanced = 2,   //고급
  Rare = 3,       //희귀
  Epic = 4,       //영웅
  Legendary = 5,  //전설
  Mythic = 6,     //신화
  Divine = 7,     //신성
  Infernal = 8,   //지옥
  Immortal = 9,   //불멸
}

public enum GradeType
{
  D = 1,
  C = 2,
  B = 3,
  A = 4,
  S = 5,
  SS = 6,
  SSS = 7,
  SSSR = 8,     //SSS+등급
}

public enum StatType
{
  None = 0,
  //기본 속성
  Health = 1,                   //체력                          
  AttackPower = 2,              //공격력                        
  Defense = 3,                  //방어력                        
  CriticalChance = 4,           //치명타 확률 (1=1%)            
  CriticalDamage = 5,           //치명타 피해 (1=1%)             
  DodgeRate = 6,                //회피율      (1=1%)            
  AccuracyRate = 7,             //명중률      (1=1%)              
  AttackSpeed = 8,              //공격속도    (1 = 1초당 공격 횟수)
  DamageReduction = 9,          //피해 감소               
  SkillDamageIncrease = 10,     //스킬 피해 증가
  SkillCriticalChance = 12,     //스킬 치명타 확률                 
  SkillCriticalDamage = 13,     //스킬 치명타 피해

  //신규
  SkillAreaDamageIncrease = 14, //범위 스킬 피해 증가
  HealthPer = 15,               //체력 증가 %
  AttackPowerPer = 16,          //공격력 증가 %
  DefensePer = 17,              //방어력 증가 %
  DamageReductionPer = 18,      //받는 피해 감소 %


  //TO DO
  DamageReflection = 19,        //받는 데미지 반사    
  PartyAttackPowerUpPer = 20,   //팀 전원 공격력 증가 %   

  AdditionalDamagePercent = 21,         // (신규추가)추가 피해%

  TeamAttackPowerIncreasePercent = 22,       //(신규 추가)아군 전체 공격력 증가(%)
  TeamDefensePowerIncreasePercent = 23,      //(신규 추가)아군 전체 방어력 증가(%)
  TeamDamageIncreasePercent = 24,            //(신규 추가)아군 전체 피해량 증가(%)
  TeamCriticalChanceIncreasePercent = 25,    //(신규 추가)아군 전체 치명타율 증가(%)
  TeamCriticalDamageIncreasePercent = 26,    //(신규 추가)아군 전체 치명타 피해 증가(%)
  TeamAttackSpeedIncreasePercent = 27,       //(신규 추가)아군 공격 속도 증가(%)

  //보스 관련 속성
  BossDamageIncrease = 31,      //보스 피해 증가  -> 아군이 보스를 공격했을때   
  BossDamageReduction = 32,     //보스 피해 감소  -> 보스가 플레이어를 공격했을때

  //동료 관련 속성
  PartnerDamageIncrease = 41,   //동료 피해 증가  -> 동료가 보스를 공격 했을때
  PartnerDamageReduction = 42,  //동료 피해 감소  -> 적 동료가 플레이어를 공격 했을때
  PartnerAttackSpeed = 43,      //동료 기본 공속 증가
}

public enum DamageFontType
{
  Normal = 1,
  Critial = 2,
}

public enum BuffType
{
  None = 0,

  EnemyAttackSpeedDownAll = 71,    //적 전체 공격 속도 감소
  EnemyMoveSpeedDownAll = 72,      //적 전체 이동 속도 감소
  EnemyCriticalChanceDownAll = 73, //적 전체 치명타 확률 감소 %
  EnemyAccuracyDown = 74,          //적 명중률 감소 %
  EnemyAttackSpeedDown = 75,       //적 공격 속도 감소 %
}

/// <summary>
/// CC기 (한번에 여러가지 상태이상을 가지고 있을 수 있다.)
/// </summary>
public enum CrowdControlType
{
  None = 0,

  Stun = 100,        //스턴
  Knockback = 101,   //밀어냄
  Slow = 102,        //둔화
  Immobilize = 103   //이동 불가
}

/// <summary>
/// CC기 테스트
/// </summary>
[System.Flags]
public enum CrowdControlTestType
{
  None = 0,
  Stun = 1 << 0,         // 1
  Knockback = 1 << 1,    // 2
  Slow = 1 << 2,         // 4
  Immobilize = 1 << 3    // 8
}

#endregion

public enum PartnerSpineType
{
  //이거 100단위로 나눠도 될듯
  Female1 = 0,
  Male1 = 1,
  Animal_Quadruped1 = 2,
  Female2 = 3,
  Male2 = 4,
}

public enum WagonType
{
  Basic = 0,
  Crab = 1,
  Animal = 2,
  Animals = 3,
}

#region Skill

public enum PresetType
{
  Equipment = 1,
  Skill = 2,
  Partner = 3,
}

public enum SkillActionType
{
  PassiveDebuff = 0,
  PassiveBuff = 1,
  ActiveDebuff = 2,
  ActiveBuff = 3,

  ActiveCrowdControl = 5,
}

public enum SkillAttackType
{
  NormalAttack = 0, //일반 공격
  SkillAttack = 1,  //스킬 공격
}


/// <summary>
/// 일단 범위 즉발형, 투사체형 2개로 작업하자
/// </summary>
public enum SkillCategory
{
  Common  = 1,  //공용 - 직업 평타 스킬
  Job     = 2,  //직업 - 직업 고유 스킬(액티브, 패시브)
  Item    = 3,  //아이템 - 아이템 스킬
  Partner = 4,  //파트너 - 파트너 스킬
}

/// <summary>
/// 투사체, 범위스킬, 소환스킬 등등
/// </summary>
public enum SkillType
{
  None = -1,
  Passive = 0,
  Projectile = 1,
  Area = 2,
  ForwardArea = 3,
  Summon = 4,

}

public enum AdjustTargetType
{
  DEFAULT = 0,    //가장 가까운 적
  RANDOM = 1,     //랜덤
  MANY_ENEMY = 2, //적이 가장 많은 곳 탐색 (HitEnemyCount 데이터 기반)
}

public enum JobType
{
  All = 0, //전부 가능 직업 (코스튬)
  Tanker = 1,
  Archer = 2,
  Warrior = 3,
  Mage = 4,
  Assasin = 5,
  Commoner = 100,
}


/// <summary>
/// 스킬 Visual 활성화 타입
/// </summary>
[System.Flags]
public enum SkillVisualType
{
  NONE = 0,
  PLAYER = 1 << 0,
  ENEMY = 1 << 1,
  All = PLAYER | ENEMY
}

/// <summary>
/// 스킬을 플레이어가 사용할 수도 있고 몬스터, 유저가 사용할수도 있다.
/// </summary>
public enum FactionType
{
  NONE = 0,
  Friendly = 1, //게임 플레이어, 아군 팀
  Enemy = 2,  //몬스터, 적 PVP 상대
}

public enum UnitType
{
  None = 0,
  Player = 1,
  Partner = 2,


  Monster = 10,      //몬스터 및 적 플레이어
  BossMonster = 11, //보스 몬스터
}

public enum UnitState
{
  Idle,
  Attack,
  Move,
  Dead
}

#region Content
public enum MerchantGuildSkillType
{
  Health = 1,                   //체력                          
  AttackPower = 2,              //공격력                        
  Defense = 3,                  //방어력
  PartnerAttackSpeed = 4,      //동료 기본 공속 증가                                
  PartnerDamageIncrease = 5,   //동료 피해 증가  -> 동료가 보스를 공격 했을때
  SkillDamageIncrease = 6,     //스킬 피해 증가
  ResearchGainUp = 7,       //클라에서 안씀
  ResearchMaxLimitUp = 8,   //연구비 최대 보유량 증가
  CreatureSpeedUp = 9,      //생명체 속도 증가
  BreedingSlotAdd = 10,     //클라에서 안씀
  BreedingCreatureAdd = 11, //클라에서 안씀

}

public enum GroundObjectType
{
  Research = 6,
  Can = 7,
  Poop = 8,
}
#endregion


/// <summary>
/// 슬롯 해제 관련 콘텐츠 타입
/// </summary>
public enum SlotUnlockContentType
{
  Skill = 2,
  Partner = 3,
  Prayer = 4,
  BlessingStatueSlot = 5,
  BlessingStatuePreset = 6,
  Recruitment = 7, //동료 모집소
  PartnerCraft = 8, //동료 제작소
  SkillCraft = 9,   //스킬 제작소
  EquipmentGachaCount = 10, //자동 뽑기 1회 뽑기 수량 조건
  EquipmentGachaGrade = 11, //자동 뽑기 등급 조건
  BreedingGrounds = 12, //사육장
}

/// <summary>
/// 슬롯 해제 조건
/// </summary>
public enum SlotUnlockConditionType
{
  None = 0,
  Day = 1,
  Level = 2,
  BlessingStatueLv = 3,
  StageClear = 4,
  EquipmentGachaLv = 5,

  ResearchBuy = 12,
  MerchantGuild = 13,
}

#endregion