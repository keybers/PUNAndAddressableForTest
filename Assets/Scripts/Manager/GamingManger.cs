using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingManger : MonoBehaviour
{
    public CameraCanvas cameraCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraCanvas.FirstInitialize();
        }
    }
}
