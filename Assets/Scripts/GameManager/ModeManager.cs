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
        Debug.Log($"{editMode} Mode Innit 호출");
        InnitGameModeObject();
    }

    public void SetModeFrom(ModeFrom modeFrom)
    {
        this.modeFrom = modeFrom;
    }

    public void InnitGameModeObject()
    {
        // 초기화
        if(modeObj != null)
            DestroyGameModeObject();

        ClearPlatforms();

        if (editMode == EditMode.Edit)
        {
            Debug.Log("에디트 모드로");
            GameManager.UI.BlockSlotsEnable(); // 블럭 UI 활성화
            modeObj = GameManager.Resource.Instantiate<GameObject>("Mode/EditController");
        }
        else
        {
            Debug.Log("플레이 모드로");
            GameManager.UI.BlockSlotsDisable();  // 블럭 UI 비활성화
            GameManager.Data.SelectedBlock = ""; // 블럭 선택 초기화
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
