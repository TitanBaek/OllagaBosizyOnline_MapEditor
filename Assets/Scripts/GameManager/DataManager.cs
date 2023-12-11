
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
    // 암호화에 사용되는 키
    private string key128;

    // 에디터 모드 
    private EditMode editState;
    public EditMode EditState { get { return editState; } set { editState = value; } }

    // 테스트 여부
    private bool isTestDone;
    public bool IsTestDone { get { return isTestDone; } set { isTestDone = value; } }

    // 저장 불러오기 구현부
    private bool testComplete;  // 테스트 여부
    public bool TestComplete { get { return testComplete; } set { testComplete = value; } }

    public MapData mapData;                 // 세이브 할 맵 정보를 담을 Mapdata 클래스
    public MapData savedMapData;                // 현재 저장된 MapData
    private DirectoryInfo directInfo;       // 데이터가 저장될 디렉토리 정보를 담은 변수
    private FileInfo[] maps;                // 위 DirectInfo 내의 파일정보들을 담는 배열

    private string SAVE_DATA_DIRECTORY;     // 저장위치 경로
    public string _SAVE_DATA_DIRECTORY { get { return SAVE_DATA_DIRECTORY; } }

    private string SAVE_FILENAME;           // 저장 할 파일 이름
    public string _SAVE_FILENAME { get { return SAVE_FILENAME; } }

    private string LOAD_FILENAME;           // 불러올 파일 이름
    public string _LOAD_FILENAME { get { return LOAD_FILENAME; } }

    private GameObject topMostPlatform;
    public GameObject TopMostPlatform { get { return topMostPlatform; } set { TopMostPlatform = value; } }

    // EditArea 여부 구현부    
    [SerializeField] private bool isEditArea;
    public bool IsEditArea { get { return isEditArea; } set { isEditArea = value; } }

    // 겹치는 블럭 존재여부 구현부

    // 구현예정

    // 블럭 설치 및 삭제 구현부
    private string selectedBlock;
    public string SelectedBlock { get { return selectedBlock; } set { selectedBlock = value; } }

    private List<GameObject> currentBlocks;
    public List<GameObject> CurrentBlocks { get { return currentBlocks; } set { currentBlocks = value; } }

    /*
    private GameObject lastPlatform;
    public GameObject LastPlatform { get { return lastPlatform; } set {  lastPlatform = value; } }
    */

    private GameObject newPlatform;
    public GameObject NewPlatform { get { return newPlatform; } set { newPlatform = value; currentBlocks.Add(newPlatform); mapData.AddPlatforms(newPlatform); } }// 세이브 데이터에 플랫폼 추가         } }

    private GameObject removePlatform;
    public GameObject RemovePlatform { get { return removePlatform; } set {  removePlatform = value; currentBlocks.Remove(removePlatform); } }

    // 블럭 선택 구현부
    private List<GameObject> selectedBlocks;
    public List<GameObject> SelectedBlocks { get { return selectedBlocks; } set { ClearBlocksRenderer(); selectedBlocks = value; SetBlocksRenderer(); PlatformList.Clear(); } }
    
    // selectedBlocks 로 담기 직전 예비로 블럭 개체를 담을 리스트
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
        //외부폴더에 저장되게끔
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

    public void LoadMaps() // 맵 리스트 갱신 
    { 
        topMostPlatform = null;
        maps = GetMapList();
    }

    // json으로 변환한 현재 맵 데이터와 최근 저장한 맵 데이터가 같은 경우 저장을 진행하지 않음.
    // 리스트끼리 비교로 수정, json 변환후 비교와 리스트 비교중 부화가 덜한걸로 진행
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
        GameManager.Mode.Innit(); // 플레이모드 바꾸기

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
            // 화면상의 모든 블럭들을초기화
            // 맵에 플랫폼들 배치
            for (int i = 0; i < mapData.platform_index_name.Count; i++)
            {
                BlockData innitData = new BlockData();
                innitData.index_name = mapData.platform_index_name[i];
                innitData.Prefab_Name = mapData.platform_prefab_name[i];
                innitData.platform_position = mapData.platform_pos[i];
                innitData.platform_rotate = mapData.platform_rot[i];
                //Debug.Log($"불러온 블럭의 로테이션 값 {innitData.platform_rotate.x} {innitData.platform_rotate.y} {innitData.platform_rotate.z}");
                GameObject loadedBlock = GameManager.Resource.Instantiate<GameObject>($"Platforms/{mapData.platform_prefab_name[i].Replace("(Clone)", "")}", mapData.platform_pos[i], mapData.platform_rot[i]);
                Block block = loadedBlock.GetComponent<Block>();
                loadedBlock.gameObject.name = block.StruckBlockData.index_name;
                //Debug.Log($"맵 데이터 골 여부 : {mapData.platform_isGoal[i]}");
                block.IsGoal = mapData.platform_isGoal[i];
                block.InitBlockData(innitData);
                currentBlocks.Add(loadedBlock);
            }
        }
    }

    public void ClearMap()
    {
        for (int i = 0;i < currentBlocks.Count; i++) // 배치된 블럭 Destroy
        {
            GameManager.Resource.Destroy(currentBlocks[i]);
        }
        currentBlocks.Clear();
        mapData.ClearMapData(); // save 될 데이터 초기화
    }

    public FileInfo[] GetMapList()
    {
        FileInfo[] files = directInfo.GetFiles("*.json");
        //Debug.Log($"{files.Length}개의 파일이 저장됨");
        return files;
    }

    public GameObject FindTopMostPlatform()
    {
        PriorityQueue<Block> priorityQueue = new PriorityQueue<Block>();

        foreach(GameObject blocks in currentBlocks)
        {
            //Debug.Log($"{blocks.name} 의 높이 값 {blocks.transform.position.y} ");
            priorityQueue.Enqueue(blocks,blocks.transform.position.y);
        }

        if (priorityQueue.Count > 0)
        {
            ClearBlockGoal();
            //Debug.Log($"최상위치의 오브젝트의 Y 값 : {priorityQueue.Peek().transform.position.y}");
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

    // New Map 이나 Load로 인한 NullException 을 막기위한 블럭 선택 리스트 초기화
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