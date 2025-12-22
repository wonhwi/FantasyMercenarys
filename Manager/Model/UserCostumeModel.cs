using FantasyMercenarys.Data;
using System.Collections.Generic;
using static RestPacket;

public class UserCostumeModel
{
  public UserCostumeData userConstumeData;
  /// <summary>
  /// 내가 현재 해당 장비 타입의 코스튬을 가지고 있는지
  /// </summary>
  /// <param name="costumePart"></param>
  /// <returns></returns>
  public bool IsUsingPartCostume(CostumePart costumePart)
  {
    List<int> costumeList = GetCostumeItemList(costumePart);

    return costumeList != null && costumeList.Count > 0;
  }

  /// <summary>
  /// 현재 장착중인 코스튬 아이템 Index 반환
  /// </summary>
  /// <param name="costumePart"></param>
  /// <returns></returns>
  public int GetMountCostumeItemIdx(CostumePart costumePart) => costumePart switch
  {
    CostumePart.Weapon => userConstumeData.equipWeapon,
    CostumePart.Hat => userConstumeData.equiphead,
    CostumePart.Accessory => userConstumeData.equipAccessory,
    CostumePart.Body => userConstumeData.equipArmor,
    _ => 0,
  };

  /// <summary>
  /// 소유하고 있는 해당 파츠 코스튬 ItemList
  /// </summary>
  /// <param name="costumePart"></param>
  /// <returns></returns>
  public List<int> GetCostumeItemList(CostumePart costumePart) => costumePart switch
  {
    CostumePart.Weapon => userConstumeData.weaponItems,
    CostumePart.Hat => userConstumeData.headItems,
    CostumePart.Accessory => userConstumeData.accessoryItems,
    CostumePart.Body => userConstumeData.armorItems,
    _ => null,
  };

  /// <summary>
  /// 코스튬 데이터 업데이트
  /// </summary>
  /// <param name="mountType"></param>
  /// <param name="itemIdx"></param>
  public void UpdateCostumeList(EquipmentMountType mountType, int itemIdx)
  {
    List<int> costumeList = GetCostumeItemList((CostumePart)(int)mountType);

    if (!costumeList.Contains(itemIdx))
    {
      if (costumeList.Count == 0)
      {
        APIManager.getInstance.REQ_CostumeChange<RES_CostumeChange>((int)mountType, itemIdx);

      }
      costumeList.Add(itemIdx);
    }
  }

  /// <summary>
  /// 장착중인 코스튬 장비 업데이트
  /// </summary>
  /// <param name="mountType"></param>
  /// <param name="itemIdx"></param>
  public void UpdateCostumeChange(EquipmentMountType mountType, int itemIdx)
  {
    switch (mountType)
    {
      case EquipmentMountType.Weapon:  userConstumeData.equipWeapon    = itemIdx; break;
      case EquipmentMountType.Helmet:  userConstumeData.equiphead      = itemIdx; break;
      case EquipmentMountType.Glasses: userConstumeData.equipAccessory = itemIdx; break;
      case EquipmentMountType.Armor:   userConstumeData.equipArmor     = itemIdx; break;
    }
  }

}
