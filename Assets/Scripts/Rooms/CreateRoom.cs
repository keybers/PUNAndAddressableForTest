using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;

    private RoomCanvases _roomCanvases;

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
    }

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (_roomName.text.Length == 0)
        {
            Debug.LogError("roomName is null");
            return;
        }

        //create room
        //join or create room
        RoomOptions option = new RoomOptions();
        option.BroadcastPropsChangeToAll = true;//数据改变时，分发消息
        option.PublishUserId = true;
        option.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, option, TypedLobby.Default);//加入房间
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room successfuly.",this);
        _roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Created Room failed." + message, this);
    }
}
