using System.Collections.Generic;

public interface IAStarNode<T> where T : IAStarNode<T>
{
    // 父节点，通过泛型使它的类型与具体类一致
    public T Parent { get; set; }
    // 自身单步花费代价
    public float SelfCost { get; set; }
    // 距初始状态的代价
    public float GCost { get; set; }
    // 距目标状态的代价
    public float HCost { get; set; }
    // 总评估代价
    public float FCost { get; }

    /// <summary>
    /// 获取与指定节点的预测代价
    /// </summary>
    public float GetDistance(T otherNode);
    
    /// <summary>
    /// 获取后继（邻居）节点，在此过程中，需要更新邻居的SelfCost（自己到邻居节点的代价）
    /// </summary>
    /// <param name="nodeMap">寻路所在的地图</param>
    /// <returns>后继（邻居）节点列表</returns>
    public List<T> GetSuccessors(object nodeMap);

    /* IComparable实现的CompareTo函数，主要用于优先队列的比较，一般比较可用以下函数
    public int CompareTo(AStarNode other)
    {
    	var res = (int)(FCost - other.FCost);
        if(res == 0)
            res = (int)(HCost - other.HCost);
        return res;
    }*/

    /* IEquatable实现的Equals函数，可以自定义HashSet和Dictionary的Contains判断依据（但同样要重写GetHashCode）
       以及在寻路时用于比对某点是否为终点，可以根据类的特点自行继承 */
}
