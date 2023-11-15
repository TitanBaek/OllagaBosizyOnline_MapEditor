using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject spawnObejct;

    private void OnEnable()
    {
        spawnObejct = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");

        Debug.Log($"x {spawnObejct.transform.position.x} y {spawnObejct.transform.position.y} z {spawnObejct.transform.position.z} 로 이동하자");
        if(spawnObejct == null)
        {
            Debug.Log("스폰포인트 NULL");
        }
        this.gameObject.transform.position = spawnObejct.transform.position;
    }
}
