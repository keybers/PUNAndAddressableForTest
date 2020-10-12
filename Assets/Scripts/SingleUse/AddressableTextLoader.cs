using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableTextLoader : MonoBehaviour
{
    public Transform _EnterPosition;//当前位置

    private AddressablePrefabPool addressablePrefabPool;


    void Start()
    {
        //playerPrefab = addressablePrefabPool._assetReference;
        this.addressablePrefabPool = MasterManager.AddressablePrefabPool;
        PhotonNetwork.PrefabPool = this.addressablePrefabPool;

        this.addressablePrefabPool.PrefabPoolReady += addressablePrefabPool.OnPrefabPoolReady;//准备好后调用

        //异步加载物体之前的准备
        this.addressablePrefabPool.LoadAsset(addressablePrefabPool.AssetReference);

    }

    //关闭程序释放池中资源
    //private void OnDestroy()
    //{
    //    this.addressablePrefabPool.PrefabPoolReady -= addressablePrefabPool.OnPrefabPoolReady;
    //}

    //private void OnPrefabPoolReady(string prefabName)
    //{
    //    GameObject go = PhotonNetwork.Instantiate(prefabName, this._EnterPosition.position, Quaternion.identity);
    //    Debug.LogFormat("ViewID: {0}", go.GetComponent<PhotonView>().ViewID);
    //}
}
