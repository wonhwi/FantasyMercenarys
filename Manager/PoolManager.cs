using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class PoolManager : LazySingleton<PoolManager>
{
    private Dictionary<Type, NewObjectPool<IPoolable>> poolDict = null;
    private Dictionary<string, NewObjectPool<IPoolable>> monsterPoolDict = null;
  private Dictionary<string, NewObjectPool<IPoolable>> partnerPoolDict = null;
  private Dictionary<SkillType, NewObjectPool<IPoolable>> skillPoolDict = null;


    public PoolManager()
    {
        poolDict = new Dictionary<Type, NewObjectPool<IPoolable>>();
        monsterPoolDict = new Dictionary<string, NewObjectPool<IPoolable>>();
        skillPoolDict = new Dictionary<SkillType, NewObjectPool<IPoolable>>();
        partnerPoolDict = new Dictionary<string, NewObjectPool<IPoolable>>();
        poolDict.Clear();
        monsterPoolDict.Clear();
        skillPoolDict.Clear();
        partnerPoolDict.Clear();
    }

    ~PoolManager()
    {
        poolDict.Clear();
        monsterPoolDict.Clear();
        skillPoolDict.Clear();
        partnerPoolDict.Clear();
        poolDict = null;
        monsterPoolDict = null;
        skillPoolDict = null;
        partnerPoolDict = null;
    }

    public void RegisterObjectPool<T>(NewObjectPool<IPoolable> _pool) where T : IPoolable
    {
        Type type = typeof(T);
        if (poolDict.ContainsKey(type))
        {
            throw new Exception("Duplication Key");
        }

        poolDict.Add(type, _pool);
    }

    public void RemoveObjectPool<T>() where T : IPoolable
    {
        Type type = typeof(T);
        if (!poolDict.ContainsKey(type))
        {
            throw new KeyNotFoundException();
        }

        var pool = poolDict[type];
        pool.OnRelease();

        poolDict.Remove(type);
    }

    public NewObjectPool<IPoolable> GetObjectPool<T>() where T : IPoolable
    {
        poolDict.TryGetValue(typeof(T), out var pool);
        if (pool == null)
        {
            //.. FIXME? :: throw를 던질까?
            pool = new NewObjectPool<IPoolable>();
            RegisterObjectPool<T>(pool);
        }

        return pool;
    }

    public Dictionary<string, NewObjectPool<IPoolable>> GetMonsterObjectPool<T>(string _key) where T : IPoolable
    {
        monsterPoolDict.TryGetValue(_key, out var pool);
        if(pool == null)
        {
            pool = new NewObjectPool<IPoolable>();
            monsterPoolDict.Add(_key, pool);
        }
        return monsterPoolDict;
    }

  public Dictionary<SkillType, NewObjectPool<IPoolable>> GetSkillObjectPool<T>(SkillType skillType) where T : IPoolable
  {
    skillPoolDict.TryGetValue(skillType, out var pool);
    if (pool == null)
    {
      pool = new NewObjectPool<IPoolable>();
      skillPoolDict.Add(skillType, pool);
    }
    return skillPoolDict;
  }

  public Dictionary<string, NewObjectPool<IPoolable>> GetPartnerObjectPool<T>(string _key) where T : IPoolable
  {
    partnerPoolDict.TryGetValue(_key, out var pool);
    if (pool == null)
    {
      pool = new NewObjectPool<IPoolable>();
      partnerPoolDict.Add(_key, pool);
    }
    return partnerPoolDict;
  }



}
