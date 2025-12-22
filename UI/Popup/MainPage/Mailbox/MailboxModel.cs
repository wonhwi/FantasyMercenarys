using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotState = MailboxView.SlotState;

public class MailboxModel
{
  public List<MailData> mailDataList { get; private set; } = new List<MailData>();

  public void LoadMailDataList(List<MailData> mailDataList)
  {
    this.mailDataList = mailDataList;
  }

  public void UpdateMailData(MailData mailData)
  {
    int findIndex = mailDataList.FindIndex(n => n.mailIdx == mailData.mailIdx);

    if(findIndex != -1)
    {
      mailDataList[findIndex] = mailData;
    }
  }

  public MailData GetMailData(long mailIdx)
  {
    return mailDataList.Find(n => n.mailIdx == mailIdx);
  }

}
