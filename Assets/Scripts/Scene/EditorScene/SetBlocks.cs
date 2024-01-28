using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

public class SetBlocks : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    DragObject dragObject;
    string selectedBlock { get { return GameManager.Data.SelectedBlock; } }
    bool pressLeftClick = false;
    float x = 0f; // 드래그 오브젝트용
    float y = 0f; // 드래그 오브젝트용
    float z = 0f; // 플랫폼 회전용

    private Vector3 GetPointerPosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
        Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(GameManager.Data.EditState != EditMode.Edit)
            return;

        GameManager.Data.MousePress = true;

        if (eventData.button == InputButton.Left)
        {

            if (selectedBlock == "")  // 선택된 블럭이 없을 때 클릭한 지점에 반투명 사각 오브젝트 생성
            {
                dragObject = GameManager.Resource.Instantiate<DragObject>("UI/DragObject_WithRender", GetPointerPosition(), Quaternion.identity);
                return;
            }
            else
            {
                SetPlatform(GetPointerPosition());
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.Data.EditState != EditMode.Edit)
            return;

        GameManager.Data.MousePress = false;

        if (selectedBlock == "")  // 선택된 블럭이 없을 때 생성된 사각형 오브젝트의 Trigger내에 있는 Platforms를 SelectedInstalledBlocks 리스트에 추가
        {
            GameManager.Resource.Destroy(dragObject.gameObject);
            dragObject = null;
        }
    }

    public void SetPlatform(Vector3 blockSettingPosition)
    {
        if (selectedBlock == "")
        {
            return;
        }
        GameManager.Data.NewPlatform = GameManager.Resource.Instantiate<GameObject>($"Platforms/{selectedBlock}", blockSettingPosition, Quaternion.identity);
    }

    public void SavePlatform()
    {
        if (selectedBlock == "")
        {
            return;
        }
        GameManager.Data.mapData.AddPlatforms(GameManager.Data.NewPlatform); // 세이브 데이터에 플랫폼 추가        
    }

    public void RemovePlatform()
    {
        GameManager.Data.mapData.RemovePlatforms(GameManager.Data.RemovePlatform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Data.EditState != EditMode.Edit)
            return;

        if (eventData.button != InputButton.Left)
            return;

        if (selectedBlock == "") // 선택된 블럭이 없을 때 생성된 사각형 오브젝트를 드래그 시 마우스 포인터 위치만큼 크기를 늘리거나 줄여줌.
        {
            if(dragObject != null)
            {
                x = eventData.delta.x * Time.deltaTime * 12.5f;
                y = eventData.delta.y * Time.deltaTime * 12.5f;
                dragObject.transform.localScale += new Vector3(x,-y);
            }
        }
        else
        {
            z = eventData.delta.x * Time.deltaTime * 50;
            GameManager.Data.NewPlatform.transform.Rotate(0f, 0f, z, Space.World);
            GameManager.Data.mapData.ChangePlatformsRotation(GameManager.Data.NewPlatform);
        }
    }

    }
