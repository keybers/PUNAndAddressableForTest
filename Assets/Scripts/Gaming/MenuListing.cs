using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class MenuListing : MonoBehaviourPunCallbacks
{
    private CameraCanvas _cameraCanvas;

    public void OnClick_BackToGame()
    {
        //由暂停游戏到继续游戏
        _cameraCanvas.Hide();

    }

    public void OnClick_OpenSettingMenu()
    {
        _cameraCanvas.SettingCanvas.Show();
    }

    public void OnClick_GameToExit()
    {
        //关闭UI
        _cameraCanvas.Hide();

        //结束游戏退回房间

    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void FirstInitalize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        this.Show();
    }
}
