using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPrefab
{
    public GameObject _prefab;
    public string _path;

    public NetworkPrefab(GameObject obj,string path)
    {
        _prefab = obj;
        _path = ReturnPrefabPathModified(path);
    }

    private string ReturnPrefabPathModified(string path)
    {
        int extensionLength = System.IO.Path.GetExtension(path).Length;
        int startIndex = path.ToLower().IndexOf("resources");

        if(startIndex == -1)
        {
            return string.Empty;
        }
        else
        {
            return path.Substring(startIndex, path.Length - (startIndex + extensionLength));
        }
    
    }

}
