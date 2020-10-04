using System.Collections;
using System.Collections.Generic;
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
                Instantiate(loaderObject, _EnterPosition.position, Quaternion.identity);
                break;

            case AsyncOperationStatus.Failed:
                Debug.LogError("AsyncOperationStatus.Failed");
                break;

            default:
                break;
        }
    }
}
