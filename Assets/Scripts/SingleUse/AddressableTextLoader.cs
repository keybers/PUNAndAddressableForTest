using Photon.Pun;
using UnityEngine;

public class AddressableTextLoader : MonoBehaviour
{
    [Tooltip("对象池")]
    private AddressablePrefabPool addressablePrefabPool;
    
    [Tooltip("ageng生成位置")]
    public Transform spawnPoint;

    void Awake()
    {
        addressablePrefabPool = MasterManager.AddressablePrefabPool;
        PhotonNetwork.PrefabPool = this.addressablePrefabPool;

        this.addressablePrefabPool.PrefabPoolReady += addressablePrefabPool.OnPrefabPoolReady;//准备好后调用
        foreach (var assetReferences in addressablePrefabPool.AssetReferences)
        {
            this.addressablePrefabPool.LoadAsset(assetReferences, spawnPoint);
        }
    }

    private void OnDestroy()
    {
        addressablePrefabPool.OnDestroy();
    }

}
