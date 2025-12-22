using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartnerManager : LazySingleton<PartnerManager>
{
  private NewInGameManager inGameManager;
  /// <summary>
  /// 현재 활성화 되어있는 PartnerControllerList 이거 아예 InGameManager에서 관리하게 하고 이런것들 다 삭제해버리자
  /// </summary>
  public List<PartnerController> partnerList = new List<PartnerController>();

  //GameSceneController의 Awake 부분에서 Action 추가
  public void Initialize()
  {
    inGameManager = NewInGameManager.getInstance;


    inGameManager.OnUpdatePartnerInvenData += UpdatePartner;
    inGameManager.OnMountPartnerData       += UpdatePartner;



  }

  /// <summary>
  /// 게임 시작시 내 파트너 프리셋 기반 동료들 활성화
  /// </summary>
  public void InitPartner()
  {
    this.InitPartnerItem(GameDataManager.getInstance.GetPresetInvenDataList(PresetType.Partner));
  }

  private void InitPartnerItem(IEnumerable<InvenData> invenDataList)
  {
    int findIndex = 0;
    foreach (InvenData invenData in invenDataList)
    {
      if (invenData != null)
      {
        PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

        PartnerController partnerController = inGameManager.PartnerTypeGetObject(partnerData, ((PartnerSpineType)partnerData.groupSpine).ToString());

        if (partnerController != null)
        {
          (int orderLayer, Vector3 pos) = ConstantManager.PARTNER_SPAWN_POSITION[findIndex];

          partnerController.InitPartner(partnerData, pos);
          partnerController.OnActivate();

          partnerController.spineSkin.SetSkin(partnerData.partnerSpine);
          partnerController.spineSkin.SetLayer(orderLayer);

          this.partnerList.Add(partnerController);
          inGameManager.partnerList.Add(partnerController);
        }
      }
      else
      {
        this.partnerList.Add(null);
      }

      findIndex++;
    }
  }



  public void UpdatePartner(int index, InvenData invenData)
  {
    //Mount
    if (invenData != null)
    {
      MountPartner(index, invenData);
    }
    //UnMount
    else
    {
      UnMountPartner(index);
    }

  }

  public void UpdatePartner(IEnumerable<InvenData> invenDataList)
  {
    int findIndex = 0;
    //현재 장착한 스킬들에 대한 설정 , 아이템 레벨에 따른 증가량까지 포함해서 스텟을 포함시켜줘야함
    foreach (InvenData invenData in invenDataList)
    {
      UpdatePartner(findIndex, invenData);
      findIndex++;
    }
  }

  public void MountPartner(int index, InvenData invenData)
  {
    PartnerController partnerController;

    PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

    PartnerSpineType spineType = (PartnerSpineType)partnerData.groupSpine;


    //이미 존재하면
    if (partnerList[index] != null)
    {
      PartnerSpineType currentMountType = (PartnerSpineType)partnerList[index].partnerData.groupSpine;

      //같으면 기존꺼 그대로 가져다 써라
      if (spineType == currentMountType)
        partnerController = partnerList[index];
      //만약 다르면 안에 넣어두고 새로 풀링해야함
      else
      {
        UnMountPartner(index);

        partnerController = inGameManager.PartnerTypeGetObject(partnerData, spineType.ToString());
      }
    }
    else
    {
      partnerController = inGameManager.PartnerTypeGetObject(partnerData, spineType.ToString());
    }


    if (partnerController != null)
    {
      (int orderLayer, Vector3 pos) = ConstantManager.PARTNER_SPAWN_POSITION[index];

      partnerController.InitPartner(partnerData, pos);
      partnerController.OnActivate();

      partnerController.spineSkin.SetSkin(partnerData.partnerSpine);
      partnerController.spineSkin.SetLayer(orderLayer);

      inGameManager.partnerList.Add(partnerController);

      this.partnerList[index] = partnerController;
    }

    Debug.Log($"파트너 추가 완료");
  }

  public void UnMountPartner(int index)
  {
    inGameManager.ReturnObjectPoolTypePartner(partnerList[index], ((PartnerSpineType)partnerList[index].partnerData.groupSpine).ToString());
    inGameManager.partnerList.Remove(partnerList[index]);

    this.partnerList[index] = null;
    Debug.Log($"플레이어 파트너 삭제 완료");
  }

  public void UpdatePartnerPosition()
  {
    for (int i = 0; i < partnerList.Count; i++)
    {
      if (partnerList[i] != null)
      {
        partnerList[i].InitPosition();
      }
    }
  }

  public void PartnerAnimationControll(string animation, bool loop)
  {
    for (int i = 0; i < partnerList.Count; i++)
    {
      if (partnerList[i] != null)
      {
        partnerList[i].spineAnimation.SetAnimation(animation, loop : loop);
      }
    }
  }
  /// <summary>
  /// 현재 활성화된 스킬들, 스킬 관련 컨트롤러 이용하는 것들 Clear
  /// </summary>
  public void ClearPartnerList()
  {
    foreach (var partner in partnerList)
    {
      //if (partner)
      //  skill.DestroySkill();
    }

    partnerList.Clear();
  }

  /// <summary>
  /// 스킬 리스소 해제 및 초기상태
  /// </summary>
  public void Release()
  {
    ClearPartnerList();
  }

}
