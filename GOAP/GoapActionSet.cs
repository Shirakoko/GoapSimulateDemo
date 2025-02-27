using System.Collections.Generic;
using JufGame.Collections.Generic;

public class GoapActionSet
{
    // GOAP图，节点是GoapWorldState，边是动作名称
    public MyGraph<GoapWorldState, string> actionGraph;
    // 动作名称到具体动作的映射
    private readonly Dictionary<string, GoapAction> actionSet;

    public GoapActionSet()
    {
        actionGraph = new MyGraph<GoapWorldState, string>();
        actionSet = new Dictionary<string, GoapAction>();
    }

    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="name">动作名称</param>
    /// <returns></returns>
    public GoapAction this[string name] => actionSet[name];

    /// <summary>
    /// 动作集中是否有某动作
    /// </summary>
    /// <param name="name">动作名</param>
    /// <returns></returns>
    public bool HasAction(string name) {
        return actionSet.ContainsKey(name);
    }

    /// <summary>
    /// 添加动作至动作集合中
    /// </summary>
    /// <param name="actionName">动作名</param>
    /// <param name="newAction">对应动作</param>
    /// <returns>动作集，方便连续添加</returns>
    public GoapActionSet AddAction(string actionName, GoapAction newAction)
    {
        actionSet.Add(actionName, newAction);

        // 使用状态池获取或创建状态
        var preconditionState = GoapStatePool.Instance.GetOrCreateState(newAction.PrecondState);
        var effectState = GoapStatePool.Instance.GetOrCreateState(newAction.EffectState);

        actionGraph.AddNode(preconditionState);
        actionGraph.AddNode(effectState);
        actionGraph.AddEdge(preconditionState, effectState, actionName);

        // 添加邻居状态和连接代价（GoapAction的权重）
        if (preconditionState.Neighbors != null)
        {
            if (preconditionState.Neighbors.ContainsKey(effectState)) {
                preconditionState.Neighbors[effectState] = newAction.Cost;
            } else {
                preconditionState.Neighbors.Add(effectState, newAction.Cost);
            }
        } else {
            preconditionState.Neighbors = new Dictionary<GoapWorldState, float> { { effectState, newAction.Cost } };
        }
    
        return this;
    }

    /// <summary>
    /// 获取状态转换的动作名
    /// </summary>
    public string GetTransAction(GoapWorldState from, GoapWorldState to)
    {
        return actionGraph.FindEdge(from, to)[0];
    }
}