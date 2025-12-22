using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MultipleSkeletonSkin : SpineSkinBase
{
  [SerializeField] private SkeletonDataContainer skeletonDataContainer;
  
  private StringBuilder stringBuilder = new StringBuilder();

  public SkeletonDataContainer SkeletonDataContainer
  {
    get
    {
      return skeletonDataContainer;
    }
    set
    {
      skeletonDataContainer = value;


    }
  }

  public override int SkinCount => skeletonDataContainer.GetSkeletonAnimationCount();

  /// <summary>
  /// 원하는 파츠 Default Skin 설정
  /// </summary>
  /// <param name="costumePart"></param>
  public void SetDefaultSkin(CostumePart costumePart)
  {
    this.SetSkin(costumePart, string.Format(ConstantManager.DEFAULT_SKIN_NAME, costumePart.ToString().ToLower()));
  }

  /// <summary>
  /// 모든 파츠 Default Skin 설정
  /// </summary>
  public void SetAllDefaultSkin()
  {
    for (int i = 0; i < skeletonDataContainer.skeletonDataList.Length; i++)
    {
      CostumePart costumePart = skeletonDataContainer.skeletonDataList[i].costumePart;

      SetDefaultSkin(costumePart);
    }
  }

  public override void SetSkin(CostumePart costumePart, string skinName)
  {
    stringBuilder.Clear();

    var skeletonList = SkeletonDataContainer.GetSkeletonAnimation(costumePart);

    if (skeletonList == null)
    {
      Debug.Log($"Skeleton Data가 없습니다 파츠 : {costumePart}, 이름 :{skinName}", this.transform);
      return;
    }

    if(skeletonList.Length == 0)
    {
      Debug.Log("컴포넌트에 할당된 SkeletonData 가 없습니다", this.transform);
      return;
    }

    stringBuilder.Append(costumePart.ToString().ToLower());

    if (skeletonList.Length > 1)
    {
      SpineLayerData spineLayerData = SpineLayerTable.getInstance.GetSpineLayerData(skinName);

      if (spineLayerData.IsNull())
      {
        if(!skinName.Contains("default"))
          Debug.Log($"spineLayerData가 없습니다 {stringBuilder}/{skinName} 추가 해주세요, Default Skin을 적용 시킵니다");

        base.SetSkin(skeletonList[0].skeleton, $"{stringBuilder}/{string.Format(ConstantManager.DEFAULT_SKIN_NAME, stringBuilder)}_F");
        base.SetSkin(skeletonList[1].skeleton, $"{stringBuilder}/{string.Format(ConstantManager.DEFAULT_SKIN_NAME, stringBuilder)}_B");

        return;
      }

      //Debug.LogError($"{stringBuilder}/{spineLayerData.spineSkin} 적용");
      base.SetSkin(skeletonList[0].skeleton, $"{stringBuilder}/{spineLayerData.front}");
      base.SetSkin(skeletonList[1].skeleton, $"{stringBuilder}/{spineLayerData.back}");
      return;
    }

    base.SetSkin(skeletonList[0].skeleton, $"{stringBuilder}/{skinName}");
  }

  public override void SetLayer(int orderLayer)
  {
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].GetComponent<MeshRenderer>().sortingOrder = orderLayer;
      }
    }

  }

  public override void SetColor(Color color)
  {
    for (int i = 0; i < SkeletonDataContainer.skeletonDataList.Length; i++)
    {
      for (int j = 0; j < SkeletonDataContainer.skeletonDataList[i].skeletonAnimations.Length; j++)
      {
        SkeletonDataContainer.skeletonDataList[i].skeletonAnimations[j].skeleton.SetColor(color);
      }
    }
  }

  protected override void SetInitialSkin(string skinName) { }

}
