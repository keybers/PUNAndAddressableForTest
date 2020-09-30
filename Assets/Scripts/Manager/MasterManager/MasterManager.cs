using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    private List<NetworkPrefab> _networkPrefabs = new List<NetworkPrefab>();

    public static GameObject NetworkInstantiate(GameObject obj,Vector3 position, Quaternion rotarion)
    {
        foreach(NetworkPrefab networkPrefab in Instance._networkPrefabs )
        {
            if(networkPrefab.Prefab == obj)
            {
                GameObject result = PhotonNetwork.Instantiate(networkPrefab.Path, position, rotarion);
                return result;
            }
            else
            {
                Debug.LogError("Path is empty for gameobject name " + networkPrefab.Prefab);
                return null;
            }
        }

        return null;
    }

    //在加载场景前运行
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PopulateNetworkedPrefabs()
    {
#if UNITY_EDITOR

        //确保当前列表为空，才可加载
        Instance._networkPrefabs.Clear();

        //将所有gameobject加载到result中，含有photonView组件的就添加到列表中去
        GameObject[] result = Resources.LoadAll<GameObject>("");
        for(int i = 0; i < result.Length; i++)
        {
            if(result[i].GetComponent<PhotonView>()!= null)
            {
                string path = AssetDatabase.GetAssetPath(result[i]);
                Instance._networkPrefabs.Add(new NetworkPrefab(result[i], path));
            }
        }

        for(int i = 0;i<Instance._networkPrefabs.Count; i++)
        {
            Debug.Log(Instance._networkPrefabs[i].Prefab.name + "," + Instance._networkPrefabs[i].Path);
        }
#endif

    }
}
