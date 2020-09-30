using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObjectMover : MonoBehaviourPun
{
    [SerializeField]
    private float _moveSpeed = 1f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (base.photonView.IsMine)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            transform.position += (new Vector3(x, y, 0f)) * _moveSpeed;

            UpdateMovingBoolean((x != 0f || y != 0f));
        }
    }

    private void UpdateMovingBoolean(bool moving)
    {
        _animator.SetBool("Moving", moving);
    }
}
