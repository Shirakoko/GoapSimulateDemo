using System;
using System.Collections.Generic;
using System.Linq;

public class GoapWorldState : IAStarNode<GoapWorldState>, IComparable<GoapWorldState>, IEquatable<GoapWorldState>
{
    public Dictionary<StateKey, bool> State { get; private set; } // 存储状态
    public GoapWorldState Parent { get; set; }
    public float SelfCost { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;
    public Dictionary<GoapWorldState, float> Neighbors { get; set; } // 邻居节点及其连接代价（Action的Cost）

    public GoapWorldState()
    {
        Neighbors = new Dictionary<GoapWorldState, float>();
        State = new Dictionary<StateKey, bool>();
    }

    public GoapWorldState(GoapWorldState other)
    {
        Neighbors = new Dictionary<GoapWorldState, float>();
        State = new Dictionary<StateKey, bool>(other.State); // 深拷贝状态
    }

    /// <summary>
    /// 新增或设置状态值
    /// </summary>
    public void SetState(StateKey key, bool value)
    {
        if (State.ContainsKey(key)) {
            State[key] = value;
        } else{
            State.Add(key, value);
        }
    }

    /// <summary>
    /// 获取状态值
    /// </summary>
    public object GetState(StateKey key)
    {
        return State.ContainsKey(key) ? State[key] : null;
    }

    /// <summary>
    /// 计算与另一个状态的距离（启发式函数）
    /// </summary>
    public float GetDistance(GoapWorldState other)
    {
        // 如果当前状态是目标状态，返回 0
        if (this.Equals(other))
            return 0;
        
        // 如果当前状态与目标状态直接相连，返回 0
        if (Neighbors.ContainsKey(other))
            return 0;
        
        // 计算状态差异（通过哈希值的不同位）
        int thisHash = GoapStatePool.CalculateHash(this.State);
        int otherHash = GoapStatePool.CalculateHash(other.State);
        int differenceBits = thisHash ^ otherHash; // 异或运算，不同位为 1
        float stateDifference = 0;

        // 统计不同位的数量
        foreach (StateKey key in Enum.GetValues(typeof(StateKey)))
        {
            if ((differenceBits & (int)key) != 0) // 检查该位是否为 1
            {
                stateDifference += 1;
            }
        }

        // 获取到邻居的最小连接代价
        float edgeCost = Neighbors.Count > 0 ? Neighbors.Values.Min() : 1.0f;

        // 启发式函数 = 状态差异 × 边代价
        return stateDifference * edgeCost; // stateDifference * [0, 1]
    }

    /// <summary>
    /// 获取后继状态
    /// </summary>
    public List<GoapWorldState> GetSuccessors(object nodeMap)
    {
        var successors = new List<GoapWorldState>();

        // 遍历邻居节点
        foreach (var neighbor in Neighbors.Keys)
        {
            // 更新邻居节点的 SelfCost
            neighbor.SelfCost = Neighbors[neighbor];
            // 将邻居节点加入列表
            successors.Add(neighbor);
        }

        return successors;
    }

    public int CompareTo(GoapWorldState other)
    {
        var res = (int)(FCost - other.FCost);
        if(res == 0)
            res = (int)(HCost - other.HCost);
        return res;
    }

    public bool Equals(GoapWorldState other)
    {
        if (other == null)
            return false;

        // 使用 GoapStatePool 的哈希值来比较状态是否相同
        int thisHash = GoapStatePool.CalculateHash(this.State);
        int otherHash = GoapStatePool.CalculateHash(other.State);

        return thisHash == otherHash;
    }
}