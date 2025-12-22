using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;



public class BlessingStatueTable : LazySingleton<BlessingStatueTable>, ITableFactory
{

  public Dictionary<GradeType, List<BlessingStatueData>> dictBlessingStatueData = new Dictionary<GradeType, List<BlessingStatueData>>();

  public void Load()
  {
    dictBlessingStatueData.Clear();
    string blessingStatueJson;

    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.BlessingStatue, out blessingStatueJson))
    {
      List<BlessingStatueData> blessingStatueDataList = JsonConvert.DeserializeObject<List<BlessingStatueData>>(blessingStatueJson);
      foreach (var data in blessingStatueDataList)
      {
        if (!dictBlessingStatueData.ContainsKey((GradeType)data.blessingGrade))
          dictBlessingStatueData.Add((GradeType)data.blessingGrade, new List<BlessingStatueData>());

        dictBlessingStatueData[(GradeType)data.blessingGrade].Add(data);


      }
      Debug.Log("BlessingStatueData Table Load Success");
    }
  }

  public List<BlessingStatueData> GetBlessingStatueData(GradeType buffGradeType)
  {
    return dictBlessingStatueData[buffGradeType];
  }

  public int GetBlessingStatueDataCount(GradeType buffGradeType)
  {
    return dictBlessingStatueData[buffGradeType].Count;
  }


  public void Reload()
  {
    throw new System.NotImplementedException();
  }

}


