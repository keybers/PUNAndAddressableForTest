using Photon.Pun;
using UnityEngine;

public class MenuListing : MonoBehaviourPunCallbacks
{
    private CameraCanvas _cameraCanvas;

    public void OnClick_BackToGame()
    {
        //由暂停游戏到继续游戏
        _cameraCanvas.Hide();
        Time.timeScale = 1f;
    }

    public void OnClick_OpenSettingMenu()
    {
        _cameraCanvas.SettingCanvas.Show();
        _cameraCanvas.SettingCanvas.SettingMenu.Show();
    }

    public void OnClick_GameToExit()
    {
        //关闭UI
        _cameraCanvas.Hide();

        //结束游戏退回房间
        PhotonNetwork.LeaveRoom(true);
        PhotonNetwork.LoadLevel(0);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void FirstInitalize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        _cameraCanvas.MenuCanvas.Show();
        this.Show();
    }
}
