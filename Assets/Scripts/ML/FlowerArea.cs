using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理花草和附加花的集合
/// </summary>
public class FlowerArea : MonoBehaviour
{
    [Tooltip("区域的直径，用于观察代理和花之间的距离")] 
    public const float AreaDiameter = 20f;

    [Tooltip("某花区内所有花的列表")]
    public List<GameObject> flowerPlants;

    [Tooltip("根据字典<对撞机,花蜜>来管理查找花蜜")]
    public Dictionary<Collider, Flower> nectarFlowerDictionary;

    /// <summary>
    /// 在区域中所有花的集合
    /// </summary>
    public List<Flower> Flowers { get; private set; }

    /// <summary>
    /// 重置花园
    /// </summary>
    public void ResetFlowers()
    {
        // 宏观上设置花的朝向，绕Y轴旋转每株花草，并巧妙地围绕X和Z轴旋转
        foreach (GameObject flower in flowerPlants)
        {
            float xRotation = UnityEngine.Random.Range(-5f, 5f);
            float yRotation = UnityEngine.Random.Range(-180f, 180f);
            float zRotation = UnityEngine.Random.Range(-5f, 5f);
            flower.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        //狭义上重置每一朵花
        foreach(Flower flower in Flowers)
        {
            flower.ResetFlower();
        }
    }

    /// <summary>
    /// 从字典中获取对象flower
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public Flower GetFlowerFromNectar(Collider collider)
    {
        return nectarFlowerDictionary[collider];
    }

    private void Awake()
    {
        //初始化
        flowerPlants = new List<GameObject>();
        nectarFlowerDictionary = new Dictionary<Collider, Flower>();
        Flowers = new List<Flower>();

        //查找所有花园下所有花的数量
        FindChildFlowers(transform);

    }

    /// <summary>
    /// 递归地查找父变换的所有花和花草
    /// </summary>
    /// <param name="parent"></param>
    private void FindChildFlowers(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("flower_plant"))
            {
                //找到花团，添加当前花
                flowerPlants.Add(child.gameObject);

                //递归调用
                FindChildFlowers(child);
            }
            else
            {
                //根据flower组件查早花，不是更具
                Flower flower = child.GetComponent<Flower>();
                if(flower != null)
                {
                    //找到了则添加到花列表中
                    Flowers.Add(flower);

                    //添加对应的花蕊到字典中
                    nectarFlowerDictionary.Add(flower.nectarCollider, flower);

                }
                else
                {
                    //没找到花组件，继续查找
                    FindChildFlowers(child);
                }
            }
        }
    }
}
