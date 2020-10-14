using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCanvas : MonoBehaviour
{
    private CameraCanvas _cameraCanvas;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void FirstInitialize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
}
