using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public enum UIPoolType
{
  EquipmentItemGrade,
  ItemSelect
}

public class UIPoolManager : SingletonTemplate<UIPoolManager>
{
  private Dictionary<PartnerSpineType, ObjectPool<IPoolableUI>> uiPartnerSpineDict = new Dictionary<PartnerSpineType, ObjectPool<IPoolableUI>>();

  private Dictionary<UIPoolType, ObjectPool<IPoolableUI>> uiPoolDict = new Dictionary<UIPoolType, ObjectPool<IPoolableUI>>();

  protected override void AwakeSetting()
  {
    base.AwakeSetting();
    
    foreach (PartnerSpineType type in Enum.GetValues(typeof(PartnerSpineType)))
      InitUIPooling<UISpineController>(type, string.Format(NewResourcePath.PATH_UI_SPINE_PARTNER_PREFAB, type), 10);

    InitUIPooling<UISpineGrade>(UIPoolType.EquipmentItemGrade, NewResourcePath.PREFAB_UI_ITEM_GRADE, 10);
    InitUIPooling<UISpineSelect>(UIPoolType.ItemSelect, NewResourcePath.PREFAB_UI_ITEM_SELECT, 5);

  }



  public void InitUIPooling<T>(PartnerSpineType spineType, string path, int defaultCapacity) where T : IPoolableUI
  {
    if (!uiPartnerSpineDict.ContainsKey(spineType))
    {
      ObjectPool<IPoolableUI> pool = new ObjectPool<IPoolableUI>(
        createFunc      : () => CreatePoolObject<T>(path),
        actionOnGet     : OnGetObjectPool,
        actionOnRelease : OnReleasePool,
        actionOnDestroy : OnDestroyPool,
        collectionCheck : false,
        defaultCapacity : defaultCapacity
        );

      uiPartnerSpineDict.Add(spineType, pool);
    }
  }

  public void InitUIPooling<T>(UIPoolType poolType, string path, int defaultCapacity) where T : IPoolableUI
  {
    if (!uiPoolDict.ContainsKey(poolType))
    {
      ObjectPool<IPoolableUI> pool = new ObjectPool<IPoolableUI>(
        createFunc: () => CreatePoolObject<T>(path),
        actionOnGet: OnGetObjectPool,
        actionOnRelease: OnReleasePool,
        actionOnDestroy: OnDestroyPool,
        collectionCheck: false,
        defaultCapacity: defaultCapacity
        );

      uiPoolDict.Add(poolType, pool);
    }
  }


  protected T CreatePoolObject<T>(string path) where T : IPoolableUI
  {
    GameObject prefab = NewResourceManager.getInstance.LoadResource<GameObject>(path);

    if (prefab == null)
    {
      Debug.LogError($"Prefab not found at path: {path}");
    }

    GameObject poolObject = Instantiate(prefab, this.transform);

    T component = poolObject.GetComponent<T>();

    component.OnDeactivate();

    return component;
  }

  protected void OnGetObjectPool<T>(T pool) where T : IPoolableUI
  {
    pool.OnActivate();

  }

  protected void OnReleasePool<T>(T pool) where T : IPoolableUI
  {
    pool.OnSetParent(this.transform);
    pool.OnDeactivate();
  }

  protected void OnDestroyPool<T>(T pool) where T : IPoolableUI
  {
    //Destroy(itemSlot.gameObject);
  }

  public T GetPool<T>(PartnerSpineType spineType) where T : class
  {
    return uiPartnerSpineDict[spineType].Get() as T;
  }

  public void ClearReturnUIPool<T>(PartnerSpineType spineType, Transform tfParent) where T : class
  {
    ObjectPool<IPoolableUI> pool = uiPartnerSpineDict[spineType];

    for (int i = tfParent.childCount - 1; i >= 0; i--)
    {
      pool.Release(tfParent.GetChild(i).GetComponent<IPoolableUI>());
    }
  }

  public void ClearReturnUIPool<T>(UIPoolType uiPoolType, Transform tfParent) where T : class
  {
    ObjectPool<IPoolableUI> pool = uiPoolDict[uiPoolType];

    for (int i = tfParent.childCount - 1; i >= 0; i--)
    {
      pool.Release(tfParent.GetChild(i).GetComponent<IPoolableUI>());
    }
  }

  public void ClearReturnUIPool<T>(UIPoolType uiPoolType, IPoolableUI target) where T : class
  {
    ObjectPool<IPoolableUI> pool = uiPoolDict[uiPoolType];

    pool.Release(target);
  }

  public T GetPool<T>(UIPoolType uiPoolType) where T : class
  {
    return uiPoolDict[uiPoolType].Get() as T;
  }

}
