using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue
{
    public class PriorityQueue<T>
    {
        private Node[] minMaxHeap;

        public int Length { get; private set; }
        public int MaxSize { get; private set; }

        public PriorityQueue(int size)
        {
            this.MaxSize = size;
            this.Length = 0;

            this.minMaxHeap = new Node[size];
            for (int i = 0; i < this.minMaxHeap.Length; i++)
            {
                this.minMaxHeap[i].active = false;
            }
        }

        public void Enqueue(T value, int priority)
        {
            int freeIndex = this.Length;
            if (freeIndex >= this.minMaxHeap.Length)
            {
                throw new Exception(String.Format("Enqueue failed since queue is full. Max queue size is {0}.", this.MaxSize));
            }

            this.minMaxHeap[freeIndex].key = priority;
            this.minMaxHeap[freeIndex].value = value;
            this.minMaxHeap[freeIndex].active = true;

            int parentIndex;
            if (GetParent(freeIndex, out parentIndex)) // If this isn't the root, we need to bubble up
            {
                bool isMaxLevel = (int)Math.Floor(Math.Log((double)(freeIndex + 1), 2D)) % 2 != 0;
                int newKey = priority;
                int parentKey = this.minMaxHeap[parentIndex].key;
                if (isMaxLevel) // Max level
                {
                    if (newKey < parentKey) // Inserted at max level, but new key is less than parent key
                    {
                        this.Swap(freeIndex, parentIndex);
                        this.BubbleUpMin(parentIndex);
                    }
                    else
                    {
                        this.BubbleUpMax(freeIndex);
                    }
                }
                else // Min level
                {
                    if (newKey > parentKey) // Inserted at min level, but new key is greater than parent key
                    {
                        this.Swap(freeIndex, parentIndex);
                        this.BubbleUpMax(parentIndex);
                    }
                    else
                    {
                        this.BubbleUpMin(freeIndex);
                    }
                }
            }

            this.Length++;
        }

        public bool DequeueMin(out T value)
        {
            bool result = false;
            value = default(T);

            if (this.minMaxHeap[0].active) // Min value is always in the root, if it exists
            {
                value = this.minMaxHeap[0].value;
                this.minMaxHeap[0].active = false;
                this.minMaxHeap[0].value = default(T);
                if (this.Length > 1)
                {
                    this.Swap(0, this.Length - 1);
                    this.Length--;
                    this.TrickleDownMin(0);
                }
                else
                {
                    this.Length = 0;
                }

                result = true;
            }

            return result;
        }

        public bool DequeueMax(out T value)
        {
            bool result = false;
            value = default(T);

            int[] childIndices = this.GetChildren(0); // Max value is always largest child of root, if they exist
            int indexOfLargest = -1;
            int largestKey = 0;
            for (int i = 0; i < childIndices.Length; i++)
            {
                int childKey = this.minMaxHeap[childIndices[i]].key;
                if (indexOfLargest == -1 || childKey > largestKey)
                {
                    indexOfLargest = childIndices[i];
                    largestKey = childKey;
                }
            }

            if (indexOfLargest != -1)
            {
                value = this.minMaxHeap[indexOfLargest].value;
                this.minMaxHeap[indexOfLargest].active = false;
                this.minMaxHeap[indexOfLargest].value = default(T);
                if (this.Length > 1)
                {
                    this.Swap(indexOfLargest, this.Length - 1);
                    this.Length--;
                    this.TrickleDownMax(indexOfLargest);
                }
                else
                {
                    this.Length = 0;
                }

                result = true;
            }
            else if (this.minMaxHeap[0].active) // Root is the only node left
            {
                value = this.minMaxHeap[0].value;
                this.minMaxHeap[0].active = false;
                this.minMaxHeap[0].value = default(T);
                this.Length = 0;

                result = true;
            }

            return result;
        }

        private void TrickleDownMin(int index)
        {
            int[] childIndices = this.GetChildren(index);
            int[] grandChildrenIndices = this.GetGrandchildren(index);

            int indexOfSmallest = -1;
            int smallestKey = 0;
            bool smallestIsGrandchild = false;
            for (int i = 0; i < childIndices.Length; i++)
            {
                int childKey = this.minMaxHeap[childIndices[i]].key;
                if (indexOfSmallest == -1 || childKey < smallestKey)
                {
                    indexOfSmallest = childIndices[i];
                    smallestKey = childKey;
                }
            }
            for (int i = 0; i < grandChildrenIndices.Length; i++)
            {
                int grandChildKey = this.minMaxHeap[grandChildrenIndices[i]].key;
                if (indexOfSmallest == -1 || grandChildKey < smallestKey)
                {
                    indexOfSmallest = grandChildrenIndices[i];
                    smallestKey = grandChildKey;
                    smallestIsGrandchild = true;
                }
            }

            if (indexOfSmallest != -1)
            {
                if (smallestIsGrandchild)
                {
                    if (this.minMaxHeap[indexOfSmallest].key < this.minMaxHeap[index].key)
                    {
                        this.Swap(index, indexOfSmallest);
                        int parentIndex;
                        if (this.GetParent(indexOfSmallest, out parentIndex))
                        {
                            if (this.minMaxHeap[indexOfSmallest].key > this.minMaxHeap[parentIndex].key)
                            {
                                this.Swap(indexOfSmallest, parentIndex);
                            }
                        }
                        this.TrickleDownMin(indexOfSmallest);
                    }
                }
                else // Smallest is child
                {
                    if (this.minMaxHeap[indexOfSmallest].key < this.minMaxHeap[index].key)
                    {
                        this.Swap(index, indexOfSmallest);
                    }
                }
            }
        }

        private void TrickleDownMax(int index)
        {
            int[] childIndices = this.GetChildren(index);
            int[] grandChildrenIndices = this.GetGrandchildren(index);

            int indexOfLargest = -1;
            int largestKey = 0;
            bool largestIsGrandchild = false;
            for (int i = 0; i < childIndices.Length; i++)
            {
                int childKey = this.minMaxHeap[childIndices[i]].key;
                if (indexOfLargest == -1 || childKey > largestKey)
                {
                    indexOfLargest = childIndices[i];
                    largestKey = childKey;
                }
            }
            for (int i = 0; i < grandChildrenIndices.Length; i++)
            {
                int grandChildKey = this.minMaxHeap[grandChildrenIndices[i]].key;
                if (indexOfLargest == -1 || grandChildKey > largestKey)
                {
                    indexOfLargest = grandChildrenIndices[i];
                    largestKey = grandChildKey;
                    largestIsGrandchild = true;
                }
            }

            if (indexOfLargest != -1)
            {
                if (largestIsGrandchild)
                {
                    if (this.minMaxHeap[indexOfLargest].key > this.minMaxHeap[index].key)
                    {
                        this.Swap(index, indexOfLargest);
                        int parentIndex;
                        if (this.GetParent(indexOfLargest, out parentIndex))
                        {
                            if (this.minMaxHeap[indexOfLargest].key < this.minMaxHeap[parentIndex].key)
                            {
                                this.Swap(indexOfLargest, parentIndex);
                            }
                        }
                        this.TrickleDownMax(indexOfLargest);
                    }
                }
                else // Largest is child
                {
                    if (this.minMaxHeap[indexOfLargest].key > this.minMaxHeap[index].key)
                    {
                        this.Swap(index, indexOfLargest);
                    }
                }
            }
        }

        private void BubbleUpMin(int index)
        {
            int grandparentIndex;
            if (this.GetGrandparent(index, out grandparentIndex))
            {
                if (this.minMaxHeap[index].key < this.minMaxHeap[grandparentIndex].key)
                {
                    this.Swap(index, grandparentIndex);
                    this.BubbleUpMin(grandparentIndex);
                }
            }
        }

        private void BubbleUpMax(int index)
        {
            int grandparentIndex;
            if (this.GetGrandparent(index, out grandparentIndex))
            {
                if (this.minMaxHeap[index].key > this.minMaxHeap[grandparentIndex].key)
                {
                    this.Swap(index, grandparentIndex);
                    this.BubbleUpMax(grandparentIndex);
                }
            }
        }

        private int[] GetChildren(int index)
        {
            int leftChildIndex = (2 * index) + 1;
            int rightChildIndex = leftChildIndex + 1;

            List<int> childIndexList = new List<int>();
            if (leftChildIndex > 0 && leftChildIndex < this.minMaxHeap.Length && this.minMaxHeap[leftChildIndex].active)
            {
                childIndexList.Add(leftChildIndex);
            }
            if (rightChildIndex > 0 && rightChildIndex < this.minMaxHeap.Length && this.minMaxHeap[rightChildIndex].active)
            {
                childIndexList.Add(rightChildIndex);
            }

            return childIndexList.ToArray();
        }

        private int[] GetGrandchildren(int index)
        {
            int[] childIndices = this.GetChildren(index);

            List<int> grandchildIndexList = new List<int>();
            for (int i = 0; i < childIndices.Length; i++)
            {
                int[] grandchildIndices = this.GetChildren(childIndices[i]);
                for (int j = 0; j < grandchildIndices.Length; j++)
                {
                    grandchildIndexList.Add(grandchildIndices[j]);
                }
            }

            return grandchildIndexList.ToArray();
        }

        private bool GetParent(int index, out int parentIndex)
        {
            bool result = false;
            parentIndex = -1;

            if (index != 0 && this.minMaxHeap[index].value != null)
            {
                parentIndex = (int)Math.Floor(((double)index - 1D) / 2D);
                result = true;
            }

            return result;
        }

        private bool GetGrandparent(int index, out int grandparentIndex)
        {
            bool result = false;
            grandparentIndex = -1;

            int parentIndex;
            if (GetParent(index, out parentIndex))
            {
                result = GetParent(parentIndex, out grandparentIndex);
            }

            return result;
        }

        private void Swap(int i1, int i2)
        {
            int i1Key = this.minMaxHeap[i1].key;
            T i1Value = this.minMaxHeap[i1].value;
            bool i1Active = this.minMaxHeap[i1].active;
            this.minMaxHeap[i1].key = this.minMaxHeap[i2].key;
            this.minMaxHeap[i1].value = this.minMaxHeap[i2].value;
            this.minMaxHeap[i1].active = this.minMaxHeap[i2].active;
            this.minMaxHeap[i2].key = i1Key;
            this.minMaxHeap[i2].value = i1Value;
            this.minMaxHeap[i2].active = i1Active;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{\n  ");
            for (int i = 0; i < this.minMaxHeap.Length; i++)
            {
                builder.AppendFormat("[{0}:{1}] ", this.minMaxHeap[i].key, this.minMaxHeap[i].value);
            }
            builder.Append("\n}");
            return builder.ToString();
        }

        private struct Node
        {
            public T value;
            public int key;
            public bool active;
        }
    }
}
