using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VcamController : MonoBehaviour
{
    [SerializeField] CinemachineConfiner2D confiner;
    [SerializeField] Collider2D cameraArea;

    private void Start()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
        cameraArea = GameObject.FindGameObjectWithTag("CamArea").GetComponent<Collider2D>();
        confiner.m_BoundingShape2D = cameraArea;
    }
}
