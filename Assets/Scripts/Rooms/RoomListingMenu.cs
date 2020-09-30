using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public RoomListing _roomListing;

    [SerializeField]
    private Transform _content;

    private RoomCanvases _roomCanvases;

    private List<RoomListing> _listings = new List<RoomListing>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if( index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else//if null,add to rooms list
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index == -1)
                {
                    RoomListing listing = Instantiate(_roomListing, _content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
                else
                {
                    //Modify listing here.
                    //_listings[index].dowhatever
                }

            }
        }
    }

    public void FirstInitialize(RoomCanvases roomCanvases)
    {
        _roomCanvases = roomCanvases;
    }

    public override void OnJoinedRoom()
    {
        _roomCanvases.CurrentRoomCanvas.Show();
        _content.DetachChildren();//销毁自身信息，否则会显示自己信息
        _listings.Clear();//当我加入房间的时候，应该清除房间列表，否则退出到大厅的时候，存储的列表中的房间仍然存在，不会添加再添加显示。
    }
}
