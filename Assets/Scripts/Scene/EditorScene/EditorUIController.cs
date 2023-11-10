using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorUIController : MonoBehaviour 
{
    private bool canSave;
    [SerializeField] private Button newButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Text modeText;

    public void Start()
    {
        Innit();
    }

    public void Innit()
    {
        modeText.text = GameManager.Data.EditState.ToString();
    }
}
