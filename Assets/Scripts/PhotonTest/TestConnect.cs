using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Server");

        PhotonNetwork.SendRate = 10;//每秒发送数据多少次
        PhotonNetwork.SerializationRate = 5; //每秒接收多少序列化，如果比接收高，则接收数据会受到影响
        PhotonNetwork.AutomaticallySyncScene = true;//确保加载场景的时候，所有玩家异步加载同样的场景
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.ConnectUsingSettings();//使用设置进行连接

    }

    public override void OnConnectedToMaster()//重写连接主服务器函数
    {
        Debug.Log("Connected to Server");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);//当前客户端用户名
        
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Connected to fail:" + cause.ToString());
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Join a Lobby");
    }
}
