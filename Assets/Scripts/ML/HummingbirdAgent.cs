using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

/// <summary>
/// A hummingbird Machine Leaning Agent
/// </summary>
public class HummingbirdAgent : Agent
{
    [Tooltip("移动时施加的力")]
    public float moveForce = 2f;

    [Tooltip("向上或向下俯仰的速度")]
    public float pitchSpeed = 100f;

    [Tooltip("绕向上轴旋转的速度")]
    public float yawSpeed = 100f;

    [Tooltip("在喙尖变换")]
    public Transform beakTip;

    [Tooltip("代理人摄像头")]
    public Camera agentCamera;

    [Tooltip("无论是训练模式还是游戏模式")]
    public bool trainingMode;


    [Tooltip("代理人的刚体")]
    new private Rigidbody rigidbody;

    [Tooltip("代理所在的花园环境")]
    private FlowerArea flowerArea;

    [Tooltip("最近代理附近的花")]
    private Flower nearestFlower;

    [Tooltip("允许更平滑的头部俯仰变化")]
    private float smoothPitchChange = 0f;

    [Tooltip("允许更平滑的旋转变化")]
    private float smoothYawChange = 0f;

    [Tooltip("代理鸟能俯仰的最大角度")]
    private const float MaxPitchAngle = 80f;

    [Tooltip("从喙尖到接受花蜜碰撞的最大距离")]
    private const float BeakTipRedius = 0.008f;

    [Tooltip("代理是否被冻结（故意不飞）")]
    private bool frozen = false;

    /// <summary>
    /// 代理获得的花蜜量
    /// </summary>
    public float NectarObtained { get; private set; }

    /// <summary>
    /// 初始化代理
    /// </summary>
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        flowerArea = GetComponentInParent<FlowerArea>();

        //如果不是训练模式，没有最大步数，永远玩
        if (!trainingMode) MaxStep = 0;

    }

    /// <summary>
    /// reset the agent when an episord begings
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            //当每个训练区域只剩下一个代理的时候，只重置花
            flowerArea.ResetFlowers();
        }

        //重置花蜜量
        NectarObtained = 0f;

        //将速度调零，以便在新的回合开始前停止运动
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        //默认为在花前生产
        bool inFrontOfFlower = true;
        if (trainingMode)
        {
            //在试验过程中有50%的概率在花的外面生产
            inFrontOfFlower = Random.value > .5f;
        }

        //将代理移动到一个新的随机位置
        MoveToSafeRandomPosition(inFrontOfFlower);

        //现在代理已经移动，重新计算最近的花
        UpdateNearestFlower();

    }

    /// <summary>
    /// 更新离代理最近的花
    /// </summary>
    private void UpdateNearestFlower()
    {
        foreach(Flower flower in flowerArea.Flowers)
        {
            if (nearestFlower == null && flower.HasNectar)
            {
                //目前没有最近的花并且这朵花有花蜜，所以设置这花为最近花
                nearestFlower = flower;
            }
            else if (flower.HasNectar)
            {
                //计算到该花的距离和到当前最近花的距离
                float distanceToFlower = Vector3.Distance(flower.transform.position, beakTip.position);
                float distanceToCurrentNearestFlower = Vector3.Distance(nearestFlower.transform.position, beakTip.position);

                //如果当前最近的花为空或此花较近，请更新该花
                if(nearestFlower.HasNectar || distanceToFlower < distanceToCurrentNearestFlower)
                {
                    nearestFlower = flower;
                }
            }

        }
    }

    /// <summary>
    /// 把代理移到一个安全的随机位置(ie does not collide with anthing)
    /// 如果在花前面，也要用嘴指向花
    /// </summary>
    /// <param name="inFrontOfFlower">是否在花前的地方</param>
    private void MoveToSafeRandomPosition(bool inFrontOfFlower)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100;//预测无限循环尝试
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        //循环直到找到一个安全的位置或者没有尝试次数
        while (!safePositionFound && attemptsRemaining > 0)
        {
            attemptsRemaining--;
            if (inFrontOfFlower)
            {
                //随便从中挑一朵花
                Flower randomFlower = flowerArea.Flowers[Random.Range(0, flowerArea.Flowers.Count)];

                //在花的前面10到20厘米的位置，鸟和花的距离
                float distanceFromFlower = Random.Range(.1f, .2f);

                //预测位置 = 花位置 + 花向上矢量 * 随机距离长度,预测位置是鸟位置
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                //用嘴指着花矢量 (代理的头是transform的中心)
                Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;

                //代理朝向
                potentialRotation = Quaternion.LookRotation(toFlower,Vector3.up);
            }
            else
            {
                //从地面随机选取一个高度
                float height = Random.Range(1.2f, 2.5f);

                //从区域中心随机选取一个半径
                float radius = Random.Range(2f, 7f);

                //选择一个随机目标环绕Y轴转
                Quaternion direction = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);

                //结合“高度”、“半径”和“方向”来选择一个潜在的位置
                potentialPosition = flowerArea.transform.position + Vector3.up * height + direction * Vector3.forward * radius;

                //choose and set random starting pitch and yam
                float pitch = Random.Range(-60f, 60f);
                float yam = Random.Range(-180, 180f);
                potentialRotation = Quaternion.Euler(pitch, yam, 0f);
            }

            //检查是否代理碰撞到任何东西,检测3D相加球范围内的物体
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

            //在没有对撞机重叠的情况下，已经找到了安全位置
            safePositionFound = colliders.Length == 0;
        }

        Debug.Assert(safePositionFound, "Could not find a safe position to spawn");

        //set position and rotation
        transform.position = potentialPosition;
        transform.rotation = potentialRotation;
    }

    /// <summary>
    /// 当从玩家输入或神经网络接收到动作时调用
    ///
    /// vectorAction[i] represents:
    /// input 0: move vector x (+1 = right, -1 = left)
    /// input 1: move vector y (+1 = up, -1 = down)
    /// input 2: move vector z (+1 = forward,-1 = backward)
    /// index 3: pitch angle (+1 = pitch up, -1 = pitch down)
    /// index 4: yaw angle (+1 =  turn right, -1 = turn left)
    /// </summary>
    /// <param name="vectorAction">要采取的行动</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        

    }
}
