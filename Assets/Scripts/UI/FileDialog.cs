using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ookii.Dialogs;
using System.IO;
using System.Windows.Forms;

public class FileDialog : MonoBehaviour
{
    VistaOpenFileDialog fileDialog;
    Stream openStream = null;

    private void Start()
    {
        fileDialog = new VistaOpenFileDialog();
        fileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
        fileDialog.FilterIndex = 0;
        fileDialog.Title = "Open Game Map File(Json)";
        fileDialog.Multiselect = false;
    }

    public void FileOpen()
    {
        if(fileDialog.ShowDialog() == DialogResult.OK) // 다이얼로그가 열렸다
        {
            if((openStream = fileDialog.OpenFile()) != null) // 파일이 선택 됐다.
            {
                // 탐색기로 찾은 Json 데이터의 경로를 보내 맵 불러오기
                GameManager.Data.LoadMap(fileDialog.FileName);
            } else
            {

            }
        }
    }


    /*
    public void OnGUI()
    {
        if (GUI.Button(new Rect(100,100,100,50), "FileOpen"))
        {
            string fileName = FileOpen();
            if (!string.IsNullOrEmpty(fileName) )
            {
                Debug.Log(fileName);
            }
        }
    }
    */
}
