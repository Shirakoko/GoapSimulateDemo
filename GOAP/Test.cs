using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GoapActionSet actionSet;
    private Dictionary<StateKey, object> stateData;
    private GoapWorldState goal;
    private GoapAgent agent;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // 枚举StateKey中的每个键都要覆盖到
        stateData = new Dictionary<StateKey, object>()
        {
                { StateKey.HasLeg, true },
                { StateKey.IsWalking, false},
                { StateKey.HasTarget, false},
                { StateKey.CanFly, false},
                { StateKey.IsNearby, false},
        };

        actionSet = new GoapActionSet()
        .AddAction("走", new GoapAction()
                .SetPrecond(StateKey.HasLeg, true)
                .SetEffect(StateKey.IsWalking, true, (value) => {return true;}))

        .AddAction("选目标", new GoapAction()
                .SetPrecond(StateKey.IsWalking, true)
                .SetEffect(StateKey.HasTarget, true, (value) => {return true;}))

        .AddAction("想飞", new GoapAction(0.3f)
                .SetPrecond(StateKey.IsWalking, true)
                .SetEffect(StateKey.CanFly, true, (value) => {return true;}))

        .AddAction("飞近", new GoapAction()
                .SetPrecond(StateKey.CanFly, true)
                .SetEffect(StateKey.IsNearby, true, (value) => {return true;}))

        .AddAction("靠近", new GoapAction()
                .SetPrecond(StateKey.HasTarget, true)
                .SetEffect(StateKey.IsNearby, true, (value) => {return true;}));
        
        
        
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
