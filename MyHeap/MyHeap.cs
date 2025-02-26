using System;
using System.Collections.Generic;

namespace JufGame.Collections.Generic
{
    public class MyHeap<T> where T : IComparable<T>
    {
        public int NowLength { get; private set; }
        public int MaxLength { get; private set; }
        public T Top => heap[0];
        public bool IsEmpty => NowLength == 0;
        public bool IsFull => NowLength >= MaxLength - 1;
        private readonly Dictionary<T, int> nodeIdxTable; // 记录结点在数组中的位置，方便查找
        private readonly bool isReverse;
        private readonly T[] heap;

        public MyHeap(int maxLength, bool isReverse = false)
        {
            NowLength = 0;
            MaxLength = maxLength;
            heap = new T[MaxLength + 1];
            nodeIdxTable = new Dictionary<T, int>();
            this.isReverse = isReverse;
        }
        public T this[int index]
        {
            get => heap[index];
        }

        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="value">需要插入的元素</param>
        public void PushHeap(T value)
        {
            if (NowLength < MaxLength)
            {
                if (nodeIdxTable.ContainsKey(value))
                    nodeIdxTable[value] = NowLength;
                else
                    nodeIdxTable.Add(value, NowLength);
                heap[NowLength] = value;
                Swim(NowLength);
                ++NowLength;
            }
        }

        /// <summary>
        /// 弹出堆顶元素（最大的元素）
        /// </summary>
        public void PopHeap()
        {
            if (NowLength > 0)
            {
                // 堆顶元素的索引记为-1
                nodeIdxTable[heap[0]] = -1; 
                // 堆顶元素用最后一个节点的元素替换
                heap[0] = heap[--NowLength];
                // 更新堆顶元素的索引
                nodeIdxTable[heap[0]] = 0;
                // 堆顶元素下沉
                Sink(0);
            }
        }

        /// <summary>
        /// 判断某个元素是否在堆中
        /// </summary>
        /// <param name="value">要查找元素</param>
        /// <returns>如果元素存在于堆中且其索引有效，则返回true；否则返回false</returns>
        public bool Contains(T value)
        {
            return nodeIdxTable.ContainsKey(value) && nodeIdxTable[value] != -1;
        }

        // <summary>
        /// 在堆中查找指定的元素并返回该元素
        /// </summary>
        /// <param name="value">要查找的元素</param>
        /// <returns>如果元素存在于堆中，则返回该元素；否则返回默认值</returns>
        public T Find(T value)
        {
            if (Contains(value))
                return heap[nodeIdxTable[value]];
            return default;
        }

        /// <summary>
        /// 清空堆
        /// </summary>
        public void Clear()
        {
            nodeIdxTable.Clear();
            NowLength = 0;
        }

        private void SwapValue(T a, T b)
        {
            var aIdx = nodeIdxTable[a];
            var bIdx = nodeIdxTable[b];
            heap[aIdx] = b;
            heap[bIdx] = a;
            nodeIdxTable[a] = bIdx;
            nodeIdxTable[b] = aIdx;
        }

        // 叶子节点上浮
        private void Swim(int index)
        {
            int father;
            while (index > 0)
            {
                // 得到父节点索引
                father = (index - 1) >> 1;
                // 如果插入的元素比父节点大
                if (IsBetter(heap[index], heap[father]))
                {
                    // 与父节点交换值
                    SwapValue(heap[father], heap[index]);
                    // 继续处理父节点上浮
                    index = father;
                }
                else return;
            }
        }

        // 根节点下沉
        private void Sink(int index)
        {
            int largest, left = (index << 1) + 1;
            while (left < NowLength)
            {
                // 获取较大子节点的索引
                largest = left + 1 < NowLength && IsBetter(heap[left + 1], heap[left]) ? left + 1 : left;
                // 如果堆顶元素大于较大子节点，满足大根堆性质，直接返回
                if (IsBetter(heap[index], heap[largest])) {
                    return;
                }
                // 交换堆顶和较大子节点的值
                SwapValue(heap[largest], heap[index]);
                // 继续处理较大子节点下沉
                index = largest;
                // 更新左子节点的索引
                left = (index << 1) + 1;
            }
        }

        // 比大小，如果isReverse == true，是大根堆，否则是小根堆
        private bool IsBetter(T v1, T v2)
        {
            return isReverse ? (v2.CompareTo(v1) < 0 ): (v1.CompareTo(v2) < 0);
        }
    }
}
