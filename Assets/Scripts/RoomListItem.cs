using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public RoomInfo _info;
    
    public void SetUp(RoomInfo info)
    {
        _info = info;
        text.text = _info.Name;
    }
    
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(_info);
    }
}
