using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行结果状态枚举
/// </summary>
public enum EStatus
{
    Failure, Success, Running
}

public class GoapAgent
{
    private readonly Dictionary<StateKey, object> _curWorldState; // 当前世界状态
    private readonly GoapActionSet _actionSet; // 动作集
    private readonly AStarSearcher<GoapActionSet, GoapWorldState> goapAStar; // A* 搜索器
    private readonly Dictionary<string, Func<EStatus>> _actionFuncs; // 动作名称对应的动作函数
    private Queue<string> _actionPlan; // 规划出的动作序列
    private Stack<GoapWorldState> _statePath; // 规划出的状态路径

    private EStatus curState; // 当前动作的执行结果
    private bool canContinue; // 是否能够继续执行
    private GoapAction curAction; // 当前执行的动作
    private Func<EStatus> curActionFunc; // 当前运行的动作函数

    public GoapAgent(Dictionary<StateKey, object> worldState, GoapActionSet actionSet)
    {
        this._curWorldState = worldState;
        this._actionSet = actionSet;
        this.goapAStar = new AStarSearcher<GoapActionSet, GoapWorldState>(this._actionSet);
        this._actionFuncs = new Dictionary<string, Func<EStatus>>();
        this._actionPlan = new Queue<string>();
        this._statePath = new Stack<GoapWorldState>();
    }

    /// <summary>
    /// 为动作名设置对应的动作函数
    /// </summary>
    public void SetActionFunc(string actionName, Func<EStatus> func)
    {
        if (_actionSet.HasAction(actionName))
        {
            _actionFuncs[actionName] = func;
        }
    }

    /// <summary>
    /// 规划GOAP并运行
    /// </summary>
    public void RunPlan(GoapWorldState curWorldState, GoapWorldState goal)
    {
        // 从状态池中获取共享状态
        var sharedCurWorldState = GoapStatePool.Instance.GetOrCreateState(curWorldState.State);
        var sharedGoal = GoapStatePool.Instance.GetOrCreateState(goal.State);

        if (curState == EStatus.Failure) // 当前状态为「失败」，表示动作执行失败
        {
            // 重新规划，找出新的动作序列
            _actionPlan.Clear();
            goapAStar.FindPath(sharedCurWorldState, sharedGoal, _statePath);

            _statePath.TryPop(out var cur);
            while (_statePath.Count != 0)
            {
                // 从当前状态和下一个状态得到动作序列
                _actionPlan.Enqueue(_actionSet.GetTransAction(cur, _statePath.Peek()));
                cur = _statePath.Pop();
            }
        }
        else if (curState == EStatus.Success) // 执行结果为「成功」，表示动作顺利执行完
        {
            curAction.EffectOnRun(_curWorldState); // 动作对全局世界状态造成影响
            // foreach (var kvp in sharedCurWorldState.State)
            // {
            //     this._curWorldState.SetState(kvp.Key, kvp.Value);
            // }
        }

        // 如果执行结果不是「运行中」，表示上个动作要么成功，要么失败；都该取出动作序列中新的动作来执行
        if (curState != EStatus.Running)
        {
            canContinue = _actionPlan.TryDequeue(out string curActionName);
            if (canContinue) // 如果成功取出动作，就根据动作名，选出对应函数和动作
            {
                curActionFunc = _actionFuncs[curActionName];
                curAction = _actionSet[curActionName];
            }
        }

        if(canContinue && curAction.MetCondition(GoapWorldState.ConvertStateData(this._curWorldState))) {
            curState = curActionFunc();
        } else {
            Debug.Log("动作条件不满足，计划失败");
            curState = EStatus.Failure;
        }
    }
}