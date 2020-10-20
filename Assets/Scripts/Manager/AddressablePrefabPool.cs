using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Manager/AddressablePrefabPool")]
[InfoBox("游戏对象池")]
public class AddressablePrefabPool : ScriptableObject,IPunPrefabPool
{
    [SceneObjectsOnly]
    [InfoBox("自定义对象池中的物体")]
    [SerializeField]
    private List<AssetReference> _assetReferences;
    
    public List<AssetReference> AssetReferences
    {
        get
        {
            return _assetReferences;
        }
    }

    private Dictionary<string, Pool> prefabs = new Dictionary<string, Pool>();

    public void OnDestroy()
    {
        this.PrefabPoolReady -= OnPrefabPoolReady;
        PhotonNetwork.DestroyAll();
    }

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
                        this.prefabs[key] = new Pool(key, handle.Result);
                        if (this.prefabs[key] != null)
                        {
                            this.PrefabPoolReady(key);
                        }
                    }
                    else
                    {
                        photonView = prefab.AddComponent<PhotonView>();
                    }
                    Debug.Log("AsyncOperationStates.SUCCESSEDED");
                    break;

                case AsyncOperationStatus.Failed:
                    Debug.Log("AsyncOperationStates.FAIL");
                    break;
            }
        });
    }

    public delegate void PrefabPoolReadyDelegate(string prefabName);

    public event PrefabPoolReadyDelegate PrefabPoolReady;

    public void OnPrefabPoolReady(string prefabName)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
        Debug.LogFormat("ViewID: {0}", go.GetComponent<PhotonView>().ViewID);
    }


    public void Destroy(GameObject gameObject)
    {
        PrefabReference prefabReference = gameObject.GetComponent<PrefabReference>();
        Pool pool;
        if (prefabReference && this.prefabs.TryGetValue(prefabReference.originalPrefabName, out pool))//返回找出正确的对象池
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
        if (this.prefabs.TryGetValue(prefabId, out var pool))
        {
            GameObject go = pool.Get(position, rotation);
            if (go.activeSelf)
            {
                go.SetActive(false);//return GameObject must be deactivated
            }
            Debug.LogFormat("ViewID: {0}", go.GetComponent<PhotonView>().ViewID);
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
        public Pool(string name, GameObject prefab, int capacity, int warmSize) : this(name, prefab, capacity)
        {
            for (int i = 0; i < warmSize; i++)
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

        public Pool(string name, GameObject result)
        {
            this.name = name;
            this.prefab = result;
            this.pooled = new List<GameObject>();
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject go;
            int pooledCount = this.pooled.Count;
            if (pooled.Count > 0)
            {
                go = this.pooled[pooledCount - 1];
                go.transform.SetPositionAndRotation(position, rotation);
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
