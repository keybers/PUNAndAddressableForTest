using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField]
    private MenuListing _menuListing;

    private CameraCanvas _cameraCanvas;

    public void FirstInitialize(CameraCanvas cameraCanvas)
    {
        _cameraCanvas = cameraCanvas;
        _menuListing.FirstInitalize(_cameraCanvas);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}
