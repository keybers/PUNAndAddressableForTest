using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
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
}
