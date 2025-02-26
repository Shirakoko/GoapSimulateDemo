using System.Collections.Generic;

namespace JufGame.Collections.Generic
{
    public class MyGraph<TNode, TEdge>
    {
        public readonly HashSet<TNode> NodeSet; // 节点列表
        public readonly Dictionary<TNode, List<TNode>> NeighborList; // 邻居列表
        public readonly Dictionary<(TNode, TNode), List<TEdge>> EdgeList; // 边列表
        public MyGraph()
        {
            NodeSet = new HashSet<TNode>();
            NeighborList = new Dictionary<TNode, List<TNode>>();
            EdgeList = new Dictionary<(TNode, TNode), List<TEdge>>();
        }

        /// <summary>
        /// 寻找指定节点
        /// </summary>
        /// <returns>找到的节点，没找到时返回null</returns>
        public TNode FindNode(TNode node)
        {
            NodeSet.TryGetValue(node, out TNode res);
            return res;
        }

        /// <summary>
        /// 寻找指定两点之间连接的边列表
        /// </summary>
        /// <param name="source">起点</param>
        /// <param name="target">终点</param>
        /// <returns>找到的边列表，没找到时返回null</returns>
        public List<TEdge> FindEdge(TNode source, TNode target)
        {
            var s = FindNode(source);
            var t = FindNode(target);
            if (s != null && t != null)
            {
                var nodePairs = (s, t);
                if (EdgeList.ContainsKey(nodePairs))
                {
                    return EdgeList[nodePairs];
                }
            }
            return null;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        public bool AddNode(TNode node)
        {
            return NodeSet.Add(node);
        }

        /// <summary>
        /// （前提是边两端结点已添加进图）添加指定边
        /// </summary>
        /// <param name="source">边起点</param>
        /// <param name="target">边终点</param>
        /// <param name="edge">指定边</param>
        /// <returns>添加成功与否</returns>
        public bool AddEdge(TNode source, TNode target, TEdge edge)
        {
            var s = FindNode(source);
            var t = FindNode(target);
            if (s == null || t == null)
                return false;
            var nodePairs = (s, t);
            if(!EdgeList.ContainsKey(nodePairs))
            {
                EdgeList.Add(nodePairs, new List<TEdge>());
            }
            var allEdges = EdgeList[nodePairs];
            if(!allEdges.Contains(edge))
            {
                allEdges.Add(edge);
                // 添加起点的邻居
                if(!NeighborList.ContainsKey(source))
                {
                    NeighborList.Add(source, new List<TNode>());
                }
                NeighborList[source].Add(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定节点
        /// </summary>
        /// <returns>移除成功与否</returns>
        public bool RemoveNode(TNode node)
        {
            return NodeSet.Remove(node);
        }

        /// <summary>
        /// 移除指定起、终点的指定边
        /// </summary>
        /// <param name="source">边起点</param>
        /// <param name="target">边终点</param>
        /// <param name="edge">指定边</param>
        /// <returns>移除成功与否</returns>
        public bool RemoveEdge(TNode source, TNode target, TEdge edge)
        {
            var allEdges = FindEdge(source, target);
            return allEdges != null && allEdges.Remove(edge);
        }

        /// <summary>
        /// 移除指定起、终点的所有边
        /// </summary>
        /// <param name="source">边起点</param>
        /// <param name="target">边终点</param>
        /// <returns>移除成功与否</returns>
        public bool RemoveEdgeList(TNode source, TNode target)
        {
            return EdgeList.Remove((source, target));
        }

        /// <summary>
        /// 获取指定节点可抵达的所有邻居节点
        /// </summary>
        public List<TNode> GetNeighbor(TNode node)
        {
            NeighborList.TryGetValue(node, out List<TNode> res);
            return res;
        }
        
        /// <summary>
        /// 获取指定节点所延伸出的所有边
        /// </summary>
        public List<TEdge> GetConnectedEdge(TNode node)
        {
            var resEdge = new List<TEdge>();
            var neighbor = GetNeighbor(node);
            for(int i = 0; i < neighbor.Count; ++i)
            {
                var curEdgeList = EdgeList[(node, neighbor[i])];
                for(int j = 0; j < curEdgeList.Count; ++j)
                {
                    resEdge.Add(curEdgeList[j]);
                }
            }
            return resEdge;
        }
    }
}