using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GoapActionSet actionSet;
    private Dictionary<StateKey, object> stateData;
    private GoapWorldState goal;
    private GoapAgent agent;

    void Start()
    {
        // 枚举StateKey中的每个键都要覆盖到
        stateData = new Dictionary<StateKey, object>()
        {
                { StateKey.HasLeg, true },
                { StateKey.IsWalking, false},
                { StateKey.HasTarget, false},
                { StateKey.CanFly, "不能飞行"},
                { StateKey.IsNearby, 50},
        };

        // 设置值类型状态的比较函数
        GoapWorldState._stateComparers.Add(StateKey.CanFly, (value) => {
                return (string)value == "可以飞行";
        });
        GoapWorldState._stateComparers.Add(StateKey.IsNearby, (value) => {
                return (int)value < 10;
        });

        // 构建GOAP图，动作代价必须在(0,1]范围内
        actionSet = new GoapActionSet()
        .AddAction("走", new GoapAction()
                .SetPrecond(StateKey.HasLeg, true)
                .SetEffect(StateKey.IsWalking, true, (value) => {return true;}))

        .AddAction("选目标", new GoapAction()
                .SetPrecond(StateKey.IsWalking, true)
                .SetEffect(StateKey.HasTarget, true, (value) => {return true;}))

        .AddAction("想飞", new GoapAction(0.3f)
                .SetPrecond(StateKey.IsWalking, true)
                .SetEffect(StateKey.CanFly, true, (value) => {return "可以飞行";}))

        .AddAction("飞近", new GoapAction()
                .SetPrecond(StateKey.CanFly, true)
                .SetEffect(StateKey.IsNearby, true, (value) => {return (int)value - 45;}))

        .AddAction("靠近", new GoapAction()
                .SetPrecond(StateKey.HasTarget, true)
                .SetEffect(StateKey.IsNearby, true, (value) => {return (int)value - 45;}));
        
        
        agent = new GoapAgent(stateData, actionSet);
        agent.SetActionFunc("走", () => { Debug.Log(" 走"); return EStatus.Success; });
        agent.SetActionFunc("想飞", () => { Debug.Log(" 想飞"); return EStatus.Success; });
        agent.SetActionFunc("选目标", () => { Debug.Log(" 选目标"); return EStatus.Success; });
        agent.SetActionFunc("飞近", () => { Debug.Log(" 飞近"); return EStatus.Success; });
        agent.SetActionFunc("靠近", () => { Debug.Log(" 靠近"); return EStatus.Success; });
        
        goal = new GoapWorldState();
        goal.SetState(StateKey.IsNearby, true);

        InvokeRepeating("RunPlan", 0, 2);
    }

    private void RunPlan()
    {
        agent.RunPlan(new GoapWorldState(stateData), goal);
    }
}
