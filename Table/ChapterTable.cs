using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChapterTable : LazySingleton<ChapterTable>, ITableFactory
{
    public Dictionary<int, ChapterData> dictChapterData = new Dictionary<int, ChapterData>();

    private const int REGION_CHAPTER_COUNT = 10;

    public void Load()
    {
        dictChapterData.Clear();

        //string tempPath = Application.dataPath;
        //string ChangeTempPath = Path.GetFullPath(Path.Combine(tempPath, @"../"));
        //ChangeTempPath = Path.GetDirectoryName(ChangeTempPath);
        //string newTempPath = ChangeTempPath.Replace("\\", "/");
        //string fileFath = newTempPath + ConstantManager.TABLE_DEFAULT_PATH + ConstantManager.TABLE_CHAPTHER_FILE_NAME;

        ////int index = 0;
        //if (File.Exists(fileFath))
        //{
        //    string fileData = File.ReadAllText(fileFath);
        //    if (fileData != null)
        //    {
        //        List<ChapterData> chapterDataList = JsonConvert.DeserializeObject<List<ChapterData>>(fileData);
        //        foreach (var chapterData in chapterDataList)
        //        {
        //            if (!dictChapterData.ContainsKey(chapterData.chapterIdx))
        //            {                        
        //                dictChapterData.Add(chapterData.chapterIdx, chapterData);
        //            }
        //        }
        //    }
        //}
        string chapterJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.ChapterInfo, out chapterJson))
        {
            List<ChapterData> chapterDataList = JsonConvert.DeserializeObject<List<ChapterData>>(chapterJson);
            foreach (var data in chapterDataList)
            {
                if (!dictChapterData.ContainsKey(data.chapterIdx))
                {
                    dictChapterData.Add(data.chapterIdx, data);
                }
                else
                    Debug.Log("Chapter Table Load Error Index : " + data.chapterIdx);
            }
            Debug.Log("Chapter Table Load Success");
        }
    }

    public ChapterData GetChapterData(int _key)
    {
        if (dictChapterData.ContainsKey(_key))
        {
            return dictChapterData[_key];
        }
        else
        {
            Debug.Log($"No Chapter Data Key : {_key}");
            throw new System.NotImplementedException();
        }
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
