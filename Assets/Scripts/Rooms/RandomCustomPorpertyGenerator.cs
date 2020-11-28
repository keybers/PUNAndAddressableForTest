using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomCustomPorpertyGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ID数值")]
    private Text _text;

    [Tooltip("定义PUN的玩家ID参数hashtable")]
    private ExitGames.Client.Photon.Hashtable currentIDProperties = new ExitGames.Client.Photon.Hashtable();

    /// <summary>
    /// 设置ID随机数
    /// </summary>
    private void SetCustomNumber()
    {
        System.Random rmd = new System.Random();
        int result = rmd.Next(0, 99);

        _text.text = result.ToString();

        currentIDProperties["RandomNumber"] = result;
        PhotonNetwork.SetPlayerCustomProperties(currentIDProperties);
        
        //PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
    }

    /// <summary>
    /// 点击当前按钮
    /// </summary>
    public void OnClick_Button()
    {
        SetCustomNumber();
    }
}
