using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartTable : LazySingleton<CartTable>, ITableFactory
{
  //CarLevel Key 별 CartData
  public Dictionary<int, CartData> dictCartData = new Dictionary<int, CartData>();

  //스파인 이미지 별 최소 레벨, 최대 레벨 출력
  public Dictionary<string, (int minLv, int maxLv)> dictCartSpineData = new Dictionary<string, (int minLv, int maxLv)>();

  //코스튬 관련된 것들 압축 List
  public List<CartData> cartCostumeList = new List<CartData>();


  public int cartMaxLv { get; private set; }
  public void Load()
  {
    dictCartData.Clear();

    string cartDataJson;

    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.CartInfo, out cartDataJson))
    {
      List<CartData> cartDataList = JsonConvert.DeserializeObject<List<CartData>>(cartDataJson);

      //dictCartData 초기화
      foreach (var data in cartDataList)
      {
        if (!dictCartData.ContainsKey(data.cartLevel))
        {
          dictCartData.Add(data.cartLevel, data);
        }
        else
          Debug.Log("CartData Table Load Error Index : " + data.cartLevel);

        if(cartMaxLv < data.cartLevel)
          cartMaxLv = data.cartLevel;
      }

      //dictCartSpineData 초기화
      foreach (var data in cartDataList)
      {
        if (!dictCartSpineData.ContainsKey(data.cartSpine))
        {
          dictCartSpineData.Add(data.cartSpine, (data.cartLevel, data.cartLevel));
        }
        else
        {
          int minLv = dictCartSpineData[data.cartSpine].minLv;
          int maxLv = dictCartSpineData[data.cartSpine].maxLv;

          if (maxLv < data.cartLevel)
            maxLv = data.cartLevel;

          dictCartSpineData[data.cartSpine] = (minLv, maxLv);
        }
  
      }

      //cartCostumeList 초기화
      foreach (var data in cartDataList)
      {
        if (!cartCostumeList.Exists(n => n.cartSpine == data.cartSpine))
          cartCostumeList.Add(data);
      }

      Debug.Log("CartData Table Load Success");
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }


  

  public WagonType GetUserWagonType()
  {
    var userCartData = GameDataManager.getInstance.userContentsData.contentCart.cartInfoData;

    CartData cartData = GetCartData(userCartData.cartLv);

    bool isCostume = userCartData.selectedCostumeIdx != 0;

    if (isCostume)
    {
      int costumeIdx = userCartData.selectedCostumeIdx;

      cartData = GetCartDataIndex(costumeIdx);
    }

    return GetWagonType(cartData);
  }

  public WagonType GetWagonType(CartData cartData)
  {
    return GetWagonTypeBySpine(cartData.cartSpine);
  }

  /// <summary>
  /// Cart Spine 기반으로 현재 수레 작업되어있는 Enum 매칭 시켜서 반환하는 코드
  /// </summary>
  /// <param name="cartSpine"></param>
  /// <returns></returns>
  private WagonType GetWagonTypeBySpine(string cartSpine) => cartSpine switch
  {
    "basic_wagon"        => WagonType.Basic,
    "coconut_crab_wagon" => WagonType.Crab,
    "pig_wagon"          => WagonType.Animal,
    "crystal_deer_wagon" => WagonType.Animals

  };

  public CartData GetCostumeData(int index)
  {
    if (cartCostumeList.Count > index)
      return cartCostumeList[index];
    else
      return default;
  }

  public int GetCostumeIdx(string cartSpine)
  {
    int findIndex = cartCostumeList.FindIndex(n => n.cartSpine == cartSpine);

    return findIndex;
  }

  public (int minLv, int maxLv) GetLevelRange(string cartSpine)
  {
    return dictCartSpineData[cartSpine];
  }

  /// <summary>
  /// 스파인 이름에 따른 레벨 범위 반환
  /// </summary>
  /// <param name="cartSpine"></param>
  /// <returns></returns>
  public string GetLevelRangeText(string cartSpine)
  {
    int minLv = dictCartSpineData[cartSpine].minLv;
    int maxLv = dictCartSpineData[cartSpine].maxLv;

    return $"{minLv} ~ {maxLv}";
  }

  public CartData GetCartData(int cartLv)
  {
    if (dictCartData.ContainsKey(cartLv))
      return dictCartData[cartLv];
    else
      return default;
  }

  public CartData GetCartDataIndex(int cartIdx)
  {
    foreach (var cartData in dictCartData)
    {
      if (cartData.Value.cartIdx == cartIdx)
        return cartData.Value;
    }

    return default;
  }
}
