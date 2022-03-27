using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using agora_gaming_rtc;
using ExitGames.Client.Photon.StructWrapping;
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
        public String UId;
       
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
        }

        public void CreateController(uint uid)
        {
            UId = uid.ToString();

            GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0, 1, 0),
                Quaternion.identity);
            
            player.GetComponent<PlayerVideo>().UId = UId;
            player.name = UId;
            GameObject childVideo = GetChildVideoLocation(player);
            VideoSurface videoSurface = MakeImageVideoSurface(childVideo);
            

        }
        public void OnUserJoined(uint uid, int elapsed)
        {
            Debug.Log("user with id: " + uid.ToString() + " joined");

           
            GameObject remoteSpawn = GameObject.Find("PlayerController(Clone)");
            remoteSpawn.name = uid.ToString();
            GameObject feed = remoteSpawn.transform.Find("WebCamFeed").gameObject;
            
            VideoSurface videoSurface = MakeImageVideoSurface(feed);

            if (videoSurface != null)
            {
                // VideoSurface videoSurface = feed.GetComponent<VideoSurface>();
               
                videoSurface.SetForUser(uid);
                // videoSurface.SetEnable(true);
                // videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            }
           
        }
        

        private VideoSurface MakeImageVideoSurface(GameObject go)
        {
            go.AddComponent<RawImage>();

            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.localRotation =
                new Quaternion(0, rectTransform.localRotation.y, -180.0f, rectTransform.localRotation.w);

            return go.AddComponent<VideoSurface>();
        }
        
       
        
        public GameObject GetChildVideoLocation(GameObject go)
        {
            GameObject childVideo = go.transform.Find("WebCamFeed")?.gameObject;

            if (childVideo == null)
            {
                childVideo = new GameObject($"{UId}");
                childVideo.transform.parent = go.transform;
            }

            return childVideo;
        }



    }
}