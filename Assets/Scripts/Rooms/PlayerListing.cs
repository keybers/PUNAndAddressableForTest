using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using Photon.Pun;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    [Tooltip("房间信息表示框")]
    private Text _test;

    [Tooltip("判断房间是否准备")]
    public bool Ready = false;

    public Player Player { get; private set; }

    /// <summary>
    /// 设置进入玩家对象的信息
    /// </summary>
    /// <param name="player">玩家对象</param>
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        SetPlayerText(player);
    }

    /// <summary>
    /// 重载方法，更新传入的玩家参数
    /// </summary>
    /// <param name="targetPlayer">玩家对象</param>
    /// <param name="changedProps">更改参数</param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if(targetPlayer != null && targetPlayer == Player)
        {
            if (changedProps.ContainsKey("RandomNumber"))
            {
                SetPlayerText(targetPlayer);
            }
        }
    }

    /// <summary>
    /// 设置玩家对象的信息框
    /// </summary>
    /// <param name="player">玩家对象</param>
    private void SetPlayerText(Player player)
    {
        int result = -1;
        if (player.CustomProperties.ContainsKey("RandomNumber"))
            result = (int)player.CustomProperties["RandomNumber"];


        _test.text = result.ToString() + "," + player.NickName;
    }

}
