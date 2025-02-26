using System;
using System.Collections.Generic;

public enum StateKey
{
    HasLeg = 1 << 0,    // 0000 0001
    IsWalking = 1 << 1, // 0000 0010
    CanFly = 1 << 2,    // 0000 0100
    HasTarget = 1 << 3, // 0000 1000
    IsNearby = 1 << 4,   // 0001 0000
}

public class GoapStatePool
{
    // 单例实例
    private static GoapStatePool _instance;
    public static GoapStatePool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GoapStatePool();
            }
            return _instance;
        }
    }

    // 状态池，存储状态实例
    private readonly Dictionary<int, GoapWorldState> _statePool = new Dictionary<int, GoapWorldState>();

    // 私有构造函数，确保单例
    private GoapStatePool() { }

    /// <summary>
    /// 获取或创建状态实例
    /// </summary>
    public GoapWorldState GetOrCreateState(Dictionary<StateKey, bool> stateData)
    {
        int hash = CalculateHash(stateData);
        if (_statePool.TryGetValue(hash, out var existingState))
        {
            return existingState;
        }

        var newState = new GoapWorldState();
        foreach (var kvp in stateData)
        {
            newState.SetState(kvp.Key, kvp.Value);
        }
        _statePool[hash] = newState;
        return newState;
    }

    /// <summary>
    /// 计算状态的哈希值
    /// </summary>
    public static int CalculateHash(Dictionary<StateKey, bool> stateData)
    {
        int hash = 0;
        foreach (StateKey key in Enum.GetValues(typeof(StateKey)))
        {
            if (stateData.TryGetValue(key, out bool value) && value)
            {
                hash |= (int)key; // 设置对应位为1
            }
        }
        return hash;
    }
}