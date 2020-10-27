using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCanvas : MonoBehaviourPunCallbacks
{
    [SerializeReference]
    private MenuCanvas _menuCanvas;
    public MenuCanvas MenuCanvas
    {
        get
        {
            return _menuCanvas;
        }
    }

    [SerializeReference]
    private SettingCanvas _settingCanvas;
    public SettingCanvas SettingCanvas
    {
        get
        {
            return _settingCanvas;
        }
    }

    public void FirstInitialize()
    {
        if (!this.gameObject.activeSelf)
            this.gameObject.SetActive(true);
        this._menuCanvas.FirstInitialize(this);
        this._settingCanvas.FirstInitialize(this);

        Time.timeScale = 0;
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
