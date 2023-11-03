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

        isClicked = true;
        GameManager.Data.SaveMap();
        StartCoroutine(SwitchClicked());
    }

    public void DoLoad()
    {
        // �ҷ����� FileDialog ����

    }

    public void DoNewMapData()
    {
        // �ʱ�ȭ
        GameManager.Data.ClearMap();
    }

    IEnumerator SwitchClicked()
    {
        Debug.Log("�ڷ�ƾ ����");
        yield return new WaitForSeconds(1f);
        isClicked = false;
        Debug.Log("�ڷ�ƾ ��");
        yield return null;
    }
}
