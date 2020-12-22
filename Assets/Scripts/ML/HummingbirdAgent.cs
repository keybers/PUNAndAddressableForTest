using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
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

    [Tooltip("从喙尖到接受花蜜碰撞的最大距离，喙尖半径")]
    private const float BeakTipRedius = 0.008f;

    [Tooltip("代理是否被冻结（故意不飞）")]
    private bool frozen = false;

    /// <summary>
    /// 代理获得的花蜜量
    /// </summary>
    public float NectarObtained { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化代理
    /// </summary>
    public override void Initialize()
    {
        //绑定
        rigidbody = GetComponent<Rigidbody>();
        flowerArea = GetComponentInParent<FlowerArea>();

        //设置位置
        transform.parent = GameObject.FindWithTag("spawnpoint").transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

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
        //如果冻结,不做任何事
        if (frozen) return;

        //收集移动方向
        Vector3 move = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);

        //添加能产生动作的力
        rigidbody.AddForce(moveForce * move);

        //获取当前朝向
        Vector3 rotationVector = transform.rotation.eulerAngles;

        //收集起降方向和头部左右摆动控制
        float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];

        //收集平滑旋转改变后的量 target -> current
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        //收集基于平滑旋转和起伏的对应值
        //最终起伏值 = 当前旋转x + 平滑旋转改变值 * 平滑时间 * 旋转速度大小
        //如果值大于180度 则变化为-180度，否则值只会越来越大,限定在范围内
        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);

        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        //激活旋转起伏的数值
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    /// <summary>
    /// 从环境中收集观测数据
    /// </summary>
    /// <param name="sensor">矢量传感器</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //如果附近花不存在，则观测当前数值(1观测量)
        if(nearestFlower == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        //观测代理当前的朝向数据(4 观察量)
        sensor.AddObservation(transform.localRotation.normalized);

        //代理位置指向花中心位置的矢量
        Vector3 toFlower = nearestFlower.FlowerCenterPosition - beakTip.position;

        //观测代理位置指向花中心位置的矢量数据(3 观测量)
        sensor.AddObservation(toFlower.normalized);

        //观察一个点积，表明喙尖是否在花的前面(1 观测量)
        //(+1 意味着喙尖在花的正前方 , -1 意思是在后方)确保代理在花的头顶上
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));

        //观察一个点积，它能诱导喙是否指向花(1 观测量)
        //(+1 意味着喙直接指向花, -1 意思是直接离开)
        sensor.AddObservation(Vector3.Dot(beakTip.forward, -nearestFlower.FlowerUpVector.normalized));

        //观察喙尖到花的相对距离，在圆面区域中，矢量原则上一定比直径要小(1 观测量)
        sensor.AddObservation(toFlower.magnitude / FlowerArea.AreaDiameter);

        //11
    }

    /// <summary>
    /// 当行为类型在代理行为参数中设置为“仅人工操作”
    /// 该方法被调用，并且返回输入值。
    /// <see cref="OnActionReceived(float[])"/> 替换掉神经网络
    /// </summary>
    /// <param name="actionsOut">输出动作数组,actionsOut中的数据要对应神经网络的vectorAction来定</param>
    public override void Heuristic(float[] actionsOut)
    {
#if UNITY_EDITOR
        Debug.Log("机器学习模式");
#else
        if (!GetComponent<PhotonView>().IsMine)
            return;
#endif
        //create placeholders for all movement/turning
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        //将键盘输入转换为移动和旋转
        //所有值应介于-1和+1之间

        //向前向后移动
        if (Input.GetKey(KeyCode.W))
        {
            forward = transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forward = -transform.forward;
        }

        //向左向右移动
        if (Input.GetKey(KeyCode.A))
        {
            left = -transform.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            left = transform.right;
        }

        //起降移动
        if (Input.GetKey(KeyCode.E))
        {
            up = transform.up;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            up = -transform.up;
        }

        //pitch up/down
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pitch = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            pitch = -1;
        }

        //turn right/left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            yaw = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            yaw = -1;
        }

        //将移动数据和朝向和升降结合整合到一起
        Vector3 combined = (forward + left + up).normalized;

        //添加三个动作数据到动作数组中
        actionsOut[0] = combined.x;
        actionsOut[1] = combined.y;
        actionsOut[2] = combined.z;
        actionsOut[3] = yaw;
        actionsOut[4] = pitch;
    }

    /// <summary>
    /// 阻止代理移动和执行操作
    /// </summary>
    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/UnFreeze not supported in training");

        frozen = true;
        rigidbody.Sleep();
    }


    /// <summary>
    /// 唤醒代理移动和执行操作
    /// </summary>
    public void UnFreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/UnFreeze not supported in training");

        frozen = false;
        rigidbody.WakeUp();
    }

    /// <summary>
    /// 当代理的对撞机进入触发器对撞机时调用
    /// </summary>
    /// <param name="other">触发的对撞机</param>
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// 当代理的对撞机停留在触发器对撞机中时调用
    /// </summary>
    /// <param name="other">触发的对撞机</param>
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// 管理当代理的对撞机进入或者停留在碰撞器中
    /// </summary>
    /// <param name="other">触发的对撞机</param>
    private void TriggerEnterOrStay(Collider collider)
    {
        //检查代理是否碰撞到花蜜
        if (collider.CompareTag("nectar"))
        {
            //碰撞点的离得代理喙尖最边缘位置
            Vector3 closestPointToBeakTip = collider.ClosestPointOnBounds(beakTip.position);

            //确认是否喙尖位置与最近碰撞点距离小于喙尖半斤
            if (Vector3.Distance(closestPointToBeakTip,beakTip.position) < BeakTipRedius)
            {
                //根据对撞机查看花字典中存储的花
                Flower flower = flowerArea.GetFlowerFromNectar(collider);

                //准备喝0.01花蜜
                //这是每个固定的时间阶段，意味着它每发生一次。
                float nectarReceived = flower.Feed(.01f);

                //跟踪获得的花蜜
                NectarObtained += nectarReceived;
                //Debug.Log("NectarObtained:" + NectarObtained);

                //只有当训练模式的情况才有加分
                if (trainingMode)
                {
                    //代理朝向前方 * 花位置向下矢量的值越大，获得的分数越多
                    float bouns = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.FlowerUpVector.normalized));
                    AddReward(bouns + .01f);
                }

                //如果花蜜是空的，更新花
                if (!flower.HasNectar)
                {
                    UpdateNearestFlower();
                }
            }
        }
    }

    /// <summary>
    /// 当代理与固体碰撞时调用
    /// </summary>
    /// <param name="collision">对撞机信息</param>
    private void OnCollisionEnter(Collision collision)
    {
        //如果在训练下代理碰到边界，则减分
        if(trainingMode && collision.collider.CompareTag("boundary"))
        {
            AddReward(-.5f);
        }
    }

    private void Update()
    {
        //draw a line from beak tip to the nearest flower
        if(nearestFlower != null)
        {
            Debug.DrawLine(beakTip.position, nearestFlower.FlowerCenterPosition, Color.green);
        }
    }
    
    /// <summary>
    /// 每.02帧都会检测是否除最近花其它全部花的花蜜都没有，则重置花
    /// </summary>
    private void FixedUpdate()
    {
        if(nearestFlower != null && !nearestFlower.HasNectar)
        {
            UpdateNearestFlower();
        }
    }

}
