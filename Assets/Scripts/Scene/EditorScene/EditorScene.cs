using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorScene : BaseScene
{

    protected override void Start()
    {
        //GameManager.Data.GetMapList();
        GameManager.Data.LoadMap(GameManager.Data._SAVE_DATA_DIRECTORY + GameManager.Data._LOAD_FILENAME);           
    }
    
    public override void Clear()
    {

    }

    protected override IEnumerator LoadingRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        while (!InitBlockSlots())
        {
            yield return null;
        }
        progress = 1f;
    }

    public bool InitBlockSlots()
    {
        GameManager.UI.Init();
        GameManager.Mode.Innit();   // 컨트롤러 instantiate     
        GameManager.UI.SystemLogTextEnable();                               // 화면 중앙 텍스트 생성
        return true;
    }
}
