using System;
using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    private string AppId = "a866550422764bd481355cbcfa1f8560";
    public static VoiceChatManager Instance;

    private IRtcEngine _rtcEngine;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rtcEngine = IRtcEngine.GetEngine(AppId);

        _rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        _rtcEngine.OnLeaveChannel += OnLeaveChannel;
        _rtcEngine.OnError += OnError;
        _rtcEngine.EnableSoundPositionIndication(true);
    }

    private void OnError(int error, string msg)
    {
        Debug.LogError("Error with Agora: "+ msg);
    }

    void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log("Left channel with duration: " + stats.duration);
    }

    void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Joined Channel: " + channelName );

        Hashtable hash = new Hashtable();
        hash.Add("agoraID", uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash); 
    }

    // Update is called once per frame
    public override void OnJoinedRoom()
    {
        _rtcEngine.EnableVideo();
        _rtcEngine.EnableVideoObserver();
        _rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
        
    }

    public override void OnLeftRoom()
    {
        _rtcEngine.LeaveChannel();
    }

    public IRtcEngine GetRtcEngine()
    {
        return _rtcEngine;
    }

    private void OnDestroy()
    {
        IRtcEngine.Destroy();
    }
    
}
