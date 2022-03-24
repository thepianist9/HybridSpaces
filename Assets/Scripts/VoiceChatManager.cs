using System;
using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace HybridSpaces
{


    public class VoiceChatManager : MonoBehaviourPunCallbacks
    {
        public string AppId = "a866550422764bd481355cbcfa1f8560";
        private static VoiceChatManager _Instance;
        public string channelName;

        public static VoiceChatManager Instance
        {
            get { return _Instance; }
        }
        private uint myId;

        private IRtcEngine _rtcEngine;

        public IRtcEngine GetEngine()
        {
            return _rtcEngine;
        }
        public uint UId
        {
            get
            {
                return myId;
            }   
        }

        private void Awake()
        {
            if (_Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                _Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (_rtcEngine != null)
            {
                return;
            }
            _rtcEngine = IRtcEngine.GetEngine(AppId);
            _rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
            _rtcEngine.OnUserOffline += OnUserOffline;
            _rtcEngine.OnLeaveChannel += OnLeaveChannel;
            _rtcEngine.OnUserJoined += OnUserJoined;
            _rtcEngine.OnError += OnError;
            _rtcEngine.EnableSoundPositionIndication(true);
            _rtcEngine.EnableVideo();
            _rtcEngine.EnableVideoObserver();

        }

        private void OnUserJoined(uint uid,int elapsed )
        {
            PlayerManager.Instance.OnUserJoined(uid, elapsed);
        }
        
        private void OnError(int error, string msg)
        {
            Debug.LogError("Error with Agora: " + msg);
        }

        void OnLeaveChannel(RtcStats stats)
        {
            Debug.Log("Left channel with duration: " + stats.duration);

        }
        private void OnUserOffline(uint id, USER_OFFLINE_REASON msg)
        {
            GameObject go = GameObject.Find($"{myId}");
            if(go != null) Destroy(go);
        }
        
        void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("Joined Channel: " + channelName);
            
            Hashtable hash = new Hashtable();
            hash.Add("agoraID", uid.ToString());
            PhotonNetwork.SetPlayerCustomProperties(hash);
            myId = uid;

        }
      
        
       
        // Update is called once per frame
        public override void OnJoinedRoom()
        {
            channelName = PhotonNetwork.CurrentRoom.Name;
           
            _rtcEngine.JoinChannel(channelName);

        }
        private void OnDestroy()
        {
            IRtcEngine.Destroy();
            _rtcEngine = null;
        }

  
        
        public void Leave()
        {
            Debug.Log($"Leaving Channel");
            if (_rtcEngine == null) return;
            _rtcEngine.LeaveChannel();
            _rtcEngine.DisableVideoObserver();
            GameObject go = GameObject.Find($"{myId}");
            if (go != null) Destroy(go);
            
            _rtcEngine = null;
            IRtcEngine.Destroy();
        }
        
        private void OnApplicationQuit()
        {
            Leave();
        }

    }
}