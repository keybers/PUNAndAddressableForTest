using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuListing : MonoBehaviourPunCallbacks
{
    private SettingCanvas _settingCanvas;

    public void FirstInitialize(SettingCanvas settingCanvas)
    {
        _settingCanvas = settingCanvas;
    }

    public void OnClick_BackToGame()
    {
        //由暂停游戏到继续游戏
    }

    public void OnClick_GameToExit()
    {
        //结束游戏退回房间
    }

    public void OnClick_OpenSettingMenu()
    {
        //打开设置菜单
        _settingCanvas.Show();
    }

}
