using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class AgoraInterface : MonoBehaviour
{
    [SerializeField] private string appId = "f44c0ba5267c4fa295f8e0afdc661da5";
    [SerializeField] private string channelKey = "";
    [SerializeField] private int totalUsers = 5;
    private int usersCount;

    public IRtcEngine mRtcEngine;

    public uint mRemotePeer;

    public void loadEngine()
    {
        Debug.Log("initializing engine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine already loaded. Please unload first");
        }
        //init rtc engine
        mRtcEngine = IRtcEngine.getEngine(appId);
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG);
    }

    public void joinChannel(string channelKey, string channelName)
    {
        Debug.Log("Joining Channel:" +channelName);

        if (mRtcEngine == null)
        {
            Debug.Log("Engine needs to be initialized before joining a channel");
            return;
        }
        
        //callbacks
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = (uid, elapsed) => OnUserJoined(uid, elapsed);
        mRtcEngine.OnUserOffline = OnUserOffline;

        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.JoinChannelByKey(channelKey, channelName, null, 0);

    }

    public void leaveChannel()
    {
        totalUsers -= 1;
        Debug.Log("leaving channel");
        if (mRtcEngine == null)
        {
            Debug.Log("Engine needs to be initialized before leaving channel");
        }

        mRtcEngine.LeaveChannel();
        mRtcEngine.DisableVideoObserver();
    }

    public void unloadEngine()
    {
        Debug.Log("Unloading Agora Engine");

        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }
    
    //user callbacks
    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Successfully joined channel: " + channelName + " uid: " + uid);
        GameObject versionText = GameObject.Find("VersionText");
        versionText.GetComponent<Text>().text = "Version: " + getSdkVersion();
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("New user has joined with id: "+uid);
        
        //add remote stream to scene
        
        //create game object
        GameObject go;
        GameObject parent = GameObject.FindGameObjectWithTag("Parent");
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.parent = parent.transform;
        go.name = uid.ToString();
        (Vector3 pos, Quaternion rot) = getNextUserPosition();
        
        go.transform.position = pos;
        go.transform.rotation = rot;
        //configure video 
        VideoSurface remoteVideoSurface = go.AddComponent<VideoSurface>();
        remoteVideoSurface.SetForUser(uid);
        remoteVideoSurface.SetGameFps(30);
        remoteVideoSurface.SetEnable(true);

        mRemotePeer = uid;

    }

    (Vector3 pos, Quaternion rot) getNextUserPosition()
    {
        usersCount += 1;
        int radius = 4;
        float angle = usersCount * Mathf.PI * 2 / totalUsers;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 pos = new Vector3(x, 0, z);
        float angleDegrees = -angle*Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, angleDegrees, 180); 
        return (pos, rot);
    }

    public string getSdkVersion()
    {
        return IRtcEngine.GetSdkVersion();
    }
    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("User with id: "+uid+" has left the channel");
        totalUsers -= 1;
        //remove gameobject from scene
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }

    private void OnTransformDelegate(uint uid, string objName, ref Transform transform)
    {
        if (uid == 0)
        {
            transform.position = new Vector3(0f, 2f, 0f);
            transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
            transform.Rotate(0f,1f, 0f);
        }
        else
        {
            transform.Rotate(0.0f, 1.0f, 0.0f);
        }
    }

    public void OnChatSceneLoaded()
    {
        //access gameobj from scene1
        GameObject go = GameObject.Find("Cube");
        //set transform delegate for go
        if (ReferenceEquals(go, null))
        {
            Debug.Log("Unable to find cube object");
        }

        VideoSurface o = go.GetComponent<VideoSurface>();
    }
}