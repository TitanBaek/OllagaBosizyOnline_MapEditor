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
            // 맵에 변동사항이 없다.
            return;
        }

        GameManager.Data.SetGoal();

        // 테스트 여부를 확인하여, 테스트 전이라면 테스트 모드로 돌입
        if (!GameManager.Data.IsTestDone)
        {
            // 테스트 플레이가 진행되지 않았다.
            GameManager.Data.EditState = EditMode.Play;
            GameManager.Mode.SetModeFrom(ModeFrom.SaveButton);
            GameManager.Mode.Innit(); // 플레이모드 바꾸기
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

