using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GoapActionSet actionSet;
    private GoapWorldState worldState;
    private GoapWorldState goal;
    private GoapAgent agent;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        worldState = new GoapWorldState();
        worldState.SetState(StateKey.HasLeg, true);

        actionSet = new GoapActionSet()
        .AddAction("走", new GoapAction()
                .SetPrecondition(StateKey.HasLeg, true)
                .SetEffect(StateKey.IsWalking, true))

        .AddAction("想飞", new GoapAction()
                .SetPrecondition(StateKey.IsWalking, true)
                .SetEffect(StateKey.CanFly, true))

        .AddAction("选目标", new GoapAction(0.9f)
                .SetPrecondition(StateKey.IsWalking, true)
                .SetEffect(StateKey.HasTarget, true))

        .AddAction("飞近", new GoapAction()
                .SetPrecondition(StateKey.CanFly, true)
                .SetEffect(StateKey.IsNearby, true))

        .AddAction("靠近", new GoapAction()
                .SetPrecondition(StateKey.HasTarget, true)
                .SetEffect(StateKey.IsNearby, true));
        
        agent = new GoapAgent(worldState, actionSet);
        
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
        agent.RunPlan(worldState, goal);
    }
}
