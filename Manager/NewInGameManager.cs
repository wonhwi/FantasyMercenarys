using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FantasyMercenarys.Data;

public enum StageState
{
    StageStart = 0,
    StagePlaying,
    StageEnd
}

public enum WaveState
{
    WaveStart = 0,
    WavePlaying,
    WaveEnd,    
}

public enum StageMapState
{
    StageMapMove = 0,
    StageMapPause
}

public class NewInGameManager : LazySingleton<NewInGameManager>
{
    public StageState currentStageState;
    public WaveState currentWaveState;
    public StageMapState currentMapState;

    public bool isGameStartMove = false;
    public bool isStageClear = false;
    public bool isPlayerPossibleAttack = false;
    public bool isCameraMove = false;
    public bool isCreateMonster = false;
    public bool isStageMove = false;
    public bool isClearPopupClose = true;

    public bool isEquipmentItemSale = true; // 뽑은 장비를 팔았는지 여부
    public bool isAutoGacha = false; // 자동 뽑기 진행중인지 여부
    public bool isAutoGachaItemSale = true; // 자동뽑기 결과가 2개 이상일때 아이템 목록을 모두 팔았는지 여부

  public int monsterKillCount = 0;
  public int monsterDropTicket = 0;

    public float playerMoveSpeed;
    public float stageMoveTime;    

    public NewPlayerController player;
    public GachaEffectSpineController gachaEffectController;
    public PlayerSkillController playerSkillController;
    public StageClearDimPanelController stageClearDimController;
  public BossStageUIController bossStageUIController;
    public Dictionary<WagonType, WagonController> wagonContollerDic = new Dictionary<WagonType, WagonController>();
    public WagonController currentWagon;
    public List<MonsterController> monsterList = new List<MonsterController>();
    public List<PartnerController> partnerList = new List<PartnerController>();
    

    public NewObjectPool<IPoolable> hudBarPool;
    #region MonsterPool
    public Dictionary<string, NewObjectPool<IPoolable>> monsterPoolDict;
    public Dictionary<SkillType, NewObjectPool<IPoolable>> skillPoolDict;
    public Dictionary<string, NewObjectPool<IPoolable>> partnerPoolDict;
    #endregion
    public NewObjectPool<IPoolable> damageTextPool;
    public NewObjectPool<IPoolable> dropItemPool;

    public NewObjectPool<IPoolable> deathEffectPool;
    public NewObjectPool<IPoolable> hitEffectPool;


    public Transform mainCameraTransform;
    public RectTransform dropGoldTransform;

    public Action StageWaveTextUpdate; //현재 Stage Wave 텍스트 업데이트 Action
    public Action StageWaveProgressBarUpdate; //현재 Stage Wave 진행도 Bar 업데이트 Action
    public Action StageWaveRepeatBattle; //현재 웨이브에 따라 Stage Wave 진행도 Bar 꺼주고 중간보스 도전 / 스테이지 보스 도전 버튼 Active Action
    public Action BossChallenge; //보스 도전 버튼 Action으로 ResetGame(죽고 다시 플레이 시작할 때..)와 같은 기능.
    public Action ResetWaveStart; //반복 전투 로직으로 들어가는 RepeatBattleStart Action으로 중간보스 웨이브 / 보스 웨이브에서 사망시 실행.
    public Action<float> LimitTimeSetting; //보스 웨이브 제한 시간을 초기화 하는 Action
    public Action<List<InvenData>> ItemListUpdate; //자동 뽑기에서 조건에 맞는 아이템이 2개 이상일때 나오는 리스트 팝업의 Item List에 따라 슬릇 데이터를 갈아치우는 Action
    public Action AutoGacha; //자동 뽑기 실행 Action
    public Action AutoGachaPlayingActiveOn; //자동 뽑기 진행중 오브젝트 켜주는 Action
    public Action AutoGachaPlayingActiveOff; //자동 뽑기 진행중 오브젝트 꺼주는 Action
    public Action GachaSaleEffect; //장비 판매 연출 실행Action
    public Action GachaPlayEffect; //방비 뽑기 연출 실행Action
    public Action AutoGachaPlayEffect; //자동 뽑기 연출 실행Action..해당 연출은 자동 뽑기에서는 상황에 따라 계속 호출되므로 따로 분리.
    public Action GachaButtonInteractableTrue; //뽑기 연출끝난 후 뽑기 버튼쪽 (장비 뽑기 레벨, 자동 뽑기 셋팅, 뽑기 버튼)Interactable을 켜주는 Action
    public Action GachaButtonInteractableFalse; //뽑기 연출동안 뽑기 버튼쪽 (장비 뽑기 레벨, 자동 뽑기 셋팅, 뽑기 버튼) Interactable을 꺼주는 Action
    public Action<int> UpdateBackgroundMapImage; //Chapter 진행도에 따라 맵 배경 이미지를 바꿔주는 Action
    public Action UpdatePlayerExp; //플레이어 Exp 업데이트 Action
    public Action<string> UpdateEquipmentGachaLevelText; // MainUI 장비뽑기 레벨 업데이트 Action



    /// <summary>
    /// 파트너 팝업 창에서 Partner 아이템을 장착, 장착해제, 강화, 합성, 일괄 장착 시 실행 시켜주는 Action
    /// </summary>
    public Action<IEnumerable<InvenData>> OnUpdatePartnerInvenData;

    /// <summary>
    /// 스킬 팝업 창에서 Skill 아이템을 장착, 장착해제, 강화, 합성, 일괄 장착 시 실행 시켜주는 Action
    /// 이게 실행되면 PlayerSkillController에서 GameSkillData를 생성
    /// 장착여부 판단해서 쿨타임 돌릴지 안돌릴지 판단
    /// </summary>
    public Action<IEnumerable<InvenData>> OnUpdateSkillInvenData;

    /// <summary>
    /// UI PopUP -> PlayerSkillController 함수 실행 후 PlayerSkillController가 OnUpdateSkillUI 실행
    /// int = index, invenData 존재하면 장착, 없으면 장착 해제
    /// </summary>
    public Action<int, InvenData> OnMountSkillData;
    public Action<int, GameSkillData> OnUpdateSkillIUI;

  /// <summary>
  /// 파트너 교체시 실행되는 함수
  /// </summary>
  public Action<int, InvenData> OnMountPartnerData;



    /// <summary>
    /// Wave클리어(몬스터 수가 0인가) 됐는지 체크하는 함수.
    /// </summary>
    /// <returns></returns>
    public bool WaveClearSuccess()
    {
    int mounsterCount = 0;


    List<MonsterController> monsterController = GetMosterControllerList();

    for (int i = 0; i < monsterController.Count; i++)
    {
      if (!monsterController[i].IsDead() && monsterController[i].gameObject.activeInHierarchy)
      {
        mounsterCount++;
      }
    }
    return mounsterCount == 0;
    }
    /// <summary>
    /// Stage와 Wave가 Start 상태인지 판단
    /// </summary>
    /// <returns></returns>
    public bool StartWaveStage()
    {
        return currentStageState == StageState.StageStart && currentWaveState == WaveState.WaveStart;
    }
    /// <summary>
    /// Stage와 Wave가 Playing 상태인지 판단
    /// </summary>
    /// <returns></returns>
    public bool PlayingWaveStage()
    {
      return currentStageState == StageState.StagePlaying && currentWaveState == WaveState.WavePlaying;
    }
    /// <summary>
    /// Stage는 진행중이나 현재 Wave가 Start 상태인지 판단
    /// </summary>
    /// <returns></returns>
    public bool PlayingStageAndWaveStart()
    {
        return currentStageState == StageState.StagePlaying && currentWaveState == WaveState.WaveStart;
    }
    /// <summary>
    /// Stage는 진행중이나 현재 Wave가 End 상태인지 판단
    /// </summary>
    /// <returns></returns>
    public bool PlayingStageAndWaveEnd()
    {
        return currentStageState == StageState.StagePlaying && currentWaveState == WaveState.WaveEnd;
    }
    /// <summary>
    /// Stage와 Wave가 End 상태인지 판단
    /// </summary>
    /// <returns></returns>
    public bool FinishWaveStage()
    {
      return currentStageState == StageState.StageEnd && currentWaveState == WaveState.WaveEnd;
    }

    public List<MonsterController> GetMosterControllerList()
       => monsterList;


    public void ReturnObjectPoolTypeHitEffect(HitEffect _object)
    {
        hitEffectPool.EnqueueObject(_object);
    }

    public void ReturnObjectPoolTypeDeathEffect(DeathEffect _object)
    {
        deathEffectPool.EnqueueObject(_object);
    }

    /// <summary>
    /// HUDBar 오브젝트 사용 후 Pool 반환.
    /// </summary>
    /// <param name="_object">HUDBar</param>
    public void ReturnObjectPoolTypeHUDBar(HUDBar _object)
    {
        hudBarPool.EnqueueObject(_object);
    }

    /// <summary>
    /// 몬스터 오브젝트 사용 후 Pool 반환.
    /// </summary>
    /// <param name="_object">몬스터</param>
    public void ReturnObjectPoolTypeMonster(MonsterController _object, string _key)
    {
        monsterPoolDict.TryGetValue(_key , out var monsterPool);
        if(monsterPool != null)
        {
            monsterPool.EnqueueObject(_object);
        }
    }

    /// <summary>
    /// DamageText 오브젝트 사용 후 Pool 반환.
    /// </summary>
    /// <param name="_object">DamageText</param>
    public void ReturnObjectPoolTypeDamageText(DamageText _object)
    {
        damageTextPool.EnqueueObject(_object);
    }

  public void ReturnObjectPoolTypeDropItem(DropItem _object)
  {
    dropItemPool.EnqueueObject(_object);
  }


  public void ReturnObjectPoolTypePartner(PartnerController _object, string _key)
  {
    partnerPoolDict.TryGetValue(_key, out var partnerPool);

    if (partnerPool != null)
    {
      partnerPool.EnqueueObject(_object);
    }

  }
  public void ReturnObjectPoolTypeSkill(ActiveSkillBase _object, SkillType skillType)
  {
    skillPoolDict.TryGetValue(skillType, out var skillpool);

    if (skillpool != null)
    {
      skillpool.EnqueueObject(_object);
    }
  }

  public ActiveSkillBase SkillTypeGetObject(GameSkillData skillData, SkillType skillType)
  {
    skillPoolDict.TryGetValue(skillType, out var skillPool);

    if (skillPool == null)
    {
      Debug.Log($"{skillData.skillIdx} 봐바");
    }

    return (ActiveSkillBase)skillPool.GetObject();
  }

  public PartnerController PartnerTypeGetObject(PartnerData partnerData, string _key)
  {
    partnerPoolDict.TryGetValue(_key, out var partnerPool);

    if (partnerPool == null)
    {
      Debug.Log($"{partnerData.partnerIdx} 봐바");
    }

    return (PartnerController)partnerPool.GetObject();
  }


  public WagonController SetWagonController(WagonType wagonType)
  {
    int wagonTypes = wagonContollerDic.Count;

    for (int i = 0; i < wagonTypes; i++)
    {
      WagonType type = (WagonType)i;

      wagonContollerDic[type].gameObject.SetActive(type == wagonType);
    }

    return currentWagon = wagonContollerDic[wagonType];

  }
}
