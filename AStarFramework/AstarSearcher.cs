using System;
using System.Collections.Generic;
using JufGame.Collections.Generic;

/// <summary>
/// A星搜索器，T_Node额外实现IComparable用于优先队列的比较，实现IEquatable用于HashSet和Dictionary等同一性的判断
/// </summary>
/// <typeparam name="T_Map">搜索的图类</typeparam>
/// <typeparam name="T_Node">搜索的节点类</typeparam>
public class AStarSearcher<T_Map, T_Node> where T_Node: IAStarNode<T_Node>, IComparable<T_Node>, IEquatable<T_Node>
{   
    // 探索集，是一个哈希集合
    private readonly HashSet<T_Node> closeList;
    // 边缘集，是一个堆
    private readonly MyHeap<T_Node> openList;
    // 搜索空间（地图）
    private readonly T_Map nodeMap;
    public AStarSearcher(T_Map map, int maxNodeSize = 200)
    {
        nodeMap = map;
        closeList = new HashSet<T_Node>();
        // maxNodeSize用于限制路径节点的上限，避免陷入无止境搜索的情况
        openList = new MyHeap<T_Node>(maxNodeSize);
    }

    /// <summary>
    /// 搜索（寻路）
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="target">终点</param>
    /// <param name="pathRes">生成的路径</param>
    public void FindPath(T_Node start, T_Node target, Stack<T_Node> pathRes)
    {
        T_Node currentNode;
        pathRes.Clear();
        closeList.Clear();
        openList.Clear();
        openList.PushHeap(start);
        while (!openList.IsEmpty)
        {
            // 取出边缘集中最小代价的节点（堆顶元素）
            currentNode = openList.Top;
            openList.PopHeap();
            // 拟定移动到该节点，将其放入探索集
            closeList.Add(currentNode);
            // 如果找到了或图都搜完了也没找到时
            if (currentNode.Equals(target) || openList.IsFull)
            {
                // 生成路径并保存到pathRes中
                GenerateFinalPath(start, currentNode, pathRes);
                return;
            }
            // 更新探索集和边缘集
            UpdateTwoLists(currentNode, target);
        }
    }

    private void GenerateFinalPath(T_Node startNode, T_Node endNode, Stack<T_Node> pathStack)
    {
        pathStack.Push(endNode); // 因为回溯，所以用栈储存生成的路径
        var tpNode = endNode.Parent;
        while (tpNode != null && !tpNode.Equals(startNode))
        {
            pathStack.Push(tpNode);
            tpNode = tpNode.Parent;
        }
        pathStack.Push(startNode);
    }

    private void UpdateTwoLists(T_Node curNode, T_Node endNode)
    {
        T_Node sucNode; // 用于存储当前节点的后继节点
        float tpCost; // 用于存储从起点到后继节点的临时总代价
        bool isNotInOpenList; // 用于标记后继节点是否不在开放列表中

        // 找出当前节点的所有后继节点
        var successors = curNode.GetSuccessors(nodeMap);
        if(successors == null)
        {
            return;
        }

        // 遍历所有后继节点
        for (int i = 0; i < successors.Count; ++i)
        {
            sucNode = successors[i]; // 获取当前后继节点

            // 如果后继节点已经在边缘集中（已被探索过），则跳过
            if (closeList.Contains(sucNode))
                continue;
            
            // 计算从起点到当前后继节点的总代价
            tpCost = curNode.GCost + sucNode.SelfCost;

            // 检查后继节点是否不在开放列表中
            isNotInOpenList = !openList.Contains(sucNode);

            // 如果后继节点不在开放列表中，或者新的总代价比之前的更小
            if (isNotInOpenList || tpCost < sucNode.GCost)
            {
                sucNode.GCost = tpCost; // 更新后继节点的总代价
                sucNode.HCost = sucNode.GetDistance(endNode); // 计算后继节点的启发式估计值（到终点的估计代价）
                sucNode.Parent = curNode; // 设置后继节点的父节点为当前节点，方便回溯

                // 如果后继节点不在探索集中，将其加入开放列表
                if (isNotInOpenList)
                {
                    openList.PushHeap(sucNode);
                }
            }
        }
    }
}