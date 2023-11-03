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
        if(fileDialog.ShowDialog() == DialogResult.OK) // ���̾�αװ� ���ȴ�
        {
            if((openStream = fileDialog.OpenFile()) != null) // ������ ���� �ƴ�.
            {
                // Ž����� ã�� Json �������� ��θ� ���� �� �ҷ�����
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
