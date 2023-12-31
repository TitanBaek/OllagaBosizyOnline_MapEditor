using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtons : MonoBehaviour
{
    [SerializeField] bool isClicked;
    Coroutine buttonCoroutine;

    private void Awake()
    {
        isClicked = false;
    }

    public void Goto(string sceneName)
    {
        if (isClicked)
            return;

        isClicked = true;
        GameManager.Scene.LoadScene(sceneName);
    }

    public void DoSave()
    {
        if(isClicked)
            return;

        // 테스트 여부를 확인하여, 테스트 전이라면 테스트 모드로 돌입
        if (!GameManager.Data.IsTestDone)
        {
            // Play == TestPlay
            GameManager.Data.EditState = EditMode.Play;

        }

        isClicked = true;
        GameManager.Data.SaveMap();
        StartCoroutine(SwitchClicked());
    }

    public void DoLoad()
    {
        // 불러오기 FileDialog 실행

    }

    public void DoNewMapData()
    {
        // 초기화
        GameManager.Data.ClearMap();
    }

    IEnumerator SwitchClicked()
    {
        Debug.Log("코루틴 시작");
        yield return new WaitForSeconds(1f);
        isClicked = false;
        Debug.Log("코루틴 끝");
        yield return null;
    }
}
