using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private List<NetworkPrefab> _networkPrefabs = new List<NetworkPrefab>();

    public static GameObject NetworkInstantiate(GameObject obj,Vector3 position, Quaternion rotarion)
    {
        foreach(NetworkPrefab networkPrefab in Instance._networkPrefabs )
        {
            if(networkPrefab._prefab == obj)
            {
                GameObject result = PhotonNetwork.Instantiate(networkPrefab._path, position, rotarion);
                return result;
            }
        }

        return null;
    }

    //在加载场景前运行
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PopulateNetworkedPrefabs()
    {
        //只有在打包后才这样做
        if (!Application.isEditor)
            return;
        
        GameObject[] result = Resources.LoadAll<GameObject>("");
        for(int i = 0; i < result.Length; i++)
        {
            if(result[i].GetComponent<PhotonView>()!= null)
            {
                string path = AssetDatabase.GetAssetPath(result[i]);
                Instance._networkPrefabs.Add(new NetworkPrefab(result[i], path));
            }
        }
    
    }
}
