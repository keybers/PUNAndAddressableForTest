using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableTextLoader : MonoBehaviour
{
    private AddressablePrefabPool addressablePrefabPool;

    void Awake()
    {
        addressablePrefabPool = MasterManager.AddressablePrefabPool;
        PhotonNetwork.PrefabPool = this.addressablePrefabPool;

        this.addressablePrefabPool.PrefabPoolReady += addressablePrefabPool.OnPrefabPoolReady;//准备好后调用

        //异步加载物体之前的准备
        foreach(var assetReferences in addressablePrefabPool.AssetReferences)
        {
            this.addressablePrefabPool.LoadAsset(assetReferences);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            addressablePrefabPool.OnDestroy();
        }

    }
}
