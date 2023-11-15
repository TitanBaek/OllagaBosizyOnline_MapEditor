using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Canvas inGameCanvas;
    Stack<PopUpUI> popUpStack;

    GameObject blockSlots;
    GameObject systemLog;
    TMP_Text textForSystemLog;

    Coroutine systemLogCoroutine;

    public void FadeIn(float timing)
    {

    }

    public void FadeOut(float timing)
    {

    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        inGameCanvas = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
    }

    public void BlockSlotsEnable()
    {
        // 2개 생성되는 걸 막기위해 null이 아니면 return
        if (blockSlots != null)
            return;

        Debug.Log("블럭 슬롯 생성");
        blockSlots = GameManager.Resource.Instantiate<GameObject>("UI/BlockSlots");
        blockSlots.transform.parent = inGameCanvas.gameObject.transform;
        SetRectTransform(blockSlots);
    }

    public void BlockSlotsDisable()
    {
        if (blockSlots == null)
            return;

        Debug.Log($"Block Slot Name {blockSlots.gameObject.name}");
        GameManager.Resource.Destroy(blockSlots.gameObject);
    }

    public void SystemLogTextEnable()
    {
        systemLog = GameManager.Resource.Instantiate<GameObject>("UI/SystemLogText");
        systemLog.transform.parent = inGameCanvas.gameObject.transform;
        InitSystemLog();
        SetRectTransform(systemLog);
    }

    public void InitSystemLog()
    {
        textForSystemLog = systemLog.GetComponent<TMP_Text>();
    }

    public void SystemLogTextDisable()
    {
        GameManager.Resource.Destroy(systemLog);
    }

    public void SetRectTransform(GameObject go)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
    }

    public void ShowSystemLog(string fileName)
    {
        systemLogCoroutine = StartCoroutine(SystemLogCoroutine($"filename : {fileName} save complete!"));
    }

    IEnumerator SystemLogCoroutine(string fileName)
    {
        systemLog.SetActive(true);
        yield return null;
        textForSystemLog.text = fileName;
        yield return new WaitForSeconds(2f);
        textForSystemLog.text = "";
        systemLog.SetActive(false);

    }

    /*
    // Pop UI
    public void ShowPopUpUI(string path)
    {

    }

    public void ClosePopUpUI()
    {

    }
    */

}
