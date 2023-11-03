using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class MapData
{
    // 무조건 제일 높은 위치에 있는 블럭이 List의 맨 뒤에 위치하게 

    public Transform playerStartPoint;
    public List<Block> blocks;
    public List<string> platform_name;
    public List<Vector3> platform_pos;
    public List<Quaternion> platform_rot;
    public List<bool> platform_isGoal;

    public MapData()
    {
        platform_name = new List<string>();
        platform_pos = new List<Vector3>();
        platform_rot = new List<Quaternion>();
        platform_isGoal = new List<bool>();
    }

    public MapData(List<string> nameList,List<Vector3> posList,List<Quaternion> rotList,List<bool> goalList)
    {
        platform_name = nameList;
        platform_pos = posList;
        platform_rot = rotList;
        platform_isGoal = goalList;
    }

    public void SetStartPoint(Transform location)
    {
        playerStartPoint = location;
    }

    public void AddPlatforms(GameObject platform)
    {
        Block block = platform.GetComponent<Block>(); // isGoal 넣기
        platform_name.Add(platform.name);
        platform_pos.Add(platform.transform.position);
        platform_rot.Add(platform.transform.rotation);
        platform_isGoal.Add(block.IsGoal);
    }

    public void RemovePlatforms(GameObject platform)
    {
        // 인덱스 찾기
        platform_name.Remove(platform.name);
        platform_pos.Remove(platform.transform.position);
        platform_rot.Remove(platform.transform.rotation);
        GameManager.Resource.Destroy(platform);
    }

    public MapData MakeSavedMapData()
    {
        MapData resultMapData = new MapData();
        resultMapData.platform_name = this.platform_name.ToList<string>();
        resultMapData.platform_pos = this.platform_pos.ToList<Vector3>();
        resultMapData.platform_rot = this.platform_rot.ToList<Quaternion>();
        return resultMapData;
    }

    public void ClearMapData()
    {
        platform_name.Clear();
        platform_pos.Clear();
        platform_rot.Clear();
    }
}
