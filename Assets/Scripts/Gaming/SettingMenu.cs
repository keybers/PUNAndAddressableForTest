using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    private CameraCanvas _cameraCanvas;
    private Toggle Rtoggle;

    public void FirstInitialize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        Rtoggle = gameObject.GetComponentInChildren<Toggle>();
        Rtoggle.isOn = false;

        Back();
    }

    public void Back()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClick_BackButton()
    {
        Back();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_ResolutionToggle()
    {
        //需要全局通知
        if (Rtoggle.isOn)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.SetResolution(1600, 960, false);
        }
    }

}
