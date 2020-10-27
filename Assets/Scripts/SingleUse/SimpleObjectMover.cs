using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObjectMover : MonoBehaviourPun,IPunObservable
{
    [SerializeField]
    private float _moveSpeed = 1f;

    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private Animator _animator;

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
        _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_cinemachineVirtualCamera.isActiveAndEnabled)
        {
            _cinemachineVirtualCamera.Follow = this.transform;
            _cinemachineVirtualCamera.LookAt = this.transform;
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
