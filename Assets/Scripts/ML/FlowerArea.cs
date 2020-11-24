using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ݺ͸��ӻ��ļ���
/// </summary>
public class FlowerArea : MonoBehaviour
{
    [Tooltip("�����ֱ�������ڹ۲����ͻ�֮��ľ���")] 
    public const float AreaDiameter = 20f;

    [Tooltip("ĳ���������л����б�")]
    public List<GameObject> flowerPlants;

    [Tooltip("�����ֵ�<��ײ��,����>��������һ���")]
    public Dictionary<Collider, Flower> nectarFlowerDictionary;

    /// <summary>
    /// �����������л��ļ���
    /// </summary>
    public List<Flower> Flowers { get; private set; }

    /// <summary>
    /// ���û�԰
    /// </summary>
    public void ResetFlowers()
    {
        // ��������û��ĳ�����Y����תÿ�껨�ݣ��������Χ��X��Z����ת
        foreach (GameObject flower in flowerPlants)
        {
            float xRotation = UnityEngine.Random.Range(-5f, 5f);
            float yRotation = UnityEngine.Random.Range(-180f, 180f);
            float zRotation = UnityEngine.Random.Range(-5f, 5f);
            flower.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        //����������ÿһ�仨
        foreach(Flower flower in Flowers)
        {
            flower.ResetFlower();
        }
    }

    /// <summary>
    /// ���ֵ��л�ȡ����flower
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public Flower GetFlowerFromNectar(Collider collider)
    {
        return nectarFlowerDictionary[collider];
    }

    private void Awake()
    {
        //��ʼ��
        flowerPlants = new List<GameObject>();
        nectarFlowerDictionary = new Dictionary<Collider, Flower>();
        Flowers = new List<Flower>();

        //�������л�԰�����л�������
        FindChildFlowers(transform);

    }

    /// <summary>
    /// �ݹ�ز��Ҹ��任�����л��ͻ���
    /// </summary>
    /// <param name="parent"></param>
    private void FindChildFlowers(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("flower_plant"))
            {
                //�ҵ����ţ���ӵ�ǰ��
                flowerPlants.Add(child.gameObject);

                //�ݹ����
                FindChildFlowers(child);
            }
            else
            {
                //����flower������绨�����Ǹ���
                Flower flower = child.GetComponent<Flower>();
                if(flower != null)
                {
                    //�ҵ�������ӵ����б���
                    Flowers.Add(flower);

                    //��Ӷ�Ӧ�Ļ��ﵽ�ֵ���
                    nectarFlowerDictionary.Add(flower.nectarCollider, flower);

                }
                else
                {
                    //û�ҵ����������������
                    FindChildFlowers(child);
                }
            }
        }
    }
}
