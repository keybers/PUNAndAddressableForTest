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


    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>(playerPrefab).Completed += OnPlayerLoader;
    }

    private void OnPlayerLoader(AsyncOperationHandle<GameObject> obj)
    {
        
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                GameObject loaderObject = obj.Result;
                GameObject go = Instantiate(loaderObject, _EnterPosition.position, Quaternion.identity);
                Debug.Log("ViewID:" + go.GetComponent<PhotonView>());
                break;

            case AsyncOperationStatus.Failed:
                Debug.LogError("AsyncOperationStatus.Failed");
                break;

            default:
                break;
        }
    }

    public class AddressablePrefabPool : IPunPrefabPool
    {
        private Dictionary<string, Pool> prefabs = new Dictionary<string, Pool>();

        public void LoadAsset(AssetReference assetReference)
        {
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

            public Pool(string name, GameObject prefab, int capacity)
            {
                this.name = name;
                this.prefab = prefab;
                this.pooled = new List<GameObject>(capacity);
            }

            public Pool(string name,GameObject prefab,int capacity,int warmSize) : this(name, prefab, capacity)
            {


            }

            public Pool(string name,GameObject result)
            {
                this.name = name;
                this.prefab = result;
                this.pooled = new List<GameObject>();
            }

            public GameObject Get(Vector3 position, Quaternion rotation)
            {
                //
                GameObject go = null;
                int pooledCount = pooled.Count;
                if(pooled.Count > 0)
                {
                    go = this.pooled[pooledCount - 1];
                }
                else
                {
                    //go = Instantiate(prefab, position, rotation);
                }


                return go;
            }

            public void Add(GameObject gameObject)
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                this.pooled.Add(gameObject);
            }
        }
    }
}
