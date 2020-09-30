using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private PlayerListingMenu _playerListingMenu;

    [SerializeField]
    private LeaveRoomMenu _leaveRoomMenu;

    public LeaveRoomMenu LeaveRoomMenu
    {
        get
        {
            return _leaveRoomMenu;
        }
    }

    private RoomCanvases _roomCanvases;

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
        _playerListingMenu.FirstInitialize(roomCanvases);
        _leaveRoomMenu.FirstInitialize(roomCanvases);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
