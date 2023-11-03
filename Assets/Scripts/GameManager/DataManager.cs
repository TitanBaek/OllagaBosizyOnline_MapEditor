
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    // 현재 플레이 씬이 에디터에서 넘어온 테스트 용도인지, 메인메뉴에서 Start를 눌러 넘어온 플레이 용도인지 체크하는 부분
    private bool isTestMode;
    public bool IsTestMode { get { return isTestMode; } set { isTestMode = value; } }

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

    public bool CheckMapDatas()
    {
        bool resultData = false;
        if(Enumerable.SequenceEqual(mapData.platform_index_name, savedMapData.platform_index_name) &&
            Enumerable.SequenceEqual(mapData.platform_pos, savedMapData.platform_pos) &&
            Enumerable.SequenceEqual(mapData.platform_rot, savedMapData.platform_rot))
        {
            resultData = true;
        }
        return resultData;
    }

    public void SaveMap()
    {
        // json으로 변환한 현재 맵 데이터와 최근 저장한 맵 데이터가 같은 경우 저장을 진행하지 않음.
        // 리스트끼리 비교로 수정, json 변환후 비교와 리스트 비교중 부화가 덜한걸로 진행
        if (CheckMapDatas())
        {
            return;
        }

        topMostPlatform = FindTopMostPlatform();
        SetGoalPlatform(topMostPlatform);

        string json = JsonUtility.ToJson(mapData);
        SAVE_FILENAME = $"Custom_MAP_{maps.Length + 1}.json";
        File.WriteAllText($"{SAVE_DATA_DIRECTORY}{SAVE_FILENAME}", json);

        GameManager.UI.ShowSystemLog(SAVE_FILENAME);

        savedMapData = mapData.MakeSavedMapData();
        LoadMaps();
    }

    public void LoadMap(string path = null)
    {
        if (path.Contains("New Map"))
            return;

        AllDataClear();

        string loadJson = File.ReadAllText(path);

        if (currentBlocks.Count >  0)
            ClearMap();

        mapData = JsonUtility.FromJson<MapData>(loadJson);

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
                GameObject loadedBlock = GameManager.Resource.Instantiate<GameObject>($"Platforms/{mapData.platform_prefab_name[i].Replace("(Clone)", "")}", mapData.platform_pos[i], mapData.platform_rot[i]);
                Block block = loadedBlock.GetComponent<Block>();
                loadedBlock.gameObject.name = block.StruckBlockData.index_name;
                block.InitBlockData(innitData);
                //block.StruckBlockData.platform_position = new Vector3();
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
        Debug.Log($"{files.Length}개의 파일이 저장됨");
        return files;
    }

    public GameObject FindTopMostPlatform()
    {
        PriorityQueue<Block> priorityQueue = new PriorityQueue<Block>();

        foreach(GameObject blocks in currentBlocks)
        {
            Debug.Log($"{blocks.name} 의 높이 값 {blocks.transform.position.y} ");
            priorityQueue.Enqueue(blocks,blocks.transform.position.y);
        }

        if (priorityQueue.Count > 0)
        {
            Debug.Log($"최상위치의 오브젝트의 Y 값 : {priorityQueue.Peek().transform.position.y}");
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

    public void ClearBlocksRenderer()
    {
        Debug.Log($"{selectedBlocks.Count}");
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

}