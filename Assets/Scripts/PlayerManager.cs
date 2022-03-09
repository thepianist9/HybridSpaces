using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using agora_gaming_rtc;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;

    [SerializeField]
    GameObject player;

    private GameObject feed;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        feed = player.transform.Find("PlayerArmature/Cube").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController(); 
            
            
        }
    }

    // private void OnDestroy()
    // {
    //     if (feed != null)
    //     {
    //         DestroyImmediate(feed.GetComponent<VideoSurface>(),true);
    //     }
    // }

    void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0,1,0), Quaternion.identity);
        
        VideoSurface remoteVideoSurface = feed.AddComponent<VideoSurface>();
        // feed.transform.Rotate(0,0,180, Space.Self);

        remoteVideoSurface.SetEnable(true);

    }
}
