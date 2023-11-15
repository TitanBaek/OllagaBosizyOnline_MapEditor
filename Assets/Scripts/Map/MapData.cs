using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class MapData
{
    // ������ ���� ���� ��ġ�� �ִ� ���� List�� �� �ڿ� ��ġ�ϰ� 

    public Transform playerStartPoint;
    public List<Block> blocks;
    public List<string> platform_index_name;
    public List<string> platform_prefab_name;
    public List<Vector3> platform_pos;
    public List<Quaternion> platform_rot;
    public List<bool> platform_isGoal;

    public MapData()
    {
        platform_index_name = new List<string>();
        platform_prefab_name = new List<string>();
        platform_pos = new List<Vector3>();
        platform_rot = new List<Quaternion>();
        platform_isGoal = new List<bool>();
    }

    public MapData(List<string> nameList,List<Vector3> posList,List<Quaternion> rotList,List<bool> goalList)
    {
        platform_index_name = nameList;
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
        Block block = platform.GetComponent<Block>();

        platform_index_name.Add(block.StruckBlockData.index_name);
        platform_prefab_name.Add(block.StruckBlockData.Prefab_Name);
        platform_pos.Add(block.StruckBlockData.platform_position);
        platform_rot.Add(block.StruckBlockData.platform_rotate);
        platform_isGoal.Add(block.StruckBlockData.isGoal);
    }

    public void ChangePlatformsPosition(GameObject platform)
    {
        Debug.Log($"{platform.name} �̵�");
        // ������ �Ǿ��ִ� ���� ����Ʈ �� �ε��� ã��
        int index = platform_index_name.IndexOf(platform.gameObject.name);

        // StructData ��������
        Block block = platform.GetComponent<Block>();
        platform_pos[index] = block.StruckBlockData.platform_position;
    }

    public void ChangePlatformsRotation(GameObject platform)
    {
        Debug.Log($"{platform.name} ȸ��");
        // ������ �Ǿ��ִ� ���� ����Ʈ �� �ε��� ã��
        int index = platform_index_name.IndexOf(platform.gameObject.name);

        // StructData ��������
        Block block = platform.GetComponent<Block>();
        block.InitBlockTransform();

        platform_rot[index] = block.StruckBlockData.platform_rotate;
    }


    public void RemovePlatforms(GameObject platform)
    {
        // ������ �Ǿ��ִ� ���� ����Ʈ �� �ε��� ã��
        int index = platform_index_name.IndexOf(platform.gameObject.name);

        platform_index_name.RemoveAt(index);
        platform_prefab_name.RemoveAt(index);
        platform_pos.RemoveAt(index);
        platform_rot.RemoveAt(index);
        platform_isGoal.RemoveAt(index);

        GameManager.Resource.Destroy(platform);
    }

    public MapData MakeSavedMapData()
    {
        MapData resultMapData = new MapData();
        resultMapData.platform_index_name = this.platform_index_name.ToList<string>();
        resultMapData.platform_prefab_name = this.platform_prefab_name.ToList<string>();
        resultMapData.platform_pos = this.platform_pos.ToList<Vector3>();
        resultMapData.platform_rot = this.platform_rot.ToList<Quaternion>();
        resultMapData.platform_isGoal = this.platform_isGoal.ToList<bool>();
        return resultMapData;
    }

    public void ClearMapData()
    {
        platform_index_name.Clear();
        platform_prefab_name.Clear();
        platform_pos.Clear();
        platform_rot.Clear();
        platform_isGoal.Clear();
    }

    public void SetGoal(GameObject obj)
    {
        Block block = obj.GetComponent<Block>();
        ClearGoal();
        int index = platform_index_name.IndexOf(block.StruckBlockData.index_name);
        platform_isGoal[index] = true;
    }

    public void ClearGoal()
    {
        for(int i = 0; i < platform_isGoal.Count; i++) { 
            platform_isGoal[i] = false;
        }
    }
}
