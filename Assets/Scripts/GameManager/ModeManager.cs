using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ModeFrom { None,SaveButton,ChangeModeButton }
public class ModeManager : MonoBehaviour
{

    public EditMode editMode { get { return GameManager.Data.EditState; } }
    public GameObject modeObj;
    public ModeFrom modeFrom;

    public void Innit()
    {
        Debug.Log($"{editMode} Mode Innit ȣ��");
        InnitGameModeObject();
    }

    public void SetModeFrom(ModeFrom modeFrom)
    {
        this.modeFrom = modeFrom;
    }

    public void InnitGameModeObject()
    {
        // �ʱ�ȭ
        if(modeObj != null)
            DestroyGameModeObject();

        ClearPlatforms();

        if (editMode == EditMode.Edit)
        {
            Debug.Log("����Ʈ ����");
            GameManager.UI.BlockSlotsEnable(); // �� UI Ȱ��ȭ
            modeObj = GameManager.Resource.Instantiate<GameObject>("Mode/EditController");
        }
        else
        {
            Debug.Log("�÷��� ����");
            GameManager.UI.BlockSlotsDisable();  // �� UI ��Ȱ��ȭ
            GameManager.Data.SelectedBlock = ""; // �� ���� �ʱ�ȭ
            modeObj = GameManager.Resource.Instantiate<GameObject>("Mode/PlayController",new Vector3(0,0,0),Quaternion.identity);
        }
    }

    public void ClearPlatforms()
    {
        GameManager.Data.ClearBlocksRenderer();
        GameManager.Data.AllDataClear();
    }

    public void DestroyGameModeObject()
    {
        GameManager.Resource.Destroy(modeObj);
    }
}
