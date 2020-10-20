using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class AddressableTextLoader : MonoBehaviour
{
    private AddressablePrefabPool addressablePrefabPool;

    void Awake()
    {

        addressablePrefabPool = MasterManager.AddressablePrefabPool;
        PhotonNetwork.PrefabPool = this.addressablePrefabPool;

        this.addressablePrefabPool.PrefabPoolReady += addressablePrefabPool.OnPrefabPoolReady;//准备好后调用
        foreach (var assetReferences in addressablePrefabPool.AssetReferences)
        {
            this.addressablePrefabPool.LoadAsset(assetReferences);
        }
    }

    private void OnDestroy()
    {
        addressablePrefabPool.OnDestroy();
    }

}
