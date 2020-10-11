using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InstantiateExample : MonoBehaviour
{
    private AddressablePrefabPool addressablePrefabPool;
    private AssetReference _assetReference;

    private void Awake()
    {
        addressablePrefabPool.LoadAsset(_assetReference);
    }
}
