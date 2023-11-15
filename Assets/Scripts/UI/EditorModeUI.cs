using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorModeUI : BaseUI
{
    private bool isClicked;

    protected override void Awake()
    {
        base.Awake();
        isClicked = false;
        buttons["NewButton"].onClick.AddListener(() => { ClickNewButton(); });
        buttons["SaveButton"].onClick.AddListener(() => { ClickSaveButton(); });
        buttons["LoadButton"].onClick.AddListener(() => { ClickLoadButton(); });
        buttons["ModeButton"].onClick.AddListener(() => { ClickModeButton(); });
    }    

    private void ClickNewButton()
    {
        GameManager.Data.ClearMap();
    }

    private void ClickSaveButton()
    {
        if (isClicked)
            return;

        if (GameManager.Data.CheckMapDatas())
        {
            Debug.Log("�� �������� ����");
            return;
        }

        Debug.Log($"�׽�Ʈ ���� {GameManager.Data.IsTestDone}");

        GameManager.Data.SetGoal();


        // �׽�Ʈ ���θ� Ȯ���Ͽ�, �׽�Ʈ ���̶�� �׽�Ʈ ���� ����
        if (!GameManager.Data.IsTestDone)
        {
            Debug.Log("�׽�Ʈ �÷��̰� �ʿ��ؿ�.");
            // Play == TestPlay
            GameManager.Data.EditState = EditMode.Play;
            GameManager.Mode.SetModeFrom(ModeFrom.SaveButton);
            GameManager.Mode.Innit(); // �÷��̸�� �ٲٱ�
            return;
        }

        isClicked = true;
        GameManager.Data.SaveMap();
        StartCoroutine(SwitchClicked());
    }

    private void ClickLoadButton()
    {
        GameManager.File.FileOpen();
    }

    private void ClickModeButton()
    {
        if (isClicked)
            return;

        GameManager.Data.SetGoal();

        if (GameManager.Data.EditState == EditMode.Play)
        {
            GameManager.Data.EditState = EditMode.Edit;
        }
        else
        {
            GameManager.Data.EditState = EditMode.Play;
            GameManager.Mode.SetModeFrom(ModeFrom.ChangeModeButton);
        }

        GameManager.Mode.Innit();
        isClicked = true;
        StartCoroutine(SwitchClicked());
    }

    IEnumerator SwitchClicked()
    {
        yield return new WaitForSeconds(1f);
        isClicked = false;
        yield return null;
    }
}

