using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class JobTable : LazySingleton<JobTable>, ITableFactory
{
  private Dictionary<int, JobData> dictJobData = new Dictionary<int, JobData>();

  public JobData CommonerData;
  public Dictionary<int, JobData> dictFirstJobData = new Dictionary<int, JobData>();
  public Dictionary<int, JobData> dictSecondJobData = new Dictionary<int, JobData>();
  public Dictionary<int, JobData> dictThirdJobData = new Dictionary<int, JobData>();
  public Dictionary<int, JobData> dictFourthJobData = new Dictionary<int, JobData>();
  public Dictionary<int, JobData> dictFifthJobData = new Dictionary<int, JobData>();

  public Dictionary<int, JobData> GetJobDataDict()
    => dictJobData;

  public void Load()
  {
    dictJobData.Clear();
    dictFirstJobData.Clear();
    dictSecondJobData.Clear();
    dictThirdJobData.Clear();
    dictFourthJobData.Clear();
    dictFifthJobData.Clear();

    string jobJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.JobInfo, out jobJson))
    {
      List<JobData> jobDataList = JsonConvert.DeserializeObject<List<JobData>>(jobJson);
      foreach (var data in jobDataList)
      {
        if (!dictJobData.ContainsKey(data.jobIdx))
        {
          if (data.jobIdx.Equals(0))
            continue;

          AddJobData(data);
        }
        else
          Debug.Log("JobInfo Table Load Error Index : " + data.jobIdx);
      }
      Debug.Log("JobInfo Table Load Success");
    }
  }

  private void AddJobData(JobData jobData)
  {
    dictJobData.Add(jobData.jobIdx, jobData);


    switch ((JobChangeType)jobData.jobChangeGrade)
    {
      case JobChangeType.Commoner:
        CommonerData = jobData;
        break;
      case JobChangeType.FirstChange:
        dictFirstJobData.Add(jobData.jobIdx, jobData);
        break;
      case JobChangeType.SecondChange:
        dictSecondJobData.Add(jobData.jobIdx, jobData);
        break;

      case JobChangeType.ThirdChange:
        dictThirdJobData.Add(jobData.jobIdx, jobData);
        break;

      case JobChangeType.FourthChange:
        dictFourthJobData.Add(jobData.jobIdx, jobData);
        break;

      case JobChangeType.FifthChange:
        dictFifthJobData.Add(jobData.jobIdx, jobData);
        break;
    }
  }

  public Dictionary<int, JobData> GetJobChangeData(JobChangeType jobChangeType) => jobChangeType switch
  {
    JobChangeType.FirstChange  => dictFirstJobData,
    JobChangeType.SecondChange => dictSecondJobData,
    JobChangeType.ThirdChange  => dictThirdJobData,
    JobChangeType.FourthChange => dictFourthJobData,
    JobChangeType.FifthChange  => dictFifthJobData,
  };


  public JobData GetCurrentJobData()
  {
    return GetJobData(GameDataManager.getInstance.userInfoModel.GetJobCode());
  }

  public JobData GetJobData(int _key)
  {
    if (dictJobData.ContainsKey(_key))
    {
      return dictJobData[_key];
    }
    else
    {
      Debug.Log($"No Job Data key : {_key}");
      return default;
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
