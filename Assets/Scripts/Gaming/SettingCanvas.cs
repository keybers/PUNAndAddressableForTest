using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCanvas : MonoBehaviour
{
    [SerializeField]
    private SettingMenu _settingMenu;
    public SettingMenu SettingMenu
    {
        get
        {
            return _settingMenu;
        }
    }

    private CameraCanvas _cameraCanvas;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void FirstInitialize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        _settingMenu.FirstInitialize(_cameraCanvas);
        Close();
    }
}
