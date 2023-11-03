using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private List<GameObject> PlatformList { get { return GameManager.Data.PlatformList; } }

    private bool MousePress { get { return GameManager.Data.MousePress; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ISelectable iselectable = collision.gameObject.GetComponent<ISelectable>();
        if(iselectable != null)
        {
            iselectable.ISelected();
            PlatformList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (MousePress)     // ���콺�� �����ִ� ���¿��� Remove�� �̷�����( �� ������ ���� ������ �巡�׸� ���������� ��� ����Ƽ�� ���� ����Ʈ���� ���� ��)
        {
            ISelectable iselectable = collision.gameObject.GetComponent<ISelectable>();
            if (iselectable != null)
            {
                iselectable.DisSelected();
                PlatformList.Remove(collision.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        // �����ɶ� DataManager�� ����� Platform�� ����
        GameManager.Data.SelectedBlocks = PlatformList.ToList();
        Debug.Log($"�÷�������Ʈ ����{GameManager.Data.PlatformList.Count} ����Ƽ�帮��Ʈ ����:{GameManager.Data.SelectedBlocks.Count}");
    }
}
