using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    private CameraCanvas _cameraCanvas;

    public void FirstInitialize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        Close();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClick_CloseButton()
    {
        _cameraCanvas.SettingCanvas.Close();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_ResolutionToggle()
    {
        //需要全局通知
        Screen.SetResolution(1920, 1080, true);
    }

}
