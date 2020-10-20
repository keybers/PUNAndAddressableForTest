using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviourPunCallbacks
{
    [SerializeReference]
    private CreateRoom _createRoom;

    [SerializeField]
    private RoomListingMenu _roomListingMenu;

    private RoomCanvases _roomCanvases;

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
        _createRoom.FirstInitialize(_roomCanvases);
        _roomListingMenu.FirstInitialize(_roomCanvases);
    }

    public void OnClick_ExitGame()
    {
        PhotonNetwork.LeaveLobby();
        Application.Quit();
    }
}
