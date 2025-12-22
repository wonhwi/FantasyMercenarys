using FantasyMercenarys.Data;
using Newtonsoft.Json;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RestPacket;

public class PlayerSkeletonController : MonoBehaviour
{
  [System.Serializable]
  public struct ClassSkeletonData
  {
    public JobType classType;
    public SkeletonDataContainer skeletonDataContainer;
    public ResourceSkinDataContainer skinDataContainer;
  }

  public ClassSkeletonData[] classSkeletonData;

  public MultipleSkeletonAnimation skeletonAnimation;
  public MultipleSkeletonSkin skeletonSkin;
  public ResourceSkin resourceSkin;


  public JobType classType = JobType.Commoner;


  private void OnEnable()
  {
    APIEventManager.AddListener<int>(APIEventType.UpdateJob, SetPlayerSkeleton);
    APIEventManager.AddListener<RES_CostumeChange>(APIEventType.CostumeChange, SetPlayerSkin);
  }

  private void OnDisable()
  {
    APIEventManager.RemoveListener<int>(APIEventType.UpdateJob, SetPlayerSkeleton);
    APIEventManager.RemoveListener<RES_CostumeChange>(APIEventType.CostumeChange, SetPlayerSkin);
  }

  /// <summary>
  /// 내 플레이어가 직업 바꿨을때 실행하는 함수
  /// 직업 Update 되었을때 Skeleton 변경 및 Costume Update
  /// </summary>
  /// <param name="jobIdx"></param>
  public void SetPlayerSkeleton(int jobIdx)
  {
    SetUserSkeleton(jobIdx, GameDataManager.getInstance.costumeData);
  }


  /// <summary>
  /// 공통적으로 사용하는 유저 Costume 시 실행 함수
  /// </summary>
  /// <param name="jobIdx">직업 인덱스</param>
  /// <param name="userCostumeData">코스튬 정보</param>
  public void SetUserSkeleton(int jobIdx, UserCostumeData userCostumeData)
  {
    JobType classType = (JobType)JobTable.getInstance.GetJobData(jobIdx).jobType;

    SetUserJobSkeleton(classType);

    if(classType != JobType.Commoner)
      SetUserCostume(userCostumeData);
  }

  /// <summary>
  /// JobType기반 Skeleton 설정
  /// </summary>
  /// <param name="classType"></param>
  public void SetUserJobSkeleton(JobType classType)
  {
    skeletonSkin.SetAllDefaultSkin();

    this.classType = classType;

    int findIndex = GetDataFindIndex(classType);

    string currentAnimationName = skeletonAnimation.GetAnimationState().GetCurrent(0).Animation.Name;

    skeletonAnimation.SkeletonDataContainer = skeletonSkin.SkeletonDataContainer 
      = classSkeletonData[findIndex].skeletonDataContainer;

    resourceSkin.ResourceSkinDataContainer = classSkeletonData[findIndex].skinDataContainer;


    for (int i = 0; i < classSkeletonData.Length; i++)
    {
      if (i == findIndex)
        classSkeletonData[i].skeletonDataContainer.gameObject.SetActive(true);
      else
        classSkeletonData[i].skeletonDataContainer.gameObject.SetActive(false);
    }

    //새로운 SkeletonAnimation에서 저장된 애니메이션 재생
    SetAnimation(currentAnimationName, 0);
  }

  /// <summary>
  /// Costume 정보 기반 Player Spine Skin 설정
  /// 다른 플레이어 일때 다른 플레이어의 UserCostumeData 넣으면 됩니다
  /// </summary>
  public void SetUserCostume(UserCostumeData userCostumeData)
  {
    if(userCostumeData.equiphead != ConstantManager.DATA_NONE_INTEGER_VALUE)
      SetUserSkin(userCostumeData.equiphead);

    if (userCostumeData.equipWeapon != ConstantManager.DATA_NONE_INTEGER_VALUE)
      SetUserSkin(userCostumeData.equipWeapon);

    if (userCostumeData.equipAccessory != ConstantManager.DATA_NONE_INTEGER_VALUE)
      SetUserSkin(userCostumeData.equipAccessory);

    if (userCostumeData.equipArmor != ConstantManager.DATA_NONE_INTEGER_VALUE)
      SetUserSkin(userCostumeData.equipArmor);

  }

  /// <summary>
  /// 스킨 교체 시에 해당 애니메이션의 Duration을 저장한것을 사용하면될듯
  /// </summary>
  /// <param name="responseResult"></param>
  private void SetPlayerSkin(RES_CostumeChange responseResult)
  {
    if(GameDataManager.getInstance.userInfoModel.IsCommoner())
    {
      Debug.Log("Player의 Skin 교체 시도 중 직업이 Commoner라 스킵합니다");
      return;
    }

    SetUserSkin(responseResult.updateCostumData.itemIdx);
  }

  public void SetUserSkin(int itemIdx)
  {
    EquipmentItemData itemData = EquipmentItemTable.getInstance.GetEquipmentItemData(itemIdx);

    CostumePart costumePart = GetPlayerCostumePart((EquipmentMountType)itemData.mountType);

    if (costumePart == CostumePart.Accessory)
      resourceSkin?.SetSkin(CostumePart.Accessory, itemData.GetCoustumeSpine(classType));
    else if (costumePart != CostumePart.None)
      skeletonSkin.SetSkin(costumePart, itemData.GetCoustumeSpine(classType));
  }

  public bool IsPlaying(string animationName)
  {
    return skeletonAnimation.IsPlaying(animationName);
  }

  public void SetAnimation(string animationName, int trackIndex = 0, bool loop = true, float timeScale = 1f)
  {
    skeletonAnimation.SetAnimation(animationName, trackIndex, loop, timeScale);
  }

  public void AddAnimation(string animationName, int trackIndex = 0, bool loop = true)
  {
    skeletonAnimation.AddAnimation(animationName, trackIndex, loop);
  }

  public CostumePart GetPlayerCostumePart(EquipmentMountType mountType) => mountType switch
  {
    EquipmentMountType.Weapon => CostumePart.Weapon,
    EquipmentMountType.Helmet => CostumePart.Hat,
    EquipmentMountType.Glasses => CostumePart.Accessory,
    EquipmentMountType.Armor => CostumePart.Body,
    _ => CostumePart.None
  };

  private int GetDataFindIndex(JobType classType)
  {
    int findIndex = -1;

    for (int i = 0; i < classSkeletonData.Length; i++)
    {
      if (classSkeletonData[i].classType == classType)
      {
        findIndex = i;
      }
    }

    return findIndex;
  }

  [ContextMenu("SetClass")]
  public void SetClass()
  {
    SetUserJobSkeleton(classType);
  }

  [ContextMenu("DefaultAllSkin")]
  public void DefaultAllSkin()
  {
    skeletonSkin.SetAllDefaultSkin();
  }

}
