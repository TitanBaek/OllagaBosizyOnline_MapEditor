using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : BaseScene
{
    FileInfo[] maps;
    [SerializeField] private TMP_Dropdown dropDown;

    protected override void Awake()
    {
        base.Awake();
        dropDown.onValueChanged.AddListener(delegate {
            OnDropdownValueChanged(dropDown);
        });
    }
    protected override void Start()
    {
        maps = GameManager.Data.GetMapList();
        SetDropDown();
    }

    public void SetDropDown()
    {
        List<FileInfo> mapList = maps.ToList();
        foreach (FileInfo map in mapList)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = map.Name;
            dropDown.options.Add(option);
        }
    }

    private void OnDropdownValueChanged(TMP_Dropdown dropdown)
    {
        Debug.Log($"{dropdown.options[dropdown.value].text}∑Œ º±≈√");
        GameManager.Data.SetLoadFileName(dropdown.options[dropdown.value].text);
    }

    public override void Clear()
    {
    }


    protected override IEnumerator LoadingRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        progress = 1f;
    }

}
