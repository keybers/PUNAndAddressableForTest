using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public PlayerListing _playerListing;

    [SerializeField]
    private Transform _content;

    [SerializeField]
    private Text _readyUpText;

    private List<PlayerListing> _listings = new List<PlayerListing>();

    private RoomCanvases _roomCanvases;

    private bool _ready = false;

    public override void OnEnable()
    {
        base.OnEnable();
        SetReadyUp(false);
        GetCurrentRoomPlayers();
    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (_ready)
        {
            _readyUpText.text = "R";
            _readyUpText.color = Color.white;
        }
        else
        {
            _readyUpText.text = "N";
            _readyUpText.color = Color.grey;
        }

    }

    public override void OnDisable()
    {
        base.OnDisable();
        for(int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }

        _listings.Clear();
    }

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
    }

    /// <summary>
    /// 获取当前房间的玩家信息
    /// </summary>
    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if(PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach(KeyValuePair<int,Player> playInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playInfo.Value);
        }

    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);//match表示匹配条件
        if(index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                _listings.Add(listing);
                listing.SetPlayerInfo(player);
            }
        }
    }

    private void LeftPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LeftPlayerListing(otherPlayer);
    }

    //当换房主的时候,前任房主退出房间，和按退出按钮一样的效果
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //检查每个玩家，除本人外看是否都准备好了
            for (int i = 0; i < _listings.Count; i++)
            {
                if (_listings[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if (!_listings[i].Ready)
                        return;
                }
            }

            //上锁房间
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel(1);
        }
    }

    public void OnClick_ReadyUp()
    {
        //只有不是房主才可选准备或不准备
        if (!PhotonNetwork.IsMasterClient)
        {
            SetReadyUp(!_ready);
            //无论客户端发送的是什么，都将执行RPC代码
            base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _ready);
            //base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient,true, PhotonNetwork.LocalPlayer, _ready);,加密数据，防止数据被篡改
        }
    }

    [PunRPC]//Remote Procedure Calls，远程程序调用
    private void RPC_ChangeReadyState(Player player,bool ready)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].Ready = ready;
        }
    }

}
