using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
[InfoBox("游戏设置")]
public class GameSettings : ScriptableObject
{

    [InfoBox("游戏版本")]
    [SerializeField]
    private string _gameVersion = "0.0.0";

    public string GameVersion
    {
        get
        {
            return _gameVersion;
        }
    }

    [InfoBox("用户名称")]
    [SerializeField]
    private string _nickName = "keyber";

    public string NickName
    {
        get
        {
            int value = Random.Range(0, 9999);
            return _nickName + value.ToString();
        }
    }
}
