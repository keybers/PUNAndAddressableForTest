using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPun : MonoBehaviourPun, IPunObservable
{
    private void Start()
    {
        CinemachineVirtualCamera CMAgent = GetComponentInChildren<CinemachineVirtualCamera>();

        if (base.photonView.IsMine)
        {
            CMAgent.Priority = 2;
        }
        else
        {
            CMAgent.Priority = 1;
            //this.GetComponent<Rigidbody>().Sleep();
        }

    }

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
}
