using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingManger : MonoBehaviour
{
    public CameraCanvas cameraCanvas;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraCanvas.FirstInitialize();
        }
    }

}
