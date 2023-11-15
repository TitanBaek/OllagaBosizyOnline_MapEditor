using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject spawnObejct;

    private void OnEnable()
    {
        spawnObejct = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");

        Debug.Log($"x {spawnObejct.transform.position.x} y {spawnObejct.transform.position.y} z {spawnObejct.transform.position.z} �� �̵�����");
        if(spawnObejct == null)
        {
            Debug.Log("��������Ʈ NULL");
        }
        this.gameObject.transform.position = spawnObejct.transform.position;
    }
}
