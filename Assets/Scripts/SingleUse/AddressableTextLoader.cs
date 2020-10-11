using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;//异步加载

public class AddressableTextLoader : MonoBehaviour
{
    public AssetReference playerPrefab;

    public Transform _EnterPosition;//当前位置

    private AddressablePrefabPool addressablePrefabPool;


    // Start is called before the first frame update
    void Start()
    {
        this.addressablePrefabPool = new AddressablePrefabPool();
        PhotonNetwork.PrefabPool = this.addressablePrefabPool;
        this.addressablePrefabPool.PrefabPoolReady += this.OnPrefabPoolReady;
        this.addressablePrefabPool.LoadAsset(this.playerPrefab);

    }

    //关闭程序释放池中资源
    private void OnDestroy()
    {
        this.addressablePrefabPool.PrefabPoolReady -= this.OnPrefabPoolReady;
    }

    private void OnPrefabPoolReady(string prefabName)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, this._EnterPosition.position, Quaternion.identity);
        Debug.LogFormat("ViewID: {0}", go.GetComponent<PhotonView>().ViewID);
    }

    public class AddressablePrefabPool : IPunPrefabPool
    {
        private Dictionary<string, Pool> prefabs = new Dictionary<string, Pool>();

        //先把addressable的资源全部加载出来并且池化
        public void LoadAsset(AssetReference assetReference)
        {
            //异步
            Addressables.LoadAssetAsync<GameObject>(assetReference).Completed += (delegate (
                AsyncOperationHandle<GameObject> handle)
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        GameObject prefab = handle.Result;
                        PhotonView photonView = prefab.GetComponent<PhotonView>();
                        if (photonView)
                        {
                            string key = assetReference.AssetGUID;//GUID（全局唯一标识符）
                            //string key = assetReference.RuntimeKey.ToString();
                            Debug.Log("assetReference.AssetGUID:" + "${assetReference.AssetGUID}"+ "assetReference.RuningtimeKey:" + "${assetReference.RuningtimeKey}");
                            this.prefabs[key] = new Pool(key, handle.Result);
                            if(this.prefabs[key] != null)
                            {
                                this.PrefabPoolReady(key);
                            }
                        }
                        Debug.Log("AsyncOperationStates.SUCCESSEDED");
                        break;

                    case AsyncOperationStatus.Failed:
                        Debug.Log("AsyncOperationStates.FAIL");
                        break;
                }
            });
        }

        public delegate void PrefabPoolReadyDelegate(string prefab);

        public event PrefabPoolReadyDelegate PrefabPoolReady;

        public void Destroy(GameObject gameObject)
        {
            PrefabReference prefabReference = gameObject.GetComponent<PrefabReference>();
            Pool pool;
            if(prefabReference && this.prefabs.TryGetValue(prefabReference.originalPrefabName,out pool))//返回找出正确的对象池
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
                pool.Add(gameObject);//销毁后返回到正确的池中
            }
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if(this.prefabs.TryGetValue(prefabId,out var pool))
            {
                GameObject go = pool.Get(position, rotation);
                if (go.activeSelf)
                {
                    go.SetActive(false);//return GameObject must be deactivated
                }
                return go;
            }
            return null;
        }

        class Pool
        {
            private string name;
            private GameObject prefab;

            private List<GameObject> pooled;

            //名字，预设，容量
            public Pool(string name, GameObject prefab, int capacity)
            {
                this.name = name;
                this.prefab = prefab;
                this.pooled = new List<GameObject>(capacity);
            }

            //名字，预设，容量，大小
            public Pool(string name,GameObject prefab,int capacity,int warmSize) : this(name, prefab, capacity)
            {
                for(int i = 0; i < warmSize; i++)
                {
                    GameObject go = GameObject.Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
                    if (go.activeSelf)
                        go.SetActive(false);
                    PrefabReference prefabReference = go.GetComponent<PrefabReference>();
                    if (!prefabReference)
                        prefabReference = go.AddComponent<PrefabReference>();
                    prefabReference.originalPrefabName = this.name;
                    this.pooled.Add(go);
                }

            }

            public Pool(string name,GameObject result)
            {
                this.name = name;
                this.prefab = result;
                this.pooled = new List<GameObject>();
            }

            public GameObject Get(Vector3 position, Quaternion rotation)
            {
                GameObject go;
                int pooledCount = pooled.Count;
                if(pooled.Count > 0)
                {
                    go = this.pooled[pooledCount - 1];
                }
                else
                {
                    go = GameObject.Instantiate(this.prefab, position, rotation);
                    if (go.activeSelf)
                    {
                        go.SetActive(false);
                    }
                }
                PrefabReference prefabReference = go.GetComponent<PrefabReference>();
                if (!prefabReference)
                {
                    prefabReference = go.AddComponent<PrefabReference>();
                }
                prefabReference.originalPrefabName = this.name;
                return go;
            }

            public void Add(GameObject gameObject)
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
                this.pooled.Add(gameObject);
            }
        }
    }
}
