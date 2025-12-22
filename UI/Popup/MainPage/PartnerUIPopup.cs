using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using System.Linq;
using Cysharp.Threading.Tasks;
using Assets.ZNetwork.Data;
using Assets.ZNetwork;
using static RestPacket;
using Newtonsoft.Json;

public class PartnerUIPopup : PartnerSkillUIPopup
{

  private PartnerTable partnerTable = PartnerTable.getInstance;

  protected override void SetItemTableTarget()
  {
    popUpItemGroup = ItemGroup.Partner;
    itemTableKeyList = PartnerTable.getInstance.GetPartnerlDic().Keys.ToList();
  }


  protected override void GetInvenData()
  {
    invenDataList = gameDataManager.dictPartner;
    presetInvenDataList = gameDataManager.GetPresetInvenDataList(base.presetType);
    mountInvenDataList = presetInvenDataList.ToList();
  }


  protected override void OnClickMountAction(int findIndex, InvenData invenData)
  {
    inGameManager.OnMountPartnerData?.Invoke(findIndex, invenData);

    Debug.Log($"동료 팝업에서 {findIndex} 슬롯에 있는 InvenIdx = {invenData.invenIdx} ItemIdx = {invenData.itemIdx} 동료을 장착 하였습니다.");
  }

  protected override void OnClickUnMountAction(int findIndex, InvenData invenData)
  {
    inGameManager.OnMountPartnerData?.Invoke(findIndex, null);

    Debug.Log($"동료 팝업에서 InvenIdx = {invenData.invenIdx}인 동료를 장착 해제 하였습니다.");
  }

  protected override void OnClickMountAllItem()
  {
    base.OnClickMountAllItem();

    inGameManager.OnUpdatePartnerInvenData?.Invoke(base.mountInvenDataList);

    Debug.Log($"동료 일괄 장착 하였습니다.");

  }
}
