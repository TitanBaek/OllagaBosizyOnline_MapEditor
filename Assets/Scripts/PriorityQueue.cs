using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<Block>
{
    private struct Node
    {
        public GameObject element;            // ������
        public float priority;                // 
    }

    private List<Node> nodes;               // �迭�� ũ�Ⱑ �������̱� ������ List�� ���Ͽ� �����Ѵ�.
    private IComparer<float> comparer;  // �켱���� �񱳸� ���� ����    
    public int Count { get { return nodes.Count; } }

    public PriorityQueue()
    {
        this.nodes = new List<Node>();
    }

    public void Enqueue(GameObject element, float priority)
    {
        Node newNode = new Node() { element = element, priority = priority }; // ����ü ����

        //1. ���� ���� ������ �߰�
        nodes.Add(newNode);
        int newNodeIndex = nodes.Count - 1;

        //2. ���ο� ��带 �����°� �����ǵ��� �°� �۾� �ݺ�
        while (newNodeIndex > 0)
        {
            // 2-1. �θ� ��� Ȯ��
            int parentIndex = GetParentIndex(newNodeIndex);
            Node parentNode = nodes[parentIndex];

            // 2-2 �ڽ� ��尡 �θ� ��庸�� �켱������ ������ ��ü
            if (newNode.priority > parentNode.priority)
            {
                nodes[newNodeIndex] = parentNode;
                nodes[parentIndex] = newNode;
                newNodeIndex = parentIndex;
            }
            else
            {
                break;
            }
        }

    }

    public GameObject Dequeue()
    {
        Node rootNode = nodes[0];

        // 1. ���� ������ ��带 �ֻ������ ��ġ
        Node lastNode = nodes[nodes.Count - 1];         // ���� ���� ��
        nodes[0] = lastNode;

        // 2. ���� ������ ������ �����
        nodes.RemoveAt(nodes.Count - 1);

        // 3. 0�� ��ġ�� �Ű��� ���� ������ ��尡 ���� �� ��ġ�� ã�� �������
        int index = 0;

        while (index < nodes.Count) // index�� nodes.Count ���� ���� �� ������
        {
            // 4. �ڽ� ����� ��
            int leftChildIndex = GetLeftIndex(index);
            int rightChildIndex = GetRightIndex(index);


            if (rightChildIndex < nodes.Count) // 4-1 . �ڽ��� �� �� �ִ� ���
            {
                // ���� �ڽĳ���� �� �켱������ ���� �ڽ� �ε��� �ޱ�
                int lessChildIndex = nodes[leftChildIndex].priority > nodes[rightChildIndex].priority
                    ? leftChildIndex : rightChildIndex;

                // �� �켱������ ���� �ڽİ� �θ� ��带 ���Ͽ�
                // �θ� �켱������ �� ���� ��� �ٲٱ�
                if (nodes[lessChildIndex].priority > nodes[index].priority)
                {
                    nodes[index] = nodes[lessChildIndex];
                    nodes[lessChildIndex] = lastNode;
                    index = lessChildIndex;
                }
                else
                {
                    break;
                }
            }
            else if (leftChildIndex < nodes.Count) // 4-2 . �ڽ��� �ϳ��� �ִ� ��� == �� ������ �����ڽĸ� �ִ� ���� ����. ���� �ڽĸ� �ִ� ���
            {
                if (nodes[leftChildIndex].priority > nodes[index].priority)
                {
                    nodes[index] = nodes[leftChildIndex];
                    nodes[leftChildIndex] = lastNode;
                    index = leftChildIndex;
                }
                else
                {
                    break;
                }
            }
            else // 4-3 . �ڽ��� ���� ���
            {
                break;
            }
        }


        return rootNode.element;
    }

    public GameObject Peek()
    {
        return nodes[0].element;
    }

    private int GetParentIndex(int childIndex)
    {
        return (childIndex - 1) / 2;
    }

    private int GetLeftIndex(int parentIndex)
    {
        return parentIndex * 2 + 1;
    }
    private int GetRightIndex(int parentIndex)
    {
        return parentIndex * 2 + 2;
    }
}
