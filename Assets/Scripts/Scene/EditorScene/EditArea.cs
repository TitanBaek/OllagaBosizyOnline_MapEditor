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
        // 블럭 설치 영역에서 마우스가 눌렸다면 true로
        GameManager.Data.IsEditArea = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 블럭 설치 영역에서 눌렸던 마우스가 때졌으면 fasle로
        GameManager.Data.IsEditArea = false;
    }
}
