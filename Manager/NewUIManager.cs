using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class NewUIManager : LazySingleton<NewUIManager>
{
    private Dictionary<Type, UIBaseController> cachedPanelDict = null;
    
    private Stack<UIBaseController> panelStack = null;

    public event Action OnHideCallback;

    private const int POPUP_SORTING_ORDER = 100;

    public NewUIManager()
    {
        cachedPanelDict = new Dictionary<Type, UIBaseController>();
        cachedPanelDict.Clear();

        panelStack = new Stack<UIBaseController>();
        panelStack.Clear();
    }

    ~NewUIManager()
    {
        ClearPanels();

        cachedPanelDict = null;        
        panelStack = null;
    }

    public async UniTask<T> Show<T>(string _panelName = "", int _order = 0, bool _setAutoOrder = true) where T : UIBaseController
    {        
        var panel = await GetCachedPanel<T>(_panelName);
        if (!panel.IsShow())
        {
            panel.Show();
        }
        panelStack.Push(panel);
        if(_setAutoOrder)
          panel.SetSortingOrder(POPUP_SORTING_ORDER + panelStack.Count);

        return panel;
    }

    public void Hide()
    {
        if (panelStack.Count > 0)
        {            
            var panel = panelStack.Pop();
            panel.Hide();
            OnHideCallback?.Invoke();            
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("panelStack Count 0!!!!");
#endif
        }
    }    

    public async UniTask<T> GetCachedPanel<T>(string _panelName = "") where T : UIBaseController
    {
        cachedPanelDict.TryGetValue(typeof(T), out var panel);
        if (panel == null)
        {
            panel = await AddCachePanel<T>(_panelName);
        }

        return (T)panel;
    }

    private async UniTask<T> AddCachePanel<T>(string _panelName = "") where T : UIBaseController
    {
        var panel = FindPanelInHierarchy<T>(_panelName);
        if (panel == null)
        {
            panel = await CreatePanel<T>(_panelName);
        }

        if (!cachedPanelDict.ContainsKey(typeof(T)))
        {
            cachedPanelDict.Add(typeof(T), panel);
        }

        return panel;
    }

    private async UniTask<T> CreatePanel<T>(string _prefabPath = "") where T : UIBaseController
    {
        var prefab = await Resources.LoadAsync($"Prefabs/{_prefabPath}", typeof(GameObject)) as GameObject;
        var panelObject = GameObject.Instantiate(prefab);

        return panelObject.GetComponent<T>();
    }

    private T FindPanelInHierarchy<T>(string _panelName) where T : UIBaseController
    {
        GameObject panelObj = UnityExtension.Find(_panelName);
        if (panelObj == null)
        {
            //throw new NullReferenceException();
            return null;
        }

        return panelObj.GetComponent<T>();
    }

    public void ClearAllCachedPanel()
    {
        cachedPanelDict.Clear();
    }

    public void ClearAllPanelStack()
    {
        panelStack.Clear();
    }

    public void ClearPanels()
    {
        cachedPanelDict.Clear();
        panelStack.Clear();        
    }
}
