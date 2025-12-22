using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryTicketTable : LazySingleton<LotteryTicketTable>, ITableFactory
{
  public Dictionary<int, LotteryTicketData> dictLotteryTicketData = new Dictionary<int, LotteryTicketData>();


  public void Load()
  {
    dictLotteryTicketData.Clear();

    string lotteryTicketJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.LotteryTicket, out lotteryTicketJson))
    {
      List<LotteryTicketData> lotteryTicketDataList = JsonConvert.DeserializeObject<List<LotteryTicketData>>(lotteryTicketJson);

      foreach (var data in lotteryTicketDataList)
      {
        if (!dictLotteryTicketData.ContainsKey(data.lotteryIdx))
        {
          dictLotteryTicketData.Add(data.lotteryIdx, data);
        }
        else
          Debug.Log("LotteryTicketData Table Load Error Index : " + data.lotteryIdx);
      }
      Debug.Log("LotteryTicketData Table Load Success");
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }

}
