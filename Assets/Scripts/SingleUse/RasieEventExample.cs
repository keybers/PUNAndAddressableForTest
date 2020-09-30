using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Runtime.Remoting;

public class RasieEventExample : MonoBehaviourPun
{
    private SpriteRenderer _spriteRenderer;

    private const byte COLOR_CHANGE_EVENT = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //每次开始都将执行该操作
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    //接收到时间的信息
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code == COLOR_CHANGE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;//自定义数据
            float r = (float)datas[0];
            float g = (float)datas[1];
            float b = (float)datas[2];

            _spriteRenderer.color = new Color(r, g, b, 1f);

        }
    }

    private void Update()
    {
        //对象的拥有者可以更改
        if (base.photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        _spriteRenderer.color = new Color(r, g, b,1f);

        object[] dates = new object[] {r, g, b };

        //发送数据
        PhotonNetwork.RaiseEvent(COLOR_CHANGE_EVENT, dates, RaiseEventOptions.Default, SendOptions.SendReliable);
        
    
    }
}
