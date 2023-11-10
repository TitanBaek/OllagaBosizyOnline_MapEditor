using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeUI : BaseUI
{
    private bool isClicked;

    protected override void Awake()
    {
        base.Awake();
        isClicked = false;
        buttons["ModeButton"].onClick.AddListener(() => { ClickModeButton(); });
    }

    private void ClickModeButton()
    {
        if (isClicked)
            return;

        if (GameManager.Data.EditState == EditMode.Play)
            GameManager.Data.EditState = EditMode.Edit;
        else
            GameManager.Data.EditState = EditMode.Play;

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

