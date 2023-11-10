using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class Block : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IDragHandler,ISelectable
{
    private BlockData structBlockData;
    public BlockData StruckBlockData { get { return structBlockData; } set { structBlockData = value; } }

    private List<GameObject> PlatformList { get { return GameManager.Data.PlatformList; } } // �� ���ÿ� �ʿ��� PlatformList

    private bool setComplete;
    public bool SetComplete { get { return setComplete; } set { setComplete = value; } }

    [SerializeField] private Sprite blockIcon;
    [SerializeField] private bool isGoal;
    public Sprite BlockIcon { get { return blockIcon; } set { blockIcon = value; } }
    public bool IsGoal { get { return isGoal; } set { structBlockData.isGoal = value; isGoal = value; } }

    private Vector2 prevPosition;
    private Vector2 moveDir;
    private Renderer[] renderers;

    private bool blockMoved;   // �� ���� ���Ǻο� �� isMoved ����
    private Coroutine blockMoveCoroutine;

    private void Awake()
    {
        moveDir = new Vector2();
        renderers = GetComponentsInChildren<Renderer>();
        blockMoved = false;
    }

    private void OnEnable()
    {
        //������Ʈ �̸� ���� �� ����ü�� ������ �Է�
        if (structBlockData.index_name != null)
        {
            Debug.Log($"�ʱ�ȭ ������ {structBlockData.index_name}");
            return;
        }
        else
        {
            InitBlockData();
        }
    }

    public void InitBlockData(BlockData blockData)
    {
        this.gameObject.name = blockData.index_name;
        structBlockData = blockData;
    }

    public void InitBlockData()
    {
        Debug.Log($"�ε� �� ������Ʈ��{this.gameObject.name}");
        structBlockData.Prefab_Name = gameObject.name;
        //������ Ȱ���Ͽ� ������Ʈ �̸� ����
        this.gameObject.name = $"block_{Random.Range(1000, 3000)}";
        structBlockData.index_name = gameObject.name;
        InitBlockTransform();
    }

    public void InitBlockTransform()
    {
        structBlockData.platform_position = gameObject.transform.position;
        structBlockData.platform_rotate = gameObject.transform.rotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"���� �� ���� ����ü �� {structBlockData.Prefab_Name}  {structBlockData.platform_position}  {structBlockData.platform_rotate}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGoal)
        {
            //���� ��� �÷��̾ ��Ҵ��� Ȯ���Ͽ� Test��带 ��ġ�� ���� ����.
        }
    }

    public bool SetGoal()
    {
        if (isGoal)
            return false;

        isGoal = true;
        return isGoal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        prevPosition = this.gameObject.transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == InputButton.Left)
        {
            if (prevPosition == (Vector2)this.gameObject.transform.position) // ���� ��ġ ��ȭ�� ���ٸ� �������õǰ�
            {
                if (PlatformList != null & PlatformList.Count > 0)
                    PlatformList.Clear();

                PlatformList.Add(this.gameObject);
                // ���ð���
                GameManager.Data.SelectedBlocks = PlatformList.ToList();
            } else
            {
                InitBlockTransform();
                // mapData ���� �ʿ�
            }
        }
        if (eventData.button == InputButton.Right)
        {
            GameManager.Data.mapData.RemovePlatforms(this.gameObject);
            GameManager.Data.RemovePlatform = this.gameObject;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���� ���� ���õ� ��� (PointerDown�� ���) �巡�׿� �Բ� �� ��ġ �̵�
        Debug.Log($"{this.gameObject.name} ���� ���õǾ� �巡�� ��");
        if(GameManager.Data.SelectedBlocks != null)
        {
            blockMoved = true;
            moveDir.x = eventData.delta.x;
            moveDir.y = eventData.delta.y;
            MoveBlock(moveDir);
        }
    }

    public void ISelected()
    {
        // �� ���� ���� ��.
        if(renderers != null)
        {
            foreach(Renderer renderer in renderers)
            {
                renderer.material.color = Color.green;
            }
        } else
        {
            Debug.Log("NULL");
        }
    }

    public void DisSelected()
    {
        // �� ���� �������� ��
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = Color.white;
            }
        }
    }

    public void MoveBlock(Vector2 moveDir)
    {
        foreach(GameObject blockobj in GameManager.Data.SelectedBlocks)
        {
            Block block = blockobj.GetComponent<Block>();
            blockobj.transform.Translate(moveDir * Time.deltaTime * 1.2f, Space.World);
            block.InitBlockTransform();
            GameManager.Data.mapData.ChangePlatformsPosition(blockobj);
        }
    }

    public IEnumerator isMoved()
    {
        yield return new WaitForSeconds(1f);
        blockMoved = false;
        yield return null;

    }
}
