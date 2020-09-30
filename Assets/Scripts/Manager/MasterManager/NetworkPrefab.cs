using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//为了索引查找序列化文件
public class NetworkPrefab
{
    public GameObject Prefab;
    public string Path;

    public NetworkPrefab(GameObject obj,string path)
    {
        Prefab = obj;
        Path = ReturnPrefabPathModified(path);
        //Assets/Resources/File.prefab =》Resources/File

    }

    private string ReturnPrefabPathModified(string path)
    {
        //获取到文件的扩展名长度 .prefab
        int extensionLength = System.IO.Path.GetExtension(path).Length;
        int addtionalLength = 10;

        //全部转换为小写，查找文件索引值 assets/resources/file.prefab => /resources
        int startIndex = path.ToLower().IndexOf("resources") + addtionalLength;

        if(startIndex == -1)
        {
            return string.Empty;
        }
        else
        {
            return path.Substring(startIndex, path.Length - (startIndex + extensionLength));//检索
        }
    
    }

}
