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
        if (MousePress)     // 마우스가 눌려있는 상태에만 Remove가 이뤄지게( 이 조건을 걸지 않으면 드래그를 종료했을때 모든 셀렉티드 블럭이 리스트에서 제거 됨)
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
        // 삭제될때 DataManager에 저장된 Platform들 갱신
        GameManager.Data.SelectedBlocks = PlatformList.ToList();
        Debug.Log($"플랫폼리스트 개수{GameManager.Data.PlatformList.Count} 셀릭티드리스트 개수:{GameManager.Data.SelectedBlocks.Count}");
    }
}
