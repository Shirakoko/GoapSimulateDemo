using System.Collections.Generic;
using UnityEngine;

public class GoapAction
{
    public Dictionary<StateKey, bool> Precondition { get; private set; } // 前提条件
    public Dictionary<StateKey, bool> Effect { get; private set; } // 效果

    /// <summary>
    /// 动作代价，值必须在(0,1]范围内
    /// </summary>
    public float Cost { get; private set; }
    public GoapAction(float cost = 1.0f)
    {
        Precondition = new Dictionary<StateKey, bool>();
        Effect = new Dictionary<StateKey, bool>();
        Cost = cost;
    }

    /// <summary>
    /// 判断是否满足前提条件
    /// </summary>
    public bool MetCondition(GoapWorldState worldState)
    {
        foreach (var kvp in Precondition)
        {
            if (!worldState.TryGetState(kvp.Key, out var value) || value != kvp.Value)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 应用动作效果到世界状态
    /// </summary>
    public void Effect_OnRun(GoapWorldState worldState)
    {
        Debug.Log($"应用效果到世界状态，代价为{Cost}");
        foreach (var kvp in Effect)
        {
            worldState.SetState(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// 设置前提条件
    /// </summary>
    public GoapAction SetPrecondition(StateKey key, bool value)
    {
        Precondition[key] = value;
        return this;
    }

    /// <summary>
    /// 设置效果
    /// </summary>
    public GoapAction SetEffect(StateKey key, bool value)
    {
        Effect[key] = value;
        return this;
    }
}