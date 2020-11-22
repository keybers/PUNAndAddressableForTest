using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    [SerializeField]
    private GameSettings _gameSettings;

    public static GameSettings GameSettings
    {
        get
        {
            return Instance._gameSettings;
        }
    }

    [SerializeField]
    private AddressablePrefabPool _addressablePrefabPool;
    
    public static AddressablePrefabPool AddressablePrefabPool
    {
        get
        {
            return Instance._addressablePrefabPool;
        }
    }
}
