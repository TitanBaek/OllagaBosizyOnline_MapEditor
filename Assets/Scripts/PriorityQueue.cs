using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<Block>
{
    private struct Node
    {
        public GameObject element;            // 데이터
        public float priority;                // 
    }

    private List<Node> nodes;               // 배열의 크기가 유동적이기 때문에 List를 통하여 구현한다.
    private IComparer<float> comparer;  // 우선순위 비교를 위한 컴페어러    
    public int Count { get { return nodes.Count; } }

    public PriorityQueue()
    {
        this.nodes = new List<Node>();
    }

    public void Enqueue(GameObject element, float priority)
    {
        Node newNode = new Node() { element = element, priority = priority }; // 구조체 정의

        //1. 가장 위에 데이터 추가
        nodes.Add(newNode);
        int newNodeIndex = nodes.Count - 1;

        //2. 새로운 노드를 힙상태가 유지되도록 승격 작업 반복
        while (newNodeIndex > 0)
        {
            // 2-1. 부모 노드 확인
            int parentIndex = GetParentIndex(newNodeIndex);
            Node parentNode = nodes[parentIndex];

            // 2-2 자식 노드가 부모 노드보다 우선순위가 높으면 교체
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

        // 1. 가장 마지막 노드를 최상단으로 위치
        Node lastNode = nodes[nodes.Count - 1];         // 가장 뒤의 값
        nodes[0] = lastNode;

        // 2. 가장 마지막 데이터 지우기
        nodes.RemoveAt(nodes.Count - 1);

        // 3. 0번 위치로 옮겨진 가장 마지막 노드가 이제 제 위치를 찾게 해줘야함
        int index = 0;

        while (index < nodes.Count) // index가 nodes.Count 보다 작을 때 까지만
        {
            // 4. 자식 노드들과 비교
            int leftChildIndex = GetLeftIndex(index);
            int rightChildIndex = GetRightIndex(index);


            if (rightChildIndex < nodes.Count) // 4-1 . 자식이 둘 다 있는 경우
            {
                // 양쪽 자식노드중 더 우선순위가 높은 자식 인덱스 받기
                int lessChildIndex = nodes[leftChildIndex].priority > nodes[rightChildIndex].priority
                    ? leftChildIndex : rightChildIndex;

                // 더 우선순위가 높은 자식과 부모 노드를 비교하여
                // 부모가 우선순위가 더 낮은 경우 바꾸기
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
            else if (leftChildIndex < nodes.Count) // 4-2 . 자식이 하나만 있는 경우 == 힙 구조상 우측자식만 있는 경우는 없다. 좌측 자식만 있는 경우
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
            else // 4-3 . 자식이 없는 경우
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
