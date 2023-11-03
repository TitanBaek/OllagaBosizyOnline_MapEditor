using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EditArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // �� ��ġ �������� ���콺�� ���ȴٸ� true��
        GameManager.Data.IsEditArea = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // �� ��ġ �������� ���ȴ� ���콺�� �������� fasle��
        GameManager.Data.IsEditArea = false;
    }
}
