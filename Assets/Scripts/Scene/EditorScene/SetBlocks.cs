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
    float x = 0f; // �巡�� ������Ʈ��
    float y = 0f; // �巡�� ������Ʈ��
    float z = 0f; // �÷��� ȸ����

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

            if (selectedBlock == "")  // ���õ� ���� ���� �� Ŭ���� ������ ������ �簢 ������Ʈ ����
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

        if (selectedBlock == "")  // ���õ� ���� ���� �� ������ �簢�� ������Ʈ�� Trigger���� �ִ� Platforms�� SelectedInstalledBlocks ����Ʈ�� �߰�
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
        GameManager.Data.mapData.AddPlatforms(GameManager.Data.NewPlatform); // ���̺� �����Ϳ� �÷��� �߰�        
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

        if (selectedBlock == "") // ���õ� ���� ���� �� ������ �簢�� ������Ʈ�� �巡�� �� ���콺 ������ ��ġ��ŭ ũ�⸦ �ø��ų� �ٿ���.
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
