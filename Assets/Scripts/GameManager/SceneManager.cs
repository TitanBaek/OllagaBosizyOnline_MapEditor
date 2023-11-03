using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    private BaseScene curScene;
    public BaseScene CurScene
    {
        get
        {
            if (curScene == null)
                curScene = GameObject.FindObjectOfType<BaseScene>();

            return curScene;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadingRoutine(sceneName));
    }

    IEnumerator LoadingRoutine(string sceneName)
    {
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 0f;
        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName); // ��׶��� �ε�(�񵿱��)
        while (!oper.isDone)
        {
            // �ε����϶� ���ư��� �ݺ���
            yield return new WaitForSecondsRealtime(0.1f);
        }
        CurScene.LoadAsync();
        while (CurScene.progress < 1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(0.7f);
    }
}