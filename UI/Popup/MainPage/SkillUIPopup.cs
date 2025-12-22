using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using System.Linq;
using Newtonsoft.Json;
using static RestPacket;
using Assets.ZNetwork.Data;
using Assets.ZNetwork;
using Cysharp.Threading.Tasks;

public class SkillUIPopup : PartnerSkillUIPopup
{
  private SkillTable skillTable = SkillTable.getInstance;

  protected override void SetItemTableTarget()
  {
    popUpItemGroup = ItemGroup.Skill;
    itemTableKeyList = SkillTable.getInstance.GetSkillDic(SkillCategory.Item).Keys.ToList();
  }

  protected override void GetInvenData()
  {
    invenDataList = gameDataManager.dictSkill;
    presetInvenDataList = gameDataManager.GetPresetInvenDataList(base.presetType);
    mountInvenDataList = presetInvenDataList.ToList();
  }


  /// <summary>
  /// 플레이어 스킬 장착되어있는것 스킬계수 업데이트 해줘야 해서 아래 함수 실행
  /// </summary>
  /// <param name="invenData"></param>
  protected override void UpdateMountSlotData(InvenData invenData)
  {
    int findIndex = base.GetMountSlotInvenIndex(invenData);

    if (!findIndex.Equals(-1))
    {
      mountInvenDataList[findIndex] = invenData;
      inGameManager.OnMountSkillData?.Invoke(findIndex, invenData);
    }
  }

  protected override void OnClickMountAction(int findIndex, InvenData invenData)
  {
    inGameManager.OnMountSkillData?.Invoke(findIndex, invenData);

    Debug.Log($"스킬 팝업에서 InvenIdx = {invenData.invenIdx}인 스킬을 장착 하였습니다.");
  }

  protected override void OnClickUnMountAction(int findIndex, InvenData invenData)
  {
    inGameManager.OnMountSkillData?.Invoke(findIndex, null);

    Debug.Log($"스킬 팝업에서 {findIndex} 슬롯에 있는 InvenIdx = {invenData.invenIdx} ItemIdx = {invenData.itemIdx} 스킬을 장착 해제 하였습니다.");
  }

  protected override void OnClickMountAllItem()
  {
    base.OnClickMountAllItem();

    inGameManager.OnUpdateSkillInvenData?.Invoke(base.mountInvenDataList);

    Debug.Log($"스킬 일괄 장착 하였습니다.");
  }
}
