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

    private List<GameObject> PlatformList { get { return GameManager.Data.PlatformList; } } // 블럭 선택에 필요한 PlatformList

    private bool setComplete;
    public bool SetComplete { get { return setComplete; } set { setComplete = value; } }

    [SerializeField] private Sprite blockIcon;
    [SerializeField] private bool isGoal;
    public Sprite BlockIcon { get { return blockIcon; } set { blockIcon = value; } }
    public bool IsGoal { get { return isGoal; } set { structBlockData.isGoal = value; isGoal = value; } }

    private Vector2 prevPosition;
    private Vector2 moveDir;
    private Renderer[] renderers;

    private bool blockMoved;   // 블럭 선택 조건부에 들어갈 isMoved 변수
    private Coroutine blockMoveCoroutine;

    private void Awake()
    {
        moveDir = new Vector2();
        renderers = GetComponentsInChildren<Renderer>();
        blockMoved = false;
    }

    private void OnEnable()
    {
        //오브젝트 이름 변경 전 구조체에 데이터 입력
        if (structBlockData.index_name != null)
        {
            //Debug.Log($"초기화 미진행 {structBlockData.index_name}");
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
        //Debug.Log($"로드 된 오브젝트명{this.gameObject.name}");
        structBlockData.Prefab_Name = this.gameObject.name;
        //난수를 활용하여 오브젝트 이름 변경
        this.gameObject.name = $"block_{Random.Range(1000, 3000)}";
        structBlockData.index_name = this.gameObject.name;

        InitBlockTransform();
    }

    public void InitBlockTransform()
    {
        structBlockData.platform_position = this.gameObject.transform.position;
        structBlockData.platform_rotate = this.gameObject.transform.localRotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log($"선택 된 블럭의 구조체 값 {structBlockData.Prefab_Name}  {structBlockData.platform_position}  {structBlockData.platform_rotate}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGoal)
        {
            // 테스트가 완료된 상태거나, 맵에 변동이 없는 경우 아무것도 동작하지 않음.
            if (GameManager.Data.IsTestDone || GameManager.Data.CheckMapDatas())
                return;

            //골인 경우 플레이어가 닿았는지 확인하여 Test모드를 마치며 저장 실행.
            if (collision.gameObject.tag == "Player")
            {
                // 플레이어의 y 값과 블록의 y 값을 비교하여 플레이어 캐릭터가 이 플랫폼 위에 있다면 .. isTestDone을 True로 
                if(collision.gameObject.transform.position.y
                    > this.gameObject.transform.position.y)
                {
                    // 플레이어 골인지점 도달, 맵 저장
                    GameManager.Data.IsTestDone = true;
                    // 세이브버튼을 통한 테스트 모드라면 저장 진행
                    if (GameManager.Mode.modeFrom == ModeFrom.SaveButton)
                        GameManager.Data.SaveMap();
                }
            }
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
            if (prevPosition == (Vector2)this.gameObject.transform.position) // 블럭의 위치 변화가 없다면 개별선택되게
            {
                if (PlatformList != null & PlatformList.Count > 0)
                    PlatformList.Clear();

                PlatformList.Add(this.gameObject);
                // 선택갱신
                GameManager.Data.SelectedBlocks = PlatformList.ToList();
            } else
            {
                InitBlockTransform();
                // mapData 갱신 필요
            }
        }
        if (eventData.button == InputButton.Right)
        {
            PlatformList.Clear(); // 플랫폼 그룹 클리어
            GameManager.Data.ClearBlocksRenderer();
            GameManager.Data.SelectedBlocks.Clear();
            GameManager.Data.mapData.RemovePlatforms(this.gameObject);
            GameManager.Data.RemovePlatform = this.gameObject;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Data.EditState != EditMode.Edit)
            return;

        // 현재 블럭이 선택된 경우 (PointerDown된 경우) 드래그와 함께 블럭 위치 이동
        //Debug.Log($"{this.gameObject.name} 블럭이 선택되어 드래그 중");
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
        // 이 블럭이 선택 됨.
        if(renderers != null)
        {
            foreach(Renderer renderer in renderers)
            {
                renderer.material.color = Color.green;
            }
        } else
        {
            //Debug.Log("NULL");
        }
    }

    public void DisSelected()
    {
        // 이 블럭이 선택해제 됨
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
