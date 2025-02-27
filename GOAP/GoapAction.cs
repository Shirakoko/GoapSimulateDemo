using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoapAction
{
    public Dictionary<StateKey, Func<object, object>> Effect { get; private set; } // 效果函数

    public Dictionary<StateKey, bool> PrecondState { get; private set; }
    public Dictionary<StateKey, bool> EffectState { get; private set; }

    /// <summary>
    /// 动作代价，值必须在(0,1]范围内
    /// </summary>
    public float Cost { get; private set; }
    public GoapAction(float cost = 1.0f)
    {
        Effect = new Dictionary<StateKey, Func<object, object>>();
        PrecondState = new Dictionary<StateKey, bool>();
        EffectState = new Dictionary<StateKey, bool>();
        Cost = cost;
    }

    /// <summary>
    /// 判断是否满足前提条件
    /// </summary>
    public bool MetCondition(Dictionary<StateKey, object> worldState)
    {
        var boolWorldState = GoapWorldState.ConvertStateData(worldState);
        foreach (var kvp in PrecondState)
        {
            if (!boolWorldState.TryGetValue(kvp.Key, out var value) || value != kvp.Value)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 应用动作效果到世界状态
    /// </summary>
    public void EffectOnRun(Dictionary<StateKey, object> worldState)
    {
        Debug.Log($"应用效果到世界状态，代价为{Cost}");
        foreach (var kvp in Effect)
        {
            // 获取当前状态值
            if (worldState.TryGetValue(kvp.Key, out var currentValue))
            {
                // 调用委托函数，传入当前值并获取修改后的值
                var newValue = kvp.Value(currentValue);
                worldState[kvp.Key] = newValue;
            }
        }
    }

    /// <summary>
    /// 设置前提条件
    /// </summary>
    public GoapAction SetPrecond(StateKey key, bool preCondValue)
    {
        PrecondState[key] = preCondValue;
        return this;
    }

    /// <summary>
    /// 设置效果
    /// </summary>
    public GoapAction SetEffect(StateKey key, bool effectValue, Func<object, object> effectFunc)
    {
        Effect[key] = effectFunc;
        EffectState[key] = effectValue;
        return this;
    }
}