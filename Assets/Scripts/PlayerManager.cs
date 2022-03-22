using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using agora_gaming_rtc;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace HybridSpaces
{


    public class PlayerManager : MonoBehaviour
    {
        private PhotonView PV;

        private static PlayerManager _instance;
        
        public static PlayerManager Instance
        {
            get { return _instance; }
        }
        [SerializeField] GameObject player;

        private uint uid;
        private string appId;
        private string channelName;

        private GameObject feed;
        private IRtcEngine _rtcEngine;



        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            _rtcEngine = VoiceChatManager.Instance.GetEngine();
            appId = VoiceChatManager.Instance.AppId;
            PV = GetComponent<PhotonView>();
            uid = VoiceChatManager.Instance.LocalUserId;

            

        }

    

        // Start is called before the first frame update
        void Start()
        {
            if (PV.IsMine)
            {
                CreateController();

            }
        }

        void CreateController()
        {

            GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0, 1, 0),
                Quaternion.identity);
            player.name = uid.ToString();
            GameObject childVideo = GetChildVideoLocation(uid);
            MakeImageVideoSurface(childVideo);
            
            // if (videoSurface != null)
            // { 
            //     videoSurface.SetForUser(uid);
            //     videoSurface.SetEnable(true);
            //     videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            // }

        }
        
        public void OnUserJoined(uint uid, int elapsed)
        {
            Debug.Log("user with id: " + uid.ToString() + " joined");
            
            GameObject childVideo = GetChildVideoLocation(uid);
            VideoSurface videoSurface = MakeImageVideoSurface(childVideo);

            if (videoSurface != null)
            { 
                videoSurface.SetForUser(uid);
                videoSurface.SetEnable(true);
                videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            }
        }
       

       
        
        public GameObject GetChildVideoLocation(uint uid)
        {
            GameObject go = GameObject.Find($"{uid}");
            GameObject childVideo = go.transform.Find("WebCamFeed")?.gameObject;

            if (childVideo == null)
            {
                childVideo = new GameObject($"{uid}");
                childVideo.transform.parent = go.transform;
            }

            return childVideo;
        }

        public VideoSurface MakeImageVideoSurface(GameObject go)
        {
            go.AddComponent<RawImage>();
            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localRotation = new Quaternion(0, rectTransform.localRotation.y, -180.0f, rectTransform.localRotation.w);

            return go.AddComponent<VideoSurface>();
        }


       
    }
}