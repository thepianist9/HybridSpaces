using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class AgoraInterface : MonoBehaviour
{
    private string appId = "f44c0ba5267c4fa295f8e0afdc661da5";
    private string channelKey = "";

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
        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;

        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.JoinChannelByKey(channelKey, channelName, null, 0);

    }

    public void leaveChannel()
    {
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
        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.transform.position = new Vector3(0f, 1.0f, .0f);
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        go.transform.Rotate(new Vector3(-90.0f, -.0f, -.0f));
        go.name = uid.ToString();
        
        //configure video 
        VideoSurface remoteVideoSurface = go.AddComponent<VideoSurface>();
        remoteVideoSurface.SetForUser(uid);
        remoteVideoSurface.SetGameFps(30);
        remoteVideoSurface.SetEnable(true);

        mRemotePeer = uid;

    }

    public string getSdkVersion()
    {
        return IRtcEngine.GetSdkVersion();
    }
    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("User with id: "+uid+" has left the channel");
        
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
