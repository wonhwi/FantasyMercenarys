using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SkillTable : LazySingleton<SkillTable>, ITableFactory
{
  public Dictionary<int, IdleSkillData> dictSkillData = new Dictionary<int, IdleSkillData>();     //스킬 전체 데이터
  public Dictionary<int, IdleSkillData> dicCommonSkillData = new Dictionary<int, IdleSkillData>();   //공용 - 직업 평타 스킬
  public Dictionary<int, IdleSkillData> dicJobSkillData = new Dictionary<int, IdleSkillData>();      //직업 - 직업 고유 스킬(액티브, 패시브)
  public Dictionary<int, IdleSkillData> dicItemSkillData = new Dictionary<int, IdleSkillData>();     //아이템 - 아이템 스킬
  public Dictionary<int, IdleSkillData> dicPartnerSkillData = new Dictionary<int, IdleSkillData>();  //파트너 - 파트너 스킬

  public Dictionary<ItemGradeType, int> dictGradeCombineCount = new Dictionary<ItemGradeType, int>();

  public void Load()
  {
    dictSkillData.Clear();
    dicCommonSkillData.Clear();
    dicJobSkillData.Clear();
    dicItemSkillData.Clear();
    dicPartnerSkillData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_SKILL_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //  string fileData = File.ReadAllText(fileFath);
        //  if (fileData != null)
        //  {
        //    List<IdleSkillData> skillDataList = JsonConvert.DeserializeObject<List<IdleSkillData>>(fileData);
        //    foreach (var skillData in skillDataList)
        //    {
        //      if (!dictSkillData.ContainsKey(skillData.skillIdx))
        //      {
        //        AddSkillData(skillData);
        //      }
        //    }
        //  }
        //}
        string skillJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Skill, out skillJson))
        {
            List<IdleSkillData> skillDataList = JsonConvert.DeserializeObject<List<IdleSkillData>>(skillJson);
            foreach (var data in skillDataList)
            {
                if (!dictSkillData.ContainsKey(data.skillIdx))
                {
                    AddSkillData(data);
                    AddGradeCombineCount(data);
                }
                else
                    Debug.Log("Skill Table Load Error Index : " + data.skillIdx);
            }
            Debug.Log("Skill Table Load Success");
        }

    }

  private void AddSkillData(IdleSkillData skillData)
  {
    dictSkillData.Add(skillData.skillIdx, skillData);

    switch ((SkillCategory)skillData.skillCategory)
    {
      case SkillCategory.Common:
        dicCommonSkillData.Add(skillData.skillIdx, skillData);
        break;
      case SkillCategory.Job:
        dicJobSkillData.Add(skillData.skillIdx, skillData);
        break;
      case SkillCategory.Item:
        dicItemSkillData.Add(skillData.skillIdx, skillData);
        break;
      case SkillCategory.Partner:
        dicPartnerSkillData.Add(skillData.skillIdx, skillData);
        break;
    }
  }

  private void AddGradeCombineCount(IdleSkillData skillData)
  {
    ItemGradeType gradeType = (ItemGradeType)skillData.skillGrade;

    if(skillData.isGetCombine)
    {
      if (dictGradeCombineCount.ContainsKey(gradeType))
      {
        dictGradeCombineCount[gradeType]++;
      }
      else
      {
        dictGradeCombineCount.Add(gradeType, 1);
      }
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }


  public int GetSkillCount(SkillCategory skillType) => skillType switch
  {
    SkillCategory.Common  => dicCommonSkillData.Count,
    SkillCategory.Job     => dicJobSkillData.Count,
    SkillCategory.Item    => dicItemSkillData.Count,
    SkillCategory.Partner => dicPartnerSkillData.Count,
  };

  public IdleSkillData GetSkillData(int _key)
  {
    if (dictSkillData.ContainsKey(_key))
    {
      return dictSkillData[_key];
    }
    else
    {
      //throw new System.NotImplementedException();
      Debug.Log($"No Skill Data key : {_key}");
      return default;

    }
  }

  public IdleSkillData GetSkillData(SkillCategory skillType, int _key) => skillType switch
  {
    SkillCategory.Common    => dicCommonSkillData.ContainsKey(_key)  ?   dicCommonSkillData[_key]  :  default,
    SkillCategory.Job       => dicJobSkillData.ContainsKey(_key)     ?   dicJobSkillData[_key]     :  default,
    SkillCategory.Item      => dicItemSkillData.ContainsKey(_key)    ?   dicItemSkillData[_key]    :  default,
    SkillCategory.Partner   => dicPartnerSkillData.ContainsKey(_key) ?   dicPartnerSkillData[_key] :  default,
    _                   => dictSkillData.ContainsKey(_key)       ?   dictSkillData[_key]       :  default
  };

  public Dictionary<int, IdleSkillData> GetSkillDic(SkillCategory skillType) => skillType switch
  {
    SkillCategory.Common  => dicCommonSkillData,
    SkillCategory.Job     => dicJobSkillData, 
    SkillCategory.Item    => dicItemSkillData,
    SkillCategory.Partner => dicPartnerSkillData,
    _ => dictSkillData,
  };


  StringBuilder stringBuilder = new StringBuilder();


  /// <summary>
  /// 직업 특징 받아오기
  /// </summary>
  /// <param name="jobData"></param>
  /// <returns></returns>
  public string GetChracteristics(JobData jobData)
  {
    stringBuilder.Clear();

    IdleSkillData[] idleSkillDatas = this.GetJobSkillDatas(jobData);

    int index = 0;

    for (int i = 0; i < idleSkillDatas.Length; i++)
    {
      IdleSkillData idleSkillData = idleSkillDatas[i];

      bool isPassive =
        idleSkillData.skillActionType.Equals((int)SkillActionType.PassiveDebuff) ||
        idleSkillData.skillActionType.Equals((int)SkillActionType.PassiveBuff);

      if (!idleSkillData.IsNull() && isPassive)
      {
        if(index > 0)
        {
          stringBuilder.Append(index % 2 == 0 ? ",<br>" : ",");
        }

        stringBuilder.Append(LanguageTable.getInstance.GetLanguage(idleSkillData.skillRecordCd));

        index++;
      }

    }

    return stringBuilder.ToString();
  }


  public IdleSkillData[] GetJobSkillData(int _jobIndex)
  {
    JobData jobData = JobTable.getInstance.GetJobData(_jobIndex);

    return GetJobSkillDatas(jobData);
  }
  public IdleSkillData[] GetJobSkillDatas(JobData jobData)
  {
    IdleSkillData[] idleSkillDatas = new IdleSkillData[10]
    {
      GetSkillData(SkillCategory.Job, jobData.jobSkill1),
      GetSkillData(SkillCategory.Job, jobData.jobSkill2),
      GetSkillData(SkillCategory.Job, jobData.jobSkill3),
      GetSkillData(SkillCategory.Job, jobData.jobSkill4),
      GetSkillData(SkillCategory.Job, jobData.jobSkill5),
      GetSkillData(SkillCategory.Job, jobData.jobSkill6),
      GetSkillData(SkillCategory.Job, jobData.jobSkill7),
      GetSkillData(SkillCategory.Job, jobData.jobSkill8),
      GetSkillData(SkillCategory.Job, jobData.jobSkill9),
      GetSkillData(SkillCategory.Job, jobData.jobSkill10),
    };

    return idleSkillDatas;
  }

  public IdleSkillData GetJobActiveSkill(int jobIndex)
  {
    JobData jobData = JobTable.getInstance.GetJobData(jobIndex);

    IdleSkillData[] idleSkillDatas = GetJobSkillDatas(jobData);

    
    for (int i = 0; i < idleSkillDatas.Length; i++)
    {
      if (idleSkillDatas[i].IsNull())
        continue;

      bool isActive = IsActiveSkill(idleSkillDatas[i]);

      if (isActive)
        return idleSkillDatas[i];
    }

    return default;
  }

  public bool IsActiveSkill(IdleSkillData skillData)
  {
    return skillData.skillActionType.Equals((int)SkillActionType.ActiveDebuff) ||
           skillData.skillActionType.Equals((int)SkillActionType.ActiveBuff) ||
           skillData.skillActionType.Equals((int)SkillActionType.ActiveCrowdControl);
  }

  public IEnumerable<IdleSkillData> GetJobPassiveSkill(int jobIndex)
  {
    JobData jobData = JobTable.getInstance.GetJobData(jobIndex);

    IdleSkillData[] idleSkillDatas = GetJobSkillDatas(jobData);

    foreach (var skillData in idleSkillDatas)
    {
      if (skillData.IsNull())
        continue;

      if (skillData.skillActionType == (int)SkillActionType.PassiveBuff)
      {
        yield return skillData;
      }
    }
  }


  /// <summary>
  /// 플레이어의 모든 스킬들에 대한 보유 효과 값 반환
  /// </summary>
  /// <returns></returns>
  public float GetHasSkillsStat()
  {
    float totalValue = 0f;

    var itemMap = GameDataManager.getInstance.dictSkill;

    foreach (var item in itemMap)
    {
      InvenData invenData = item.Value;

      if (invenData != null)
      {
        int itemLevel = invenData.itemLv;

        totalValue += GetSkillHasValue(invenData.itemIdx, itemLevel);
      }
    }

    return totalValue;
  }

  /// <summary>
  /// 스킬 보유 효과 값 반환
  /// </summary>
  /// <param name="itemIdx"></param>
  /// <param name="itemLevel"></param>
  /// <returns></returns>
  public float GetSkillHasValue(int itemIdx, int itemLevel)
  {
    IdleSkillData skillData = this.GetSkillData(SkillCategory.Item, itemIdx);

    return skillData.ownershipValue + skillData.ownershipIncrease * itemLevel;
  }

}


