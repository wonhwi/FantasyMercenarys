using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LanguageTable : LazySingleton<LanguageTable>, ITableFactory
{
  public Dictionary<string, LanguageData> dictLanguageData = new Dictionary<string, LanguageData>();

  public void Load()
  {
    dictLanguageData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_LANGUAGE_FILE_NAME;

        //if (File.Exists(fileFath))
        //{
        //  string fileData = File.ReadAllText(fileFath);
        //  if (fileData != null)
        //  {
        //    List<LanguageData> languageDataList = JsonConvert.DeserializeObject<List<LanguageData>>(fileData);
        //    foreach (var languageData in languageDataList)
        //    {
        //      if (!dictLanguageData.ContainsKey(languageData.recordCd))
        //      {
        //        dictLanguageData.Add(languageData.recordCd, languageData);
        //      }
        //    }
        //  }
        //}
        string languageJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Language, out languageJson))
        {
            List<LanguageData> languageDataList = JsonConvert.DeserializeObject<List<LanguageData>>(languageJson);
            foreach (var data in languageDataList)
            {
                if (!dictLanguageData.ContainsKey(data.recordCd))
                {
                    dictLanguageData.Add(data.recordCd, data);
                }
                else
                    Debug.Log("Language Table Load Error recordCd : " + data.recordCd);
            }
            Debug.Log("Language Table Load Success");
        }
  }

  /// <summary>
  /// 포멧팅된 String 반환
  /// </summary>
  /// <param name="languageCode">언어코드</param>
  /// <param name="messageParams">포멧팅 String Params</param>
  /// <returns></returns>
  public string GetLanguage(string languageCode, params object[] messageParams) => string.Format(GetLanguage(languageCode), messageParams);


  /// <summary>
  /// 언어코드 번역된 String 반환
  /// </summary>
  /// <param name="recordCd">언어 코드</param>
  /// <returns></returns>
  public string GetLanguage(string recordCd)
  {
    if (string.IsNullOrEmpty(recordCd))
      return "none";

    //Language 누락 데이터 확인용으로 따로 뺌
    if (!dictLanguageData.ContainsKey(recordCd))
      return recordCd;

    //이 부분 SettingManager 쪽에서 받아와서 작업 하는거로 생각 중입니다.
    //SettingManager의 Language는 PlayerPrefs 방식 생각하고 있고, 최초 접속시 Application Language를 받게 할 생각
    switch (Application.systemLanguage)
    {
      case SystemLanguage.Korean:
        return dictLanguageData[recordCd].kr;
      case SystemLanguage.Japanese:
        return dictLanguageData[recordCd].jp;
      default:
        return dictLanguageData[recordCd].en;
    }
  }

  public string GetLanguageColor(string languageCode, string colorHexCode)
  {
    return $"<color={colorHexCode}>{GetLanguage(languageCode)}</color>"; 
  }


  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
