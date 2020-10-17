using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeaveRoomMenu : MonoBehaviour
{
    private RoomCanvases _roomCanvases;

    public void OnClick_LeaveRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            _roomCanvases.CurrentRoomCanvas.OnDestroy();
        }
        else
        {
            _roomCanvases.CurrentRoomCanvas.Hide();
        }

        PhotonNetwork.LeaveRoom(true);
    }

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
    }

}
