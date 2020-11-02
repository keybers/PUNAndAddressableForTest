using Cinemachine;
using Photon.Pun;
using UnityEngine;


public class SimpleObjectMover : MonoBehaviourPun,IPunObservable
{
    [SerializeField]
    private float _moveSpeed = 1f;

    private Animator _animator;
    private GameObject _CMMain;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //按照顺序
        if (stream.IsWriting)//如果数据流是读取
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)//如果数据流是写入
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GameObject.Find("/CM/CMMain").GetComponent<CinemachineVirtualCamera>();
        if (base.photonView.IsMine)
        {
            cinemachineVirtualCamera.LookAt = this.transform;
            cinemachineVirtualCamera.Follow = this.transform;
            Debug.Log("找到了");
        }
    }

    private void Update()
    {
        if (base.photonView.IsMine)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            transform.localPosition += (new Vector3(x, 0f, z)) * _moveSpeed * Time.deltaTime;
            
            UpdateMovingBoolean((x != 0f || z != 0f));
        }
    }

    private void UpdateMovingBoolean(bool moving)
    {
        _animator.SetBool("Moving", moving);
    }

}
