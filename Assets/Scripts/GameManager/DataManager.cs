
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using Unity.VisualScripting;

public enum EditMode { Edit, Play };
public class DataManager : MonoBehaviour
{
    // ��ȣȭ�� ���Ǵ� Ű
    private string key128;

    // ������ ��� 
    private EditMode editState;
    public EditMode EditState { get { return editState; } set { editState = value; } }

    // �׽�Ʈ ����
    private bool isTestDone;
    public bool IsTestDone { get { return isTestDone; } set { isTestDone = value; } }

    // ���� �ҷ����� ������
    private bool testComplete;  // �׽�Ʈ ����
    public bool TestComplete { get { return testComplete; } set { testComplete = value; } }

    public MapData mapData;                 // ���̺� �� �� ������ ���� Mapdata Ŭ����
    public MapData savedMapData;                // ���� ����� MapData
    private DirectoryInfo directInfo;       // �����Ͱ� ����� ���丮 ������ ���� ����
    private FileInfo[] maps;                // �� DirectInfo ���� ������������ ��� �迭

    private string SAVE_DATA_DIRECTORY;     // ������ġ ���
    public string _SAVE_DATA_DIRECTORY { get { return SAVE_DATA_DIRECTORY; } }

    private string SAVE_FILENAME;           // ���� �� ���� �̸�
    public string _SAVE_FILENAME { get { return SAVE_FILENAME; } }

    private string LOAD_FILENAME;           // �ҷ��� ���� �̸�
    public string _LOAD_FILENAME { get { return LOAD_FILENAME; } }

    private GameObject topMostPlatform;
    public GameObject TopMostPlatform { get { return topMostPlatform; } set { TopMostPlatform = value; } }

    // EditArea ���� ������    
    [SerializeField] private bool isEditArea;
    public bool IsEditArea { get { return isEditArea; } set { isEditArea = value; } }

    // ��ġ�� �� ���翩�� ������

    // ��������

    // �� ��ġ �� ���� ������
    private string selectedBlock;
    public string SelectedBlock { get { return selectedBlock; } set { selectedBlock = value; } }

    private List<GameObject> currentBlocks;
    public List<GameObject> CurrentBlocks { get { return currentBlocks; } set { currentBlocks = value; } }

    /*
    private GameObject lastPlatform;
    public GameObject LastPlatform { get { return lastPlatform; } set {  lastPlatform = value; } }
    */

    private GameObject newPlatform;
    public GameObject NewPlatform { get { return newPlatform; } set { newPlatform = value; currentBlocks.Add(newPlatform); mapData.AddPlatforms(newPlatform); } }// ���̺� �����Ϳ� �÷��� �߰�         } }

    private GameObject removePlatform;
    public GameObject RemovePlatform { get { return removePlatform; } set {  removePlatform = value; currentBlocks.Remove(removePlatform); } }

    // �� ���� ������
    private List<GameObject> selectedBlocks;
    public List<GameObject> SelectedBlocks { get { return selectedBlocks; } set { ClearBlocksRenderer(); selectedBlocks = value; SetBlocksRenderer(); PlatformList.Clear(); } }
    
    // selectedBlocks �� ��� ���� ����� �� ��ü�� ���� ����Ʈ
    private List<GameObject> platformList;
    public List<GameObject> PlatformList { get { return platformList; } set { platformList = value; } }

    private bool mousePress;
    public bool MousePress { get {  return mousePress; } set {  mousePress = value; } }

    private void Awake()
    {
        selectedBlock = "";
        editState = EditMode.Edit;
        key128 = "3CEC2322643FC";
        isTestDone = false;
        InitSaveLoadDatas();
    }

    public void InitBlockSettingData()
    {
        isEditArea = false;
    }
      
    public void InitSaveLoadDatas()
    {
        mapData = new MapData();
        currentBlocks = new List<GameObject>();
        selectedBlocks = new List<GameObject>();
        platformList = new List<GameObject>();
        //�ܺ������� ����ǰԲ�
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Maps/";
        directInfo = new DirectoryInfo(SAVE_DATA_DIRECTORY);
        LOAD_FILENAME = "New Map";
        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
        {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
        LoadMaps();
    }

    public void SetLoadFileName(string fileName)
    {
        LOAD_FILENAME = fileName;
    }

    public void LoadMaps() // �� ����Ʈ ���� 
    { 
        topMostPlatform = null;
        maps = GetMapList();
    }

    // json���� ��ȯ�� ���� �� �����Ϳ� �ֱ� ������ �� �����Ͱ� ���� ��� ������ �������� ����.
    // ����Ʈ���� �񱳷� ����, json ��ȯ�� �񱳿� ����Ʈ ���� ��ȭ�� ���Ѱɷ� ����
    public bool CheckMapDatas()
    {
        //Debug.Log($"mapDatas {mapData.platform_index_name.Count} {mapData.platform_pos.Count} {mapData.platform_rot.Count}");
        //Debug.Log($"savedmapDatas {savedMapData.platform_index_name.Count} {savedMapData.platform_pos.Count} {savedMapData.platform_rot.Count}");
        
        if(Enumerable.SequenceEqual(mapData.platform_index_name, savedMapData.platform_index_name) &&
            Enumerable.SequenceEqual(mapData.platform_pos, savedMapData.platform_pos) &&
            Enumerable.SequenceEqual(mapData.platform_rot, savedMapData.platform_rot))
        {
            return true;
        }
        return false;
    }

    public void SetGoal()
    {
        topMostPlatform = FindTopMostPlatform();
        SetGoalPlatform(topMostPlatform);
    }

    public void SaveMap()
    {                        
        string json = Encrypt(JsonUtility.ToJson(mapData),key128);
        SAVE_FILENAME = $"Custom_MAP_{maps.Length + 1}.json";
        File.WriteAllText($"{SAVE_DATA_DIRECTORY}{SAVE_FILENAME}", json);

        GameManager.UI.ShowSystemLog(SAVE_FILENAME);

        savedMapData = mapData.MakeSavedMapData();
        LoadMaps();

        GameManager.Data.EditState = EditMode.Edit;
        GameManager.Mode.Innit(); // �÷��̸�� �ٲٱ�

        GameManager.Data.IsTestDone = false;
    }

    public void LoadMap(string path = null)
    {
        if (path.Contains("New Map"))
            return;

        AllDataClear();

        string loadJson = Decrypt(File.ReadAllText(path),key128);

        if (currentBlocks.Count >  0)
            ClearMap();


        mapData = JsonUtility.FromJson<MapData>(loadJson);
        savedMapData = mapData.MakeSavedMapData();

        if (mapData != null)
        {
            // ȭ����� ��� �������ʱ�ȭ
            // �ʿ� �÷����� ��ġ
            for (int i = 0; i < mapData.platform_index_name.Count; i++)
            {
                BlockData innitData = new BlockData();
                innitData.index_name = mapData.platform_index_name[i];
                innitData.Prefab_Name = mapData.platform_prefab_name[i];
                innitData.platform_position = mapData.platform_pos[i];
                innitData.platform_rotate = mapData.platform_rot[i];
                //Debug.Log($"�ҷ��� ���� �����̼� �� {innitData.platform_rotate.x} {innitData.platform_rotate.y} {innitData.platform_rotate.z}");
                GameObject loadedBlock = GameManager.Resource.Instantiate<GameObject>($"Platforms/{mapData.platform_prefab_name[i].Replace("(Clone)", "")}", mapData.platform_pos[i], mapData.platform_rot[i]);
                Block block = loadedBlock.GetComponent<Block>();
                loadedBlock.gameObject.name = block.StruckBlockData.index_name;
                //Debug.Log($"�� ������ �� ���� : {mapData.platform_isGoal[i]}");
                block.IsGoal = mapData.platform_isGoal[i];
                block.InitBlockData(innitData);
                currentBlocks.Add(loadedBlock);
            }
        }
    }

    public void ClearMap()
    {
        for (int i = 0;i < currentBlocks.Count; i++) // ��ġ�� �� Destroy
        {
            GameManager.Resource.Destroy(currentBlocks[i]);
        }
        currentBlocks.Clear();
        mapData.ClearMapData(); // save �� ������ �ʱ�ȭ
    }

    public FileInfo[] GetMapList()
    {
        FileInfo[] files = directInfo.GetFiles("*.json");
        //Debug.Log($"{files.Length}���� ������ �����");
        return files;
    }

    public GameObject FindTopMostPlatform()
    {
        PriorityQueue<Block> priorityQueue = new PriorityQueue<Block>();

        foreach(GameObject blocks in currentBlocks)
        {
            //Debug.Log($"{blocks.name} �� ���� �� {blocks.transform.position.y} ");
            priorityQueue.Enqueue(blocks,blocks.transform.position.y);
        }

        if (priorityQueue.Count > 0)
        {
            ClearBlockGoal();
            //Debug.Log($"�ֻ���ġ�� ������Ʈ�� Y �� : {priorityQueue.Peek().transform.position.y}");
            mapData.SetGoal(priorityQueue.Peek().gameObject);
            return priorityQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }    

    public bool SetGoalPlatform(GameObject platform)
    {
        Block block = platform.GetComponent<Block>();
        if(block == null)
            return false;

        return block.SetGoal();
    }

    public void ClearBlockGoal()
    {
        foreach (GameObject blocks in currentBlocks)
        {
            Block block = blocks.GetComponent<Block>();
            block.IsGoal = false;
        }

    }

    public void ClearBlocksRenderer()
    {
        //Debug.Log($"{selectedBlocks.Count}");
        foreach (GameObject blocks in selectedBlocks)
        {
            Block block = blocks.GetComponent<Block>();
            block?.DisSelected();
        }
    }

    public void SetBlocksRenderer()
    {
        foreach (GameObject blocks in selectedBlocks)
        {
            Block block = blocks.GetComponent<Block>();
            block?.ISelected();
        }
    }

    // New Map �̳� Load�� ���� NullException �� �������� �� ���� ����Ʈ �ʱ�ȭ
    public void AllDataClear()
    {
        platformList.Clear();
        selectedBlocks.Clear();
    }

    public static string Decrypt(string textToDecrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;

        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.KeySize = 128;

        rijndaelCipher.BlockSize = 128;

        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)

        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;

        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(plainText);
    }


    public static string Encrypt(string textToEncrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;

        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.KeySize = 128;

        rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)

        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;

        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

}