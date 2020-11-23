using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Tooltip("当前花有花蜜的颜色")]
    public Color fullFlowerColor = new Color(1f, 0, 1f);

    [Tooltip("当前花没有有花蜜的颜色")]
    public Color emptyFlowerColor = new Color(.5f, 0, 1f);

    /// <summary>
    /// 代表花蜜的触发对撞机
    /// </summary>
    [HideInInspector]
    public Collider nectarCollider;

    //代表花的实体对撞机
    private Collider flowerCollider;

    //花的材质
    private Material flowerMaterial;

    /// <summary>
    /// 指向花上方的矢量
    /// </summary>
    public Vector3 FlowerUpVector
    {
        get
        {
            return nectarCollider.transform.up;
        }
    }

    /// <summary>
    /// 花蕊的中心位置
    /// </summary>
    public Vector3 FlowerCenterPosition
    {
        get
        {
            return nectarCollider.transform.position;
        }
    }

    /// <summary>
    /// 花蕊数量
    /// </summary>
    public float NectarAmount { get; private set; }

    /// <summary>
    /// 判断是否有花蕊
    /// </summary>
    public bool HasNectar
    {
        get
        {
            return NectarAmount > 0f;
        }
    }

    /// <summary>
    /// 减少花蜜喂给代理
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public float Feed(float amount)
    {
        //去除花蜜数限制在一定范围内，超过最大则取NectarAmount，超过最小取0
        float nectarTaken = Mathf.Clamp(amount, 0, NectarAmount);

        //总花蜜 - 取走数
        NectarAmount -= nectarTaken;

        if(NectarAmount <= 0)
        {
            NectarAmount = 0;

            flowerCollider.gameObject.SetActive(false);
            nectarCollider.gameObject.SetActive(false);

            flowerMaterial.SetColor("_BaseColor", emptyFlowerColor);
        }

        //返回获取花蜜的数量
        return nectarTaken;
    }

    /// <summary>
    /// 重置花
    /// </summary>
    public void ResetFlower()
    {
        //初始化花蕊为1
        NectarAmount = 1f;

        //打开碰撞器
        flowerCollider.gameObject.SetActive(true);
        nectarCollider.gameObject.SetActive(true);

        //改变花蕊颜色表示为充满花蕊数量
        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);
    }

    /// <summary>
    /// 唤醒参数
    /// </summary>
    public void Awake()
    {
        //找到花的网格渲染并且获得主材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        flowerMaterial = meshRenderer.material;

        //获取花和花蕊碰撞器
        flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }

}



















